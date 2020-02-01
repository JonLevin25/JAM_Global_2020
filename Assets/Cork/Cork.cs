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
    [SerializeField] AudioClip CollisionSound;
    [SerializeField] AudioClip PickUpSound;

    bool CanPlaySound = true;
    AudioSource _audio;
    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        _audio = gameObject.AddComponent<AudioSource>();
        _audio.loop = _audio.playOnAwake = false;
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
            PlaySound(PickUpSound);


    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Ground"))
        {
            if (CanPlaySound)
                PlaySound(CollisionSound);
            if (PlaySoundOnlyOnce)
                CanPlaySound = false;
        }
    }

    void PlaySound(AudioClip clip)
    {
        if (!clip) return;
        _audio.clip = clip;
        _audio.Play();
    }
}