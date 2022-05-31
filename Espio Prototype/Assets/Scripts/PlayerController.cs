using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Animator anim;
    Rigidbody playerRB;
    private Vector3 moveDirection;

    [Header("Spin Attack")]
    public GameObject playerModel, spinMesh;
    [SerializeField] float spinSpeed;

    [Header("Movement")]
    [SerializeField] float currentSpeed, maxSpeed, acceleration, rotateSpeed;
    [SerializeField] bool isSprinting, slowingDown;

    [Header("Jumping")]
    public Transform groundCheck;
    public LayerMask ground;
    [SerializeField] float jumpForce;
    [SerializeField] bool canPressSpace, hasJumped, isGrounded;

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        playerRB = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        //Jumping
        canPressSpace = true;
        hasJumped = false;
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, 0.5f, ground);

        if (Input.GetButtonUp("Jump") && !hasJumped) //Check to stop infinite jumping.
        {
            canPressSpace = true;
        }

        if (isGrounded)
        {
            if (Input.GetButton("Jump") && canPressSpace)  //Sets Y position to match jumpSpeed identifies that player has performed the Jump action.
            {
                hasJumped = true;
            }

            if (hasJumped)  //Sets Jump animation and prevents player from additional jumps once the Jump action is performed.
            {
                anim.SetBool("Jumping", true);
                canPressSpace = false;
                hasJumped = false;
            }
            else anim.SetBool("Jumping", false);
        }

        moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;

        anim.SetFloat("Speed", currentSpeed);

        if (moveDirection.magnitude > Mathf.Epsilon && !slowingDown)
        {
            currentSpeed += acceleration * Time.deltaTime;

            if (currentSpeed >= 75)
            {
                isSprinting = true;
            }

            if (currentSpeed >= maxSpeed)
            {
                currentSpeed = maxSpeed;
            }
        }
        else
        {
            if (isSprinting)
            {
                if (playerRB.velocity.x != 0 || playerRB.velocity.z != 0)
                {
                    isSprinting = false;
                    slowingDown = true;
                }
            }

            currentSpeed = 0;
        }

        if(slowingDown)
        {
            if (playerRB.velocity.x != 0 || playerRB.velocity.z != 0)
            {
                anim.SetBool("SlowDown", true);
            }
            else
            {
                slowingDown = false;
                anim.SetBool("SlowDown", false);
            }
        }

        if (Input.GetKey(KeyCode.Mouse0))
        {
            anim.SetBool("Spinning", true);
            playerModel.transform.Rotate(Vector3.up, spinSpeed);
            spinMesh.SetActive(true);
            spinMesh.transform.Rotate(Vector3.up, spinSpeed);
        }
        else
        {
            anim.SetBool("Spinning", false);
            playerModel.transform.rotation = transform.rotation;
            spinMesh.SetActive(false);
            spinMesh.transform.rotation = transform.rotation;
        }
    }

    private void FixedUpdate()
    {
        if (moveDirection.magnitude > Mathf.Epsilon && !slowingDown)
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

        if (Input.GetKeyDown(KeyCode.Space) && !hasJumped)
        {
            hasJumped = true;
            Jump();
        }
    }

    void MovePlayer()
    {
        playerRB.drag = 0;
        playerRB.velocity = new Vector3(moveDirection.x * currentSpeed, playerRB.velocity.y, moveDirection.z * currentSpeed);
    }

    void RotatePlayer()
    {
        transform.rotation = Quaternion.LookRotation(playerRB.velocity);
    }

    void Jump()
    {
        Debug.Log("Has Jumped " + hasJumped);
        playerRB.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    void OnDrawGizmos()
    {   
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(groundCheck.position, 0.5f);
    }
}

