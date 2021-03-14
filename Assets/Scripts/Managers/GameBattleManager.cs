using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ProjectDynamax.GameLogic.FSM;

namespace ProjectDynamax.GameLogic
{
    public class GameBattleManager : MonoBehaviour
    {
        [SerializeField] private List<Minion> _minionPrefabsList = default;

        [SerializeField] private Transform _minionsParent = default;

        [SerializeField] private Boss _currentBoss = default;
        public bool HasFinishedPlayerTurn { get; private set; }

        private GameBattleUIManager _battleUIManager;

        private BattleDeck _battleDeck;

        private Queue<IEnumerator> _attackQueue = new Queue<IEnumerator>();

        private int _currentBattleMana;
        private int _maxBattleMana = 8;

        private BattleFieldPlacer _battleFieldPlacer;

        private StatePlayerTurnInput _statePlayerTurnInput;
        private StateBossTurnInput _stateBossTurnInput;

        private StateMachine _stateMachine = new StateMachine();


        #region Unity_Lifecycle
        private void Awake() { }
        private void OnEnable() { }
        private void Start() { InitializeBattleManager(); }
        private void FixedUpdate() { }
        private void Update() { _stateMachine.ExecuteStateUpdate(); }
        private void LateUpdate() { }
        private void OnDisable() { }
        private void OnDestroy() { }
        #endregion


        public void OnPressSpawnMinionButton(int index)
        {
            var minion = _battleDeck.GetCurrentTurnMinionByIndex(index);

            if (!TryCostMana(minion.ManaCost)) { return; }

            SpawnMinions(minion);

            EnqueueAttackAnimations();
        }

        public void TryPlayAttackAnimations() => StartCoroutine(PlayAttackAnimations());

        public void ResetCurrentBattleMana()
        {
            _currentBattleMana = _maxBattleMana;
            _battleUIManager.UpdateManaBar((float)_currentBattleMana / _maxBattleMana);
        }

        private void InitializeBattleManager()
        {
            InitializeBattleDeck();

            _battleUIManager = GetComponent<GameBattleUIManager>();
            _battleUIManager.InitializeBattleUI(_battleDeck.GetMinionsColor(), _battleDeck.GetCurrentMinionsCost());

            _battleFieldPlacer = GetComponent<BattleFieldPlacer>();

            ResetCurrentBattleMana();

            _statePlayerTurnInput = new StatePlayerTurnInput(this, _battleUIManager, _battleDeck, PlayerTurnResultCallback);
            _stateBossTurnInput = new StateBossTurnInput(_currentBoss, BossTurnResultCallback);

            _stateMachine.ChangeState(new StatePlayerTurn(_statePlayerTurnInput));
        }

        private void InitializeBattleDeck()
        {
            _battleDeck = new BattleDeck(_minionPrefabsList);
        }

        private bool TryCostMana(int cost)
        {
            if (_currentBattleMana < cost) { return false; }

            _currentBattleMana -= cost;
            _battleUIManager.UpdateManaBar((float)_currentBattleMana / _maxBattleMana);
            return true;
        }

        private bool HasEnoughMana()
        {
            if (_currentBattleMana >= _battleDeck.GetCurrentTurnMinionByIndex(0).ManaCost) { return true; }
            if (_currentBattleMana >= _battleDeck.GetCurrentTurnMinionByIndex(1).ManaCost) { return true; }
            if (_currentBattleMana >= _battleDeck.GetCurrentTurnMinionByIndex(2).ManaCost) { return true; }

            return false;
        }

        private void SpawnMinions(Minion minion)
        {
            var newMinion = Instantiate(minion, _battleFieldPlacer.OccupyNextAvailablePosition(), Quaternion.identity, _minionsParent);
            newMinion.InitializeMinion(_currentBoss);
        }

        private void EnqueueAttackAnimations()
        {
            _attackQueue.Enqueue(Minion.MinionsAttack(Minion.GetCurrentAllMinions()));
        }

        private IEnumerator PlayAttackAnimations()
        {
            HasFinishedPlayerTurn = false;

            while (_attackQueue.Count > 0 || HasEnoughMana())
            {
                if (_attackQueue.Count > 0) { yield return StartCoroutine(_attackQueue.Dequeue()); }

                yield return null;
            }

            HasFinishedPlayerTurn = true;
        }

        private void PlayerTurnResultCallback(StatePlayerTurnOutput playerTurnResult)
        {
            _battleUIManager.DisableCurrentTurnSpawnButtons();
            _battleDeck.MoveToNextTurn();
            _battleUIManager.UpdateButtonsColors(_battleDeck.GetMinionsColor());
            _battleUIManager.UpdateButtonsCost(_battleDeck.GetCurrentMinionsCost());

            _stateMachine.ChangeState(new StateBossTurn(_stateBossTurnInput));
        }

        private void BossTurnResultCallback(StateBossTurnOutput bossTurnResult)
        {
            ResetCurrentBattleMana();
            _stateMachine.ChangeState(new StatePlayerTurn(_statePlayerTurnInput));
        }
    }
}