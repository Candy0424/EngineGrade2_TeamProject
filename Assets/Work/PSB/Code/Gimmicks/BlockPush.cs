using System.Collections;
using Ami.BroAudio;
using Chuh007Lib.Dependencies;
using Chuh007Lib.ObjectPool.Runtime;
using UnityEngine;
using Work.CIW.Code.Grid;
using Work.CIW.Code.Player;
using Work.CUH.Chuh007Lib.EventBus;
using Work.CUH.Code.Commands;
using Work.CUH.Code.GameEvents;
using Work.CUH.Code.Test;
using Work.ISC.Code.Effects;

namespace Work.PSB.Code.Test
{
    public class BlockPush : GridObjectBase, ICommandable, IMoveableTest
    {
        [SerializeField] private MonoBehaviour gridServiceMono;
        [SerializeField] private float moveTime = 0.15f;
        [SerializeField] private bool canMoveBlock = true;

        [SerializeField] private PoolingItemSO pushEffect;

        [Header("Sound Setting")]
        [SerializeField] private SoundID pushSound;
        
        [Inject] private PoolManagerMono _poolManager;

        private IGridDataService _gridService;
        private bool _isMoving = false;

        public bool isMoving
        {
            get => _isMoving;
            set => _isMoving = value;
        }

        public override Vector3Int CurrentGridPosition { get; set; }
        public override void OnCellDeoccupied()
        {
        }

        public override void OnCellOccupied(Vector3Int newPos)
        {
        }

        private void Awake()
        {
            if (gridServiceMono is IGridDataService service)
                _gridService = service;
        }

        private void Start()
        {
            CurrentGridPosition = Vector3Int.RoundToInt(transform.position);
            transform.position = CurrentGridPosition;
            _gridService.SetObjectInitialPosition(this, CurrentGridPosition);
        }
        
        public void TryMoveByCommand(Vector3Int dir)
        {
            MoveCommand moveCommand = new MoveCommand(this, new Vector2(dir.x, dir.z));
            
            if (moveCommand.CanExecute())
            {
                Bus<CommandEvent>.Raise(new CommandEvent(moveCommand));
            }
        }

        public void HandleInput(Vector2 input)
        {
            if (_isMoving || !canMoveBlock)
                return;

            Vector3Int dir = new Vector3Int(Mathf.RoundToInt(input.x), 0, Mathf.RoundToInt(input.y));

            if (!CanMove(dir))
            {
                return;
            }

            StartCoroutine(MoveRoutine(dir));
            _gridService.UpdateObjectPosition(this, CurrentGridPosition, CurrentGridPosition + dir);

        }

        public bool CanMove(Vector3Int dir)
        {
            if (!canMoveBlock) return false;

            Vector3Int targetPos = CurrentGridPosition + dir;
            
            if (!_gridService.CanMoveTo(CurrentGridPosition, dir, out Vector3Int validatedPos))
            {
                Bus<CommandEvent>.Raise(new CommandEvent(new NothingCommand(this)));
                Bus<TurnUseEvent>.Raise(new TurnUseEvent());
                return false;
            }

            Collider[] hits = Physics.OverlapSphere((Vector3)targetPos, 0.45f);
            foreach (Collider hit in hits)
            {
                if (hit == null) continue;
                if (hit.GetComponent<BlockPush>() != null)
                {
                    Bus<CommandEvent>.Raise(new CommandEvent(new NothingCommand(this)));
                    Bus<TurnUseEvent>.Raise(new TurnUseEvent());
                    return false;
                }
                if (hit.CompareTag("Wall") || hit.CompareTag("Spike"))
                {
                    Bus<CommandEvent>.Raise(new CommandEvent(new NothingCommand(this)));
                    Bus<TurnUseEvent>.Raise(new TurnUseEvent());
                    return false;
                }
            }
            
            CreateEffect();
            return true;
        }

        public IEnumerator MoveRoutine(Vector3Int dir)
        {
            if (_isMoving) yield break;

            Vector3Int oldPos = CurrentGridPosition;
            Vector3Int targetPos = oldPos + dir;

            _isMoving = true;
            Vector3 start = transform.position;
            Vector3 end = (Vector3)targetPos;

            float elapsed = 0f;
            while (elapsed < moveTime)
            {
                transform.position = Vector3.Lerp(start, end, elapsed / moveTime);
                elapsed += Time.deltaTime;
                yield return null;
            }

            transform.position = end;
            CurrentGridPosition = targetPos;
            
            _isMoving = false;
        }

        public async void CreateEffect()
        {
            PoolingEffect effect = _poolManager.Pop<PoolingEffect>(pushEffect);
            Vector3 pos = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
            effect.PlayVFX(pos);
            BroAudio.Play(pushSound);
            await Awaitable.WaitForSecondsAsync(2f);
            _poolManager.Push(effect);
        }

        public GameObject GetObject() => gameObject;
        
    }
}
