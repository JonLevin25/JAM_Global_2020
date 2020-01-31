using System;
using Character.Scripts.States;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Character.Scripts
{
	[RequireComponent(typeof(Rigidbody2D))]
	[RequireComponent(typeof(Collider2D))]
	[RequireComponent(typeof(Animator))]
	public class PlayerScript : MonoBehaviour
	{
		private enum State {grounded, ladder, jump}
		internal Rigidbody2D rigidbody;
		internal Collider2D collider;
		internal Animator animator;

		[SerializeField] private float horizontalSpeed = 1;
		[SerializeField] internal float ladderSpeed = 1;
		[SerializeField] internal float totalSpeedMultiplier = 1;
		[SerializeField] internal float jumpForce = 1;

		private State oldState;

		internal float walk, ladder;

		internal Collider2D currentLadder;
		
		internal Cork currentCork;
		[SerializeField] private Transform corkPivot;

		internal StateMachine stateMachine;
		internal GroundState groundState;
		internal LadderState ladderState;
		internal JumpState jumpState;
		internal DieState dieState;

		internal UnityAction movementAditionAction;

		internal bool isGrounded;
		[SerializeField] private LayerMask whatIsGround;
		[SerializeField] private Transform legTransform;

		[SerializeField] internal GameObject flashLight;

		private void Awake()
		{
			rigidbody = GetComponent<Rigidbody2D>();
			collider = GetComponent<Collider2D>();
			animator = GetComponent<Animator>();
			
			stateMachine = ScriptableObject.CreateInstance<StateMachine>();
			groundState = new GroundState(this);
			ladderState = new LadderState(this);
			jumpState = new JumpState(this);
			dieState = new DieState(this);
			
			stateMachine.ChangeState(groundState);
		}

		private void Update()
		{
			stateMachine.Update();

			SetDirection();
			
			animator.SetFloat("absoluteHorizontalVelocity", Mathf.Abs(walk));
		}

		private void FixedUpdate()
		{
			GroundCheck();
			
			if (stateMachine.currentState != ladderState && currentLadder != null && ladder != 0)
			{
				stateMachine.ChangeState(ladderState);
			}
			else if (stateMachine.currentState != groundState && isGrounded)
			{
				stateMachine.ChangeState(groundState);
			}
			
			
			stateMachine.FixedUpdate();
			
			
			Movement();
		}

		private void SetDirection()
		{
			float x = walk;
			if (x == 0)
			{
				return;
			}
			bool isRight = x > 0;
			Vector3 temp = transform.localScale;
			temp.x = isRight? Mathf.Abs(temp.x) : -Mathf.Abs(temp.x);
			transform.localScale = temp;
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
			// rigidbody.AddForce(Vector2.right * (walk * horizontalSpeed * totalSpeedMultiplier));
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
				rigidbody.AddForce(Vector2.up * jumpForce);
				stateMachine.ChangeState(jumpState);
			}
		}

		private void OnTriggerEnter2D(Collider2D other)
		{
			if (other.CompareTag("Ladder"))
			{
				currentLadder = other;
			}
			if (currentCork == null && other.CompareTag("Cork"))
			{
				//take cork
				currentCork = other.GetComponent<Cork>();
				if (currentCork.player == null)
				{
					Transform corkTransform;
					(corkTransform = currentCork.transform).SetParent(corkPivot);
					corkTransform.parent = corkPivot;
					Physics2D.IgnoreCollision(collider, currentCork.hardCollider);
					currentCork.rigidbody.bodyType = RigidbodyType2D.Kinematic;
					corkTransform.localPosition = Vector3.zero;
					currentCork.player = this;
				}
			}
		}

		private void OnTriggerExit2D(Collider2D other)
		{
			if (other == currentLadder)
			{
				currentLadder = null;
			}
		}

		public void Die()
		{
			stateMachine.ChangeState(dieState);
		}
	}
}
