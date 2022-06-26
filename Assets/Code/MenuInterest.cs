using System.Collections;
using System.Collections.Generic;
using LootLocker.Requests;
using TMPro;
using UnityEngine;

public class MenuInterest : MonoBehaviour
{
    public GameObject cam1, cam2, cam3, cam4, cam5, cam6;

    private Transform start;
    private Vector3 n;
    private Quaternion o;
    private bool menu = false;
    private bool menu1 = false;


    private bool scroll = false;
    private float width;
    private float scrollSpeed = 2;
    private Vector3 textStartPos;
    private float scrollPos = 0;

    public TextMeshProUGUI TMPComp;
    //public RectTransform TMRect;

    public GameObject hsUI;
    public GameObject menuUI;
    public GameObject credUI;

    public Leaderboard leaderboard;
    public TextMeshProUGUI lPlayerNames, lPlayerScores;

    public float smoothPos;
    public float smoothRot;
    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1f;
        start = transform;
        StartCoroutine(SetupRoutine());
        smoothPos = .09f;
        smoothRot = .2f;

        //width = TMPComp.preferredWidth;
        //TMRect = TMPComp.GetComponent<RectTransform>();
        scrollSpeed = 20;
        //textStartPos = TMRect.position;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(Camera.main.ScreenToViewportPoint(Input.mousePosition));
        Vector3 m = Camera.main.ScreenToViewportPoint(Input.mousePosition);

        if (menu)
        {
            n = Vector3.Lerp(cam3.transform.position, cam4.transform.position, m.y);
            o = Quaternion.Lerp(cam3.transform.rotation, cam4.transform.rotation, m.x);
        }
        else if (menu1)
        {
            n = Vector3.Lerp(cam5.transform.position, cam6.transform.position, m.y);
            o = Quaternion.Lerp(cam5.transform.rotation, cam6.transform.rotation, m.x);
        }
        else
        {
            n = Vector3.Lerp(cam1.transform.position, cam2.transform.position, m.y);
            o = Quaternion.Lerp(cam1.transform.rotation, cam2.transform.rotation, m.x);
        }

        

    }

    private void FixedUpdate()
    {
        PosUpdate();
        RotUpdate();
        //ScrollText();
    }

    private void PosUpdate() => transform.position = Vector3.Lerp(transform.position, n, smoothPos);

    private void RotUpdate() => transform.rotation = Quaternion.Lerp(transform.rotation, o, smoothRot);

    public void hsButton()
    {
        menuUI.SetActive(false);
        hsUI.SetActive(true);
        ShowOnlineScores();
        ShowLocalScores();
        menu = true;
        menu1 = false;
    }

    public void credButton()
    {
        menuUI.SetActive(false);
        credUI.SetActive(true);
        scrollPos = 0;
        scroll = true;
        menu1 = true;
        menu = false;
    }

   /* public void ScrollText()
    {
            TMRect.position = new Vector3(-scrollPos % width, textStartPos.y, textStartPos.z);
            scrollPos += scrollSpeed * 20 * Time.deltaTime;
    }*/

    public void menuBack()
    {
        menuUI.SetActive(true);
        hsUI.SetActive(false);
        credUI.SetActive(false);
        menu = false;
        menu1 = false;
        scroll = false;
    }

    IEnumerator SetupRoutine()
    {
        yield return LoginRoutine();
        //yield return leaderboard.FetchTopHighscoreRoutine();
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

    public void ShowOnlineScores()
    {
        StartCoroutine(leaderboard.FetchTopHighscoreRoutine());
    }

    public void ShowLocalScores()
    {
        string tempPlayerNames = "\n";
        string tempPlayerScores = "\n";

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

   /* public IEnumerator ShowOnlineScores()
    {
        yield return leaderboard.FetchTopHighscoreRoutine();
    }*/
    
}
