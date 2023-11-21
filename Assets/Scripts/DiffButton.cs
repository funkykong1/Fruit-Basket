using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class DiffButton : MonoBehaviour
{
    //info text, cool camera gameobjects
    [HideInInspector]
    public GameObject bg, go;
    void Awake()
    {
        if(this.gameObject.name == "Easy Button")
        {
            bg = GameObject.Find("bg easy");
            go = GameObject.Find("Good");
        }
        else
        {
            bg = GameObject.Find("bg hard");
            go = GameObject.Find("Bad");
        }
#if UNITY_IOS || UNITY_ANDROID
        ToggleMeshes(false);
        bg.SetActive(false);
#endif
    }

    public void ToggleMeshes(bool toggle)
    {
#if !UNITY_IOS || UNITY_ANDROID
        foreach (MeshRenderer item in go.GetComponentsInChildren<MeshRenderer>())
        {
            item.enabled = toggle;
        }
#endif
    }

}
