using System;
using Character.Scripts;
using UnityEngine;

public class Cork : MonoBehaviour
{
    internal PlayerScript player;
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