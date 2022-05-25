using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Vector3 playerMovementInput;
    Rigidbody playerRB;

    [SerializeField] float speed, jumpForce, rotateSpeed;
    [SerializeField] Vector3 xRot;

    private void Awake()
    {
        playerRB = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        playerMovementInput = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        MovePlayer();
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(playerMovementInput), 10);
    }

    void MovePlayer()
    {
        Vector3 moveVector = transform.TransformDirection(playerMovementInput) * speed;
        playerRB.velocity = new Vector3(moveVector.x, playerRB.velocity.y, moveVector.z);
        transform.Rotate(xRot);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            playerRB.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }
}

