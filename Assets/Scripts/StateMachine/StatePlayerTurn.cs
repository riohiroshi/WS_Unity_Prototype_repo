using UnityEngine;

namespace ProjectDynamax.GameLogic.FSM
{
    public class StatePlayerTurn : IState
    {
        private StatePlayerTurnInput _input;

        public StatePlayerTurn(StatePlayerTurnInput statePlayerTurnInput)
        {
            _input = statePlayerTurnInput;
        }

        public void Enter()
        {
            _input.BattleUIManager.EnableCurrentTurnSpawnButtons();
            _input.BattleManager.TryPlayAttackAnimations();
        }

        public void Execute()
        {
            if (!_input.BattleManager.HasFinishedPlayerTurn) { return; }

            _input.PlayerTurnResultCallback(new StatePlayerTurnOutput());
        }

        public void Exit() { }
    }

    public class StatePlayerTurnInput
    {
        public GameBattleManager BattleManager { get; private set; }
        public GameBattleUIManager BattleUIManager { get; private set; }
        public BattleDeck BattleDeck { get; private set; }
        public System.Action<StatePlayerTurnOutput> PlayerTurnResultCallback { get; private set; }

        public StatePlayerTurnInput(GameBattleManager battleManager, GameBattleUIManager battleUIManager, BattleDeck battleDeck, System.Action<StatePlayerTurnOutput> playerTurnResultCallback)
        {
            BattleManager = battleManager;
            BattleUIManager = battleUIManager;
            BattleDeck = battleDeck;
            PlayerTurnResultCallback = playerTurnResultCallback;
        }
    }

    public class StatePlayerTurnOutput { }
}