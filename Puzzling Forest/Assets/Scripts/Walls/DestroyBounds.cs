using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This just handles deleting blocks after a certain depth (wherever collider is placed)
///  so that blocks do not infinitely fall.
/// </summary>
public class DestroyBounds : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        Destroy(other.gameObject);
    }
}
