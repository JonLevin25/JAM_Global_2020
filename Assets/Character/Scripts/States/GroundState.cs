namespace Character.Scripts.States
{
	public class GroundState : PlayerState
	{
		public GroundState(PlayerScript player) : base(player)
		{
		}

		public override void Enter()
		{
			player.movementAditionAction += player.SetHorizontalMovement;
		}

		public override void ExecuteUpdate()
		{
		}

		public override void ExecuteFixedUpdate()
		{
		}

		public override void Exit()
		{
			player.movementAditionAction -= player.SetHorizontalMovement;
		}
	}
}
