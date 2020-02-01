using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroySelf : MonoBehaviour
{
    public float SelfDestructIn = 1.5f;
    // Start is called before the first frame update
    void Start()
    {
        Invoke("SelfDestruct", SelfDestructIn);
    }

    void SelfDestruct()
    {
        Destroy(this.gameObject);
    }

}
