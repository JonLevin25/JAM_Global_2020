using System;
using Character.Scripts;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Cork : MonoBehaviour
{
    [SerializeField] internal Collider2D hardCollider;

    internal PlayerScript player;
    internal Rigidbody2D rigidbody;
    [SerializeField] bool PlaySoundOnlyOnce = false;
    [SerializeField] GameObject CollisionSoundParticle;

    bool CanPlaySound = true;
    AudioSource _audio;
    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        _audio = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Pipe"))
        {
            var pipe = other.GetComponent<Pipe>();
            if (pipe.IsLeaking)
            {
                pipe.FixPipe();
                player = null;
                Destroy(gameObject);
            }
        }
        else if (other.CompareTag("Player"))
            _audio.Play();

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Ground"))
        {

            if (CanPlaySound && CollisionSoundParticle)
            {
                Instantiate(CollisionSoundParticle, transform.position, Quaternion.identity);

            }
            if (PlaySoundOnlyOnce)
                CanPlaySound = false;
        }
    }

}