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
    protected bool mousePosChanged = true; //Flag especifica para extensão do formato
    private bool mousePosMoveChanged = true; //Flag usada somente na BaseApplication

    protected Pointer pointer;
    Vector3 MouseVec, oldMouseVec;
    //Ray lastRay;

    bool leftMouseHold;

    enum MouseFunction { nothing, move, resizeHor, resizeVert, resizeDiag};
    MouseFunction function = MouseFunction.nothing;

    public void passWindow(WindowCapture window)
    {
        Debug.Log(window.windowInfo.title);
        Debug.Log(windowObject == null);
        windowsRender = window;
        windowObject.name = window.windowInfo.title;
    }

    public void Move(Vector3 pos)
    {
        windowObject.transform.position = pos;
    }

    public void Rotate(Vector3 rot, float angle)
    {
        windowObject.transform.RotateAround(new Vector3(0, 0, 0), rot, angle);
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
        windowObject.transform.localEulerAngles = new Vector3(0, -180, 0);
    }

    protected void Start()
    {
        pointer = GameObject.Find("Pointer").GetComponent<Pointer>();
    }

    private Vector3 GetPlayerPlaneMousePos()
    {
        Plane plane = new Plane(windowObject.transform.position-Camera.main.transform.position, windowObject.transform.position);
        //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float dist;
        if (plane.Raycast(pointer.rayPointer, out dist))
        {
            return pointer.rayPointer.GetPoint(dist);
        }
        return Vector3.zero;
    }

    // Update is called once per frame
    protected void Update()
    {
        //Funcionando para tela centrada na camera sem rotação. Passivel de ser necessário adicionar uma matriz de rotação caso o plano não esteja em 0,0,0.
        //Converte um raio de luz que indica o mouse em um ponto no plano, compila info em um unico int
        MouseVec = GetPlayerPlaneMousePos();
        //Multiplica por 10 para ajustar a unidade

        Vector3 MouseVecRel = Quaternion.FromToRotation(windowObject.transform.position, Vector3.forward) * MouseVec;
        Int32 xMouse = (Int32)(MouseVecRel.x / (10*windowScale) + windowsRender.windowWidth/2.0);
        Int32 yMouse = (Int32)(windowsRender.windowHeight / 2.0 - MouseVecRel.y / (10*windowScale));
        Int32 mousePos = (Int32)((yMouse << 16) | xMouse);
        if (oldMousePos != mousePos && xMouse >= 0 && yMouse >= 0 && xMouse <= windowsRender.windowWidth && yMouse <= windowsRender.windowHeight)
        {
            mousePosMoveChanged = true;
            mousePosChanged = true;
        }
        oldMousePos = mousePos;
        Debug.Log($"X: {xMouse} Y: {yMouse}");


        if (Input.GetMouseButtonDown(0))
        {
            //Debug.Log($"aaa {Win32Funcs.SendMessage(windowsRender.hwnd, Win32Types.command.WM_NCHITTEST, 0, mousePos)}");
            leftMouseHold = true;
            oldMouseVec = MouseVec;
            if (xMouse >= -3 && xMouse <= windowsRender.windowWidth + 3 && yMouse >= -3 && yMouse <= windowsRender.windowHeight + 3)
            {
                if(yMouse < 20) function = MouseFunction.move;
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            function = MouseFunction.nothing;
            leftMouseHold = false;
        }

        switch (function) {
            case MouseFunction.move:
                Vector3 relPos = windowObject.transform.position - oldMouseVec;
                Vector3 endPos = oldMouseVec.magnitude * pointer.rayPointer.direction + relPos;
                //Usa Vector2 pq pega o angulo projetado em x-y, camera e pos inicial foram ajustada para permitir isso
                //roda em torno do eixo vertical
                windowObject.transform.RotateAround(new Vector3(0, 0, 0), Vector3.forward, Vector2.SignedAngle(windowObject.transform.position, endPos));
                Vector3 vecPerp = Vector3.Cross(Vector3.back, windowObject.transform.position);
                //roda em torno do eixo horizontal relativo
                windowObject.transform.RotateAround(new Vector3(0, 0, 0), vecPerp, Vector3.SignedAngle(windowObject.transform.position, endPos, vecPerp));
                oldMouseVec = MouseVec;
                break;
                }


        bool didChangeTex;
        if (windowObject != null)
        {
            Texture2D windowTexture = windowsRender.GetWindowTexture(out didChangeTex);
            if (didChangeTex)
            {
                windowObject.GetComponent<Renderer>().material.mainTexture = windowTexture;
                Debug.Log($"Dim: {windowsRender.windowWidth} {windowsRender.windowHeight}");
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
