using System;
using System.Collections.Generic;
using UnityEngine;
using Work.CIW.Code.Player;
using Work.PSB.Code.Test;

namespace Work.CIW.Code.Grid
{
    /// <summary>
    /// ���� ���� �����͸� �����ϴ� �߾� �ý���
    /// </summary>
    public class GridSystem : MonoBehaviour, IGridDataService
    {
        public static GridSystem Instance { get; private set; }

        [Header("Group Setup")]
        [SerializeField] GridCell cellPrefab;
        [SerializeField] private Vector3Int gridCenter = new Vector3Int(0, 0, 0);
        [SerializeField] Vector3Int gridSize = new Vector3Int(10, 10, 10);
        [SerializeField] List<Transform> gridParent; // ���� Cell���� ��ġ�� �θ� ������Ʈ
        [SerializeField] GameObject cellObjPrefab;
        [SerializeField] Vector3 gridStartWorldPosition = Vector3.zero;
        float _cellSize = 1f;

        [Header("Movement Check Data")]
        [SerializeField] LayerMask whatIsWalkable;

        [Header("Dependencies")]
        [SerializeField] MonoBehaviour turnServiceMono;
        ITurnService _turnService;

        Dictionary<Vector3Int, GridCell> _gridMap = new Dictionary<Vector3Int, GridCell>();

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(Instance);

            if (turnServiceMono is ITurnService service)
            {
                _turnService = service;
            }
            else
            {
                Debug.LogError("ITurnService dependency not met. Assign TurnSystemAdapter to turnServiceMono.");
            }

            InitializeCellSize();
            InitializeGrid();
        }

        /// <summary>
        /// Cell 프리팹의 크기를 기반으로 Grid의 셀 크기 계산
        /// </summary>
        private void InitializeCellSize()
        {
            if (cellObjPrefab == null)
            {
                Debug.LogError("Cell Prefab 빼먹었다 멍청한 놈아");
                return;
            }

            gridStartWorldPosition = Vector3.zero;
            //gridStartWorldPosition = new Vector3(_cellSize / 2f, 0f, _cellSize / 2f);
        }

        // �� �ʱ�ȭ -> �� gridCell���� 3���� ������ ��ġ
        private void InitializeGrid()
        {
            _gridMap = new Dictionary<Vector3Int, GridCell>();

            int startX = gridCenter.x - (gridSize.x / 2);
            int startZ = gridCenter.z - (gridSize.z / 2);

            foreach (Transform parent in gridParent)
            {
                float currentFloorY = parent.position.y;

                for (int x = 0; x < gridSize.x; x++)
                {
                    for (int z = 0; z < gridSize.z; z++)
                    {
                        Vector3Int pos = new Vector3Int(startX + x, Mathf.RoundToInt(currentFloorY), startZ + z);

                        float worldX = pos.x * _cellSize;
                        float worldY = currentFloorY + gridStartWorldPosition.y;
                        float worldZ = pos.z * _cellSize;

                        Vector3 worldPos = new Vector3(worldX, worldY, worldZ);

                        Quaternion rotation = Quaternion.Euler(-90f, 0f, 0f);

                        GameObject visualObj = Instantiate(cellObjPrefab, worldPos, rotation);
                        visualObj.transform.SetParent(parent, true);

                        GridCell newCell = visualObj.AddComponent<GridCell>();

                        newCell.InitializeCoodinates(pos);

                        if (!_gridMap.ContainsKey(pos))
                        {
                            _gridMap.Add(pos, newCell);
                        }
                        else
                        {
                            Debug.LogWarning($"Duplicate cell position found at {pos}. Skipping.");
                        }
                    }
                }
            }
            
        }

        #region I Grid Data Service ����

        public bool CanMoveTo(Vector3Int curPos, Vector3Int dir, out Vector3Int targetPos)
        {
            targetPos = curPos + dir;

            //Debug.Log($"[GRID CHECK] Requesting check from {curPos} in direction {dir}.");

            if (_turnService != null && !_turnService.HasTurnRemaining)
            {
                Debug.LogWarning("[GRID CHECK] FAILED (0): No turns remaining. Movement blocked.");
                return false;
            }

            if (!_gridMap.TryGetValue(targetPos, out GridCell targetCell))
            {
                Debug.LogWarning($"[GRID CHECK] FAILED (1): Target position {targetPos} is outside map boundaries.");
                return false;
            }

            // Grid Cell�� ���� �� �ְ�, ���� �װ��� ��ġ�ϰ� ���� �ʴ°�?
            if (!targetCell.IsWalkable)
            {
                Debug.LogWarning($"[GRID CHECK] FAILED (2): Cell at {targetPos} is not marked as walkable.");
                return false;
            }
            if (targetCell.IsOccupant)
            {
                GridObjectBase occupant = targetCell.Occupant;
                
                if (occupant is BlockPush block)
                {
                    if (block.CanMove(dir))
                    {
                        return false;
                    }
                }
                
                Debug.LogWarning($"[GRID CHECK] FAILED (3): Cell at {targetPos} is already occupied.");
                return false;
            }

            Vector3 rayOrigin = new Vector3(targetPos.x, targetPos.y + 0.5f, targetPos.z);
            Vector3 rayDir = Vector3.down;
            float maxDistance = targetPos.y + 6f;
            //Vector3 startPos = curPos;

            // ���� ĭ���� �̵����� �� ���� ��� ������ �ִ°�?
            if (Physics.Raycast(rayOrigin, rayDir, out RaycastHit hit, maxDistance, whatIsWalkable))
            {
                //Debug.Log($"[GRID CHECK] SUCCESS! Raycast hit: {hit.collider.gameObject.name}. Move is approved.");

                if (Mathf.Abs(hit.point.y - targetPos.y) < 0.1f)
                {
                    return true;
                }
                else
                {
                    Debug.LogWarning($"[GRID CHECK] FAILED (4a): Raycast hit an object at Y={hit.point.y}, but expected Y={targetPos.y}.");
                    return false;
                }
            }

            Debug.LogWarning($"[GRID CHECK] FAILED (4): Raycast failed to hit 'whatIsWalkable' ground at {targetPos}. Check LayerMask!");
            return false;
        }

        public void UpdateObjectPosition(GridObjectBase movingObj, Vector3Int oldPos, Vector3Int newPos)
        {
            if (_turnService != null)
            {
                // 턴 사용 로직 (주석 해제 시)
                // _turnService.UseTurn();
                //Debug.Log($"Turn Used. Current Turns Remaining: {_turnService.HasTurnRemaining}");
            }

            // 이전 셀 비우기
            if (_gridMap.TryGetValue(oldPos, out GridCell oldCell))
            {
                // 🌟 IGridObject 대신 GridObjectBase 사용
                if (oldCell.Occupant == movingObj)
                {
                    oldCell.SetOccupant(null); // GridCell.Occupant 및 SetOccupant가 GridObjectBase를 사용한다고 가정

                    // 🌟 GridObjectBase의 상태 갱신 로직 호출 (핵심 변경)
                    movingObj.OnCellDeoccupied();
                }
            }

            // 새 셀 점유
            if (_gridMap.TryGetValue(newPos, out GridCell newCell))
            {
                newCell.SetOccupant(movingObj); // GridCell.SetOccupant가 GridObjectBase를 사용한다고 가정

                // 🌟 GridObjectBase의 상태 갱신 로직 호출 (핵심 변경)
                movingObj.OnCellOccupied(newPos);
            }
            else
            {   
                Debug.LogError($"[GridSystem] Target position {newPos} is not a valid cell in the grid map!");
            }
        }

        public void SetObjectInitialPosition(GridObjectBase obj, Vector3Int initPos)
        {
            if (_gridMap.TryGetValue(initPos, out GridCell initCell))
            {
                if (!initCell.IsOccupant)
                {
                    initCell.SetOccupant(obj);

                    obj.OnCellOccupied(initPos);
                }
                else
                {
                    Debug.LogWarning($"Cell {initPos} already occupied at start. Cannot place {obj.gameObject.name}");
                }
            }
        }

        public void RemoveObjectPosition(GridObjectBase obj, Vector3Int targetPos)
        {
            if (_gridMap.TryGetValue(targetPos, out GridCell oldCell))
            {
                if (oldCell.Occupant != null)
                {
                    oldCell.SetOccupant(null); 
                    obj.OnCellDeoccupied();
                }
            }
        }
        
        // Grid Cell�� ��ǥ�� �ʱ�ȭ���ش�
        public GridCell GetCell(Vector3Int pos)
        {
            _gridMap.TryGetValue(pos, out GridCell cell);
            return cell;
        }

        #endregion
    }
}