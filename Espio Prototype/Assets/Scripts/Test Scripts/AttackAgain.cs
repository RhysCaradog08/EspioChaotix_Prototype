using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackAgain : MonoBehaviour
{
    PlayerController pc;
    public float attackAgainTimer;

    // Start is called before the first frame update
    void Start()
    {
        pc = FindObjectOfType<PlayerController>();
        attackAgainTimer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(attackAgainTimer > 0)
        {
            attackAgainTimer -= Time.deltaTime;
        }
        else if(attackAgainTimer <= 0)
        {
            attackAgainTimer = 0;
        }
    }
}
