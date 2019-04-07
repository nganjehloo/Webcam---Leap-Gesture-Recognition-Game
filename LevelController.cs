using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Leap.Unity;


public class LevelController : MonoBehaviour
{

    public WebCamTexture realCam;
	public GameObject player;
	public GameObject AI;

    public Text Wins;
    public Text Losses;
    public Text Gamestate;
    public Text AIPose;
    public Text PPose;


    public string aiPose;
    public string playerPose;

    public RawImage camViewTexture;
    Texture2D postProcess;
    public float rpsTime = 3.0f;
    public float resetTime = 2.0f;
    public bool gameStart = false;
    public bool gameReset = false;
    public bool useRealCam = false;

    string pose = "";
    //This is called everytime the detector identifys R P or S.
    //What ever this is at the end of the game would be the final pose.
    public void updatePlayerCurrentPose(){
        if (useRealCam)
            pose = player.GetComponent<PlayerController>().finalPose;
        else
            pose = player.GetComponent<ExtendedFingerDetector>().currentPose;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (realCam == null)
        {
            realCam = new WebCamTexture(640, 480);
            camViewTexture.texture = realCam;
        }
    }

    // Update is called once per frame
    void Update()
    {
       
        if (useRealCam) {
           // Destroy();
           player.GetComponent<PlayerController>().proessHand(realCam, ref postProcess);
            camViewTexture.texture = postProcess;
        }

        if (Input.GetKey(KeyCode.C))
        {
            if (!realCam.isPlaying)
            {
                useRealCam = true;
                realCam.Play();
            }
            else
            {
                useRealCam = false;
                realCam.Stop();
            }
        }

        //round not started
        if (!(gameReset || gameStart))
        {
            AI.GetComponentInChildren<AIAnimator>().doIdle();
        }

        //Start a round 
        if (Input.GetKey(KeyCode.Space) && !gameReset && !gameStart)
        {
            //A.I animates through R P S
            AI.GetComponentInChildren<AIAnimator>().doRPS();
            gameStart = true;
            Gamestate.text = "Game is: Started";
        }

        if (gameStart)
        {
            rpsTime -= Time.deltaTime;
            if (rpsTime <= 0)
            {
                gameReset = true;
                gameStart = false;
                rpsTime = 3.0f;

                //AI Animates final pose
                AI.GetComponentInChildren<AIAnimator>().doFinalHand();

                //At end of interval
                //Get Player pose
                updatePlayerCurrentPose();
               

            }
        }

        if (gameReset) {
            resetTime -= Time.deltaTime;
            if (resetTime <= 0)
            {
                //Compare Player Pose vs A.I Pose
                 aiPose = AI.GetComponent<AIController>().finalPose;
                 playerPose = pose;

                Debug.Log(aiPose);
                Debug.Log(playerPose);

                AIPose.text = aiPose;
                PPose.text = playerPose;
                //string playerPose = player.GetComponent<PlayerController>().finalPose;
                int result = 0; //0 - tie, 1 - win, 2- lose

                //Determine winner
                if (aiPose.Contains("rock") && playerPose.Contains("rock"))
                    result = 0;

                if (aiPose.Contains("rock") && playerPose.Contains("paper"))
                    result = 2;

                if (aiPose.Contains("rock") && playerPose.Contains("scissor"))
                    result = 1;

                if (aiPose.Contains("paper") && playerPose.Contains("rock"))
                    result = 1;

                if (aiPose.Contains("paper") && playerPose.Contains("paper"))
                    result = 0;

                if (aiPose.Contains("paper") && playerPose.Contains("scissor"))
                    result = 2;

                if (aiPose.Contains("scissor") && playerPose.Contains("rock"))
                    result = 2;

                if (aiPose.Contains("scissor") && playerPose.Contains("paper"))
                    result = 1;

                if (aiPose.Contains("scissor") && playerPose.Contains("scissor"))
                    result = 0;


                Debug.Log("result" + result);

                //Update Difficulty
                if (result == 1)
                    AI.GetComponent<AIController>().wins += 1;
                if (result == 2)
                    AI.GetComponent<AIController>().losses += 1;

                Wins.text = "Wins: " + AI.GetComponent<AIController>().losses.ToString();
                Losses.text = "Losses: " + AI.GetComponent<AIController>().wins.ToString();

                //resetLevel
                resetTime = 2.0f;
                AI.GetComponentInChildren<AIAnimator>().doReset();
                gameReset = false;

                Gamestate.text = "Game is: Ended";
            }
        }
    }
}
