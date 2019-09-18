using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Esri.APP;
using UnityEngine.UI;

public class ButtonMapstyle : MonoBehaviour {

    // Use this for initialization
    // Use this for initialization
    public void Start()
    {
        GameObject tc = GameObject.Find("Map");
        MapLoading mapLoading = tc.GetComponent<MapLoading>();
        if (mapLoading.currentStyle == "satellite")
        {
            GetComponentInChildren<Text>().text = "street";
        }
        else
        {
            GetComponentInChildren<Text>().text = "satellite";
        }

        Button btn = this.GetComponent<Button>();
        btn.onClick.AddListener(OnClick);
    }

    public void OnClick()
    {
        string text = GetComponentInChildren<Text>().text;
        if (text == "satellite")
        {
            GetComponentInChildren<Text>().text = "street";
        }
        else
        {
            GetComponentInChildren<Text>().text = "satellite"; 
        }
        GameObject map = GameObject.Find("Map");
        map.SendMessage("OnChangeMapstyle", text);
    }
}
