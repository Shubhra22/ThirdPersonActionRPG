namespace JoystickLab
{
    public class States
    {
        public static StateMachine StateMachine = new StateMachine();
        public static readonly IState IdleState = new PlayerIdleState();
        public static readonly IState WalkState = new PlayerWalkState();
        public static readonly IState RunState = new PlayerRunState();
        public static readonly IState JumpState = new PlayerJumpState();
    }
}