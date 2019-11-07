using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//key to a save system working: serializable
[System.Serializable]
public class SaveData
{
    private static SaveData _current;
    public static SaveData current
    {
        get
        {
            if (_current == null)
            {
                _current = new SaveData();
            }
            return _current;
        }
    }

    public List<BuildingData> buildings;


}