using System;
using System.Collections;
using System.Collections.Generic;
using WinCapture;
using UnityEngine;

public class NotePadApp : BaseApplication
{
    // Update is called once per frame
    new void Update()
    {
        //TOMAR CUIDADO, ORDENADAS DA JANELA EH DE CIMA PRA BAIXO, TELA DE BAIXO PRA CIMA?
        
        var pointer = Win32Funcs.FindWindowEx(windowsRender.windowInfo.hwnd, IntPtr.Zero, "edit", null);
        if (pointer != IntPtr.Zero)
        {
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
                int foo = Win32Funcs.PostMessage(pointer, Win32Types.command.WM_MOUSEMOVE, 0, oldMousePos - upLeftPos);
                Debug.Log(foo);
                mousePosChanged = false;
            }


            if (Input.GetMouseButtonDown(0))
            {
                Debug.Log("Pressed primary button");
                int foo = Win32Funcs.PostMessage(pointer, Win32Types.command.WM_LBUTTONDOWN, 0, oldMousePos - upLeftPos);
                Debug.Log(foo);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                Debug.Log("Unpressed primary button");
                int foo = Win32Funcs.PostMessage(pointer, Win32Types.command.WM_LBUTTONUP, 0, oldMousePos - upLeftPos);
                Debug.Log(foo);
            }
            else if (Input.anyKeyDown)
            {
                string str = Input.inputString;
                Debug.Log("Trying click " + str);
                foreach (char c in str)
                {
                    int foo;
                    foo = Win32Funcs.PostMessage(pointer, Win32Types.command.WM_KEYDOWN, c, 0x0001);
                    Debug.Log($"{foo}  { Convert.ToInt32(c)}");
                }
            }
        }
        else
            Debug.Log("FALSE");


        base.Update();
    }
}
