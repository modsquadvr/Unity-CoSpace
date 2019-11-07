using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingLocations : MonoBehaviour
{

    public Dictionary<Tuple<double, double>, string> dict;
    private int count = 1;
   


    public BuildingLocations()
    {
        dict = new Dictionary<Tuple<double, double>, string>();
    }

    public void AddLocation(double x, double y, string name)
    {
        

        dict.Add(new Tuple<double, double>(x, y), name);
   

        count++;
    }

   

}
