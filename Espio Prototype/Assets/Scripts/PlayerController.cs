using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Animator anim;
    private Vector3 moveDirection;
    Rigidbody playerRB;

    public GameObject model, spinMesh;

    [SerializeField] float currentSpeed, maxSpeed, acceleration, deceleration, jumpForce, rotateSpeed, spinSpeed, time;

    [SerializeField] bool hasJumped;

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
            if (currentSpeed >= maxSpeed)
            {
                currentSpeed = maxSpeed;
            }

            MovePlayer();
            RotatePlayer();
        }
        else
        {
            time = 0;
            transform.rotation = transform.rotation;

            currentSpeed -= deceleration * Time.deltaTime;

            if (currentSpeed <= 0)
            {
                currentSpeed = 0;
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            playerRB.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
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

    void MovePlayer()
    {
        transform.Translate(moveDirection * currentSpeed * Time.deltaTime, Space.World);
    }

    void RotatePlayer()
    {
        Quaternion targetRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotateSpeed);
    }
}

