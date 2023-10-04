using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

//less clutter for gamemanager
public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI loseText;
    public GameObject nextButton;
    internal Canvas start, loser, adjust;
    private GameManager gm;
    private DiffButton good,bad;


    void Awake()
    {
        start = GameObject.Find("Menu Canvas").GetComponent<Canvas>();
        loser = GameObject.Find("Loser Canvas").GetComponent<Canvas>();
        adjust = GameObject.Find("Adjustment Canvas").GetComponent<Canvas>();
        nextButton = GameObject.Find("Next Button");

        good = GameObject.Find("Easy Button").GetComponent<DiffButton>();
        bad = GameObject.Find("Hard Button").GetComponent<DiffButton>();
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
    }
}
