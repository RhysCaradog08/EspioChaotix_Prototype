using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinDash : MonoBehaviour
{
    Rigidbody rb;

    public float dashSpeed, dashTime;
    public bool isDashing;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        isDashing = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(Input.GetKeyDown(KeyCode.Mouse1))
        {
            /*isDashing = false;
            dashSpeed = 0;*/
            dashTime = 0;
        }

        if (Input.GetKey(KeyCode.Mouse1))
        {
            //rb.constraints = RigidbodyConstraints.FreezeAll;

            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                dashSpeed += 25;
            }

            if (dashSpeed >= 100)
            {
                dashSpeed = 100;
            }
        }
        else
        {
            rb.constraints = RigidbodyConstraints.FreezeRotation;
        }

        if (Input.GetKeyUp(KeyCode.Mouse1) && dashTime < 0.01)
        {
            dashTime = 3;
        }

        if (dashTime > 0)
        {
            dashTime -= Time.deltaTime;
            isDashing = true;
        }
        else isDashing = false;

        /*if (Input.GetKeyUp(KeyCode.Mouse1) && dashSpeed > 0)
        {
            isDashing = true;
        }*/

        if (isDashing)
        {
            rb.velocity = Vector3.forward * dashSpeed;
        }
        else rb.velocity = Vector3.zero;

    }
}
