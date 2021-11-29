using System;
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

    protected Int32 oldMousePos;
    protected bool mousePosChanged = true;

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

    private Vector3 GetPlayerPlaneMousePos()
    {
        Plane plane = new Plane(windowObject.transform.position-Camera.main.transform.position, windowObject.transform.position);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float dist;
        if (plane.Raycast(ray, out dist))
        {
            return ray.GetPoint(dist);
        }
        return Vector3.zero;
    }

    // Update is called once per frame
    protected void Update()
    {
        //TOMAR CUIDADO, ORDENADAS DA JANELA EH DE CIMA PRA BAIXO, TELA DE BAIXO PRA CIMA?
        //Int32 mousePos;
        //{
        //    Int32 xMouse = (Int32)Input.mousePosition.x;
        //    Int32 yMouse = (Int32)Input.mousePosition.y;
        //    mousePos = (Int32)((yMouse << 16) | xMouse);
        //    if (oldMousePos != mousePos)
        //    {
        //        mousePosChanged = true;
        //    }
        //    oldMousePos = mousePos;
        //}

        //Funcionando para tela centrada na camera sem rotação. Passivel de ser necessário adicionar uma matriz de rotação caso o plano não esteja em 0,0,0.
        Int32 mousePos;
        {
            Vector3 pointVec = GetPlayerPlaneMousePos();
            Int32 xMouse = (Int32)(pointVec.x / (10*windowScale) + windowsRender.windowWidth/2.0);
            Int32 yMouse = (Int32)(windowsRender.windowHeight / 2.0 - pointVec.y / (10*windowScale));
            mousePos = (Int32)((yMouse << 16) | xMouse);
            if (oldMousePos != mousePos && xMouse >= 0 && yMouse >= 0 && xMouse <= windowsRender.windowWidth && yMouse <= windowsRender.windowHeight)
            {
                mousePosChanged = true;
            }
            oldMousePos = mousePos;
        }

        bool didChangeTex;
        if (windowObject != null)
        {
            Texture2D windowTexture = windowsRender.GetWindowTexture(out didChangeTex);
            if (didChangeTex)
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
