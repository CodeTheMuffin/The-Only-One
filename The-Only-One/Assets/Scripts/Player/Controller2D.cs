using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 Inspired by 
 
    Movement and indirectly, animation: https://stuartspixelgames.com/2018/06/24/simple-2d-top-down-movement-unity-c/

    Mister Taft Creates (YouTube):    
    - Movement  : https://www.youtube.com/watch?v=--N5IgSUQWI&list=PL4vbr3u7UKWp0iM1WIfRjCDTI03u43Zfu&index=3
    - Animations: https://www.youtube.com/watch?v=Vfq13LRggwk&list=PL4vbr3u7UKWp0iM1WIfRjCDTI03u43Zfu&index=4

     */

public class Controller2D : MonoBehaviour
{
    Rigidbody2D body;

    float horizontal;
    float vertical;
    float moveLimiter = 0.71f; //~ sqrt(2)/2 
    Vector3 change;
    Vector3 direction;
    private Animator animator;

    public float WalkSpeed = 10.0f;
    public float RunSpeed = 15.0f;

    public float KickingRange = 10;
    CircleCollider2D KickCollider;

    /*
         * let sqrt(2)/2 be Z which is ~0.707 or 0.71
         * 
         * 
                             Looking up (0,1)
                Looking NW (-Z,Z)               Looking NE (Z,Z)
          Looking Left (-1,0)         center          Looking right (0,0)
                Looking SW (-Z,-Z)               Looking SE (Z,-Z)
                             Looking down (0,-1)
         */

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        KickCollider = GetComponent<CircleCollider2D>();
        animator = GetComponent < Animator >();
        direction = new Vector2(0,0);
    }

    /*void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");

        if (horizontal != 0 && vertical != 0) // Check for diagonal movement
        {
            // limit movement speed diagonally, so you move at 70% speed
            horizontal *= moveLimiter;
            vertical *= moveLimiter;
        }

        body.velocity = new Vector2(horizontal * runSpeed, vertical * runSpeed);
    }*/


    void FixedUpdate()
    {
        change = Vector3.zero;
        change.x = Input.GetAxisRaw("Horizontal");
        change.y = Input.GetAxisRaw("Vertical");

        UpdateAnimationAndMove();
    }

    void UpdateAnimationAndMove()
    {
        if (change != Vector3.zero)//
        {
            if (!Input.GetButton("Fire4") && Input.GetAxis("Fire4")==0)//if not holding down the [left Ctrl] OR [B button]
            {//then move
                MoveCharacter(true);
                animator.SetBool("isWalking", true);

            }
            else // stay in place but move in different direction
            {
                MoveCharacter(false);
                animator.SetBool("isWalking", false);
            }
            
        }
        else //not moving at all
        {
            body.velocity = Vector2.zero;
            animator.SetBool("isWalking", false);
        }
    }


    void MoveCharacter(bool isWalking)
    {
        if (change.x != 0 && change.y != 0) // Check for diagonal movement
        {
            change.x *= moveLimiter;
            change.y *= moveLimiter;
        }
        // limit movement speed diagonally, so you move at 71% speed
        if (isWalking)
        {
            body.MovePosition(transform.position + change * WalkSpeed * Time.deltaTime);
        }
        animator.SetFloat("moveX", change.x);
        animator.SetFloat("moveY", change.y);
    }

    void Attack()
    {
        if (Input.GetKeyDown("joystick button 2"))
        {
            Debug.Log("X pressed.");
        }
        
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        Debug.Log(col.gameObject.name + " : " + gameObject.name + " : " + Time.time);
    }

    /*
    private void FixedUpdate()
    {
        if (horizontal != 0 && vertical != 0) // Check for diagonal movement
        {
            // limit movement speed diagonally, so you move at 70% speed
            horizontal *= moveLimiter;
            vertical *= moveLimiter;
        }

        body.velocity = new Vector2(horizontal * runSpeed, vertical * runSpeed);
    }*/
}
