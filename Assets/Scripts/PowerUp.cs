using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class PowerUp : MonoBehaviour
{
    void OnTriggerEnter(Collider other){

    Projectile proj = other.GetComponent<Projectile>();
    if (proj != null)
    {
        Slingshot.ADD_POWER_PROJECTILE();
        Destroy(gameObject);
    }
    }
}
