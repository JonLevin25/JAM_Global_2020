using System;
using Character.Scripts.States;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Character.Scripts
{
	[RequireComponent(typeof(Rigidbody2D))]
	[RequireComponent(typeof(Collider2D))]
	public class PlayerScript : MonoBehaviour
	{
		private enum State {grounded, ladder, jump}
		internal Rigidbody2D rigidbody;
		internal Collider2D collider;

		[SerializeField] private float horizontalSpeed = 1;
		[SerializeField] internal float ladderSpeed = 1;
		[SerializeField] internal float totalSpeedMultiplier = 1;
		[SerializeField] internal float jumpForce = 1;

		private State oldState;

		internal float walk, ladder;

		internal Collider2D currentLadder;

		internal StateMachine stateMachine;
		internal GroundState groundState;
		internal LadderState ladderState;
		internal JumpState jumpState;

		internal UnityAction movementAditionAction;

		private bool isGrounded;
		[SerializeField] private LayerMask whatIsGround;
		[SerializeField] private Transform legTransform;

		private void Awake()
		{
			rigidbody = GetComponent<Rigidbody2D>();
			collider = GetComponent<Collider2D>();
			
			stateMachine = ScriptableObject.CreateInstance<StateMachine>();
			groundState = new GroundState(this);
			ladderState = new LadderState(this);
			jumpState = new JumpState(this);
			
			stateMachine.ChangeState(groundState);
		}

		private void Update()
		{
			stateMachine.Update();
		}

		private void FixedUpdate()
		{
			GroundCheck();
			
			if (stateMachine.currentState != ladderState && currentLadder != null && ladder != 0)
			{
				stateMachine.ChangeState(ladderState);
			}
			else if (stateMachine.currentState != ladderState && isGrounded)
			{
				stateMachine.ChangeState(groundState);
			}
			
			
			stateMachine.FixedUpdate();
			
			
			Movement();
		}

		private void GroundCheck()
		{
			var hit = Physics2D.Raycast(legTransform.position, Vector2.down, 0.1f, whatIsGround);
			isGrounded = hit;
		}


		private void Movement()
		{
			movementAditionAction?.Invoke();
		}

		internal void SetHorizontalMovement()
		{
			Vector2 velocity = rigidbody.velocity;
			velocity.x = walk * horizontalSpeed * totalSpeedMultiplier;
			rigidbody.velocity = velocity;
		}

		public void OnWalk(InputValue value)
		{
			walk = value.Get<float>();
		}
		
		public void OnLadder(InputValue value)
		{
			ladder = value.Get<float>();
		}
		
		public void OnJump(InputValue value)
		{
			if (stateMachine.currentState != jumpState && isGrounded)
			{
				stateMachine.ChangeState(jumpState);
			}
		}

		private void OnTriggerEnter2D(Collider2D other)
		{
			if (other.CompareTag("Ladder"))
			{
				currentLadder = other;
			}
		}

		private void OnTriggerExit2D(Collider2D other)
		{
			if (other == currentLadder)
			{
				currentLadder = null;
			}
		}
	}
}
