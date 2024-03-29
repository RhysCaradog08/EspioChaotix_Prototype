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
    Vector3 spinAttackDir, targetPos;
    public GameObject playerModel, spinMesh;
    public Transform target;
    [SerializeField] float spinSpeed, chargeSpeed, spinTime, dashTime, attackRadius, attackSpeed, attackAgainTime, orbitSpeed;
    [SerializeField] bool chargingSpin, spinDashing, isAttacking;
    public bool isSpinning;

    [Header("Knockback")]
    [SerializeField] float knockbackForce;
    ContactPoint contactPoint;
    public bool isKnockedBack;


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
        //target = null;
        spinTime = 0;
        attackAgainTime = 0;
        chargingSpin = false;
        spinDashing = false;
        isAttacking = false;

        //Knockback
        isKnockedBack = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(target != null)
        {
            Debug.Log("Target: " + target.name);
        }

        isGrounded = Physics.CheckSphere(groundCheck.position, 0.5f, ground); //CheckSphere to determine if gorunded.

        if (Input.GetKeyUp(KeyCode.Space) && !hasJumped) //Check to stop infinite jumping.
        {
            canPressSpace = true;
        }

        if (slowingDown || chargingSpin || isAttacking) //Determine whether player can move via input control;
        {
            canMove = false;
        }
        else canMove = true;

        if (attackAgainTime > 0)
        {
            attackAgainTime -= Time.deltaTime;

            float rotateClockwise = Input.GetAxis("Horizontal") * -orbitSpeed;
            transform.RotateAround(target.transform.position, Vector3.up, rotateClockwise * Time.deltaTime);

        }
        else if (attackAgainTime <= 0)
        {
            if (target != null)
            {
                target = null;
            }

            attackAgainTime = 0;
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
                rb.velocity = Vector3.zero;
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
            Collider[] hitColliders = Physics.OverlapSphere(new Vector3(transform.position.x, transform.position.y, transform.position.z + attackRadius), attackRadius);
            foreach (Collider hitColl in hitColliders)
            {
                if(hitColl.gameObject.tag == "Target")
                {
                    Debug.Log("Target in Radius");
                    target = hitColl.gameObject.transform;
                    targetPos = new Vector3(target.position.x, transform.position.y, target.position.z);
                    spinAttackDir = targetPos - transform.position;
                    isAttacking = true;

                    rb.velocity = Vector3.zero;
                }
            }

            if(!isSpinning)
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

            if (isAttacking)
            {
                rb.velocity = spinAttackDir * attackSpeed; //Player moves towards target being attacked.
            }

            if (isKnockedBack)
            {
                rb.velocity = Vector3.zero;
                rb.ResetCenterOfMass();
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

    void Rebound()
    {
        //Calculate angle between the collision point and the player.
        Vector3 dir = contactPoint.point - transform.position;

        //Get the opposite (-Vector3) and normalise it.
        dir = new Vector3(dir.x, 1, dir.z);
        dir = -dir.normalized;
        Debug.Log("Knockback Direction: " + dir);

        rb.velocity = Vector3.zero;
        rb.ResetCenterOfMass();

        //Add force to knock player back.
        rb.AddForce(dir * knockbackForce, ForceMode.Impulse);
    }

    void CreateTargetRadiusBounds()
    {
        float radius = 400; //radius of target
        Vector3 centerPosition = target.localPosition; //center of target radius
        float distance = Vector3.Distance(transform.position, centerPosition); //distance from player to target

        if (distance > radius) //If the distance is less than the radius, it is already within the radius.
        {
            Vector3 fromOriginToObject = transform.position - centerPosition; //player - target
            fromOriginToObject *= radius / distance; //Multiply by radius //Divide by Distance
            transform.position = centerPosition + fromOriginToObject; //target + all that Math
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isSpinning)
        {
            if (collision.gameObject.layer == 7)
            {
                Destroy(collision.gameObject);
                target = null;
            }

            if (collision.gameObject.layer == 6)
            {
                ContactPoint contactPoint = collision.GetContact(0);

                if (collision.gameObject.activeInHierarchy)
                {
                    //attackAgainTime = 1;
                    Rebound();
                }
            }

            if (isAttacking)
            {
                isAttacking = false;
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(groundCheck.position, 0.5f);
        Gizmos.DrawWireSphere(new Vector3(transform.position.x, transform.position.y, transform.position.z + attackRadius), attackRadius);
    }
}

