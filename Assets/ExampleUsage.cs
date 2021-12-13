using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using WinCapture;

public class ExampleUsage : MonoBehaviour {

    Shader windowShader;
    Shader desktopShader;
    Shader chromiumShader;
    WindowCaptureManager captureManager;

    public Dictionary<IntPtr, WindowCapture> windowsRendering;
    public Dictionary<IntPtr, GameObject> windowObjects;

    DesktopCapture desktopCapture1;
    GameObject desktopObject;

    ChromiumCapture chromiumCapture;
    GameObject chromiumObject;

    // Use this for initialization
    void Start () {

        windowShader = Shader.Find("WinCapture/WindowShader");
        desktopShader = Shader.Find("WinCapture/DesktopShader");
        chromiumShader = Shader.Find("WinCapture/ChromiumShader");

        windowsRendering = new Dictionary<IntPtr, WindowCapture>();
        windowObjects = new Dictionary<IntPtr, GameObject>();
        captureManager = new WindowCaptureManager();
        captureManager.OnAddWindow += OnAddWindow;
        captureManager.OnRemoveWindow += OnRemoveWindow;
        lastUpdateTime = Time.time;
        lastPollWindowsTime = Time.time;

        int displayNum = 0;
        desktopCapture1 = new DesktopCapture(displayNum);

        desktopObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
        desktopObject.name = "desktop" + displayNum;
        desktopObject.transform.GetComponent<Renderer>().material = new Material(desktopShader);
        desktopObject.transform.localEulerAngles = new Vector3(90, 0, 0);

        /*Isso nao funciona, problema em BrowserEngine::ConnectTcp aparentemente
        //chromiumCapture = new ChromiumCapture(1024, 1024, "http://google.com");

        //chromiumObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
        //chromiumObject.name = "chromium capture";
        //chromiumObject.transform.GetComponent<Renderer>().material = new Material(chromiumShader);
        //chromiumObject.transform.localEulerAngles = new Vector3(90, 0, 0);
        */
        Debug.Log("Hello");
    }

    // You need to do this because the desktop capture API will only work if we are on the graphics thread
    void OnPostRender()
    {
        desktopCapture1.OnPostRender();
    }


    bool IsGoodWindow(WindowCapture window)
    {
        Debug.Log("Saw window: " + window.windowInfo.title);
        // You can stick whatever logic or names you want here for windows you want to keep to render

        string windowLowerTitle = window.windowInfo.title.ToLower();
        if (windowLowerTitle.Contains("sem"))
        {
            return true;
        }
        return false;
    }

    void OnAddWindow(WindowCapture window)
    {
        if (!windowsRendering.ContainsKey(window.hwnd) && IsGoodWindow(window))
        {
            GameObject windowObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
            windowObject.name = window.windowInfo.title;
            windowObject.transform.GetComponent<Renderer>().material = new Material(windowShader);
            windowObject.transform.localEulerAngles = new Vector3(90, 0, 0);
            windowsRendering[window.hwnd] = window;
            windowObjects[window.hwnd] = windowObject;
            Debug.Log("Add");
        }
    }

    void OnRemoveWindow(WindowCapture window)
    {
        Debug.Log("removed " + window.windowInfo.title);
        if (windowsRendering.ContainsKey(window.hwnd))
        {
            GameObject windowObjectRemoving = windowObjects[window.hwnd];
            Destroy(windowObjectRemoving);
            windowObjects.Remove(window.hwnd);
            windowsRendering.Remove(window.hwnd);
        }
    }

    float lastUpdateTime;
    float lastPollWindowsTime;

    public float captureRateFps = 30;
    public float windowPollPerSecond = 2;
    public float windowScale = 0.001f;

    // Update is called once per frame
    void Update()
    {
        //TOMAR CUIDADO, ORDENADAS DA JANELA EH DE CIMA PRA BAIXO, TELA DE BAIXO PRA CIMA?
        Int32 mousePos;
        {
            Int32 xMouse = (Int32)Input.mousePosition.x;
            Int32 yMouse = (Int32)Input.mousePosition.y;
            mousePos = (Int32)((yMouse << 16) | xMouse);
        }

        //NotePad
        foreach (IntPtr key in windowsRendering.Keys)
        {
            Debug.Log($"title {windowsRendering[key].windowInfo.title} {windowsRendering[key].windowInfo.hwnd}");
            var pointer = Win32Funcs.FindWindowEx(windowsRendering[key].windowInfo.hwnd, IntPtr.Zero, "edit", null);
            if (pointer != IntPtr.Zero)
            {
                Debug.Log(Win32Funcs.PostMessage(pointer, Win32Types.command.WM_MOUSEMOVE, 0, mousePos));
                if (Input.GetMouseButtonDown(0))
                {
                    Debug.Log("Pressed primary button");
                    Debug.Log(Win32Funcs.PostMessage(pointer, Win32Types.command.WM_LBUTTONDOWN, 0, mousePos));
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    Debug.Log("Unpressed primary button");
                    Debug.Log(Win32Funcs.PostMessage(pointer, Win32Types.command.WM_LBUTTONUP, 0, mousePos));
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
        }
        //end NotePad

        //var pointer = Win32Funcs.FindWindowEx(windowsRendering[key].windowInfo.hwnd, IntPtr.Zero, "MSPaintView", null); //Classe Edit funciona pro notepad
        //pointer = Win32Funcs.FindWindowEx(pointer, IntPtr.Zero, "Afx:00007FF7A92F0000:8", null);
        //if (pointer != IntPtr.Zero)
        //{
        //    if (Input.GetMouseButtonDown(0))
        //    {
        //        Debug.Log("Pressed primary button");
        //        Debug.Log(Win32Funcs.PostMessage(pointer, WM_LBUTTONDOWN, 0, 0));
        //    }
        //    foreach (char c in str)
        //        Debug.Log(Win32Funcs.PostMessage(pointer, WM_CHAR, c, 0x0001));
        //}

        bool didChange;
        //Texture2D chromiumTexture = chromiumCapture.GetChromiumTexture(out didChange);
        //if (didChange)
        //{
        //    chromiumObject.GetComponent<Renderer>().material.mainTexture = chromiumTexture;
        //}
        //chromiumObject.transform.localScale = new Vector3(chromiumTexture.width * windowScale, 0.1f, chromiumTexture.height * windowScale);


        //if (Time.frameCount == 400)
        //{
        //    chromiumCapture.SetUrl("http://reddit.com");
        //}

        //if (Time.time - lastUpdateTime < 1.0f / captureRateFps)
        //{
        //    return;
        //}
        //else
        //{
        //    lastUpdateTime = Time.time;
        //}



        // Capture the desktop
        Texture2D desktopTexture = desktopCapture1.GetWindowTexture(out didChange);

        desktopObject.transform.localScale = new Vector3(desktopCapture1.desktopWidth * windowScale, 0.1f, desktopCapture1.desktopHeight * windowScale);
        if (didChange)
        {
            desktopObject.transform.GetComponent<Renderer>().material.mainTexture = desktopTexture;
        }

        // Gets information about position and icon of the cursor so you can render it onto the captured surfaces
        //WindowCapture.UpdateCursorInfo();

        // Capture each window
        //Debug.Log($"Number of windows: {windowsRendering.Count}");
        foreach (IntPtr key in windowsRendering.Keys)
        {
            //Debug.Log($"Name of windows: {windowsRendering[key].windowInfo.title}");
            WindowCapture window = windowsRendering[key];
            GameObject windowObject = windowObjects[key];

            if (windowObject == null)
            {
                continue;
            }

            Texture2D windowTexture = window.GetWindowTexture(out didChange);
            if (didChange)
            {
                windowObject.GetComponent<Renderer>().material.mainTexture = windowTexture;
            }
            windowObject.transform.localScale = new Vector3(window.windowWidth * windowScale, 0.1f, window.windowHeight * windowScale);
        }

        // Poll for new windows
        if (Time.time - lastPollWindowsTime < 1.0f / windowPollPerSecond)
        {
            return;
        }
        else
        {
            lastPollWindowsTime = Time.time;
            // calls OnAddWindow or OnRemoveWindow above if any windows have been added or removed
            captureManager.Poll();
        }

        //if (Input.GetKeyDown("space"))
        //{
        //    Debug.Log("Trying click");
        //    Win32Funcs.PostMessage(windowPtr, 0x0204, 0, 0x0000);
        //}
    }
}
