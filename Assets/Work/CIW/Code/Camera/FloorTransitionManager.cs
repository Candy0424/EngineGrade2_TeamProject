using Chuh007Lib.Dependencies;
using echo17.EndlessBook.Demo01;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using Work.CIW.Code.Camera.Events;
using Work.CUH.Chuh007Lib.EventBus;
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
        [SerializeField] private GameObject bookObj;
        public bool IsBookTurned => _isBookTurned;
        int _currentIdx = 0;

        [Header("Transition settings")]
        [SerializeField] float moveDuration = 1.5f;
        [SerializeField] float camHeightForFloorView = 10f;

        [SerializeField] Demo01 demo01;
        bool _isBookTurned = false;

        const int ActivePriority = 11;
        const int DefaultPriority = 9;

        private void Awake()
        {
            Bus<FloorEvent>.OnEvent += HandleFloorChange;
        }

        private void OnDestroy()
        {
            Bus<FloorEvent>.OnEvent -= HandleFloorChange;
        }

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

        // 층 이동
        private void HandleFloorChange(FloorEvent evt)
        {
            Debug.Log("층 바꾸기 이벤트 받음");

            //int nextIdx = evt.TargetIdx;
            int dir = evt.Direction;

            Debug.Log("Transition Sequence 코루틴 시작");
            StartCoroutine(TransitionSequence(dir));
        }

        private void SetFloorCameraTarget(Transform targetTrm)
        {
            Vector3 newCamPos = new Vector3(targetTrm.position.x, targetTrm.position.y + camHeightForFloorView, targetTrm.position.z);

            floorCam.transform.position = newCamPos;
            floorCam.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        }

        public void SetBookState(int stateIdx)
        {
            if (demo01 != null)
            {
                try
                {
                    foreach (GameObject floors in floorObjs)
                    {
                        floors.SetActive(false);
                    }
                    playerObj.SetActive(false);

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


        public IEnumerator TransitionSequence(int direction)
        {
            Debug.Log("Sequence로 들어옴");

            GameObject curObj = floorObjs[_currentIdx];
            GameObject targetObj = floorObjs[_currentIdx + direction];

            playerObj.SetActive(false);
            curObj.SetActive(false);

            floorCam.Priority = DefaultPriority;
            transitionCam.Priority = ActivePriority;

            Debug.Log("Transition Camera 전환 시작");

            bool canSuc = false;
            //int direction = nextFloorIdx > _currentIdx ? 1 : -1;

            if(demo01 != null)
            {
                try
                {
                    demo01.GetType().GetMethod("OnTurnButtonClicked").Invoke(demo01, new object[] { direction });

                    canSuc = true;
                    _isBookTurned = false;
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
                while (!_isBookTurned)
                {
                    yield return null;
                }
            }
            else
            {
                yield return new WaitForSeconds(moveDuration);
            }

            Debug.Log("책 넘김 완료");

            float offsetY = direction > 0 ? 15f : -15f;
            bookObj.transform.position += new Vector3(0f, offsetY, 0f);

            SetFloorCameraTarget(targetObj.transform);

            targetObj.SetActive(true);

            transitionCam.Priority = DefaultPriority;
            floorCam.Priority = ActivePriority;

            playerObj.SetActive(true);

            _currentIdx = _currentIdx + direction;

            yield return new WaitForSeconds(0.5f);

            Debug.Log("카메라 전환 완료");
        }

        public void HandleBookTurnCompleted()
        {
            _isBookTurned = true;
        }
    }
}