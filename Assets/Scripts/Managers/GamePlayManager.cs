using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using GenericUtility.Singleton;

using ProjectDynamax.Gravity;

namespace ProjectDynamax.GameLogic
{
    public class GamePlayManager : Singleton<GamePlayManager>
    {
        [SerializeField] private Minion _minionPrefab = default;
        [SerializeField] private GameObject _minionPrefab2 = default;
        [SerializeField] private Transform _minionParent = default;

        [SerializeField] private Boss _currentBoss = default;
        [SerializeField] private GravityAttractor _currentPlanet = default;

        [SerializeField] private int _minionsAmount = 100;

        [SerializeField] private GUIStyle _style = default;

        [SerializeField] private bool _isControlledBySingleMinion = true;

        [SerializeField] private string _currentABTextMode = default;

        public Boss CurrentBoss { get => _currentBoss; }
        public GravityAttractor CurrentPlanet { get => _currentPlanet; }


        private Coroutine _attackCoroutine;

        private float _deltaTime = 0.0f;

        private MinionParent _minions;

        #region Unity lifecycle
        private void Awake() { /*LockFrameRate();*/ InitializeMinions(); }
        private void OnEnable() { }
        private void Reset() { }
        private void Start() { }
        private void FixedUpdate() { }
        private void Update() { _deltaTime += (Time.unscaledDeltaTime - _deltaTime) * 0.1f; MinionAttackTest(); }
        private void LateUpdate() { }
        private void OnDrawGizmos() { }
        private void OnGUI() { DrawGUI(); }
        private void OnApplicationQuit() { }
        private void OnDisable() { }
        #endregion

        private void LockFrameRate()
        {
            Application.targetFrameRate = 30;
            QualitySettings.vSyncCount = 1;
        }

        private void InitializeMinions()
        {
            _minions = _minionParent.GetComponent<MinionParent>();

            for (int i = 0; i < _minionsAmount; i++)
            {
                if (_isControlledBySingleMinion)
                {
                    Instantiate(_minionPrefab, _minionParent.position + Random.insideUnitSphere * 15, Quaternion.identity, _minionParent);
                }
                else
                {
                    Instantiate(_minionPrefab2, _minionParent.position + Random.insideUnitSphere * 15, Quaternion.identity, _minionParent);
                }
            }
        }

        private void MinionAttackTest()
        {
            if (!Input.GetMouseButtonDown(0)) { return; }
            if (_attackCoroutine != null) { StopCoroutine(_attackCoroutine); }

            _attackCoroutine = StartCoroutine(_isControlledBySingleMinion ? Minion.MinionsAttack() : _minions.MinionsAttack());
        }

        private void DrawGUI()
        {
            GUI.matrix = Matrix4x4.Rotate(Quaternion.Euler(0, 0, 0));

            var rect = new Rect(10, 10, 200, 40);

            GUI.Box(rect, $"Current FPS: {(1.0f / _deltaTime):N2}", _style);

            rect = new Rect(10, 60, 200, 40);

            var currentIndex = _isControlledBySingleMinion ? Minion.CurrentMinionIndex : _minions.CurrentMinionIndex;
            GUI.Box(rect, $"Current Remain Minion: {_minionsAmount - currentIndex}", _style);

            rect = new Rect(10, 110, 200, 40);

            GUI.Box(rect, $"Current AB Text Mode: {_currentABTextMode}", _style);
        }
    }
}