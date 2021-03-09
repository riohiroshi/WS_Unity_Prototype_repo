using UnityEngine;

namespace ProjectDynamax.Gravity
{
    public class GravityAttractor : MonoBehaviour
    {
        [SerializeField] private float _gravity = -9.8f;

        public void Attract(Rigidbody body)
        {
            var gravityUp = (body.position - transform.position).normalized;
            var localUp = body.transform.up;

            body.AddForce(gravityUp * _gravity * body.mass);
            body.rotation = Quaternion.FromToRotation(localUp, gravityUp) * body.rotation;
        }
    }
}