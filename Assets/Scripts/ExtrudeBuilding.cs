using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExtrudeBuilding : MonoBehaviour
{
    public Transform floor;
    public Transform roof;
    public Transform level;
    

    Vector3 RoofHeight;
    Vector3 LevelHeight;

    public GameObject topLevel;
    private int value;
    private int currValue;
    public int numberOfFloors = 1;

    public Slider slider;
    
    private void AddFloors(int inc)
    {
        roof.position += LevelHeight;

        GameObject newLevel = Object.Instantiate(topLevel);
        newLevel.transform.parent = GameObject.Find("Construct").transform;
        newLevel.transform.position += (LevelHeight*numberOfFloors);
        numberOfFloors++;
        newLevel.name = "Level #" + numberOfFloors;

    }

    private void RemoveFloors(int dec)
    {
        GameObject levelToDelete = GameObject.Find("Level #" + numberOfFloors);
        Object.Destroy(levelToDelete);
        roof.position -= LevelHeight;
        numberOfFloors--;
    }
    
    
    // Start is called before the first frame update
    void Start()
    {
        //keep track of level closest to roof
        topLevel = GameObject.Find("Level");
        value = (int) slider.value;

        //get a vector to add onto/subtract from current position;
        LevelHeight = new Vector3(0, (roof.transform.position.y-level.transform.position.y), 0);


    }

    // Update is called once per frame
    void Update()
    {
        //if slider value changes, trigger change floors
        currValue = (int) slider.value;
        if(value != currValue)
        {
            int increment = currValue - value;

            if(increment > 0)
            {
                AddFloors(increment);
            }

            else
            {
                increment = increment * -1;
                RemoveFloors(increment);
            }
            
            value = currValue;
        }


    }
}
