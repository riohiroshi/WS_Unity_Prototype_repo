using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

namespace ProjectDynamax.GameLogic
{
    public class MinionParent : MonoBehaviour
    {
        public int CurrentMinionIndex { get; private set; } = 0;

        [SerializeField] private Boss _boss = default;

        public IEnumerator MinionsAttack()
        {
            CurrentMinionIndex = 0;
            for (int i = 0; i < transform.childCount; i++)
            {
                var minion = transform.GetChild(i);

                Attack(minion);
                CurrentMinionIndex++;

                yield return new WaitForSeconds(0.05f);
            }
        }

        private void Attack(Transform minionTransform)
        {
            var currentPosition = minionTransform.position;
            var attackDirection = _boss.transform.position - minionTransform.position;

            DOTween.Sequence()
                .AppendCallback(() =>
                {
                    //GetComponent<Rigidbody>().isKinematic = true;
                    minionTransform.DOMove(_boss.transform.position, 0.1f);
                })
                .AppendInterval(0.1f)
                .AppendCallback(() =>
                {
                    _boss.TakeHit(attackDirection);
                    minionTransform.DOMove(currentPosition, 0.1f);
                })
                .AppendInterval(0.1f)
                /*.AppendCallback(() => GetComponent<Rigidbody>().isKinematic = false)*/;
        }
    }
}