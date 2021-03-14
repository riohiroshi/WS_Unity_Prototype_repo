using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GenericUtility.Singleton;

using ProjectDynamax.SceneSetup;

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
        private void Awake() { LockFrameRate(); /*InitializeMinions();*/ }
        private void OnEnable() { }
        private void Reset() { }
        private void Start() { }
        private void FixedUpdate() { }
        private void Update() { /*MinionAttackTest();*/ }
        private void LateUpdate() { }
        private void OnDrawGizmos() { }
        private void OnGUI() { DrawGUI(); }
        private void OnApplicationQuit() { }
        private void OnDisable() { }
        #endregion

        private void LockFrameRate()
        {
            QualitySettings.vSyncCount = 2;
            Application.targetFrameRate = 30;
        }

        //private void InitializeMinions()
        //{
        //    _minions = _minionParent.GetComponent<MinionParent>();

        //    for (int i = 0; i < _minionsAmount; i++)
        //    {
        //        if (_isControlledBySingleMinion)
        //        {
        //            Instantiate(_minionPrefab, _minionParent.position + Random.insideUnitSphere * 15, Quaternion.identity, _minionParent);
        //        }
        //        else
        //        {
        //            Instantiate(_minionPrefab2, _minionParent.position + Random.insideUnitSphere * 15, Quaternion.identity, _minionParent);
        //        }
        //    }
        //}

        //private void MinionAttackTest()
        //{
        //    if (!Input.GetMouseButtonDown(0)) { return; }
        //    if (_attackCoroutine != null) { StopCoroutine(_attackCoroutine); }

        //    _attackCoroutine = StartCoroutine(_isControlledBySingleMinion ? Minion.MinionsAttack() : _minions.MinionsAttack());
        //}

        private void DrawGUI()
        {
            GUI.matrix = Matrix4x4.Rotate(Quaternion.Euler(0, 0, 0));

            var xOffset = 10f;
            var yOffset = 10f;
            var ySpacing = 50f;
            var rectWidth = 200f;
            var rectHeight = 40f;

            var rect = new Rect(xOffset, yOffset, rectWidth, rectHeight);

            _deltaTime += (Time.unscaledDeltaTime - _deltaTime) * 0.1f;
            GUI.Box(rect, $"Current FPS: {(1.0f / _deltaTime):N2}", _style);

            yOffset += ySpacing;

            rect = new Rect(xOffset, yOffset, rectWidth, rectHeight);

            GUI.Box(rect, $"Current Remain Minion: {Minion.CurrentMinionIndex}", _style);

            yOffset += ySpacing;

            rect = new Rect(xOffset, yOffset, rectWidth, rectHeight);

            GUI.Box(rect, $"Current AB Text Mode: {_currentABTextMode}", _style);
        }
    }
}