using System;
using System.Collections;
using Character.Scripts.States;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Random = System.Random;

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

		internal float lastTimeJumpState;
		public AudioSource walkAudio;
		public AudioSource jumpAudio;
		public AudioSource ladderAudio;
		public AudioSource deathAudio;
		public AudioSource effort1Audio;
		public AudioSource effort2Audio;

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

		private void Start()
		{
			StartCoroutine(EffortSounds());
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
			
			if (stateMachine.currentState != ladderState 
				&& stateMachine.currentState != dieState 
				&& currentLadder != null 
				&& ladder != 0)
			{
				stateMachine.ChangeState(ladderState);
			}
			else if (stateMachine.currentState != ladderState 
					 && stateMachine.currentState != groundState 
					 && stateMachine.currentState != dieState 
					 && isGrounded && Time.time - lastTimeJumpState > 0.25f)
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
			if (stateMachine.currentState != jumpState 
				&& stateMachine.currentState != dieState 
				&& isGrounded)
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
					corkTransform.rotation = Quaternion.identity; // Reset rotation
					Physics2D.IgnoreCollision(collider, currentCork.hardCollider);
					currentCork.rigidbody.bodyType = RigidbodyType2D.Kinematic;
					currentCork.rigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
					corkTransform.localPosition = Vector3.zero;
					currentCork.player = this;
				}
			}

			if (other.CompareTag("Water"))
			{
				inEffort = true;
			}
		}

		private void OnTriggerExit2D(Collider2D other)
		{
			if (other == currentLadder)
			{
				currentLadder = null;
			}
			if (other.CompareTag("Water"))
			{
				inEffort = false;
			}
		}

		public void Die()
		{
			stateMachine.ChangeState(dieState);
		}

		public void LadderSoundPlay()
		{
			ladderAudio.PlayOneShot(ladderAudio.clip);
		}

		internal bool inEffort;
		public IEnumerator EffortSounds()
		{
			while (true)
			{
				yield return new WaitUntil(() => inEffort);

				AudioSource effortAudio = UnityEngine.Random.Range(0, 2) == 0 ? effort1Audio : effort2Audio;
				effortAudio.Play();
				
				yield return new WaitForSeconds(effortAudio.clip.length);
			}
		}
	}
}
