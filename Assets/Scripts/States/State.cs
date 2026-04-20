public abstract class State
{
    public bool canTransition = true;
    public bool canReEnter = false;
    
    public State()
    { }

    public abstract void Enter();
    public abstract void Update();
    public abstract void Exit();
}