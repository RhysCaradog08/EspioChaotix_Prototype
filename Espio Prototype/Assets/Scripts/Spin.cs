using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spin : MonoBehaviour
{
    public GameObject spinOverlay;
    public float spinSpeed;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            transform.Rotate(Vector3.up, -spinSpeed);
            spinOverlay.SetActive(true);
            spinOverlay.transform.Rotate(Vector3.up, -spinSpeed);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
            spinOverlay.SetActive(false);
            spinOverlay.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }
}
