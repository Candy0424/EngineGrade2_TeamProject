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
        [SerializeField] Vector3Int gridSize = new Vector3Int(10, 5, 10);
        [SerializeField] Transform gridParent; // ���� Cell���� ��ġ�� �θ� ������Ʈ

        [Header("Movement Check Data")]
        [SerializeField] LayerMask whatIsWalkable;
        [SerializeField] float raycastDistance = 1.0f; // �̵��ϴ� �Ÿ��� ��ġ�ؾ���

        Dictionary<Vector3Int, GridCell> _gridMap = new Dictionary<Vector3Int, GridCell>();

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(Instance);

            InitializeGrid();
        }

        // �� �ʱ�ȭ -> �� gridCell���� 3���� ������ ��ġ
        private void InitializeGrid()
        {
            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    for (int z = 0; z < gridSize.z; z++)
                    {
                        Vector3Int pos = new Vector3Int(x, y, z);
                        // �̰� ���߿� ������Ʈ Ǯ������ �ٲ��ٰ���
                        GridCell newCell = Instantiate(cellPrefab, pos, Quaternion.identity, gridParent);

                        newCell.InitializeCoodinates(pos);
                        _gridMap.Add(pos, newCell);
                    }
                }
            }

            Debug.Log($"Grid Initialized: {_gridMap.Count} cells created.");
        }

        #region I Grid Data Service ����

        public bool CanMoveTo(Vector3Int curPos, Vector3Int dir, out Vector3Int targetPos)
        {
            targetPos = curPos + dir;

            Debug.Log($"[GRID CHECK] Requesting check from {curPos} in direction {dir}.");

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
                Debug.Log($"[GRID CHECK] SUCCESS! Raycast hit: {hit.collider.gameObject.name}. Move is approved.");
                return true;
            }

            Debug.LogWarning($"[GRID CHECK] FAILED (4): Raycast failed to hit 'whatIsWalkable' ground at {targetPos}. Check LayerMask!");
            return false;
        }

        public void UpdateObjectPosition(IGridObject movingObj, Vector3Int oldPos, Vector3Int newPos)
        {
            // ���� ĭ ����ֱ�
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

        // Grid Cell�� ��ǥ�� �ʱ�ȭ���ش�
        public GridCell GetCell(Vector3Int pos)
        {
            _gridMap.TryGetValue(pos, out GridCell cell);
            return cell;
        }

        #endregion
    }
}