using System;
using System.Collections;
using System.Collections.Generic;
using WinCapture;
using UnityEngine;

public class PaintApp : BaseApplication
{
    int lastHit;

    // Update is called once per frame
    new void Update()
    {
        //Tentativa de implementar usando a janela pai - nao funcionou

        //var pointer = windowsRender.windowInfo.hwnd;
        ////parte do principio que a primeira janela child � a que precisamos pra desenhar
        //if (pointer != IntPtr.Zero)
        //{
        //    int foo;
        //    if (mousePosChanged)
        //    {
        //        lastHit = Win32Funcs.SendMessage(pointer, Win32Types.command.WM_NCHITTEST, 0, oldMousePos);
        //        Debug.Log(lastHit);
        //        mousePosChanged = false;
        //    }

        //    if (Input.GetMouseButtonDown(0))
        //    {
        //        Debug.Log("Pressed primary button");
        //        foo = Win32Funcs.SendMessage(pointer, Win32Types.command.WM_PARENTNOTIFY, Win32Types.command.WM_LBUTTONDOWN, oldMousePos);
        //        foo = Win32Funcs.SendMessage(pointer, Win32Types.command.WM_MOUSEACTIVATE, pointer.ToInt32(), Win32Types.command.WM_LBUTTONDOWN<<16|lastHit);
        //        Debug.Log(foo);
        //    }
        //    else if (Input.GetMouseButtonUp(0))
        //    {
        //        Debug.Log("Unpressed primary button");
        //        foo = Win32Funcs.SendMessage(pointer, Win32Types.command.WM_PARENTNOTIFY, Win32Types.command.WM_LBUTTONUP, oldMousePos);
        //        Debug.Log(foo);
        //    }
        //    else if (Input.anyKeyDown)
        //    {
        //        string str = Input.inputString;
        //        Debug.Log("Trying click " + str);
        //        foreach (char c in str)
        //        {
        //            foo = Win32Funcs.SendMessage(pointer, Win32Types.command.WM_CHAR, c, 0x0001);
        //            Debug.Log(foo);
        //        }
        //    }
        //}

        var pointer = Win32Funcs.FindWindowEx(windowsRender.windowInfo.hwnd, IntPtr.Zero, "MSPaintView", null);
        //parte do principio que a primeira janela child � a que precisamos pra desenhar
        pointer = Win32Funcs.FindWindowEx(pointer, IntPtr.Zero, null, null);
        if (pointer != IntPtr.Zero)
        {
            int foo;

            Int32 upLeftPos;
            {
                //Pega a posicao do canto superior esquerdo do canvas para posicionar corretamente o cursor
                Win32Types.RECT upLeftWindow, upLeftCanvas;
                Win32Funcs.GetWindowRect(windowsRender.windowInfo.hwnd, out upLeftWindow);
                Win32Funcs.GetWindowRect(pointer, out upLeftCanvas);
                Int32 xBorder = upLeftCanvas.Left - upLeftWindow.Left;
                Int32 yBorder = upLeftCanvas.Top - upLeftWindow.Top;
                upLeftPos = (Int32)((yBorder << 16) | xBorder);
            }
            if (mousePosChanged)
            {
                foo = Win32Funcs.PostMessage(pointer, Win32Types.command.WM_MOUSEMOVE, 0, oldMousePos - upLeftPos);
                Debug.Log(foo);
                mousePosChanged = false;
            }

            if (Input.GetMouseButtonDown(0))
            {
                Debug.Log("Pressed primary button");
                foo = Win32Funcs.PostMessage(pointer, Win32Types.command.WM_LBUTTONDOWN, 0, oldMousePos - upLeftPos);
                Debug.Log(foo);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                Debug.Log("Unpressed primary button");
                foo = Win32Funcs.PostMessage(pointer, Win32Types.command.WM_LBUTTONUP, 0, oldMousePos - upLeftPos);
                Debug.Log(foo);
            }
            else if (Input.anyKeyDown)
            {
                string str = Input.inputString;
                Debug.Log("Trying click " + str);
                foreach (char c in str)
                {
                    foo = Win32Funcs.PostMessage(pointer, Win32Types.command.WM_CHAR, c, 0x0001);
                    Debug.Log(foo);
                }
            }
        }
        else
            Debug.Log("FALSE");


        base.Update();
    }
}
