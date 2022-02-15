using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    public GameObject player; //da playa (world moves around player, player only moves on y for time being)
    public GameObject CameraRig;
    [HideInInspector] public float shakeTimer;
    [HideInInspector] public float intensity;

    public FeedbackManaganger fbm;

    public WorldSpawn ws;

    public bool TESTING_ZEROSPEED;
    
    //UI Objects to toggle on/off
    public GameObject pauseUI;
    public GameObject gameUI;
    public GameObject loseUI;

    public float worldSpeed = 15; //world speed that changes based on the situation
    public float targetWorldSpeed = 15; /*/ world speed that is always returned to*/
    public bool playerSpeeding = false;
    public bool stopped = false;
    public bool playerDead = false;

    public float speedDiff; //used in calculations for camera offset
    public float airTime; //used in camera offset + achievements?highscore?


    public GameObject[] platforms;
    public float[] platformsSkyY;
    public float[] platformSpawnY;
    public float[] platformsHellY;

    public bool gamePaused = false;

    public float distanceTraveled = 0.0f;

    public AudioSource[] audiosSources;
    public AudioClip[] audioClips;

    public enum AudioClips
    {
        BatDash,
        BatDeath,
        BatFlap,
        Jumppad,
        MushDeath,
        MushStep1,
        MushStep2,
        AmmoUp,
        HealthUp,
        PlayerLand,
        PlayerLava,
        PlayerStep,
        None
    }

    private void Start()
    {
        instance = this;
    }

    public static GameManager Instance {
        get {
            if (instance == null)
            {
                instance = new GameManager();
            }

            return instance;
        }
    }

    private void Update()
    {
        distanceTraveled += worldSpeed * Time.deltaTime;

        if (TESTING_ZEROSPEED)
        {
            worldSpeed = 0;
            return;
        }

        //return worldspeed to the target world speed
        float x = Mathf.Lerp(worldSpeed, targetWorldSpeed, Time.deltaTime);
        
        worldSpeed = x;

        if (worldSpeed != 0)
        {
            speedDiff = worldSpeed / targetWorldSpeed;
        }
        else
        {
            speedDiff = 0;
        }
    }

    public void PlayAudio(AudioClips ac)
    {
        switch (ac)
        {
            case AudioClips.BatDash:
                audiosSources[2].PlayOneShot(audioClips[(int)ac]);
                break;
            case AudioClips.BatDeath:
                audiosSources[2].PlayOneShot(audioClips[(int)ac]);
                break;
            case AudioClips.BatFlap:
                audiosSources[2].PlayOneShot(audioClips[(int)ac]);
                break;
            case AudioClips.Jumppad:
                audiosSources[1].PlayOneShot(audioClips[(int)ac]);
                break;
            case AudioClips.MushDeath:
                audiosSources[2].PlayOneShot(audioClips[(int)ac]);
                break;
            case AudioClips.MushStep1:
                audiosSources[2].PlayOneShot(audioClips[(int)ac]);
                break;
            case AudioClips.MushStep2:
                audiosSources[2].PlayOneShot(audioClips[(int)ac]);
                break;
            case AudioClips.AmmoUp:
                audiosSources[1].PlayOneShot(audioClips[(int)ac]);
                break;
            case AudioClips.HealthUp:
                audiosSources[1].PlayOneShot(audioClips[(int)ac]);
                break;
            case AudioClips.PlayerLand:
                audiosSources[1].PlayOneShot(audioClips[(int)ac]);
                break;
            case AudioClips.PlayerLava:
                audiosSources[1].PlayOneShot(audioClips[(int)ac]);
                break;
            case AudioClips.PlayerStep:
                audiosSources[1].PlayOneShot(audioClips[(int)ac]);
                break;
        }
    }

    //all changes to worldspeed use this function
    public void worldSpeedChange(bool multi, float val)
    {
        //print(worldSpeed);
        if (!stopped)
        {
            if (multi)
            {
                worldSpeed *= val;
            }
            else
            {
                worldSpeed += val;
            }
        }
    }

    //rip prefab and kids
    public void DestorySelf(GameObject g)
    {
        foreach(Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        GameObject.Destroy(g);
    }

    [ContextMenu( "WorldSpeedZero()" )]
    void WorldSpeedZero()
    {
        targetWorldSpeed = 0;
        worldSpeed = 0;
    }

    public bool playerTooFast(){
        if(worldSpeed > targetWorldSpeed * 1.43f)
        { 
            //Wind Sound
            return true;
        }
        else
        {
            return false;
        }
        
    }

    public void gmScreenShake(float duration, float inten)
    {
        shakeTimer = duration;
        intensity = inten;
    }

    public void Unpause()
    {
        Time.timeScale = 1f;
        gamePaused = false;
        gameUI.SetActive(true);
        pauseUI.SetActive(false);
    }

    public void Pause()
    {
        Time.timeScale = 0f;
        gamePaused = true;
        gameUI.SetActive(false);
        pauseUI.SetActive(true);
    }
}
