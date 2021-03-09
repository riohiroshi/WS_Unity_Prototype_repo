using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectDynamax.Gravity
{
    [RequireComponent(typeof(Rigidbody))]
    public class GravityBody : MonoBehaviour
    {
        [SerializeField] private GravityAttractor _planet = default;

        public Vector3 CurrentPosition { get; private set; }

        private Rigidbody _rigidbody;

        #region Unity_Lifecycle
        private void Awake() { }
        private void OnEnable() { }
        private void Start()
        {
            InitializeBody();
        }
        private void FixedUpdate()
        {
            GravityAttracting();
        }
        private void Update() { }
        private void LateUpdate() { }
        private void OnDisable() { }
        private void OnDestroy() { }
        #endregion

        private void InitializeBody()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.useGravity = false;
            _rigidbody.constraints = RigidbodyConstraints.FreezeRotation;

            if (!_planet) { _planet = GameLogic.GamePlayManager.Instance.CurrentPlanet; }
        }

        private void GravityAttracting()
        {
            if (_planet is null) { return; }

            if (_rigidbody.IsSleeping())
            {
                CurrentPosition = transform.position;
                _rigidbody.isKinematic = true;
                return;
            }

            _planet.Attract(_rigidbody);
        }
    }
}