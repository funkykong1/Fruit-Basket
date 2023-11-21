using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

//less clutter for gamemanager
public class UIManager : MonoBehaviour
{
    //are these used for something???????????
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI loseText;
    public GameObject nextButton;
    internal Canvas start, loser, adjust;



    void Awake()
    {
        start = GameObject.Find("Menu Canvas").GetComponent<Canvas>();
        loser = GameObject.Find("Loser Canvas").GetComponent<Canvas>();
        adjust = GameObject.Find("Adjustment Canvas").GetComponent<Canvas>();
        nextButton = GameObject.Find("Next Button");
    }
    // Start is called before the first frame update
    void Start()
    {
        adjust.enabled = false;
        loser.enabled = false;
        nextButton.SetActive(false);
    }

    public void DifficultySelect()
    {
        start.enabled = false;
        adjust.enabled = true;
        //deez nut
#if UNITY_IOS || UNITY_ANDROID
GameObject.Find("Easy Button").GetComponent<DiffButton>().ToggleMeshes(true);
GameObject.Find("Hard Button").GetComponent<DiffButton>().ToggleMeshes(true);
#endif
    }
}
