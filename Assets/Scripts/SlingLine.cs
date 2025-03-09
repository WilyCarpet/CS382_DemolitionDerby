using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class SlingLine : MonoBehaviour
{
    public GameObject slingshot;   // The Slingshot Prefab (Parent)
    private LineRenderer _line;
    private GameObject leftAnchor;
    private GameObject rightAnchor;
    private Projectile _projectile;
    // Start is called before the first frame update
    void Start()
    {
        _line = GetComponent<LineRenderer>();
        _line.positionCount = 3; // Three points: Left, Projectile, Right

        _projectile = GetComponentInParent<Projectile>();

        // Find the anchor points inside the Slingshot prefab
        leftAnchor = slingshot.transform.Find("LeftArm").gameObject;
        rightAnchor = slingshot.transform.Find("RightArm").gameObject;
    }

    void Update(){
        if (_projectile == null) return;  // Prevent errors if projectile is missing

        // Update the line positions
        _line.SetPosition(0, leftAnchor.transform.position);
        _line.SetPosition(1, _projectile.transform.position);
        _line.SetPosition(2, rightAnchor.transform.position);
    }

    public void ReleaseProjectile()
    {
        // Optional: Hide the rubber band after shooting
        _line.enabled = false;
    }

}

