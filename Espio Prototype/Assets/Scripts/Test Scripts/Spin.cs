using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spin : MonoBehaviour
{
    Transform controller;
    public GameObject model;
    public GameObject spinMesh;
    public float spinSpeed;

    private void Start()
    {
        controller = transform.root;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            model.transform.Rotate(Vector3.up, spinSpeed);
            spinMesh.SetActive(true);
            spinMesh.transform.Rotate(Vector3.up, spinSpeed);
        }
        else
        {
            model.transform.rotation = controller.rotation;
            spinMesh.SetActive(false);
            spinMesh.transform.rotation = controller.rotation;
        }
    }
}
