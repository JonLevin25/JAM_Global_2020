using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Character.Scripts.States
{
    public class DieState : PlayerState
    {
        private static readonly int Dead = Animator.StringToHash("dead");

        public DieState(PlayerScript player) : base(player)
        {
        }

        public override void Enter()
        {
            // player.GetComponent<PlayerInput>().enabled = false;
            player.GetComponent<PlayerInput>().DeactivateInput();
            player.walk = 0;
            player.rigidbody.velocity = Vector2.zero;
            player.ladder = 0;
            
            player.animator.SetTrigger(Dead);
            player.animator.Play("Die");
            player.deathAudio.Play();

            player.flashLight.SetActive(false);
        }

        public override void ExecuteUpdate()
        {
        }

        public override void ExecuteFixedUpdate()
        {
        }

        public override void Exit()
        {
            // player.GetComponent<PlayerInput>().enabled = true; //Why we need this line???
        }
    }
}
