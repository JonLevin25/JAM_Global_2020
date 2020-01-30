using UnityEngine;

namespace Character.Scripts.States
{
    public class JumpState : PlayerState
    {
        public JumpState(PlayerScript player) : base(player)
        {
        }

        public override void Enter()
        {
            player.movementAditionAction += player.SetHorizontalMovement;
            
            player.rigidbody.AddForce(Vector2.up * player.jumpForce);
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
