using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAnimator : MonoBehaviour
{

    public Animator animator;
    public AIController aiController;

    //animation weights
    public float rockAnim;
    public float paperAnim;
    public float scissorAnim;
    public float wiggleFingersAnim;
    public float confusePose1;
    public float confusePose2;
    public float playRPSAnim;

    // Start is called before the first frame update
    void Start()
    {
    }

    public void doReset()
    {
        animator.SetBool("doRock", false);
        animator.SetBool("doPaper", false);
        animator.SetBool("doScissor", false);
        animator.SetBool("gamereset", true);
        
    }

    public void doFinalHand()
    {
        if (aiController.finalPose.Contains("rock"))
        {
            animator.SetBool("doRock", true);
            animator.Play("doRock");
        }
        else if (aiController.finalPose.Contains("paper"))
        {
            animator.SetBool("doPaper", true);
            animator.Play("doPaper");
        }
        else if (aiController.finalPose.Contains("scissor"))
        {
            animator.SetBool("doScissor", true);
            animator.Play("doScissor");
        }
    }

    public void doIdle()
    {
        animator.SetBool("gamestart", false);
        animator.SetBool("gamereset", false);
        animator.SetFloat("rock", 0);
        animator.SetFloat("paper", 0);
        animator.SetFloat("scissor", 0);
        animator.SetFloat("wiggleFingers", 0);
        animator.SetFloat("confusePose1", 0);
        animator.SetFloat("confusePose2", 0);
        animator.SetFloat("rockpaperscissor", 0);
        animator.Play("Idle");
    }

    public void doRPS()
    {
        float w0 = 0.0f;
        float w1 = 0.0f;
        float w2 = 0.0f;
        float w3 = 0.0f;
        float w4 = 0.0f;
        float w5 = 0.0f;
        float w6 = 0.0f;

        w6 = 1;

        //get winning pose
        aiController.SetAIFinalPose();

        //set weights for AI animation blending 
        //based on number of times AI lost. For
        //now assume 5 is last and craziest level.
        //set weights using random nums. Random
        //weights will get larger with each case.
        switch (aiController.losses)
        {
            case 0:
                if (aiController.finalPose.Contains("rock"))
                    w0 = 1.0f;
                if (aiController.finalPose.Contains("paper"))
                    w1 = 1.0f;
                if (aiController.finalPose.Contains("scissor"))
                    w2 = 1.0f;
                Debug.Log("poop");
                break;
            case 1:
                if (aiController.finalPose.Contains("rock"))
                {
                    w0 = 0.6f;
                    w1 = 0.2f;
                    w2 = 0.2f;
                }
                if (aiController.finalPose.Contains("paper"))
                {
                    w1 = 0.6f;
                    w0 = 0.2f;
                    w2 = 0.2f;
                }
                if (aiController.finalPose.Contains("scissor"))
                {
                    w2 = 0.6f;
                    w0 = 0.2f;
                    w1 = 0.2f;

                }
                break;
            case 2:
                if (aiController.finalPose.Contains("rock"))
                {
                    w0 = 0.6f;
                    w1 = 0.2f;
                    w2 = 0.2f;
                }
                if (aiController.finalPose.Contains("paper"))
                {
                    w1 = 0.6f;
                    w0 = 0.2f;
                    w2 = 0.2f;
                }
                if (aiController.finalPose.Contains("scissor"))
                {
                    w2 = 0.6f;
                    w0 = 0.2f;
                    w1 = 0.2f;

                }
                w3 = 0.2f;
                break;
            case 3:
                if (aiController.finalPose.Contains("rock"))
                {
                    w0 = 0.3f;
                    w1 = 0.2f;
                    w2 = 0.2f;
                }
                if (aiController.finalPose.Contains("paper"))
                {
                    w1 = 0.3f;
                    w0 = 0.2f;
                    w2 = 0.2f;
                }
                if (aiController.finalPose.Contains("scissor"))
                {
                    w2 = 0.3f;
                    w0 = 0.2f;
                    w1 = 0.2f;

                }
                w3 = 0.4f;
                break;
            case 4:
                w3 = 0.4f;
                w4 = 0.3f;
                w5 = 0.3f;
                break;
            default:
                break;
        }
        animator.SetBool("gamestart", true);
        animator.SetFloat("rock", w0);
        animator.SetFloat("paper", w1);
        animator.SetFloat("scissor", w2);
        animator.SetFloat("wiggleFingers", w3);
        animator.SetFloat("confusePose1", w4);
        animator.SetFloat("confusePose2", w5);
        animator.SetFloat("rockpaperscissor", w6);
        animator.Play("Blend");
    }


    // Update is called once per frame
    void Update()
    {

    }
}
