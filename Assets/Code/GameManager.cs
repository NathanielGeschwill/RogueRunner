using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using LootLocker.Requests;
using TMPro;

public class GameManager : MonoBehaviour
{
    public Text distanceText;//, loseText, highscoreText;

    private static GameManager instance;
    public TMP_InputField playerNameField;

    public GameObject player; //da playa (world moves around player, player only moves on y for time being)
    public GameObject CameraRig;
    [HideInInspector] public float shakeTimer;
    [HideInInspector] public float intensity;

    public FeedbackManaganger fbm;
    public ParticleSystem speedLines;

    public WorldSpawn ws;

    public bool TESTING_ZEROSPEED;

    //UI Objects to toggle on/off
    public GameObject pauseUI;
    public GameObject gameUI;
    public GameObject loseUI;

    public float worldSpeed = 15; //world speed that changes based on the situation
    public float targetWorldSpeed = 15; /*/ world speed that is always returned to*/
    public float targetSpeedandDist = 0;
    public bool playerSpeeding = false;
    public bool stopped = false;
    public bool playerDead = false;
    public bool playerGrounded = false;

    public float speedDiff; //used in calculations for camera offset
    public float airTime; //used in camera offset + achievements?highscore?


    public GameObject[] platforms;
    public float[] platformsSkyY;
    public float[] platformSpawnY;
    public float[] platformsHellY;

    public bool gamePaused = false;

    public float distanceTraveled = 0.0f;
    public float vol = .25f;

    public AudioSource[] audiosSources;
    public AudioClip[] audioClips;

    public Leaderboard leaderboard;

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
        PlayerHurt,
        BatFlap1,
        SpikeTrap,
        None
    }

    private void Start()
    {
        instance = this;
        StartCoroutine(SetupRoutine());
    }

    IEnumerator LoginRoutine()
    {
        bool done = false;
        LootLockerSDKManager.StartGuestSession((response) =>
        {
            if (response.success)
            {
                Debug.Log("successfully started LootLocker session");
                PlayerPrefs.SetString("PlayerID", response.player_id.ToString());
                done = true;
            }
            else
            {
                Debug.Log("error starting LootLocker session");
                done = true;
            }
            
        });
        yield return new WaitWhile(() => done == false);
    }

    IEnumerator SetupRoutine()
    {
        yield return LoginRoutine();
        //yield return leaderboard.FetchTopHighscoreRoutine();
    }

    public IEnumerator SetLoseText()
    {
        yield return leaderboard.SubmitScoreRoutine((int)(Mathf.Round(distanceTraveled * 100f) / 100f));
        yield return leaderboard.FetchTopHighscoreRoutine();


        //print("SETLOSETEXT");
        //if(PlayerPrefs.GetFloat("Highscore", 0f) < Mathf.Round(distanceTraveled * 100f) / 100f)
        //{
        //    loseText.text = "HIGHSCORE!\n\nYou ran: " + Mathf.Round(distanceTraveled * 100f) / 100f;
        //    PlayerPrefs.SetFloat("Highscore", Mathf.Round(distanceTraveled * 100f) / 100f);
        //}
        //else
        //{
        //    loseText.text = "You ran: " + Mathf.Round(distanceTraveled * 100f) / 100f;
        //}
        //highscoreText.text = "Highscore: " + PlayerPrefs.GetFloat("Highscore");
    }

    public void SetPlayerName()
    {
        StartCoroutine(SetPlayerNameRoutine());
    }

    IEnumerator SetPlayerNameRoutine()
    {
        bool done = false;
        LootLockerSDKManager.SetPlayerName(playerNameField.text, (response) =>
        {
            if (response.success)
            {
                Debug.Log("Set Player Name");
                done = true;
            }
            else
            {
                Debug.Log("Could Not Set Player Name " + response.Error);
                done = true;
            }
        });
        yield return new WaitWhile(() => done == false);
        StartCoroutine(leaderboard.FetchTopHighscoreRoutine());
    }

    public static GameManager Instance
    {
        get
        {
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
        distanceText.text = "Distance: " + Mathf.Round(distanceTraveled);

        targetSpeedandDist = targetWorldSpeed + ((Mathf.Round(distanceTraveled * 100f) / 100f) / 250);
        Mathf.Clamp(targetSpeedandDist, 0, 48);

        if (TESTING_ZEROSPEED)
        {
            worldSpeed = 0;
            return;
        }

        //return worldspeed to the target world speed
        float x = worldSpeed;
        if (playerGrounded)
        {
            x = Mathf.Lerp(worldSpeed, targetSpeedandDist, (Time.deltaTime + (speedDiff / 4000)) * 4);
        }
        else
        {
            x = Mathf.Lerp(worldSpeed, targetSpeedandDist, (Time.deltaTime + (speedDiff / 4000)));
            //print("time " + Time.deltaTime + " speed diff " + speedDiff/2000 + " = " + (Time.deltaTime + (speedDiff / 2000)));
        }

        worldSpeed = x;
        Mathf.Clamp(worldSpeed, 0, 124);

        

        if (worldSpeed != 0)
        {
            speedDiff = worldSpeed / targetSpeedandDist;
        }
        else
        {
            speedDiff = 0;
        }

        if (playerTooFast())
        {
            audiosSources[3].volume += .2f;//Mathf.Lerp(0, 1, .25f);
            audiosSources[0].volume -= .005f;//Mathf.Lerp(.25f, 0, .25f);
            speedLines.gameObject.SetActive(true);
            //speedLines.emission.rateOverTime = speedDiff * 10;

        }
        else
        {

            if (audiosSources[0].volume < .25f) { audiosSources[0].volume += Time.deltaTime; } //if(audiosSources[0].volume > .25f) { audiosSources[0].volume = 1/4; }
            if(audiosSources[3].volume != 0f) { audiosSources[3].volume -= 0.2f; }
            speedLines.gameObject.SetActive(false); //print(audiosSources[0].volume.ToString());
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
            case AudioClips.PlayerHurt:
                audiosSources[1].PlayOneShot(audioClips[(int)ac]);
                break;
            case AudioClips.BatFlap1:
                audiosSources[2].PlayOneShot(audioClips[(int)ac]);
                break;
            case AudioClips.SpikeTrap:
                audiosSources[2].PlayOneShot(audioClips[(int)ac]);
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
        foreach (Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        GameObject.Destroy(g);
    }

    [ContextMenu("WorldSpeedZero()")]
    void WorldSpeedZero()
    {
        targetWorldSpeed = 0;
        worldSpeed = 0;
    }

    public bool playerTooFast()
    {
        if (worldSpeed > targetSpeedandDist * 1.43f)
        {
            
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
