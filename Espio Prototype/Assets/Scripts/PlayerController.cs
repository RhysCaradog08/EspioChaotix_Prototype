using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Animator anim;
    private Vector3 moveDirection;
    Rigidbody playerRB;

    public GameObject model, spinMesh;

    [SerializeField] float currentSpeed, maxSpeed, acceleration, deceleration, jumpForce, rotateSpeed, spinSpeed;

    [SerializeField] bool hasJumped, isSprinting;

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        playerRB = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;

        anim.SetFloat("Speed", currentSpeed);

        if (moveDirection.magnitude > Mathf.Epsilon)
        {
            currentSpeed += acceleration * Time.deltaTime;

            if(currentSpeed >= 75)
            {
                isSprinting = true;
            }

            if (currentSpeed >= maxSpeed)
            {
                currentSpeed = maxSpeed;
            }

            //MovePlayer();
            //RotatePlayer();
        }
        else
        {
            if(isSprinting)
            {
                anim.SetTrigger("Slowdown");
                isSprinting = false;
            }

            transform.rotation = transform.rotation;

            currentSpeed -= deceleration * Time.deltaTime;

            if (currentSpeed <= 0)
            {
                currentSpeed = 0;
            }
            anim.ResetTrigger("SlowDown");
        }

        if (Input.GetKey(KeyCode.Mouse0))
        {
            anim.SetBool("Spinning", true);
            model.transform.Rotate(Vector3.up, spinSpeed);
            spinMesh.SetActive(true);
            spinMesh.transform.Rotate(Vector3.up, spinSpeed);
        }
        else
        {
            anim.SetBool("Spinning", false);
            model.transform.rotation = transform.rotation;
            spinMesh.SetActive(false);
            spinMesh.transform.rotation = transform.rotation;
        }
    }

    private void FixedUpdate()
    {
        if (moveDirection.magnitude > Mathf.Epsilon)
        {
            MovePlayer();
            RotatePlayer();
        }
        else
        {
            if (isSprinting)
            {
                playerRB.drag = 5;
            }
            else playerRB.drag = 2;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
    }

    void MovePlayer()
    {
        //transform.Translate(moveDirection * currentSpeed * Time.deltaTime, Space.World);

        playerRB.drag = 0;
        playerRB.velocity = new Vector3(moveDirection.x * currentSpeed, playerRB.velocity.y, moveDirection.z * currentSpeed);
    }

    void RotatePlayer()
    {
        transform.rotation = Quaternion.LookRotation(playerRB.velocity);
    }

    void Jump()
    {
        playerRB.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }
}

