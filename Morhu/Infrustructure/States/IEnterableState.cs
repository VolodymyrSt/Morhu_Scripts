namespace Morhu.Infrustructure.States
{
    public interface IEnterableState
    {
        void Enter();
    }

    public interface IExitableState : IEnterableState
    {
        void Exit();
    }
    
    public interface ITickableState : IExitableState
    {
        void Tick();
    }
}
