using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spin : MonoBehaviour
{
    Transform controlller;
    public GameObject model;
    public GameObject spinOverlay;
    public float spinSpeed;

    private void Start()
    {
        controlller = transform.root;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            model.transform.Rotate(Vector3.up, spinSpeed);
            spinOverlay.SetActive(true);
            spinOverlay.transform.Rotate(Vector3.up, spinSpeed);
        }
        else
        {
            model.transform.rotation = controlller.rotation;
            spinOverlay.SetActive(false);
            spinOverlay.transform.rotation = controlller.rotation;
        }
    }
}
