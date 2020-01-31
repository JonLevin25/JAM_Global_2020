using System;
using Character.Scripts;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Cork : MonoBehaviour
{
    [SerializeField] internal Collider2D hardCollider;
    internal PlayerScript player;

    internal Rigidbody2D rigidbody;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
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
    }
}