using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using LootLocker.Requests;
using UnityEngine.Rendering.PostProcessing;
using TMPro;

public class GameManager : MonoBehaviour
{
    public Text distanceText, youRanText;
    
    public TextMeshProUGUI lPlayerNames, lPlayerScores;

    private static GameManager instance;
    public TMP_InputField playerNameField;

    public GameObject player; //da playa (world moves around player, player only moves on y for time being)
    public GameObject CameraRig;
    [HideInInspector] public float shakeTimer;
    [HideInInspector] public float intensity;

    public PostProcessVolume m_Volume;
    Bloom bloom;
    public GameObject procObj;

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
    private bool isHighestLocalScore = false;
    private int localIndex = -1;

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
        Shoot,
        None
    }

    private void Start()
    {
        instance = this;
        StartCoroutine(SetupRoutine());
        //m_Volume = PostProcessManager.instance.QuickVolume(procObj.layer, 100f, bloom);
        //m_Volume = procObj.GetComponent<PostProcessVolume>();
        //bloom.intensity.value = 0;
        m_Volume.profile.TryGetSettings(out bloom);
    }

    IEnumerator LoginRoutine()
    {
        bool done = false;
        LootLockerSDKManager.StartGuestSession((response) =>
        {
            if (response.success)
            {
                //Debug.Log("successfully started LootLocker session");
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

    public void ResetLocalFlag()
    {
        isHighestLocalScore = false;
        localIndex = -1;
    }

    public IEnumerator SlideLocalScores()
    {
        print("AT INDEX " + localIndex);
        bool done = false;
        for (int i = 9; i > localIndex; i--)
        {
            //print("SLIDE " + (int)(i - 1)+"SCORE"+ PlayerPrefs.GetInt("HS" + (int)(i - 1), 0) + " TO " + i + "SCORE" + PlayerPrefs.GetInt("HS" + i, 0));
            PlayerPrefs.SetInt("HS" + i, PlayerPrefs.GetInt("HS" + (int)(i-1), 0));
            PlayerPrefs.SetString("HN" + i, PlayerPrefs.GetString("HN" + (int)(i-1), "-----"));
        }
        done = true;

        yield return new WaitWhile(() => done == false);
    }

    public IEnumerator SetLoseText()
    {
        int score = (int)(Mathf.Round(distanceTraveled * 100f) / 100f);
        yield return leaderboard.SubmitScoreRoutine(score);
        yield return leaderboard.FetchTopHighscoreRoutine();


        print("SETLOSETEXT");
        for (int i = 0; i < 10; i++)
        {
            youRanText.text = "You ran: " + score;
            if (PlayerPrefs.GetInt("HS" + i, 0) < score && i == 0 ||
                i != 0 && PlayerPrefs.GetInt("HS" + i, 0) < score && PlayerPrefs.GetInt("HS" + (int)(i - 1), 0) >= score)
            {
                localIndex = i;
                yield return SlideLocalScores();
                youRanText.text = "HIGHSCORE\nYou ran: " + score;
                PlayerPrefs.SetInt("HS" + i, score);
                PlayerPrefs.SetString("HN" + i, "YOUR NAME");
                if (i == 0)
                {
                    isHighestLocalScore = true;
                }
                
                break;
            }
        }

        
        ShowLocalScores();
    }

    public void ShowLocalScores()
    {
        string tempPlayerNames = " \n";
        string tempPlayerScores = " \n";

        for (int i = 0; i < 10; i++)
        {
            tempPlayerNames += PlayerPrefs.GetString("HN" + i, "-----") + " ";
            tempPlayerScores += PlayerPrefs.GetInt("HS" + i, 0);
            tempPlayerScores += "\n";
            tempPlayerNames += "\n";
        }
        lPlayerNames.text = tempPlayerNames;
        lPlayerScores.text = tempPlayerScores;
    }

    public void SetPlayerName()
    {
        if (isHighestLocalScore)
        {
            StartCoroutine(SetPlayerNameRoutine());
        }
        if(localIndex != -1)
        {
            PlayerPrefs.SetString("HN" + localIndex, playerNameField.text);
            ShowLocalScores();
        }
        
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
                audiosSources[1].PlayOneShot(audioClips[(int)ac], 2);
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
            case AudioClips.Shoot:
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
