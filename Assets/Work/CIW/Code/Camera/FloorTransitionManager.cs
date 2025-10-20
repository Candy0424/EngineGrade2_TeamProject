using echo17.EndlessBook.Demo01;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

namespace Work.CIW.Code.Camera
{
    public class FloorTransitionManager : MonoBehaviour
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
        [SerializeField] float moveCamLerpSpeed = 5f;
        [SerializeField] float camHeightForFloorView = 10f;

        [SerializeField] Demo01 demo01;
        bool _isBookTurnCompleted = false;

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
                StartCoroutine(TransitionSequence(nextIdx));
            }
            else
            {
                Debug.LogWarning("더 이상 이동할 층이 없습니다.");
            }
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

        private IEnumerator TransitionSequence(int nextFloorIdx)
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

            //Vector3 targetPos = targetObj.transform.position;

            //float elapsedTime = 0f;
            //while (elapsedTime < moveDuration)
            //{
            //    floorCam.transform.position = Vector3.Lerp(floorCam.transform.position, targetPos, Time.deltaTime * moveCamLerpSpeed);

            //    yield return null;
            //    elapsedTime += Time.deltaTime;
            //}

            //floorCam.transform.position = targetPos;
            //Debug.Log("책 넘김 및 FloorCam 위치 업데이트 완료");

            //curObj.SetActive(false);
            //targetObj.SetActive(true);

            //transitionCam.Priority = DefaultPriority;
            //floorCam.Priority = ActivePriority;

            //playerObj.SetActive(true);

            //_currentIdx = nextFloorIdx;

            //yield return new WaitForSeconds(0.5f);

            //Debug.Log("카메라 전환 완료");
        }
    }
}