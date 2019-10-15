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







using UnityEngine.UI;

public class TestMap : MonoBehaviour
{

    //pivot GameObject is going to be Esri GameObject

    public MapBehaviour		map;


	public Texture	LocationTexture;
	public Texture	MarkerTexture;


    public float CameraZoom;
    private Transform camPivot;

    private float	guiXScale;
	private float	guiYScale;
	private Rect	guiRect;
	
	private bool 	isPerspectiveView = false;
	private float	perspectiveAngle = 30.0f;
	private float	destinationAngle = 0.0f;
	private float	currentAngle = 0.0f;
	private float	animationDuration = 0.5f;
	private float	animationStartTime = 0.0f;

    //Debug variables
    public int counter;
    public int mouseCounter;

    //touchscript variables
    public ScreenTransformGesture TwoFingerMoveGesture;
    public ScreenTransformGesture ManipulationGesture;
    public float PanSpeed = 1f;
    public float RotationSpeed = 200f;
    public float ZoomSpeed = .25f;


    private Transform cam;



    private List<LayerBehaviour> layers;
    private int     currentLayerIndex = 0;

    private string utfGridJsonString = "";

	bool Toolbar(MapBehaviour map)
	{
		//GUI.matrix = Matrix4x4.Scale(new Vector3(guiXScale, guiXScale, 1.0f));
		
		GUILayout.BeginArea(guiRect);
		
		GUILayout.BeginHorizontal();
		
		//GUILayout.Label("Zoom: " + map.CurrentZoom);
		
		bool pressed = false;




        /*
        if (GUILayout.RepeatButton("+", GUILayout.ExpandHeight(true)))
		{
			map.Zoom(1.0f);
			pressed = true;
		}
        */



        if (Event.current.type == EventType.Repaint)
        {
            Rect rect = GUILayoutUtility.GetLastRect();
            if (rect.Contains(Event.current.mousePosition))
                pressed = true;
        }

        if (GUILayout.Button("2D/3D", GUILayout.ExpandHeight(true)))
		{
			if (isPerspectiveView)
			{
				destinationAngle = -perspectiveAngle;
			}
			else
			{
				destinationAngle = perspectiveAngle;
			}
			
			animationStartTime = Time.time;
			
			isPerspectiveView = !isPerspectiveView;
		}
        if (Event.current.type == EventType.Repaint)
        {
            Rect rect = GUILayoutUtility.GetLastRect();
            if (rect.Contains(Event.current.mousePosition))
                pressed = true;
        }

      
        if (Event.current.type == EventType.Repaint)
        {
            Rect rect = GUILayoutUtility.GetLastRect();
            if (rect.Contains(Event.current.mousePosition))
                pressed = true;
        }

        string layerMessage = String.Empty;
        if (map.CurrentZoom > layers[currentLayerIndex].MaxZoom)
            layerMessage = "\nZoom out!";
        else if (map.CurrentZoom < layers[currentLayerIndex].MinZoom)
            layerMessage = "\nZoom in!";
        if (GUILayout.Button(((layers != null && currentLayerIndex < layers.Count) ? layers[currentLayerIndex].name + layerMessage : "Layer"), GUILayout.ExpandHeight(true)))
        {
            ++currentLayerIndex;
            if (currentLayerIndex >= layers.Count)
            {
                currentLayerIndex = 0;
            }

            map.IsDirty = true;
        }

        /*
        if (GUILayout.RepeatButton("-", GUILayout.ExpandHeight(true)))
		{
			map.Zoom(-1.0f);
			pressed = true;
		}
        */
        if (Event.current.type == EventType.Repaint)
        {
            Rect rect = GUILayoutUtility.GetLastRect();
            if (rect.Contains(Event.current.mousePosition))
                pressed = true;
        }
		
		GUILayout.EndHorizontal();
					
		GUILayout.EndArea();

        // Show any mbtiles utf string under the mouse position
        if (!string.IsNullOrEmpty(utfGridJsonString))
            GUILayout.Label(utfGridJsonString);

		return pressed;

	}

    private void Awake()
    {
        cam = GameObject.Find("Camera").transform;
        //camPivot = transform.Find("Camera");
    }

    private void OnEnable()
    {
        TwoFingerMoveGesture.Transformed += twoFingerTransformHandler;
        ManipulationGesture.Transformed += manipulationTransformedHandler;
    }

    private void OnDisable()
    {
        TwoFingerMoveGesture.Transformed -= twoFingerTransformHandler;
        ManipulationGesture.Transformed -= manipulationTransformedHandler;


    }

    private void twoFingerTransformHandler(object sender, System.EventArgs e)
    {
        cam.localPosition -= cam.rotation * TwoFingerMoveGesture.DeltaPosition * PanSpeed;
    
       
        counter++;
        Debug.Log("Transformation # " + counter);

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

        Debug.Log(CameraZoom);
        


    }

    private IEnumerator Start()
	{
        Debug.Log("Cam local positioon: "+ cam.transform.localPosition);
        Debug.Log("cam rotation: " + cam.rotation);
        CameraZoom = Camera.main.transform.position.y;
        

        // setup the gui scale according to the screen resolution
        guiXScale = (Screen.orientation == ScreenOrientation.Landscape ? Screen.width : Screen.height) / 480.0f;
        guiYScale = (Screen.orientation == ScreenOrientation.Landscape ? Screen.height : Screen.width) / 640.0f;
		// setup the gui area
		//guiRect = new Rect(16.0f * guiXScale, 4.0f * guiXScale, Screen.width / guiXScale - 32.0f * guiXScale, 32.0f * guiYScale);

		// create the map singleton
		map = MapBehaviour.Instance;
		map.CurrentCamera = Camera.main;

        //setting up the map for touchscript
        map.gameObject.AddComponent<MeshCollider>();


        map.transform.parent = GameObject.Find("Scene").transform; 

      
        map.InputDelegate += UnitySlippyMap.Input.MapInput.BasicTouchAndKeyboard;

		map.CurrentZoom = 8.0f;
		// UVic

		map.CenterWGS84 = new double[2] { -123.310900, 48.460959 };
		map.UsesLocation = true;
		map.InputsEnabled = true;
		map.ShowsGUIControls = true;

		map.GUIDelegate += Toolbar;

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
	
	void Update()
	{
		if (destinationAngle != 0.0f)
		{
            // changing from 2D to 3D ortho angle

            Vector3 cameraLeft = Quaternion.AngleAxis(-90.0f, Camera.main.transform.up) * Camera.main.transform.forward;
			if ((Time.time - animationStartTime) < animationDuration)
			{
				float angle = Mathf.LerpAngle(0.0f, destinationAngle, (Time.time - animationStartTime) / animationDuration);
				Camera.main.transform.RotateAround(Vector3.zero, cameraLeft, angle - currentAngle);
				currentAngle = angle;
			}
			else
			{
				Camera.main.transform.RotateAround(Vector3.zero, cameraLeft, destinationAngle - currentAngle);
				destinationAngle = 0.0f;
				currentAngle = 0.0f;
				map.IsDirty = true;

			}
			
			map.HasMoved = true;
		}

	}
	
#if DEBUG_PROFILE
	void LateUpdate()
	{
		Debug.Log("PROFILE:\n" + UnitySlippyMap.Profiler.Dump());
		UnitySlippyMap.Profiler.Reset();
	}
#endif
}

