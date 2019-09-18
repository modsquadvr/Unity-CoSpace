using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFollowing : MonoBehaviour
{
    // Start is called before the first frame update
    public Camera cam;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = cam.transform.position;
        this.transform.rotation = cam.transform.rotation;
    }
}
