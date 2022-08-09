using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Animator anim;
    Rigidbody rb;
    CapsuleCollider capCollider;
    [SerializeField] float capsuleStartScale, capsuleSpinScale;

    [Header("Movement")]
    private Vector3 moveDirection;
    [SerializeField] float maxSpeed, acceleration, currentSpeed, storeSpeed, rotateSpeed;
    //public float currentSpeed;
    [SerializeField] bool canMove, isSprinting, slowingDown;
    public bool knockedBack;

    [Header("Jumping")]
    public Transform groundCheck;
    public LayerMask ground;
    [SerializeField] float jumpForce, jumpHeight, timeToJumpApex, gravityScale, fallingGravityScale;
    [SerializeField] bool isGrounded, canPressSpace, hasJumped;

    [Header("Spin Attack")]
    [SerializeField ]Transform target;
    Vector3 spinAttackDir;
    public GameObject playerModel, spinMesh;
    [SerializeField] float spinSpeed, chargeSpeed, spinTime, dashTime, attackRadius, attackSpeed;
    [SerializeField] bool chargingSpin, spinDashing, isAttacking;
    public bool isSpinning;

    [Header("Knockback")]
    [SerializeField] float knockbackForce, knockbackTimer;


    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
        capCollider = GetComponent<CapsuleCollider>();
    }

    private void Start()
    {
        capCollider.radius = capsuleStartScale;

        //Movement
        canMove = true;
        knockedBack = false;

        //Jumping
        gravityScale = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        jumpForce = Mathf.Abs(gravityScale) * timeToJumpApex;
        canPressSpace = true;
        hasJumped = false;
        
        //Spin Attack
        spinTime = 0;
        chargingSpin = false;
        spinDashing = false;
        isAttacking = false;
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, 0.5f, ground); //CheckSphere to determine if gorunded.

        if (Input.GetKeyUp(KeyCode.Space) && !hasJumped) //Check to stop infinite jumping.
        {
            canPressSpace = true;
        }

        if (slowingDown || chargingSpin || isAttacking || knockbackTimer > 0) //Determine whether player can move via input control;
        {
            canMove = false;
        }
        else canMove = true;

        if(knockbackTimer > 0)
        {
            knockbackTimer -= Time.deltaTime;
        }
        else if (knockbackTimer <= 0)
        {
            knockbackTimer = 0;
        }

        if (isGrounded)
        {
            anim.SetBool("Falling", false);

            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                if(spinDashing)
                { 
                    spinDashing = false;
                }

                chargeSpeed = 0; //Resets chargeSpeed value;
            }

            if (Input.GetKey(KeyCode.Mouse1) && currentSpeed < 0.01f) //If player is stationary they can begin charging Spin Dash.
            {
                chargingSpin = true;
            }
            else chargingSpin = false;

            if (Input.GetKeyUp(KeyCode.Mouse1) && chargeSpeed > 0)
            {
                dashTime = 1;
            }

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
        else if (!isGrounded)
        {
            if (rb.velocity.y > 0)
            {
                anim.SetBool("Jumping", true); //Resets Jumping animation.
            }
            else if (rb.velocity.y < 0) //Changes to falling animation as rigidbody velocity begins descending.
            {
                anim.SetBool("Jumping", false);
                anim.SetBool("Falling", true);
            }
        }

        anim.SetFloat("Speed", currentSpeed);

        moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;

        if (moveDirection.magnitude > Mathf.Epsilon && canMove)
        {
            if (!isAttacking)
            {
                if (isGrounded)
                {
                    if (!spinDashing)
                    {
                        currentSpeed += acceleration * Time.deltaTime;  //Gradually increment player speed when not st full speed.
                    }
                }

                if (currentSpeed >= 100)
                {
                    isSprinting = true; //Player changes from running to sprinting.
                }

                if (currentSpeed >= maxSpeed)
                {
                    currentSpeed = maxSpeed; //Limit currentSpeed.
                }
            }
            else if(isAttacking)
            {
                currentSpeed = 0;
            }
        }
        else if (moveDirection.magnitude < 0.01)
        {
            if (isSprinting)
            {
                if (rb.velocity.x != 0 || rb.velocity.z != 0) //Slowly change player parameters from sprinting to slowing down.
                {
                    isSprinting = false;
                    spinDashing = false;
                    slowingDown = true;
                }
            }

            if (!isSpinning)
            {
                currentSpeed = 0;
            }
        }

        if (chargingSpin)
        {
            Debug.Log("Charging/Spin Dashing - Cannot Move");
            if (Input.GetKeyDown(KeyCode.Mouse0)) //Increment chargeSpeed.
            {
                if(chargeSpeed <= 0.01f)
                {
                    chargeSpeed = 50;
                }

                chargeSpeed += 50;
            }

            if (chargeSpeed > 0) //Set spinning animation increasing speed of spin with chargeSpeed.
            {
                isSpinning = true;
                spinSpeed = (chargeSpeed / 25) + 2;

                if (spinSpeed >= 10) //Cap spinSpeed.
                {
                    spinSpeed = 10;
                }
            }

            if (chargeSpeed >= maxSpeed) //Limit chargeSpeed to maxSpeed.
            {
                chargeSpeed = maxSpeed;
            }
        }

        if (dashTime > 0)
        {
            dashTime -= Time.deltaTime; //Player is in spinDash state whilst dashTime > 0.
            spinDashing = true;
        }
        else if (dashTime <= 0)
        {
            dashTime = 0;
        }

        if (spinDashing)
        {
            isSpinning = true;
            currentSpeed = chargeSpeed; //Match current speed to chargeSpeed.
        }

        if (slowingDown)
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

        if(Input.GetKeyDown(KeyCode.Mouse0)) //Create OverlapShere to find target for player to spin attack towards.
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRadius);
            foreach (Collider hitColl in hitColliders)
            {
                if(hitColl.gameObject.layer == 6)
                {
                    Debug.Log("Target in Radius");
                    target = hitColl.gameObject.transform;
                    spinAttackDir = target.position - transform.position;
                    isAttacking = true;
                    //StartCoroutine(TargetAttack());
                }
            }

        }

        if (Input.GetKeyDown(KeyCode.Mouse0) && !isSpinning) //Set player Spinning state.
        {
            if (isSprinting)
            {
                isSpinning = true;
            }
            else
            {
                if (!slowingDown)
                {
                    spinTime = 0.5f;
                }
            }
        }

        if (!isSprinting)
        {
            if (spinTime > 0)
            {
                isSpinning = true;
                spinTime -= Time.deltaTime;
            }
            else if (spinTime <= 0 && !chargingSpin && !spinDashing)
            {
                isSpinning = false;
                spinTime = 0;
            }
        }

        if(isSpinning) //Set player animtion state to spinning then rotate playerModel and activated spinMesh to achieve spin effect.
        {
            capCollider.radius = capsuleSpinScale;
            anim.SetBool("Spinning", true);
            playerModel.transform.Rotate(Vector3.up, spinSpeed);
            spinMesh.SetActive(true);
            spinMesh.transform.Rotate(Vector3.up, spinSpeed);
        }
        else   //Reset playerModel and spinMesh  transforms to match player rotation.
        {
            capCollider.radius = capsuleStartScale;
            anim.SetBool("Spinning", false);
            playerModel.transform.rotation = transform.rotation;
            spinMesh.SetActive(false);
            spinMesh.transform.rotation = transform.rotation;
        }
    }

    private void FixedUpdate()
    {
        if (moveDirection.magnitude > Mathf.Epsilon && !slowingDown) //Move player rigidbody.
        {
            rb.drag = 0;
            MovePlayer();
            RotatePlayer();
        }
        else
        {
            if(isAttacking)
            {
                rb.velocity = spinAttackDir * attackSpeed; //Player moves towards target being attacked.
            }

            if (spinDashing)
            {
                rb.velocity = transform.forward * (chargeSpeed + 50); //Dash in players forward vector if spin dashing.
            }

            if (isSprinting) //Set rigidbody drag to slow player velocity, higher value for moving at a faster speed.
            {
                rb.drag = 5;
            }
            else rb.drag = 2;

            if (isGrounded)
            {
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z); //Resets vertical velocity when grounded.
            }
        }

        if (isGrounded)
        {
            if (!chargingSpin)
            {
                rb.isKinematic = false;

                if (Input.GetKey(KeyCode.Space) && canPressSpace)
                {
                    Jump();
                }
            }
            else if(chargingSpin)
            {
                rb.isKinematic = true; //Keep player in place whilst chargingSpin.
            }
        }
        else rb.AddForce(Vector3.up * gravityScale, ForceMode.Acceleration); //Add gravityScale to player Rigibody to force them back to ground.
    }

    void MovePlayer()
    {
        rb.velocity = new Vector3(moveDirection.x * currentSpeed, rb.velocity.y, moveDirection.z * currentSpeed); //Move player rigidbody along x and z axis.
    }

    void RotatePlayer() //Rotate player transform to match moveDirection inputs.
    {
        Vector3 forwardDir = new Vector3(moveDirection.x, 0, moveDirection.z);
        transform.rotation = Quaternion.LookRotation(forwardDir, Vector3.up);
    }

    void Jump()
    {
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse); //Force player rigidbody upwards in Y axis to simulate jumping.
    }

    IEnumerator TargetAttack()
    {
        float startTime = Time.time;

        new WaitForSeconds(1); //Prevents player from stacking Barges.

        while (Time.time < startTime + 1)  //Player movement speed is disabled then moved by attackSpeed over 1 second;
        {
            isAttacking= true;
            //trailEffect.SetActive(true);
            currentSpeed = 0;

            rb.velocity = spinAttackDir * attackSpeed; //Player moves towards target being attacked.

            yield return null;
        }
    }

        void OnDrawGizmos()
    {   
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(groundCheck.position, 0.5f);
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(isAttacking)
        {
            isAttacking = false;
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("Target"))
        {
            target = null;
        }

        if(collision.gameObject.tag == "Destructible")
        {
            Destroy(collision.gameObject);
        }

        if (collision.gameObject.tag == "Knockback")
        {
            Debug.Log("Collided with object");

            if (isSpinning)
            {
                //Calculate angle between the collision point and the player.
                ContactPoint contactPoint = collision.GetContact(0);
                Vector3 playerPos = transform.position;
                Vector3 dir = contactPoint.point - playerPos;

                //Get the opposite (-Vector3) and normalise it.
                dir = new Vector3(dir.x, 1, dir.z);
                dir = -dir.normalized;
                Debug.Log("Knockback Direction: " + dir);

                rb.velocity = Vector3.zero;
                rb.ResetCenterOfMass();

                //Set knockBack while being forced back.
                knockbackTimer = 1;

                //Add force to knock player back.
                rb.AddForce(dir * knockbackForce, ForceMode.Impulse);
            }
        }
    }
}

