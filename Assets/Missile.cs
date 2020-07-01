using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField]
    float speed;

    GameObject arCamera;

    MeshRenderer meshRenderer;

    Transform sphereTransform; 

    void Start()
    {
        Debug.Log("Missile instance started");

        this.arCamera = GameObject.Find("AR Camera");

        this.meshRenderer = GetComponent<MeshRenderer>();

        this.sphereTransform = this.transform.parent; 

        this.speed = 0.5f;

        StartCoroutine(Flash()); 
    }

    IEnumerator Flash()
    {
        float delay = 1;

        Color original = this.meshRenderer.material.color; 

        while (delay > 0.01)
        {
            this.meshRenderer.material.color = Color.red;

            yield return new WaitForSeconds(0.05f);

            this.meshRenderer.material.color = original;

            yield return new WaitForSeconds(delay);

            delay *= 0.8f; 
        }
    }

    // Update is called once per frame
    void Update()
    {

        this.sphereTransform.position = Vector3.MoveTowards(
            this.sphereTransform.position,
            this.arCamera.transform.position,
            this.speed * Time.deltaTime
            ); 
    }
}
