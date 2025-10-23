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
        public bool IsBookTurned
        {
            get => _isBookTurned;
            set => _isBookTurned = value;
        }

        bool _isBookTurned = false;
        bool _turnCompleted = false;
        int _currentIdx = 0;

        [Header("Transition settings")]
        [SerializeField] float moveDuration = 1.5f;
        [SerializeField] float camHeightForFloorView = 10f;

        [SerializeField] Demo01 demo01;

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

        public void StartEvent()
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

            if (IsBookTurned) return;

            IsBookTurned = true;

            int dir = evt.Direction;

            int nextIdx = _currentIdx + dir;
            if (nextIdx < 0 || nextIdx >= floorObjs.Count)
            {
                if (nextIdx < 0)
                {
                    Debug.LogWarning($"Undo로 인한 유효하지 않은 층 인덱스({nextIdx}) 요청을 무시합니다. (Undo는 이미 실행됨)");
                }
                else
                {
                    Debug.LogError($"다음 층 인덱스({nextIdx})가 유효 범위를 벗어났습니다 (0 ~ {floorObjs.Count - 1}). 층 이동을 취소합니다.");
                }

                IsBookTurned = false;
                return;
            }

            Debug.Log("잘 살아남았구나! 실행해도 좋다!");
            StartCoroutine(TransitionSequence(dir)); // undo 여기서 에러
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
            Debug.Log($"Sequence로 들어옴 : {IsBookTurned}");

            _turnCompleted = false;
            yield return null;

            GameObject curObj = floorObjs[_currentIdx];
            GameObject targetObj = floorObjs[_currentIdx + direction];

            playerObj.SetActive(false);
            curObj.SetActive(false);

            floorCam.Priority = DefaultPriority;
            transitionCam.Priority = ActivePriority;

            Debug.Log($"Transition Camera 전환 시작 : {IsBookTurned}");

            bool canSuc = false;

            if(demo01 != null)
            {
                try
                {
                    demo01.GetType().GetMethod("OnTurnButtonClicked").Invoke(demo01, new object[] { direction });

                    canSuc = true;
                }
                catch (Exception e)
                {
                    Debug.LogError($"OnTurnButtonClicked 호출 실패: {e.Message}. MoveDuration을 사용합니다.");
                    canSuc = false;
                    IsBookTurned = true;
                }
            }
            else
            {
                Debug.LogWarning("Book Controller가 연결되지 않아 moveDuration을 사용합니다.");
                canSuc = false;
            }

            if (canSuc)
            {
                while (!_turnCompleted)
                {
                    yield return null;
                }
            }
            else
            {
                yield return new WaitForSeconds(moveDuration);
            }

            Debug.Log($"책 넘김 완료 : {IsBookTurned}");

            float offsetY = direction > 0 ? 15f : -15f;
            bookObj.transform.position += new Vector3(0f, offsetY, 0f);

            SetFloorCameraTarget(targetObj.transform);

            targetObj.SetActive(true);

            IsBookTurned = false;

            transitionCam.Priority = DefaultPriority;
            floorCam.Priority = ActivePriority;

            playerObj.SetActive(true);

            _currentIdx = _currentIdx + direction;

            yield return new WaitForSeconds(0.5f);

            Debug.Log($"카메라 전환 완료 : {IsBookTurned}");
        }

        public void HandleBookTurnCompleted()
        {
            _turnCompleted = true;
        }
    }
}