using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using WinCapture;
using UnityEngine;

using Debug = UnityEngine.Debug;

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
    protected bool mousePosChanged = true; //Flag especifica para extens�o do formato
    private bool mousePosMoveChanged = true; //Flag usada somente na BaseApplication

    protected Pointer pointer;
    Vector3 oldMouseVec;
    //Ray lastRay;

    bool leftMouseHold;

    int oldWidth, oldHeight;

    Quaternion lastRotation;

    enum MouseFunction { nothing, move, resizeHor, resizeVert, resizeDiag, changeDistance};
    MouseFunction function = MouseFunction.nothing;

    private void addTextureBorder(ref Texture2D tex, Color col, int top, int bottom, int left, int right)
    {
        for(int x = 0; x < tex.width; x++)
            for(int y = 0; y< tex.height; y++)
            {
                if (y < top || 
                    y >= tex.height - bottom ||
                    x < left ||
                    x >= tex.width - right) tex.SetPixel(x, y, col);
            }
    }

    public void passWindow(WindowCapture window)
    {
        windowsRender = window;
        windowObject.name = window.windowInfo.title;
        Debug.Log(windowObject.GetInstanceID());
        pointer.activeObjectID = windowObject.GetInstanceID();
    }

    public void Move(Vector3 pos)
    {
        windowObject.transform.position = pos;
    }

    public void Close()
    {
        int processId;
        Win32Funcs.GetWindowThreadProcessId(windowsRender.hwnd, out processId);
        Process p = Process.GetProcessById(processId);
        p.Kill();
    }
    
    private Vector3 GetPlayerPlaneMousePos()
    {
        Plane plane = new Plane(windowObject.transform.position - Camera.main.transform.position, windowObject.transform.position);
        //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float dist;
        if (plane.Raycast(pointer.rayPointer, out dist))
        {
            return pointer.rayPointer.GetPoint(dist);
        }
        return Vector3.zero;
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
        pointer = GameObject.Find("Pointer").GetComponent<Pointer>();
    }

    protected void Start()
    {
    }

    // Update is called once per frame
    protected void Update()
    {
        //Inicio CONVERSOR
        //Converte direcao em ponto xy na janela
        if (pointer.hit.collider != null)
            if (pointer.hit.collider.gameObject == windowObject)
            {
                Int32 xMouse = (Int32)(pointer.hit.textureCoord.x * windowsRender.windowWidth);
                Int32 yMouse = (Int32)((1.0f - pointer.hit.textureCoord.y) * windowsRender.windowHeight);
                Int32 mousePos = (Int32)((yMouse << 16) | xMouse);
                if (oldMousePos != mousePos && xMouse >= 0 && yMouse >= 0 && xMouse <= windowsRender.windowWidth && yMouse <= windowsRender.windowHeight)
                {
                    mousePosMoveChanged = true;
                    mousePosChanged = true;
                }
                oldMousePos = mousePos;
                //Fim CONVERSOR

                //Inicio HOVER
                //aplicacao de cores na esfera
                if (xMouse >= 0 && xMouse <= windowsRender.windowWidth && yMouse >= 0 && yMouse <= windowsRender.windowHeight)
                {
                    if ((xMouse <= 3 && (yMouse <= 3 || yMouse >= windowsRender.windowHeight - 3)) ||
                         (xMouse >= windowsRender.windowWidth - 3 && (yMouse <= 3 || yMouse >= windowsRender.windowHeight - 3)))
                    {
                        pointer.sphereColor = ColorSettings.hoverDiagRescaleColor;
                    }
                    else if ((xMouse <= 3 || xMouse >= windowsRender.windowWidth - 3))
                    {
                        pointer.sphereColor = ColorSettings.hoverHorRescaleColor;
                    }
                    else if ((yMouse <= 3 || yMouse >= windowsRender.windowHeight - 3))
                    {
                        pointer.sphereColor = ColorSettings.hoverVertRescaleColor;
                    }
                    else if (yMouse < 30)
                    {
                        pointer.sphereColor = ColorSettings.hoverMoveColor;
                    }
                    else
                    {
                        if(pointer.activeObjectID == windowObject.GetInstanceID())
                            pointer.sphereColor = ColorSettings.hoverActiveWindowColor;
                        else
                            pointer.sphereColor = ColorSettings.hoverInactiveWindowColor;
                    }
                }
                //Fim HOVER
                if (function == MouseFunction.changeDistance)
                    pointer.sphereColor = ColorSettings.changeDistanceColor;

                //Inicio FUNCOES JANELA
                //Determina area das janelas que executa cada funcao
                if (pointer.mouseLeftDown && function == MouseFunction.nothing)
                {
                    if (xMouse >= 0 && xMouse <= windowsRender.windowWidth && yMouse >= 0 && yMouse <= windowsRender.windowHeight)
                    {
                        leftMouseHold = true;
                        oldMouseVec = pointer.hit.point;
                        oldWidth = windowsRender.windowWidth;
                        oldHeight = windowsRender.windowHeight;
                        lastRotation = Quaternion.FromToRotation(windowObject.transform.position, new Vector3(0, windowObject.transform.position.y, windowObject.transform.position.z));
                        lastRotation = Quaternion.FromToRotation(new Vector3(0, windowObject.transform.position.y, windowObject.transform.position.z), new Vector3(0, -1, 0)) * lastRotation;
                        if ((xMouse <= 3 && (yMouse <= 3 || yMouse >= windowsRender.windowHeight - 3)) ||
                            (xMouse >= windowsRender.windowWidth - 3 && (yMouse <= 3 || yMouse >= windowsRender.windowHeight - 3)))
                        {
                            function = MouseFunction.resizeDiag;
                            Debug.Log("Resize Diagonal");
                        }
                        else if ((xMouse <= 3 || xMouse >= windowsRender.windowWidth - 3))
                        {
                            function = MouseFunction.resizeHor;
                            Debug.Log("Resize Horizontal");
                        }
                        else if ((yMouse <= 3 || yMouse >= windowsRender.windowHeight - 3))
                        {
                            function = MouseFunction.resizeVert;
                            Debug.Log("Resize Vertical");
                        }
                        else if (yMouse < 30)
                        {
                            function = MouseFunction.move;
                            Debug.Log("Move");
                        }
                    }
                }

                if (pointer.mouseMiddleDown)
                {
                    if (pointer.activeObjectID == windowObject.GetInstanceID())
                    {
                        if (function == MouseFunction.nothing)
                            function = MouseFunction.changeDistance;
                        else if (function == MouseFunction.changeDistance)
                            function = MouseFunction.nothing;
                    }
                }
            }

        if (pointer.mouseLeftUp)
            leftMouseHold = false;

        if(pointer.mouseLeftUp && (function == MouseFunction.move || function == MouseFunction.resizeDiag
            || function == MouseFunction.resizeHor || function == MouseFunction.resizeVert))
            function = MouseFunction.nothing;

        //Implementacao das funcoes da janela
        switch (function)
        {
            case MouseFunction.move:
                Vector3 MouseVecMove = GetPlayerPlaneMousePos();
                Vector3 relPos = windowObject.transform.position - oldMouseVec;
                Vector3 endPos = oldMouseVec.magnitude * pointer.rayPointer.direction + relPos;
                //Usa Vector2 pq pega o angulo projetado em x-y, camera e pos inicial foram ajustada para permitir isso
                //roda em torno do eixo vertical
                windowObject.transform.RotateAround(new Vector3(0, 0, 0), Vector3.forward, Vector2.SignedAngle(windowObject.transform.position, endPos));
                Vector3 vecPerp = Vector3.Cross(Vector3.back, windowObject.transform.position);
                //roda em torno do eixo horizontal relativo
                windowObject.transform.RotateAround(new Vector3(0, 0, 0), vecPerp, Vector3.SignedAngle(windowObject.transform.position, endPos, vecPerp));
                oldMouseVec = MouseVecMove;
                break;
            case MouseFunction.resizeHor:
                Vector3 MouseVecHor = GetPlayerPlaneMousePos();
                //Debug.Log($"{lastRotation * MouseVecHor} {windowObject.transform.position} {lastRotation * windowObject.transform.position}");
                float relIncHor = Math.Abs((lastRotation * (MouseVecHor - windowObject.transform.position)).x / (lastRotation * (oldMouseVec - windowObject.transform.position)).x);
                Win32Funcs.MoveWindow(windowsRender.hwnd, 0, 0, (int)(relIncHor * oldWidth), oldHeight, true);
                break;
            case MouseFunction.resizeVert:
                Vector3 MouseVecVert = GetPlayerPlaneMousePos();
                //Debug.Log($"{lastRotation * MouseVecVert} {windowObject.transform.position} {lastRotation * windowObject.transform.position}");
                float relIncVert = Math.Abs((lastRotation * (MouseVecVert - windowObject.transform.position)).z / (lastRotation * (oldMouseVec - windowObject.transform.position)).z);
                Win32Funcs.MoveWindow(windowsRender.hwnd, 0, 0, oldWidth, (int)(relIncVert * oldHeight), true);
                break;
            case MouseFunction.resizeDiag:
                Vector3 MouseVecDiag = GetPlayerPlaneMousePos();
                //Debug.Log($"{lastRotation * MouseVecDiag} {windowObject.transform.position} {lastRotation * windowObject.transform.position}");
                float relIncDiagHor = Math.Abs((lastRotation * (MouseVecDiag - windowObject.transform.position)).x / (lastRotation * (oldMouseVec - windowObject.transform.position)).x);
                float relIncDiagVert = Math.Abs((lastRotation * (MouseVecDiag - windowObject.transform.position)).z / (lastRotation * (oldMouseVec - windowObject.transform.position)).z);
                Win32Funcs.MoveWindow(windowsRender.hwnd, 0, 0, (int)(relIncDiagHor * oldWidth), (int)(relIncDiagVert * oldHeight), true);
                break;
            case MouseFunction.changeDistance:
                //Impede que chega no 0,0,0
                if(windowObject.transform.position.magnitude > -pointer.mouseWheelValue)
                    windowObject.transform.position += windowObject.transform.position.normalized * pointer.mouseWheelValue;
                break;
        }
        //Fim FUNCOES JANELA

        bool didChangeTex;
        if (windowObject != null)
        {
            Texture2D windowTexture = windowsRender.GetWindowTexture(out didChangeTex);
            //addTextureBorder(ref windowTexture, Color.blue, 30, 3, 3, 3);
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
