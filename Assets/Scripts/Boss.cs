using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

namespace ProjectDynamax.GameLogic
{
    public class Boss : MonoBehaviour
    {
        public bool HasFinishedAttack { get; private set; }

        private Vector3 _currentPosition;

        #region Unity_Lifecycle
        private void Awake() { }
        private void OnEnable() { }
        private void Start() { _currentPosition = transform.position; }
        private void FixedUpdate() { }
        private void Update() { }
        private void LateUpdate() { }
        private void OnDisable() { }
        private void OnDestroy() { }
        #endregion

        public void TakeHit(Vector3 hitDirection)
        {
            transform.DOMove(_currentPosition + hitDirection.normalized * 0.5f, 0.1f)
                .OnComplete(() => transform.DOMove(_currentPosition, 0.25f));

            //DOTween.Sequence()
            //    .AppendCallback(() =>
            //    {
            //        transform.DOMove(_currentPosition + hitDirection.normalized, 0.15f);
            //    })
            //    .AppendInterval(0.15f)
            //    .AppendCallback(() =>
            //    {
            //        transform.DOMove(_currentPosition, 0.1f);
            //    });
        }

        public void Attack()
        {
            HasFinishedAttack = false;

            transform.DOMove(_currentPosition + transform.forward * (-3f), 0.25f)
                .OnComplete(() =>
                {
                    transform.DOMove(_currentPosition, 0.5f);
                    HasFinishedAttack = true;
                });
        }
    }
}