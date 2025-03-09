using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Slingshot : MonoBehaviour
{
    private static Slingshot S; //a private Sinleton
    //fields set in the Unity Inspector pane
    [Header("Inscribed")]
    public GameObject projectilePrefab;
    public float velocityMult = 10f;
    public GameObject projLinePrefab;
    public GameObject slingLinePrefab;
    public AudioSource slingAudioSource;
    public AudioClip slingSound;

    public Material powerMaterial;
    public Material projectileMaterial;
    //fields set dynamically
    [Header("Dynamic")]
    public GameObject launchPoint;
    public Vector3 launchPos;
    public GameObject projectile;
    public bool aimingMode;
    public bool powerUp;

    void Awake() {
        S = this;
        Transform launchPointTrans = transform.Find("LaunchPoint");
        launchPoint = launchPointTrans.gameObject;
        launchPoint.SetActive(false);
        launchPos = launchPointTrans.position;
    }
    void OnMouseEnter()
    {
        // print("Slingshot:OnMouseEnter()");
        launchPoint.SetActive(true);
    }

    void OnMouseExit(){
        // print("Slingout:OnMousExit()");
        launchPoint.SetActive(false);
    }

    void OnMouseDown() {
        //The player has pressed the mouse button while over Slingshot
        aimingMode = true;
        //Instanitiate a projectile
        projectile = Instantiate(projectilePrefab) as GameObject;
        //Start it at the launchPoint
        projectile.transform.position = launchPos;
        //Set it to isKinematic for now
        projectile.GetComponent<Rigidbody>().isKinematic = true;
        if(powerUp){
            projectile.GetComponent<Rigidbody>().mass = 15f;
            projectile.GetComponent<Renderer>().material = powerMaterial;
            powerUp = false;
        }else{
            projectile.GetComponent<Renderer>().material = projectileMaterial;
        }
        //Add sling line to projectile
        Instantiate<GameObject>(slingLinePrefab,projectile.transform);
    }

    void Update(){
        //If slingshot is not aimingMode, don't run this code
        if(!aimingMode) return;

        //Get the current mouse position in 2D screen coordinates
        Vector3 mousePos2D = Input.mousePosition;
        mousePos2D.z = -Camera.main.transform.position.z;
        Vector3 mousePos3D = Camera.main.ScreenToWorldPoint(mousePos2D);

        //Find the delta from launchPos to the mousePos3D
        Vector3 mouseDelta = mousePos3D - launchPos;
        //Limit mouseDelta to the radiys of the slingshot spherecollider
        float maxMagnitude = this.GetComponent<SphereCollider>().radius;
        if(mouseDelta.magnitude > maxMagnitude){
            mouseDelta.Normalize();
            mouseDelta *= maxMagnitude;
        }
        //Move the projectile to this new position
        Vector3 projPos = launchPos + mouseDelta;
        projectile.transform.position = projPos;
        if(Input.GetMouseButtonUp(0)){
            //The mouse has been released
            aimingMode = false;
            
            foreach (Transform child in projectile.transform){
                if (child.gameObject.CompareTag("SlingLine")) // Ensure it has the correct tag or name
                {
                    Destroy(child.gameObject);
                }
            }
            Rigidbody projRB = projectile.GetComponent<Rigidbody>();
            projRB.isKinematic = false;
            projRB.velocity = -mouseDelta * velocityMult;
            //Play audio
            slingAudioSource.PlayOneShot(slingSound);
            FollowCam.SWITCCH_VIEW(FollowCam.eView.slingshot);
            FollowCam.POI = projectile; //Set the _MainCamera POI
            //Add a projectileLine to the Projectile
            Instantiate<GameObject>(projLinePrefab,projectile.transform);
            projectile = null;
            MissionDemolition.SHOT_FIRED();


        }
    }

    static public void ADD_POWER_PROJECTILE(){
        S.powerUp = true;
    }


}
