using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootSteps : MonoBehaviour
{
    public AudioClip[] stepClips;
    public AudioSource altSouce;
    private float swap = 0;

    void Start()
    {
        //altSource = GetComponent<AudioSource>();
    }


    private void Step()
    {
        //GameManager.Instance.PlayAudio(GameManager.AudioClips.PlayerStep);
        AudioClip clip = getRandomStep();
        GameManager.Instance.audiosSources[1].PlayOneShot(clip);
    }

    private void Flap()
    {
        if (swap == 0)
        {
            altSouce.PlayOneShot(GameManager.Instance.audioClips[(int)GameManager.AudioClips.BatFlap]);
            swap++;
        }
        else
        {
            altSouce.PlayOneShot(GameManager.Instance.audioClips[(int)GameManager.AudioClips.BatFlap1]);
            swap = 0;
        }
        
    }

    private void Squish()
    {
        if(swap == 0)
        {
            altSouce.PlayOneShot(GameManager.Instance.audioClips[(int)GameManager.AudioClips.MushStep1]);
            swap++;
        }
        else
        {
            altSouce.PlayOneShot(GameManager.Instance.audioClips[(int)GameManager.AudioClips.MushStep2]);
            swap = 0;
        }

    }

    private AudioClip getRandomStep()
    {
        return stepClips[UnityEngine.Random.Range(0, stepClips.Length)];
    }
}
