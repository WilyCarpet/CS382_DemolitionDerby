using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

public enum GameMode{
    idle,
    playing,
    levelEnd
}
public class MissionDemolition : MonoBehaviour
{
    static private MissionDemolition S; //a private Sinleton
    [Header("Inscribed")]
    public TextMeshProUGUI uitLevel; // The UIText_Level Text
    public TextMeshProUGUI uitShots; // The UIText_Shots text
    public TextMeshProUGUI uiGameOver; // The UITEXT_GameOver text
    public Button uiRestart; // the UIButton_Restart
    public Vector3 castlePos; //The place to put the castle
    public GameObject[] castles; //An array of the castles

    [Header("Dynamic")]
    public int level; //The surrent level
    public int levelmax; //The number of levels
    public int shotsTaken;
    public GameObject castle; //The current castle
    public GameMode mode = GameMode.idle;
    public string showing = "Show Slingshot"; //FollowCam mode
    // Start is called before the first frame update
    void Start()
    {
        S = this; //Define the Singleton

        level = 0;
        shotsTaken = 0;
        levelmax = castles.Length;
        uiGameOver.enabled = false;
        uiRestart.gameObject.SetActive(false); // Hide the button
        StartLevel();

    }

    void StartLevel(){
        //Get rid of the old castle if one exists
        if(castle != null){
            Destroy(castle);
        }

        //Destroy old projectiles if they exist
        Projectile.DESTROY_PROJECTILES();

        //Instansiate the new castle
        castle = Instantiate<GameObject>(castles[level]);
        castle.transform.position = castlePos;

        //Rest the goal
        Goal.goalMet = false;

        UpdateGUI();

        mode = GameMode.playing;

        //Zoom out to show both
        FollowCam.SWITCCH_VIEW(FollowCam.eView.both);
    }

    void UpdateGUI(){
        //Show the data in the GUITexts
        uitLevel.text = "Level: "+(level+1)+" of "+levelmax;
        uitShots.text = "Shots Taken: "+shotsTaken;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateGUI();

        //Check for level end
        if((mode == GameMode.playing) && Goal.goalMet){
            //Change mdoe to stop checking for level end
            mode = GameMode.levelEnd;
            //Zoom out to show both
            FollowCam.SWITCCH_VIEW(FollowCam.eView.both);
            //Start the next level in 2 seconds
            Invoke("NextLevel", 2f);
        }
    }

    void NextLevel(){
        level++;
        if(level == levelmax){
            uiGameOver.enabled = true;
            uiRestart.gameObject.SetActive(true); 
        }else{
            StartLevel();
        }
    }

    public void Restart(){
        level = 0;
        shotsTaken = 0;
        uiGameOver.enabled = false;
        uiRestart.gameObject.SetActive(false); // Hide the button
        StartLevel();
    }

    //Static method that allows code anywhere to increment shotstaken
    static public void SHOT_FIRED(){
        S.shotsTaken++;
    }

    //Static method that allows code anywhere to get the reference to S.castle
    static public GameObject GET_CASTLE(){
        return S.castle;
    }
}
