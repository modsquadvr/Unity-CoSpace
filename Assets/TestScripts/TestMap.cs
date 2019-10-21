// 
//  TestMap.cs
//  
//  Author:
//       Jonathan Derrough <jonathan.derrough@gmail.com>
//  
//  Copyright (c) 2012 Jonathan Derrough
// 
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
// 
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU Lesser General Public License for more details.
// 
//  You should have received a copy of the GNU Lesser General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

// #define DEBUG_LOG

using UnityEngine;

using TouchScript.Gestures.TransformGestures;

using System;

using UnitySlippyMap.Map;
using UnitySlippyMap.Markers;
using UnitySlippyMap.Layers;
using ProjNet.CoordinateSystems;
using ProjNet.CoordinateSystems.Transformations;
using ProjNet.Converters.WellKnownText;
using System.IO;
using System.Collections;
using System.Collections.Generic;

using TouchScript;
using TouchScript.Gestures;
using TouchScript.Layers;
using TouchScript.Gestures.TransformGestures;
using TouchScript.Gestures.TransformGestures.Base;






using UnityEngine.UI;
using TouchScript.Pointers;
using static TouchScript.Gestures.Gesture;

public class TestMap : MonoBehaviour
{

    //pivot GameObject is going to be Esri GameObject

    public MapBehaviour		map;


	public Texture	LocationTexture;
	public Texture	MarkerTexture;


    public float CameraZoom;
    private Transform camPivot;

	private bool 	isPerspectiveView = false;
	private float	perspectiveAngle = 30.0f;
	private float	destinationAngle = 0.0f;
	private float	currentAngle = 0.0f;
	private float	animationDuration = 0.5f;
	private float	animationStartTime = 0.0f;

    //touchscript variables
    public ScreenTransformGesture TwoFingerMoveGesture;
    public ScreenTransformGesture ManipulationGesture;
    public float PanSpeed = 1f;
    public float RotationSpeed = 200f;
    public float ZoomSpeed = .25f;

    //panning variable(s)
    public bool drag = false;
    public bool panningStop = false;
    public Vector3 screenPosition = Vector3.zero;
    private Plane plane;
    private ProjectionParams projection;
    private Vector3 startPosition;
    private Pointer primaryPointer;
    private static Vector3 lastHitPosition = Vector3.zero;


    private Transform cam;
    private Transform esriMap;

     private List<LayerBehaviour> layers;
    private int     currentLayerIndex = 0;


    private void Awake()
    {
        cam = GameObject.Find("Camera").transform;
       
        //camPivot = transform.Find("Camera");
    }

    private void OnEnable()
    {
        TwoFingerMoveGesture.Transformed += twoFingerTransformHandler;
        ManipulationGesture.Transformed += manipulationTransformedHandler;

        if (TouchManager.Instance != null)
        {
            //update handler
            TouchManager.Instance.PointersUpdated += pointersPressedHandler;
            //release handler
            TouchManager.Instance.PointersReleased += pointersPressedHandler2;
            //press handler
            TouchManager.Instance.PointersPressed += pointersPressedHandler1;

        }
    }

    private void OnDisable()
    {
        if (TouchManager.Instance != null)
        {
            TouchManager.Instance.PointersPressed -= pointersPressedHandler;
        }
        TwoFingerMoveGesture.Transformed -= twoFingerTransformHandler;
        ManipulationGesture.Transformed -= manipulationTransformedHandler;
    }


    
    private void pointersPressedHandler(object sender, PointerEventArgs e)
    {


            foreach (var pointer in e.Pointers)
            {
                float x = pointer.Position.x;
                float y = pointer.Position.y;

                screenPosition = new Vector3(x, y, 0);


            }

    }

    //pressing handler
    private void pointersPressedHandler1(object sender, PointerEventArgs e)
    {


        var activePointers = TwoFingerMoveGesture.ActivePointers;
        var activeCount = activePointers.Count;
        if (activeCount == 0)
        {
            drag = true;
            
        }
    }

    private void pointersPressedHandler2(object sender, PointerEventArgs e)
    {
        
        drag = false;

    }

    void Update()
    {
       // Debug.Log(screenPosition);

        if (drag)
        {

            map.UpdatesCenterWithLocation = false;

            //  Debug.Log("Panning");

            // apply the movements
            Ray ray = map.CurrentCamera.ScreenPointToRay(screenPosition);
            //Debug.Log("1. INPUT - screenposition: " + screenPosition);
            //Debug.Log("2. INPUT - ray: " + ray);
            RaycastHit hitInfo;

            if (Physics.Raycast(ray, out hitInfo))
            {
                Vector3 displacement = Vector3.zero;
                //Debug.Log("3. INPUT - Displacement instantiate: " + displacement);
                if (lastHitPosition != Vector3.zero)
                {
                    displacement = hitInfo.point - lastHitPosition;
                    //Debug.Log("4. TEST: displacement = hitInfo.point - lastHitPosition: " + displacement);
                }
                lastHitPosition = new Vector3(hitInfo.point.x, hitInfo.point.y, hitInfo.point.z);
                //Debug.Log("5. INPUT - lasthitpos: " + lastHitPosition);
                if (displacement != Vector3.zero)
                {
                    // update the centerWGS84 property to the new centerWGS84 wgs84 coordinates of the map
                    double[] displacementMeters = new double[2] {
                            displacement.x / map.RoundedScaleMultiplier,
                            displacement.z / map.RoundedScaleMultiplier
                        };
                    //Debug.Log("6. TEST: Displacement Meters: " + displacementMeters[0]
                      // + ", " + displacementMeters[1]);


                    double[] centerMeters = new double[2] {
                            map.CenterEPSG900913 [0],
                            map.CenterEPSG900913 [1]
                        };
                    Debug.Log("7. TEST: Center Meters: " + centerMeters[0]+ ", " + centerMeters[1]);


                    centerMeters[0] -= displacementMeters[0];
                    centerMeters[1] -= displacementMeters[1];
                    map.CenterEPSG900913 = centerMeters;
                    //Debug.Log("8. TEST: map centre: " + map.CenterEPSG900913[0] + ", " + map.CenterEPSG900913[1]);

#if DEBUG_LOG
    					Debug.Log("DEBUG: Map.Update: new centerWGS84 wgs84: " + centerWGS84[0] + ", " + centerWGS84[1]);
#endif
                }

                map.HasMoved = true;
            }

        }
        else if (!drag)
        {
            //Debug.Log("Not panning");

            // reset the last hit position
            lastHitPosition = Vector3.zero;

            // trigger a tile update
            map.IsDirty = true;
        }
    }

    private void twoFingerTransformHandler(object sender, System.EventArgs e)
    {

        if (esriMap == null) {
            esriMap = GameObject.Find("[Map]").transform;
        }
        Vector3 delta = new Vector3(TwoFingerMoveGesture.DeltaPosition.x, 0, TwoFingerMoveGesture.DeltaPosition.y);
        esriMap.localPosition += esriMap.rotation * delta * PanSpeed;
    
       


    }
    
    public void manipulationTransformedHandler(object sender, System.EventArgs e)
    { 
           

    

        cam.transform.localPosition -= Vector3.up * (ManipulationGesture.DeltaScale - 1f) * ZoomSpeed;

        if((Camera.main.transform.position.y) > CameraZoom)
        {
            map.Zoom(-ZoomSpeed);
        }

        else
        {
            map.Zoom(ZoomSpeed);
        }

        CameraZoom = Camera.main.transform.position.y;

        //Debug.Log(CameraZoom);
        


    }
    
 
    private IEnumerator Start()
	{

        //Debug.Log("Screen position start: " + screenPosition);
        cam = GameObject.Find("Camera").transform;
        //Debug.Log("Cam local positioon: "+ cam.transform.localPosition);
        //Debug.Log("cam rotation: " + cam.rotation);
        CameraZoom = Camera.main.transform.position.y;
        

       
		// create the map singleton
		map = MapBehaviour.Instance;
		map.CurrentCamera = Camera.main;

        //setting up the map for touchscript
        map.gameObject.AddComponent<MeshCollider>();



      
       map.InputDelegate += UnitySlippyMap.Input.MapInput.BasicTouchAndKeyboard;

		map.CurrentZoom = 8.0f;
		// UVic

		map.CenterWGS84 = new double[2] { 23.7267, 37.9715 };
		map.UsesLocation = true;
		map.InputsEnabled = true;

        //Debug.Log("Start CenterEPSG900913 [0]: " + map.CenterEPSG900913[0] + ", " +
                    //map.CenterEPSG900913[1]);



        layers = new List<LayerBehaviour>();

		// create an Esri tile layer
        EsriTileLayer esriLayer = map.CreateLayer<EsriTileLayer>("Esri");
        esriLayer.BaseURL = "https://server.arcgisonline.com/ArcGIS/rest/services/World_Topo_Map/MapServer/tile/";

        //Setting up the camera for TouchScript
        //esriLayer.transform.parent = GameObject.Find("Scene").transform;
        //Camera.main.transform.parent = GameObject.Find("Esri").transform;
		
        layers.Add(esriLayer);

        yield return null;
	
	}
	
	void OnApplicationQuit()
	{
		map = null;
	}
	

	
#if DEBUG_PROFILE
	void LateUpdate()
	{
		Debug.Log("PROFILE:\n" + UnitySlippyMap.Profiler.Dump());
		UnitySlippyMap.Profiler.Reset();
	}
#endif
}

