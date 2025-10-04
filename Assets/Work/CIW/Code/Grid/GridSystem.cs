using System;
using System.Collections.Generic;
using UnityEngine;
using Work.CIW.Code.Player;

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
        [SerializeField] Vector3Int gridSize = new Vector3Int(10, 5, 10);
        [SerializeField] Transform gridParent; // ���� Cell���� ��ġ�� �θ� ������Ʈ

        [Header("Movement Check Data")]
        [SerializeField] LayerMask whatIsWalkable;
        [SerializeField] float raycastDistance = 1.0f; // �̵��ϴ� �Ÿ��� ��ġ�ؾ���

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

            InitializeGrid();
        }

        // �� �ʱ�ȭ -> �� gridCell���� 3���� ������ ��ġ
        private void InitializeGrid()
        {
            int startX = gridCenter.x + -(gridSize.x / 2);
            int startY = gridCenter.y + -(gridSize.y / 2);
            int startZ = gridCenter.z + -(gridSize.z / 2);

            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    for (int z = 0; z < gridSize.z; z++)
                    {
                        Vector3Int pos = new Vector3Int(startX + x, startY + y, startZ + z);
                        Vector3 worldPos = new Vector3(pos.x, pos.y, pos.z);

                        // �̰� ���߿� ������Ʈ Ǯ������ �ٲ��ٰ���
                        GridCell newCell = Instantiate(cellPrefab, worldPos, Quaternion.identity, gridParent);

                        newCell.InitializeCoodinates(pos);
                        _gridMap.Add(pos, newCell);
                    }
                }
            }

            //Debug.Log($"Grid Initialized: {_gridMap.Count} cells created.");
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
                Debug.LogWarning($"[GRID CHECK] FAILED (3): Cell at {targetPos} is already occupied.");
                return false;
            }

            Vector3 startPos = curPos;

            // ���� ĭ���� �̵����� �� ���� ��� ������ �ִ°�?
            if (Physics.Raycast(startPos, dir, out RaycastHit hit, raycastDistance, whatIsWalkable))
            {
                //Debug.Log($"[GRID CHECK] SUCCESS! Raycast hit: {hit.collider.gameObject.name}. Move is approved.");
                return true;
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

        // Grid Cell�� ��ǥ�� �ʱ�ȭ���ش�
        public GridCell GetCell(Vector3Int pos)
        {
            _gridMap.TryGetValue(pos, out GridCell cell);
            return cell;
        }

        #endregion
    }
}