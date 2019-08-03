using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Basic : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log(EventSystem.current.currentSelectedGameObject.name);
        if (Input.anyKey)
        {
            //Debug.Log(Input.);
        }

        if (Input.GetKeyDown("joystick button 0" )) { Debug.Log("A pressed."); }
        if (Input.GetKeyDown("joystick button 1")) { Debug.Log("B pressed."); }
        if (Input.GetKeyDown("joystick button 2")) { Debug.Log("X pressed."); }
        if (Input.GetKeyDown("joystick button 3")) { Debug.Log("Y pressed."); }
        if (Input.GetKeyDown("joystick button 4")) { Debug.Log("Left Bumber pressed."); }
        if (Input.GetKeyDown("joystick button 5")) { Debug.Log("Right Bumber pressed."); }
        if (Input.GetButton("Fire4")) { Debug.Log("Right Cltr (Fire4) pressed."); }
        if (Input.GetAxis("Fire4")!=0) {
            Debug.Log("Right Trigger (Fire4) pressed.");
            //Debug.Log(Input.GetAxis("Fire4"));
        }
        
    }
}
