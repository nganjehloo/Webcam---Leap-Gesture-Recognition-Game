using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    public int wins;
    public int losses;
    public string finalPose;
    // Start is called before the first frame update
    void Start()
    {
        wins = 0;
	    losses = 0;
        finalPose = "rock";
    }

    public void SetAIFinalPose()
    {
        int finalPoseidx = Random.Range(0, 3);
        switch (finalPoseidx)
        {
            case 0:
                finalPose = "rock";
                break;
            case 1:
                finalPose = "paper";
                break;
            case 2:
                finalPose = "scissor";
                break;
            default:
                finalPose = "rock";
                break;
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
