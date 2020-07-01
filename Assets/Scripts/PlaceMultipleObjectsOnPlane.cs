using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation.Samples
{
    [RequireComponent(typeof(ARRaycastManager))]
    public class PlaceMultipleObjectsOnPlane : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Instantiates this prefab on a plane at the touch location.")]
        GameObject m_PlacedPrefab;

        /// <summary>
        /// The prefab to instantiate on touch.
        /// </summary>
        public GameObject placedPrefab
        {
            get { return m_PlacedPrefab; }
            set { m_PlacedPrefab = value; }
        }

        /// <summary>
        /// The object instantiated as a result of a successful raycast intersection with a plane.
        /// </summary>
        public GameObject spawnedObject { get; private set; }

        /// <summary>
        /// Invoked whenever an object is placed in on a plane.
        /// </summary>
        public static event Action onPlacedObject;

        ARRaycastManager m_RaycastManager;

        GameObject camera; 

        static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();

        List<GameObject> placedObjects = new List<GameObject>();

        GameObject drinkText;

        bool is_pc; // prototype by running on pc when possible (no ar)

        void Awake()
        {
            m_RaycastManager = GetComponent<ARRaycastManager>();

            this.camera = GameObject.Find("AR Camera");
            Debug.Log("Initial camera position: " + this.camera.transform.position.ToString());

            this.drinkText = GameObject.Find("DrinkText");
            Debug.Log("Have drink text: " + this.drinkText.ToString());
            this.drinkText.SetActive(false);

            this.is_pc = true; 
        }

        void Update()
        {
            if (this.is_pc)
            {
                this.AddMissileAt(new Pose(Vector3.zero, Quaternion.identity));
                this.camera.transform.position = this.camera.transform.position - new Vector3(0, 0, 2); 

                this.is_pc = false; 
            }

            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Began)
                {
                    if (m_RaycastManager.Raycast(touch.position, s_Hits, TrackableType.PlaneWithinPolygon))
                    {
                        Pose hitPose = s_Hits[0].pose;
                        this.AddMissileAt(hitPose); 
                    }
                }
            }

            // Debug.Log("Camera: " + this.camera.transform.position.ToString()); 

            GameObject deletedGO = null; 

            foreach (GameObject go in this.placedObjects)
            {
                // go.transform.position = Vector3.MoveTowards(go.transform.position, this.camera.transform.position, 0.005f); 

                //Debug.Log(go.transform.position.ToString()); 

                if (Vector3.Distance(go.transform.position, this.camera.transform.position) < 0.1)
                {
                    deletedGO = go; 
                }
            }

            if (deletedGO != null)
            {
                this.placedObjects.Remove(deletedGO);
                Destroy(deletedGO);
                Debug.Log("HIT");
                Handheld.Vibrate();

                this.drinkText.SetActive(true);
                Invoke("HideDrinkText", 0.5f); 

            }
        }

        void AddMissileAt(Pose hitPose)
        {
            spawnedObject = Instantiate(m_PlacedPrefab, hitPose.position, hitPose.rotation);

            this.placedObjects.Add(spawnedObject);

            Debug.Log("Placed object at : " + spawnedObject.transform.position.ToString());

            if (onPlacedObject != null)
            {
                onPlacedObject();
            }
        }

        void HideDrinkText()
        {
            this.drinkText.SetActive(false); 
        }
    }
}