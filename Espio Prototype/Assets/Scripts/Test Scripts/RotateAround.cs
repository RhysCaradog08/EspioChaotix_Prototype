using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAround : MonoBehaviour
{
    PlayerController pc;
    public Transform target;

    public float OrbitSpeed = 20f;

    private void Start()
    {
        pc = FindObjectOfType<PlayerController>();
    }

    void Update()
    {
        target = pc.target;
        float rotateClockwise = Input.GetAxis("Horizontal") * -OrbitSpeed;
        Debug.Log("Rotate Clockwise: " + rotateClockwise);
        // Spin the object around the target at 20 degrees/second.

        
        transform.RotateAround(target.transform.position, Vector3.up, rotateClockwise * Time.deltaTime);
    }

}
