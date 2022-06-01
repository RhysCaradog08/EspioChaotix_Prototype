using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jump : MonoBehaviour
{
    Rigidbody rb;
    [SerializeField]Transform groundCheck;
    [SerializeField] LayerMask ground;

    [SerializeField] float jumpForce, jumpHeight, timeToJumpApex, gravityScale;

    [SerializeField]bool isGrounded, canPressSpace, hasJumped;

    
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        gravityScale = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        jumpForce = Mathf.Abs(gravityScale) * timeToJumpApex;
    }

    private void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, 0.5f, ground);

        if (Input.GetKeyUp(KeyCode.Space) && !hasJumped) //Check to stop infinite jumping.
        {
            canPressSpace = true;
        }

        if (isGrounded)
        {
            if (Input.GetKey(KeyCode.Space) && canPressSpace)  //Sets Y position to match jumpSpeed identifies that player has performed the Jump action.
            {
                hasJumped = true;
            }

            if (hasJumped)  //Sets Jump animation and prevents player from additional jumps once the Jump action is performed.
            {
                canPressSpace = false;
                hasJumped = false;
            }
        }
    }


    void FixedUpdate()
    {
        if (isGrounded)
        {
            if (Input.GetKey(KeyCode.Space) && canPressSpace)
            {
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            }
            else rb.velocity = Vector3.zero;
        }
        else rb.AddForce(Vector3.up * gravityScale, ForceMode.Acceleration);
    }
}
