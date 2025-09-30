using System;
using System.Collections.Generic;
using UnityEngine;
using Work.CIW.Code.Player;

namespace Work.CIW.Code.Grid
{
    /// <summary>
    /// 격자 맵의 데이터를 관리하는 중앙 시스템
    /// </summary>
    public class GridSystem : MonoBehaviour, IGridDataService
    {
        public static GridSystem Instance { get; private set; }

        [Header("Group Setup")]
        [SerializeField] GridCell cellPrefab;
        [SerializeField] Vector3Int gridSize = new Vector3Int(10, 5, 10);
        [SerializeField] Transform gridParent; // 격자 Cell들을 배치할 부모 오브젝트

        [Header("Movement Check Data")]
        [SerializeField] LayerMask whatIsWalkable;
        [SerializeField] float raycastDistance = 1.0f; // 이동하는 거리와 일치해야해

        Dictionary<Vector3Int, GridCell> _gridMap = new Dictionary<Vector3Int, GridCell>();

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(Instance);

            InitializeGrid();
        }

        // 맵 초기화 -> 빈 gridCell들을 3차원 공간에 배치
        private void InitializeGrid()
        {
            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    for (int z = 0; z < gridSize.z; z++)
                    {
                        Vector3Int pos = new Vector3Int(x, y, z);
                        // 이거 나중에 오브젝트 풀링으로 바꿔줄거임
                        GridCell newCell = Instantiate(cellPrefab, pos, Quaternion.identity, gridParent);

                        newCell.InitializeCoodinates(pos);
                        _gridMap.Add(pos, newCell);
                    }
                }
            }

            Debug.Log($"Grid Initialized: {_gridMap.Count} cells created.");
        }

        #region I Grid Data Service 구현

        public bool CanMoveTo(Vector3Int curPos, Vector3Int dir, out Vector3Int targetPos)
        {
            targetPos = curPos + dir;

            Debug.Log($"[GRID CHECK] Requesting check from {curPos} in direction {dir}.");

            if (!_gridMap.TryGetValue(targetPos, out GridCell targetCell))
            {
                Debug.LogWarning($"[GRID CHECK] FAILED (1): Target position {targetPos} is outside map boundaries.");
                return false;
            }

            // Grid Cell을 걸을 수 있고, 누가 그곳에 위치하고 있지 않는가?
            if (!targetCell.IsWalkable)
            {
                Debug.LogWarning($"[GRID CHECK] FAILED (2): Cell at {targetPos} is not marked as walkable.");
                return false;
            }
            if (targetCell.IsOccupant)
            {
                Debug.LogWarning($"[GRID CHECK] FAILED (3): Cell at {targetPos} is already occupied.");
                return false;
            }

            Vector3 startPos = curPos;

            // 다음 칸으로 이동했을 때 발을 디딜 지형이 있는가?
            if (Physics.Raycast(startPos, dir, out RaycastHit hit, raycastDistance, whatIsWalkable))
            {
                Debug.Log($"[GRID CHECK] SUCCESS! Raycast hit: {hit.collider.gameObject.name}. Move is approved.");
                return true;
            }

            Debug.LogWarning($"[GRID CHECK] FAILED (4): Raycast failed to hit 'whatIsWalkable' ground at {targetPos}. Check LayerMask!");
            return false;
        }

        public void UpdateObjectPosition(IGridObject movingObj, Vector3Int oldPos, Vector3Int newPos)
        {
            // 이전 칸 비워주기
            if (_gridMap.TryGetValue(oldPos, out GridCell oldCell))
            {
                if (oldCell.Occupant == movingObj)
                {
                    oldCell.SetOccupant(null);
                }
            }

            if (_gridMap.TryGetValue(newPos, out GridCell newCell))
            {
                newCell.SetOccupant(movingObj);
            }
            else
            {
                Debug.LogError($"[GridSystem] Target position {newPos} is not a valid cell in the grid map!");
            }
        }

        public void SetObjectInitialPosition(IGridObject obj, Vector3Int initPos)
        {
            if (_gridMap.TryGetValue(initPos, out GridCell initCell))
            {
                if (!initCell.IsOccupant)
                {
                    initCell.SetOccupant(obj);
                }
                else
                {
                    Debug.LogWarning($"Cell {initPos} already occupied at start. Cannot place {obj.GetObject().name}");
                }
            }
        }

        // Grid Cell의 좌표를 초기화해준다
        public GridCell GetCell(Vector3Int pos)
        {
            _gridMap.TryGetValue(pos, out GridCell cell);
            return cell;
        }

        #endregion
    }
}