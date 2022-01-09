using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;
using UnityEngine.Events;

public class FeedbackManaganger : MonoBehaviour
{
    //public MMFeedbacks onJump;
    //public MMFeedbacks onFall;

    public ParticleSystem part;
    
    public MMFeedbacks[] Feedbacks;

    private void OnEnable()
    {
        Player.OnJump += OnJump;
        Player.OnFall += OnFall;
    }

    private void OnDisable()
    {
        Player.OnJump -= OnJump;
        Player.OnFall -= OnFall;
    }

    public void PlayFeedback(string name)
    {
        foreach (MMFeedbacks f in Feedbacks)
        {
            if(name == f.name)
            {

                f.PlayFeedbacks();
            }
        }
       
    }

    //Just for Reference DELETE
    /*public void PlayFeedback(string name, ParticleSystem part, Transform t)
    {
        foreach (MMFeedbacks f in Feedbacks)
        {
            if (name == f.name)
            {   

                ((MMFeedbackParticles)f.Feedbacks[0]).BoundParticleSystem = part;
                ((MMFeedbackScale)f.Feedbacks[1]).AnimateScaleTarget = t;
                f.PlayFeedbacks();
            }
        }

    }*/

    public void PlayFeedback(string name, ParticleSystem part, Transform t, GameObject root)
    {
        foreach (MMFeedbacks f in Feedbacks)
        {
            if (name == f.name)
            {
                switch (name)
                {

                    default:
                        ((MMFeedbackParticles)f.Feedbacks[0]).BoundParticleSystem = part;
                        ((MMFeedbackScale)f.Feedbacks[1]).AnimateScaleTarget = t;
                        ((MMFeedbackRotation)f.Feedbacks[2]).AnimateRotationTarget = t;
                        ((MMFeedbackPosition)f.Feedbacks[3]).AnimatePositionTarget = root;
                        Debug.Log(t.gameObject);
                        break;
                }

                f.PlayFeedbacks();
                return;
            }
        }

    }

    public void OnJump()
    {
        //((MMFeedbackParticles)onJump.Feedbacks[2]).BoundParticleSystem = part;
        //onJump?.PlayFeedbacks();
    }

    public void OnFall()
    {
        //onFall?.PlayFeedbacks();
    }


}
