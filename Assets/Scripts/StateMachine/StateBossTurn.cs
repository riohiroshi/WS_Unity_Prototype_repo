using UnityEngine;

namespace ProjectDynamax.GameLogic.FSM
{
    public class StateBossTurn : IState
    {
        private StateBossTurnInput _input;

        public StateBossTurn(StateBossTurnInput stateBossTurnInput)
        {
            _input = stateBossTurnInput;
        }

        public void Enter()
        {
            _input.CurrentBoss.Attack();
        }

        public void Execute()
        {
            if (!_input.CurrentBoss.HasFinishedAttack) { return; }

            _input.BossTurnResultCallback(new StateBossTurnOutput());
        }

        public void Exit() { }
    }

    public class StateBossTurnInput
    {
        public Boss CurrentBoss { get; private set; }
        public System.Action<StateBossTurnOutput> BossTurnResultCallback { get; private set; }

        public StateBossTurnInput(Boss currentBoss, System.Action<StateBossTurnOutput> bossTurnResultCallback)
        {
            CurrentBoss = currentBoss;
            BossTurnResultCallback = bossTurnResultCallback;
        }
    }

    public class StateBossTurnOutput { }
}