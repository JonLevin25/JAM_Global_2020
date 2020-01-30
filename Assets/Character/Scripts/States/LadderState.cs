using UnityEngine;

namespace Character.Scripts.States
{
    public class LadderState : PlayerState
    {

        private LadderScript ladder;
        public LadderState(PlayerScript player) : base(player)
        {
        }

        public override void Enter()
        {
            ladder = player.currentLadder.GetComponent<LadderScript>();
            
            player.movementAditionAction += player.SetHorizontalMovement;
            player.movementAditionAction += SetVerticalMovement;

            player.rigidbody.gravityScale = 0;

            foreach (var groundCollider in ladder.groundCollider2Ds)
            {
                Physics2D.IgnoreCollision(player.collider, groundCollider, true);
            }
            
        }

        public override void ExecuteUpdate()
        {
        }

        public override void ExecuteFixedUpdate()
        {
            if (player.currentLadder == null)
            {
                player.stateMachine.ChangeState(player.groundState);
            }
        }

        public override void Exit()
        {
            foreach (var groundCollider in ladder.groundCollider2Ds)
            {
                Physics2D.IgnoreCollision(player.collider, groundCollider, false);
            }
            
            player.rigidbody.gravityScale = 1;
            
            player.movementAditionAction -= SetVerticalMovement;
            player.movementAditionAction -= player.SetHorizontalMovement;
        }
        
        internal void SetVerticalMovement()
        {
            Vector2 velocity = player.rigidbody.velocity;
            velocity.y = player.ladder * player.ladderSpeed * player.totalSpeedMultiplier;
            player.rigidbody.velocity = velocity;
        }
    }
}
