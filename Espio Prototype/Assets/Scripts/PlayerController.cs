using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Animator anim;
    Rigidbody rb;
    private Vector3 moveDirection;

    [Header("Spin Attack")]
    public GameObject playerModel, spinMesh;
    public Transform spinPivot;
    [SerializeField] float spinSpeed, spinTime;
    [SerializeField] bool isSpinning;

    [Header("Movement")]
    [SerializeField] float currentSpeed, maxSpeed, acceleration, rotateSpeed;
    [SerializeField] bool isSprinting, slowingDown;

    [Header("Jumping")]
    public Transform groundCheck;
    public LayerMask ground;
    [SerializeField] float jumpForce, jumpHeight, timeToJumpApex, gravityScale, fallingGravityScale;
    [SerializeField] bool isGrounded, canPressSpace, hasJumped;

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        //Spin Attack
        spinTime = 0;

        //Jumping
        gravityScale = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        jumpForce = Mathf.Abs(gravityScale) * timeToJumpApex;
        canPressSpace = true;
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, 0.5f, ground);

        if (Input.GetKeyUp(KeyCode.Space) && !hasJumped) //Check to stop infinite jumping.
        {
            canPressSpace = true;
        }

        if (isGrounded)
        {
            anim.SetBool("Falling", false);

            if (Input.GetKey(KeyCode.Space) && canPressSpace)
            {
                hasJumped = true;
            }

            if (hasJumped)  //Sets Jump animation and prevents player from additional jumps once the Jump action is performed.
            {
                Debug.Log("Jumping");
                canPressSpace = false;
                hasJumped = false;
            }
        }
        else if(!isGrounded)
        {
            if(rb.velocity.y > 0)
            {
                anim.SetBool("Jumping", true);
            }
            else if(rb.velocity.y < 0)
            {
                anim.SetBool("Jumping", false);
                anim.SetBool("Falling", true);
            }
        }

        anim.SetFloat("Speed", currentSpeed);

        moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;

        if (moveDirection.magnitude > Mathf.Epsilon && !slowingDown)
        {
            if (isGrounded)
            {
                currentSpeed += acceleration * Time.deltaTime;
            }

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
                if (rb.velocity.x != 0 || rb.velocity.z != 0)
                {
                    isSprinting = false;
                    slowingDown = true;
                }
            }

            currentSpeed = 0;
        }

        if(slowingDown)
        {
            if (rb.velocity.x != 0 || rb.velocity.z != 0)
            {
                anim.SetBool("SlowDown", true);
            }
            else
            {
                slowingDown = false;
                anim.SetBool("SlowDown", false);
            }
        }

        if (Input.GetKeyDown(KeyCode.Mouse0) && !isSpinning)
        {
            if (isSprinting)
            {
                isSpinning = true;
            }
            else
            {
                spinTime = 0.5f;
            }
        }

        if (!isSprinting)
        {
            if (spinTime > 0)
            {
                isSpinning = true;
                spinTime -= Time.deltaTime;
            }

            else if (spinTime <= 0)
            {
                isSpinning = false;
                spinTime = 0;
            }
        }

        if(isSpinning)
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
                rb.drag = 5;
            }
            else rb.drag = 2;

            if (isGrounded)
            {
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            }
        }

        if (isGrounded)
        {
            if (Input.GetKey(KeyCode.Space) && canPressSpace)
            {
                Jump();
            }
        }
        else rb.AddForce(Vector3.up * gravityScale, ForceMode.Acceleration);
    }

    void MovePlayer()
    {
        rb.drag = 0;
        rb.velocity = new Vector3(moveDirection.x * currentSpeed, rb.velocity.y, moveDirection.z * currentSpeed);
    }

    void RotatePlayer()
    {
        Vector3 forwardDir = new Vector3(moveDirection.x, 0, moveDirection.z);
        transform.rotation = Quaternion.LookRotation(forwardDir, Vector3.up);
    }

    void Jump()
    {
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    void OnDrawGizmos()
    {   
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(groundCheck.position, 0.5f);
    }
}

