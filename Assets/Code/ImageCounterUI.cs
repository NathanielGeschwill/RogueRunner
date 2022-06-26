using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageCounterUI : MonoBehaviour
{
    public GameObject imageGO;
    public GameObject imageNO;
    public Sprite sActive;
    public Sprite sInactive;
    public string counterName;
    private int counterAmount = 0;
    private List<GameObject> images = new List<GameObject>();

    // Start is called before the first frame update
    private void OnEnable()
    {
        Player.OnIncreaseUI += Increase;
        Player.OnDecreaseUI += Decrease;
    }

    private void OnDisable()
    {
        Player.OnIncreaseUI -= Increase;
        Player.OnDecreaseUI -= Decrease;
    }

    private void Start()
    {
        images.Add(imageGO);
        //Increase(counterName);
    }

    private void Increase(string name)
    {
        if(name == counterName)
        {
            if(images.Count >= counterAmount + 1)
            {
                //print("increase active");
                images[counterAmount].GetComponent<Image>().sprite = sActive;
            }
            else
            {
                //print("increase new");
                GameObject newObj = Instantiate(imageGO, gameObject.transform);
                newObj.GetComponent<RectTransform>().anchoredPosition =
                    images[counterAmount - 1].GetComponent<RectTransform>().anchoredPosition - new Vector2(0f, 100f);
                images.Add(newObj);
            }
            counterAmount++;
        }
    }

    private void Decrease(string name)
    {
        //print("DECREASE UI");
        if (name == counterName)
        {
            counterAmount--;
            images[counterAmount].GetComponent<Image>().sprite = sInactive;
        }
    }
}
