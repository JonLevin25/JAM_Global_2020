using UnityEngine;

namespace Character.Scripts.States
{
	public class GroundState : PlayerState
	{
		private static readonly int Ground = Animator.StringToHash("ground");

		public GroundState(PlayerScript player) : base(player)
		{
		}

		public override void Enter()
		{
			player.movementAditionAction += player.SetHorizontalMovement;
			
			player.animator.SetTrigger(Ground);
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
