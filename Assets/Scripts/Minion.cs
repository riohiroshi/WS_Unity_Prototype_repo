using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

namespace ProjectDynamax.GameLogic
{
    public class Minion : MonoBehaviour
    {
        public static int CurrentMinionIndex { get; private set; } = 0;
        private static Dictionary<GameObject, Minion> _minionsDictionary = new Dictionary<GameObject, Minion>();

        public static List<KeyValuePair<GameObject, Minion>> GetCurrentAllMinions() => _minionsDictionary.ToList();

        public static IEnumerator MinionsAttack(List<KeyValuePair<GameObject, Minion>> minionsList)
        {
            CurrentMinionIndex = 0;

            foreach (var minion in minionsList)
            {
                yield return minion.Value.Attack();

                //yield return new WaitForSeconds(0.25f);
                CurrentMinionIndex++;
            }
        }

        [SerializeField] private LayerMask _bossLayer = default;

        [SerializeField] private GameObject _model = default;
        [SerializeField] private Transform _minionParent = default;

        [Range(2, 8)]
        [SerializeField] private int _manaCost = 2;

        [Min(1)]
        [SerializeField] private int _minionAmount = 1;

        [SerializeField] private Color _minionColor = default;

        public int ManaCost { get => _manaCost; }

        public Color MinionColor { get => _minionColor; }

        private Boss _boss;

        #region Unity_Lifecycle
        private void Awake() { }
        private void OnEnable() { }
        private void Start() { }
        private void FixedUpdate() { }
        private void Update() { }
        private void LateUpdate() { }
        private void OnDisable() { }
        private void OnDestroy() { }
        #endregion

        public void InitializeMinion(Boss currentBoss)
        {
            _minionsDictionary.Add(gameObject, this);

            _boss = currentBoss;

            SpawnMinions();
        }

        private void SpawnMinions()
        {
            _model.GetComponentInChildren<Renderer>()?.material.SetColor("_Color", _minionColor);

            for (int i = 0; i < _minionAmount - 1; i++)
            {
                var targetPos = _minionParent.position + Random.insideUnitSphere * 1.5f;

                var minion = Instantiate(_model, targetPos, Quaternion.identity, _minionParent);

                minion.GetComponentInChildren<Renderer>()?.material.SetColor("_Color", _minionColor);
            }
        }

        private IEnumerator Attack()
        {
            for (int i = 0; i < _minionParent.childCount; i++)
            {
                var singleMinion = _minionParent.GetChild(i);

                var currentPosition = singleMinion.position;
                var attackDirection = _boss.transform.position - currentPosition;

                var renderer = singleMinion.GetComponentInChildren<Renderer>();
                var currentColor = renderer.material.color;
                renderer.material.DOColor(Color.red, 0.05f);

                var targetDirection = _boss.transform.position - currentPosition;
                var maxDistance = Vector3.Distance(currentPosition, _boss.transform.position);

                Physics.Raycast(currentPosition, targetDirection, out RaycastHit hitInfo, maxDistance, _bossLayer);

                singleMinion.DOMove(hitInfo.point, 0.1f)
                    .OnComplete(() =>
                    {
                        _boss.TakeHit(attackDirection);
                        singleMinion.DOMove(currentPosition, 0.25f);
                        renderer.material.DOColor(currentColor, 0.5f);
                    });

                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}