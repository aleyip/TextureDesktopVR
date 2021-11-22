using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;
using UnityEngine;

public class MouseTest : MonoBehaviour
{
    //[DllImport("user32", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
    //public static extern void mouse_event(Int32 dwFlags, Int32 dx, Int32 dy, Int32 cButtons, Int32 dwExtraInfo);

    public struct mousePos
    {
        public int x;
        public int y;
    }

    [DllImport("user32.dll")]
    public static extern bool GetCursorPos(out mousePos mPos);

    private const int click_left = 0x02;
    private const int unclick_left = 0x04;
    private const int click_right = 0x08;
    private const int unclick_right = 0x10;
    //These are variables that we will send them into move function
    //Purpose of these variables is prevent confusing due to hex codes. We have equalized the hex values to the variables.

    private void SagTikla()
    {

        //X position of the cursor on screen
        //int mouse_x_location = (int)Input.mousePosition.x;

        ////Y position of the cursor on screen
        //int mouse_y_location = (int)Input.mousePosition.y;

        //Debug.Log($"Trying x:{mouse_x_location} y:{mouse_y_location}");

        //mouse_event(click_right | unclick_right, mouse_x_location, mouse_y_location, 0, 0);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        mousePos m;
        if (GetCursorPos(out m))
        {
            Debug.Log($"x:{m.x} y:{m.y}");
        }
        //Debug.Log($"x:{Input.mousePosition.x} y:{Input.mousePosition.y}");

        if (Input.GetKeyDown("space"))
        {
            Debug.Log("Hello");
            SagTikla();
        }
    }
}
