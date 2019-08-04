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
    Vector2 direction;//the last change captured (values other than (0,0))
    private Animator animator;

    public float WalkSpeed = 10.0f;
    public float RunSpeed = 15.0f;

    //[Range(5f, 50f)]
    //public float KickingRange = 10;
    CircleCollider2D KickCollider;

    public List<Rigidbody2D> employee_bodies_in_contact;
    public List<Rigidbody2D> employee_bodies_can_attack;
    public bool ReadyToAttack = false;

    [Range(0.0f, 100.0f)]
    public float KickForce = 20.0f;

    //to resemble the angle at which the player is allowed to kick the targetwhile facing a certain direction
    [Range(15f, 180f)]
    public float AngleOfAttack = 90.0f;//this will be split in half. As default, +/- 45 degrees from direction playe is facing

    public LayerMask TargetMask, ObstacleMask;
    
    public float MAX_kick_time = 0.8f;//1 second
    public float kicking_time = 0;
    public Kick_Progress_UI kicking_script;

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
        //KickCollider.radius = KickingRange;

        animator = GetComponent < Animator >();

        //forces animation and direction to start the same way.
        direction = new Vector2(0, -1);
        animator.SetFloat("moveX", direction.x);
        animator.SetFloat("moveY", direction.y);
        
        employee_bodies_in_contact = new List<Rigidbody2D>();
        kicking_script.StopWaiting();
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

        //Debug.Log("Change (RAW): " + change);

        UpdateAnimationAndMove();
        Attack();
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
            direction = change;
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

        //Debug.Log("Change: "+change);
    }

    void Attack()
    {
        if (kicking_time <= 0.0f)//if (ReadyToAttack)//&& Input.GetButton("Fire2"))
        {
            //Debug.Log("X pressed.");

            foreach (Rigidbody2D targetBody in employee_bodies_in_contact)
            {
                Vector2 dirr = targetBody.transform.position - transform.position;
                //Debug.DrawLine(transform.position, targetBody.transform.position, Color.blue);//works!
                //result is in degrees but relative to the origin where the base of the triangle is always on the x axis.
                float angle = Mathf.Atan(dirr.y / dirr.x) * Mathf.Rad2Deg;//the angle of which the player is to that target

                //the angle at which the animation is facing and 'using';
                //Debug.Log("MoveX: "+ animator.GetFloat("moveX"));
                //Debug.Log("MoveY: " + animator.GetFloat("moveY"));
                //Debug.Log("Change: " + change + "\tLast Direction: " + direction);

                //float anime_x = animator.GetFloat("moveX");
                //float anime_y = animator.GetFloat("moveY");

                float anime_x = direction.x; //would have check if other value was null but why bother?
                float anime_y = direction.y;

                float animation_angle = Mathf.Atan(anime_y / anime_x) * Mathf.Rad2Deg;

                //since origin of animation is (0,0), no need to subtract the value to find the difference between target value and origin. 
                //delta y = moveY - origin = moveY- 0= moveY  <-- See!! Same for delta X

                //Debug.Log("Current angle: " + angle );

                float x1 = transform.position.x;
                float y1 = transform.position.y;

                float x2 = targetBody.transform.position.x;
                float y2 = targetBody.transform.position.y;

                // TARGET
                if (x2 < x1)// if target body is on left side of origin
                {
                    angle += 180;//even if object is at 180 degrees (angle will be equal to 0  in this case and still want 0 + 180 = 180)
                }
                else if (x2 >= x1)//if target body is on right side of origin
                {
                    if (y2 < y1)//if target is below origin ONLY (in 4th quarter)
                    {
                        angle += 360;
                    }
                }

                // ANIMATION; origin is (0,0); which is refering to the animations in the animator and how they are positioned.
                if (anime_x < 0)// if target body is on left side of origin
                {
                    animation_angle += 180;//even if object is at 180 degrees (angle will be equal to 0  in this case and still want 0 + 180 = 180)
                }
                else if (anime_x >= 0)//if target body is on right side of origin
                {
                    if (anime_y < 0)//if target is below origin ONLY (in 4th quarter)
                    {
                        animation_angle += 360;
                    }
                }

                //Debug.Log("Radius: " + KickCollider.radius);
                // Need to divide the radius by 2, because it acts like a diameter. When you have 'gizmo' active on play mode, 
                //  you can see that the line is pass the radius of the circle.... 
                //This will resemble the direction the player is looking towards.
                float anime_angle_x = Mathf.Cos(animation_angle * Mathf.Deg2Rad) * KickCollider.radius / 2; //the radius is the magnitude
                float anime_angle_y = Mathf.Sin(animation_angle * Mathf.Deg2Rad) * KickCollider.radius / 2; //the radius is the magnitude
                Debug.DrawLine(transform.position, new Vector2(anime_angle_x + x1, anime_angle_y + y1), Color.cyan);

                //this will resemble direction the player is looking towards PLUS AngleOfAttack/2
                //works with or without mod 360
                //float anime_angle_x1 = Mathf.Cos(((animation_angle + (AngleOfAttack / 2) ) % 360 ) * Mathf.Deg2Rad) * KickCollider.radius; //the radius is the magnitude
                //float anime_angle_y1 = Mathf.Sin(((animation_angle + (AngleOfAttack / 2) ) % 360 ) * Mathf.Deg2Rad) * KickCollider.radius; //the radius is the magnitude
                float animation_angle_A = animation_angle + (AngleOfAttack / 2);
                float anime_angle_xA = Mathf.Cos(animation_angle_A * Mathf.Deg2Rad) * KickCollider.radius / 2; //the radius is the magnitude
                float anime_angle_yA = Mathf.Sin(animation_angle_A * Mathf.Deg2Rad) * KickCollider.radius / 2; //the radius is the magnitude
                Debug.DrawLine(transform.position, new Vector2(anime_angle_xA + x1, anime_angle_yA + y1), Color.magenta);

                //this will resemble direction the player is looking towards MINUS AngleOfAttack/2
                float animation_angle_B = animation_angle - (AngleOfAttack / 2);
                float anime_angle_xB = Mathf.Cos(animation_angle_B * Mathf.Deg2Rad) * KickCollider.radius / 2; //the radius is the magnitude
                float anime_angle_yB = Mathf.Sin(animation_angle_B * Mathf.Deg2Rad) * KickCollider.radius / 2; //the radius is the magnitude
                Debug.DrawLine(transform.position, new Vector2(anime_angle_xB + x1, anime_angle_yB + y1), Color.yellow);

                if (angle <= animation_angle_A && angle >= animation_angle_B)
                {
                    float distToTarget = Vector2.Distance(transform.position, targetBody.transform.position);

                    //shoot a raycast out and if there is nothing blocking it, hit the target.
                    if (!Physics2D.Raycast(transform.position, dirr.normalized, distToTarget, ObstacleMask))
                    {
                        Debug.DrawLine(transform.position, targetBody.transform.position, Color.red);

                        targetBody.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().enabled = true;//show outline on target
                        if (Input.GetButton("Fire2"))
                        {
                            kicking_time = MAX_kick_time;
                            ReadyToAttack = false;
                            kicking_script.StartWaiting();

                            Debug.DrawLine(transform.position, targetBody.transform.position, Color.white);
                            //targetBody.AddForce(new Vector2(anime_angle_x, anime_angle_y) * KickForce, ForceMode2D.Impulse);
                            targetBody.AddForce(dirr * KickForce, ForceMode2D.Impulse);
                        }
                    }
                }
                else //if not within range
                {
                    targetBody.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().enabled = false;//hide outline on target
                }



                //Debug.DrawLine(transform.position, targetBody.transform.position, Color.yellow);//works!

                //targetBody.AddForce(new Vector2(-0.71f, 0.71f)*KickForce, ForceMode2D.Impulse);

                //Debug.Log("Angle: " + angle + "\tAnime Angle: " + animation_angle);
                //Debug.Log("origin: ( " + x1 + ",  " + y1 + " )\ttarget: ( " + x2 + ",  " + y2 + " )");
            }
        }
        else
        {
            kicking_time -= Time.deltaTime;
            kicking_script.setvalue(kicking_time, MAX_kick_time);

            if (kicking_time < 0)
            {
                kicking_time = 0;
                kicking_script.StopWaiting();
            }
                

            foreach (Rigidbody2D targetBody in employee_bodies_in_contact)
            {
                targetBody.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().enabled = false;//show outline on target
            }
        }
    }

    void DrawAngleOfAttack()
    {
        //if(animation_angle )
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        GameObject targetObj = col.gameObject;
        //Debug.Log(targetObj.name + " : " + gameObject.name + " : " + Time.time);

        if (targetObj.layer == LayerMask.NameToLayer("Enemy"))//if object is part of the enemy layer
        {
            if (targetObj.transform.childCount > 0)
            {
                //targetObj.GetComponentInChildren<SpriteRenderer>().enabled = true;
                //targetObj.transform.GetChild(0).gameObject.SetActive(true);
                //targetObj.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().enabled = true;//show outline on target

                employee_bodies_in_contact.Add(targetObj.gameObject.GetComponent<Rigidbody2D>());
                ReadyToAttack = true;
            }

        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        GameObject targetObj = col.gameObject;
        //Debug.Log(targetObj.name + " : " + gameObject.name + " : " + Time.time);

        if (targetObj.layer == LayerMask.NameToLayer("Enemy"))//if object is part of the enemy layer
        {
            if (targetObj.transform.childCount > 0)
            {
                //targetObj.GetComponentInChildren<SpriteRenderer>().enabled = false;
                //targetObj.transform.GetChild(0).gameObject.SetActive(false);
                targetObj.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().enabled = false;//hide outline on target
                employee_bodies_in_contact.Remove(targetObj.gameObject.GetComponent<Rigidbody2D>());

                if (employee_bodies_in_contact.Count == 0)
                {
                    ReadyToAttack = false;
                }
            }
        }

        
    }

    /*
    void AttackOLD()
    {
        if (ReadyToAttack && Input.GetButton("Fire2") )
        {
            Debug.Log("X pressed.");

            foreach(Rigidbody2D targetBody in employee_bodies_in_contact)
            {
                float angle = Vector2.Angle(transform.position, targetBody.transform.position);
                angle /= Mathf.PI;

                angle += 90;
                angle -= angle * 2 + 180;
                // convert degrees to radians
                angle *= Mathf.PI / 180;
                float magnitude = 1;
                float deltaX = Mathf.Cos(angle) * magnitude;
                float deltaY = Mathf.Sin(angle) * magnitude;
                Vector2 new_angle = new Vector2(deltaX, deltaY);

                Vector2 dirr = targetBody.transform.position - transform.position;
                //Debug.DrawRay(transform.position, dirr, Color.black);
                //Debug.DrawLine(transform.position, dirr, Color.blue);
                Debug.DrawLine(transform.position, targetBody.transform.position, Color.yellow);
                //Debug.DrawRay(transform.position, targetBody.transform.position, Color.white);

                float x_diff = targetBody.transform.position.x - transform.position.x;
                float y_diff = targetBody.transform.position.y - transform.position.y;
                angle = Mathf.Atan2(y_diff, x_diff) * Mathf.Rad2Deg;
                angle = Mathf.Atan2(dirr.y, dirr.x) ;
                float angle4 = Mathf.Atan(dirr.y / dirr.x) * Mathf.Rad2Deg;

                Debug.Log("Current angle: " + angle +"\tangle4: "+angle4);
                Debug.Log("Manual: "+ Mathf.Atan2(-2, -3));
                angle *= Mathf.Rad2Deg;
                float angle2 = Vector2.Angle(transform.position, targetBody.transform.position);
                float angle3 = angle;


                float x1 = transform.position.x;
                float x2 = targetBody.transform.position.x;
                float y1 = transform.position.y;
                float y2 = targetBody.transform.position.y;
                
                if (x2 < x1)// if target body is on left side of origin
                {
                    Debug.Log("Left side");
                    if (y2 != y1)//if target is above/below origin   <--- DON'T REALLY NEED THIS ENTIRE IF STATEMENT
                    {
                        Debug.Log("\tAbove/Below origin: angle3: "+angle3);
                        angle3 += 180;
                        
                    }
                    angle4 += 180;
                }
                else if (x2 >= x1)//if target body is on right side of origin
                {
                    Debug.Log("Right side");
                    if (y2 < y1)//if target is below origin ONLY
                    {
                        Debug.Log("In Q4");
                        angle3 += 360;
                        angle4 += 360;
                    }
                }

                Debug.Log("Angle: " + angle + "\tAngle2: "+angle2+ "\tAngle3: " + angle3  +"\tdirr: " +dirr);
                Debug.Log("Angle4: " + angle4 );// <-- the right angle to use!!
                Debug.Log("origin: ( " + x1 + ",  " + y1 + " )\ttarget: ( " + x2 + ",  " + y2 + " )");
            }
        }
    }*/


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
