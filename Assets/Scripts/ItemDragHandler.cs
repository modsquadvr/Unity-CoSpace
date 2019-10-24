using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnitySlippyMap.Map;
using UnitySlippyMap.Markers;


public class ItemDragHandler : MonoBehaviour, IDragHandler, IDropHandler
{
    private GameObject canvas;
    private GameObject container;
    private MapBehaviour Map;

    

    public void OnDrag(PointerEventData eventData)
    {

        canvas = GameObject.Find("Canvas");
        transform.position = Input.mousePosition;
        this.transform.SetParent(canvas.transform, false);

    }

    public void OnDrop(PointerEventData eventData)
    {
        RectTransform invPanel = GameObject.Find("Menu").GetComponent<RectTransform>();


        //check to see if the mouse pointer is out of the Menu box
        if (!RectTransformUtility.RectangleContainsScreenPoint(invPanel, Input.mousePosition))
        {
            //hide the Image of 3d model
            this.gameObject.SetActive(false);
            string fileName = this.name;

            //figure out where to drop the modelin world space.
            RaycastHit hit = new RaycastHit();
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray, out hit, 1000))
            {
                GameObject building = Resources.Load<GameObject>("SKPFiles/" + fileName);
                building.transform.position = hit.point;

                //add components
                var prefab = Instantiate(building, building.transform.position, Quaternion.identity);
                prefab.AddComponent<MeshCollider>();

                //adjusting scale
                Vector3 scale = prefab.transform.localScale;
                scale.Set(0.0005f, 0.0005f, 0.0005f);
                prefab.transform.localScale = scale;


            }

        }


    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }



 
}
