using UnityEngine;

namespace Character.Scripts.States
{
    public class JumpState : PlayerState
    {
        private static readonly int Jump = Animator.StringToHash("jump");

        public JumpState(PlayerScript player) : base(player)
        {
        }

        public override void Enter()
        {
            player.movementAditionAction += player.SetHorizontalMovement;
            player.animator.SetTrigger(Jump);
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
