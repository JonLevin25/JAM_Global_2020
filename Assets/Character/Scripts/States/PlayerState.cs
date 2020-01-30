namespace Character.Scripts.States
{
	public abstract class PlayerState : IState
	{
		protected PlayerScript player;

		public PlayerState(PlayerScript player)
		{
			this.player = player;
		}

		public abstract void Enter();
		public abstract void ExecuteUpdate();
		public abstract void ExecuteFixedUpdate();
		public abstract void Exit();
	}
}
