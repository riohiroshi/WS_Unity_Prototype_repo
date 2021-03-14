namespace ProjectDynamax.GameLogic.FSM
{
    public interface IState
    {
        void Enter();
        void Execute();
        void Exit();
    }
}