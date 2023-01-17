using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knockback : MonoBehaviour
{
    PlayerController pc;
    Rigidbody rb;
    [SerializeField] float knockbackForce;

    private void Awake()
    {
        pc = FindObjectOfType<PlayerController>();
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Knockback")
        {
            Debug.Log("Collided with object");

            if (pc.isSpinning)
            {
                //Calculate angle between the collision point and the player.
                ContactPoint contactPoint = collision.GetContact(0);
                Vector3 playerPos = transform.position;
                Vector3 dir = contactPoint.point - playerPos;

                //Get the opposite (-Vector3) and normalise it.
                dir = new Vector3(dir.x, 1, dir.z);
                dir  = -dir.normalized;
                Debug.Log("Knockback Direction: " + dir);

                rb.velocity = Vector3.zero;
                rb.ResetCenterOfMass();

                //Disable PlayerController while being forced back.
                pc.enabled = false;
                Invoke("EnablePlayerController", 1);

                //Add force to knock player back.
                rb.AddForce(dir * knockbackForce, ForceMode.Impulse);
            }
        }
    }

    void EnablePlayerController()
    {
        pc.enabled = true;
    }

    void ResetKnockedBack()
    {
        pc.knockedBack = false;
    }
}
