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

    public void PlayFeedback(string name, ParticleSystem part, Vector3 t, GameObject root)  //"Feedback Name", particle System, Vector 3 = root.transform.scale, root target for scaling
    {
        foreach (MMFeedbacks f in Feedbacks)
        {
            if (name == f.name)
            {
                switch (name)
                {
                    
                    default:
                        ((MMFeedbackParticles)f.Feedbacks[0]).BoundParticleSystem = part;
                        ((MMFeedbackScale)f.Feedbacks[1]).AnimateScaleTarget = root.transform;   // root.GetComponent<Transform>(); // Root is target
                        ((MMFeedbackScale)f.Feedbacks[1]).RemapCurveZero = t.z;
                        ((MMFeedbackScale)f.Feedbacks[1]).RemapCurveOne = t.z*1.2f;
                        ((MMFeedbackRotation)f.Feedbacks[2]).AnimateRotationTarget = root.transform;
                        ((MMFeedbackPosition)f.Feedbacks[3]).AnimatePositionTarget = root;
                        //Debug.Log(t.gameObject);
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
