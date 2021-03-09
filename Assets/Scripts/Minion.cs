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

        [SerializeField] private Boss _boss = default;

        private Vector3 _currentPosition;

        private Gravity.GravityBody _gravityBody;

        #region Unity_Lifecycle
        private void Awake() { }
        private void OnEnable() { }
        private void Start() { InitializeMinion(); }
        private void FixedUpdate() { }
        private void Update() { }
        private void LateUpdate() { }
        private void OnDisable() { }
        private void OnDestroy() { }
        #endregion

        private void InitializeMinion()
        {
            _minionsDictionary.Add(gameObject, this);

            _currentPosition = transform.position;

            _gravityBody = GetComponent<Gravity.GravityBody>();

            _boss = GamePlayManager.Instance.CurrentBoss;
        }

        public static IEnumerator MinionsAttack()
        {
            CurrentMinionIndex = 0;
            foreach (var minion in _minionsDictionary)
            {
                minion.Value.Attack();
                CurrentMinionIndex++;

                yield return new WaitForSeconds(0.05f);
            }
        }

        private void Attack()
        {
            _currentPosition = transform.position;
            var attackDirection = _boss.transform.position - _currentPosition;

            DOTween.Sequence()
                .AppendCallback(() =>
                {
                    //GetComponent<Rigidbody>().isKinematic = true;
                    transform.DOMove(_boss.transform.position, 0.1f);
                })
                .AppendInterval(0.1f)
                .AppendCallback(() =>
                {
                    _boss.TakeHit(attackDirection);
                    transform.DOMove(_currentPosition, 0.1f);
                })
                .AppendInterval(0.1f)
                /*.AppendCallback(() => GetComponent<Rigidbody>().isKinematic = false)*/;
        }
    }
}