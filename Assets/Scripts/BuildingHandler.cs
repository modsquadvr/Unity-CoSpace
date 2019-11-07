using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingHandler : MonoBehaviour
{
    public BuildingData buildingData;

    // Start is called before the first frame update
    public void Start()
    {
        buildingData.id = name;
        SaveData.current.buildings.Add(buildingData);

        //GameEvents.current.onLoadEvent += DestroyMe;
    }

    void DestroyMe()
    {
        //GameEvents.current.onLoadEvent -= DestroyMe;
        Destroy(gameObject);

    }


    // Update is called once per frame
    private void Update()
    {
        buildingData.position = transform.position;
        buildingData.rotation = transform.rotation;
    }
}
