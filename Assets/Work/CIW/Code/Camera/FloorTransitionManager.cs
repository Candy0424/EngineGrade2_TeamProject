using Chuh007Lib.Dependencies;
using echo17.EndlessBook.Demo01;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using Work.CUH.Code.Command;
using Work.CUH.Code.Commands;

namespace Work.CIW.Code.Camera
{
    public class FloorTransitionManager : MonoBehaviour, ICommandable
    {
        [Header("Cinemachine Cameras")]
        [SerializeField] CinemachineCamera floorCam;
        [SerializeField] CinemachineCamera transitionCam;

        [Header("Floor Objects")]
        [SerializeField] List<GameObject> floorObjs;
        [SerializeField] GameObject playerObj;
        int _currentIdx = 0;

        [Header("Transition settings")]
        [SerializeField] float moveDuration = 1.5f;
        [SerializeField] float camHeightForFloorView = 10f;

        [SerializeField] Demo01 demo01;
        bool _isBookTurnCompleted = false;

        [Inject] CommandManager _cmdManager;

        const int ActivePriority = 11;
        const int DefaultPriority = 9;

        private void Start()
        {
            floorCam.Priority = ActivePriority;
            transitionCam.Priority = DefaultPriority;

            for (int i = 0; i < floorObjs.Count; i++)
            {
                floorObjs[i].SetActive(i == _currentIdx);
            }
            playerObj.SetActive(true);

            floorCam.transform.rotation = Quaternion.Euler(90f, 0f, 0f);

            SetFloorCameraTarget(floorObjs[_currentIdx].transform);
        }

        public void StartFloorTransition(int dir)
        {
            int nextIdx = _currentIdx + dir;

            if (nextIdx >= 0 && nextIdx < floorObjs.Count)
            {
                BookTurnCommand command = new BookTurnCommand(
                    this,
                    demo01,
                    nextIdx,
                    dir,
                    _currentIdx
                );
                command.Execute();

            }
            else
            {
                Debug.LogWarning("더 이상 이동할 층이 없습니다.");
            }
        }

        public IEnumerator UndoTransitionSequence(int prevFloorIdx, int undoDir)
        {
            GameObject curObj = floorObjs[_currentIdx]; // 현재(잘못된) 층
            GameObject targetObj = floorObjs[prevFloorIdx]; // 되돌아갈 층 (Undo 목표)

            playerObj.SetActive(false);
            curObj.SetActive(false);

            // 2. Transition Cam 활성화 (책 넘김 애니메이션을 보여주기 위함)
            floorCam.Priority = DefaultPriority;
            transitionCam.Priority = ActivePriority;

            Debug.Log("Undo Transition Camera 전환 시작");

            // 3. 책 되돌리기 애니메이션 시작
            bool canSuc = false;
            if (demo01 != null)
            {
                try
                {
                    // Reflection 대신 직접 호출 가능 (Demo01에 public OnTurnButtonClicked가 있으므로)
                    demo01.GetType().GetMethod("OnTurnButtonClicked").Invoke(demo01, new object[] { undoDir });

                    canSuc = true;
                    _isBookTurnCompleted = false; // 완료 플래그 초기화
                }
                catch (Exception e)
                {
                    Debug.LogError($"OnTurnButtonClicked (Undo) 호출 실패: {e.Message}. MoveDuration을 사용합니다.");
                    canSuc = false;
                }
            }

            // 4. 책 넘김 완료 대기
            if (canSuc)
            {
                while (!_isBookTurnCompleted)
                {
                    yield return null; // Demo01.OnBookTurnToPageCompleted 콜백을 기다림
                }
            }
            else
            {
                // Fallback: 책 컨트롤러가 없으면 임시로 moveDuration 대기
                yield return new WaitForSeconds(moveDuration);
            }

            Debug.Log("책 되돌리기 완료");

            _currentIdx = prevFloorIdx;

            SetFloorCameraTarget(targetObj.transform);
            targetObj.SetActive(true);

            transitionCam.Priority = DefaultPriority;
            floorCam.Priority = ActivePriority;

            playerObj.SetActive(true);

            yield return new WaitForSeconds(0.5f);

            Debug.Log("Undo 카메라 전환 완료");
        }

        private void SetFloorCameraTarget(Transform targetTrm)
        {
            Vector3 newCamPos = new Vector3(targetTrm.position.x, targetTrm.position.y + camHeightForFloorView, targetTrm.position.z);

            floorCam.transform.position = newCamPos;
            floorCam.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        }

        public void HandleBookTurnCompleted()
        {
            _isBookTurnCompleted = true;
        }

        public void SetBookState(int stateIdx)
        {
            if (demo01 != null)
            {
                try
                {
                    demo01.OnStateButtonClicked(stateIdx);
                }
                catch (Exception e)
                {
                    Debug.LogError($"OnStateButtonClicked 호출 실패: {e.Message}");
                }
            }
            else
            {
                Debug.LogWarning("Demo01 컨트롤러가 연결되지 않아 책 상태를 변경할 수 없습니다.");
            }
        }


        public IEnumerator TransitionSequence(int nextFloorIdx)
        {
            GameObject curObj = floorObjs[_currentIdx];
            GameObject targetObj = floorObjs[nextFloorIdx];

            playerObj.SetActive(false);
            curObj.SetActive(false);

            floorCam.Priority = DefaultPriority;
            transitionCam.Priority = ActivePriority;

            Debug.Log("Transition Camera 전환 시작");

            bool canSuc = false;
            int direction = nextFloorIdx > _currentIdx ? 1 : -1;

            if(demo01 != null)
            {
                try
                {
                    demo01.GetType().GetMethod("OnTurnButtonClicked").Invoke(demo01, new object[] { direction });

                    canSuc = true;
                    _isBookTurnCompleted = false;
                }
                catch (Exception e)
                {
                    Debug.LogError($"OnTurnButtonClicked 호출 실패: {e.Message}. MoveDuration을 사용합니다.");
                    canSuc = false;
                }
            }
            else
            {
                Debug.LogWarning("Book Controller가 연결되지 않아 moveDuration을 사용합니다.");
            }

            if (canSuc)
            {
                while (!_isBookTurnCompleted)
                {
                    yield return null;
                }
            }
            else
            {
                yield return new WaitForSeconds(moveDuration);
            }

            Debug.Log("책 넘김 완료");

            SetFloorCameraTarget(targetObj.transform);

            targetObj.SetActive(true);

            transitionCam.Priority = DefaultPriority;
            floorCam.Priority = ActivePriority;

            playerObj.SetActive(true);

            _currentIdx = nextFloorIdx;

            yield return new WaitForSeconds(0.5f);

            Debug.Log("카메라 전환 완료");
        }
    }
}