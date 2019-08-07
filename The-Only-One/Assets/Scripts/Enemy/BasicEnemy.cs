using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class BasicEnemy : MonoBehaviour
{
    public AIPath path_script;
    public ParticleSystem theStarSystem;

    [Range(5f, 50f)]
    public float RunningSpeed = 10f;

    [Range(5f, 50f)]
    public float RunningTime = 10f;//number of seconds they employee is willing to run after the player

    private void Start()
    {
        path_script = GetComponent<AIPath>();
        theStarSystem = GetComponent<ParticleSystem>();

        StopHunting();
        StopSeeingStars();
    }

    public void StartHunting()
    {
        if (path_script != null)
        {
            path_script.canSearch = true;
            path_script.canMove = true;
        }
    }

    public void StopHunting()
    {
        if (path_script != null)
        {
            path_script.canSearch = false;
            path_script.canMove = false;
        }
    }

    public void StopSeeingStars()
    {
        if (theStarSystem != null)
        {
            theStarSystem.Play(false);
        }
    }

    //when the player just hit the enemy, then start the particle effect.
    public void StartSeeingStars()
    {
        if (theStarSystem != null)
        {
            theStarSystem.Play(true);
        }
    }



}
