using System.Collections;
using System.Collections.Generic;
using WinCapture;
using UnityEngine;

public class BaseApplication : MonoBehaviour
{
    Shader windowShader;
    //Shader desktopShader;

    //DesktopCapture desktopCapture;
    //GameObject desktopObject;

    protected WindowCapture windowsRender;
    protected GameObject windowObject;

    public float windowScale = 0.001f;

    public void passWindow(WindowCapture window)
    {
        Debug.Log(window.windowInfo.title);
        Debug.Log(windowObject == null);
        windowsRender = window;
        windowObject.name = window.windowInfo.title;
    }

    // Start is called before the first frame update
    protected void Awake()
    {
        windowShader = Shader.Find("WinCapture/WindowShader");
        //desktopShader = Shader.Find("WinCapture/DesktopShader");

        //int displayNum = 0;
        //desktopCapture = new DesktopCapture(displayNum);

        //desktopObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
        //desktopObject.name = "desktop" + displayNum;
        //desktopObject.transform.GetComponent<Renderer>().material = new Material(desktopShader);
        //desktopObject.transform.localEulerAngles = new Vector3(90, 0, 0);

        windowObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
        windowObject.transform.GetComponent<Renderer>().material = new Material(windowShader);
        windowObject.transform.localEulerAngles = new Vector3(90, 0, 0);
    }

    // Update is called once per frame
    protected void Update()
    {
        bool didChange;
        if (windowObject != null)
        {
            Texture2D windowTexture = windowsRender.GetWindowTexture(out didChange);
            if (didChange)
            {
                windowObject.GetComponent<Renderer>().material.mainTexture = windowTexture;
            }
            windowObject.transform.localScale = new Vector3(windowsRender.windowWidth * windowScale, 0.1f, windowsRender.windowHeight * windowScale);
        }
    }

    private void OnDestroy()
    {
        if(windowObject != null)
            UnityEngine.Object.Destroy(windowObject);
        if(windowsRender != null)
            windowsRender.Dispose();
    }
}
