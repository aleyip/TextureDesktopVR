using System;
using System.Collections;
using System.Collections.Generic;
using WinCapture;
using UnityEngine;

public class PaintApp : BaseApplication
{
    // Update is called once per frame
    new void Update()
    {
        var pointer = Win32Funcs.FindWindowEx(windowsRender.windowInfo.hwnd, IntPtr.Zero, "MSPaintView", null);
        Debug.Log($"A {pointer != IntPtr.Zero}");
        //parte do principio que a primeira janela child é a que precisamos pra desenhar
        pointer = Win32Funcs.FindWindowEx(pointer, IntPtr.Zero, null, null);
        Debug.Log($"B {pointer != IntPtr.Zero}");
        if (pointer != IntPtr.Zero)
        {
            if (mousePosChanged)
            {
                Debug.Log(Win32Funcs.PostMessage(pointer, Win32Types.command.WM_MOUSEMOVE, 0, oldMousePos));
                mousePosChanged = false;
            }

            if (Input.GetMouseButtonDown(0))
            {
                Debug.Log("Pressed primary button");
                Debug.Log(Win32Funcs.PostMessage(pointer, Win32Types.command.WM_LBUTTONDOWN, 0, oldMousePos));
            }
            else if (Input.GetMouseButtonUp(0))
            {
                Debug.Log("Unpressed primary button");
                Debug.Log(Win32Funcs.PostMessage(pointer, Win32Types.command.WM_LBUTTONUP, 0, oldMousePos));
            }
            else if (Input.anyKeyDown)
            {
                string str = Input.inputString;
                Debug.Log("Trying click " + str);
                foreach (char c in str)
                    Debug.Log(Win32Funcs.PostMessage(pointer, Win32Types.command.WM_CHAR, c, 0x0001));
            }
        }
        else
            Debug.Log("FALSE");


        base.Update();
    }
}
