using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Esri.APP;

public class ButtonChangeView : MonoBehaviour {

	// Use this for initialization

    public void Start()
    {
        GameObject tc = GameObject.Find("Map");
        MapLoading mapLoading = tc.GetComponent<MapLoading>();
        if (mapLoading.currentView == "scene")
        {
            GetComponentInChildren<Text>().text = "Map";
        }
        else
        {
            GetComponentInChildren<Text>().text = "scene";
        }

        Button btn = this.GetComponent<Button>();
        btn.onClick.AddListener(OnClick);
    }

    public void OnClick()
    {
        string text = GetComponentInChildren<Text>().text;
        if (text == "scene")
        {
            GetComponentInChildren<Text>().text = "map";
        }
        else
        {
            GetComponentInChildren<Text>().text = "scene";
        }
        GameObject map = GameObject.Find("Map");
        map.SendMessage("OnClickChangeView", text);
    }

}
