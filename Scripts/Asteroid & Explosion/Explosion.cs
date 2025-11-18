using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    // Destroys the explosion object after 3 seconds
    void Start()
    {
        Destroy(this.gameObject, 3f);
    }
}
