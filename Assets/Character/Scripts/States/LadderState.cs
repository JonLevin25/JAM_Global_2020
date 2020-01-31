using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Character.Scripts.States
{
    public class LadderState : PlayerState
    {
        private LadderScript ladder;
        private static readonly int Climb = Animator.StringToHash("climb");

        private IEnumerable<Collider2D> GroundColliders 
            => ladder.groundCollider2Ds.Where(col => col != null);

        public LadderState(PlayerScript player) : base(player)
        {
        }

        public override void Enter()
        {
            ladder = player.currentLadder.GetComponent<LadderScript>();
            
            player.movementAditionAction += player.SetHorizontalMovement;
            player.movementAditionAction += SetVerticalMovement;

            player.rigidbody.gravityScale = 0;
            
            foreach (var groundCollider in GroundColliders)
            {
                Physics2D.IgnoreCollision(player.collider, groundCollider, true);
            }
            
            player.animator.SetTrigger(Climb);
        }

        public override void ExecuteUpdate()
        {
        }

        public override void ExecuteFixedUpdate()
        {
            if (player.currentLadder == null)
            {
                if (player.isGrounded)
                {
                    player.stateMachine.ChangeState(player.groundState);
                }
                else
                {
                    player.stateMachine.ChangeState(player.jumpState);
                }
            }
        }

        public override void Exit()
        {
            foreach (var groundCollider in GroundColliders)
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
