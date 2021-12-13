using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pointer : MonoBehaviour
{
    public Ray rayPointer;

    public enum controlOption {mouse};
    public controlOption control;

    public RaycastHit hit;

    [NonSerialized]
    public int activeObjectID;

    [NonSerialized]
    public bool mouseRightDown, mouseLeftDown, mouseMiddleDown;

    [NonSerialized]
    public bool mouseRightUp, mouseLeftUp, mouseMiddleUp;

    [NonSerialized]
    public float mouseWheelValue;

    [NonSerialized]
    public string inputString;

    [NonSerialized]
    public Color sphereColor;

    public float radius;

    Renderer rend;

    // Start is called before the first frame update
    void Start()
    {
        mouseLeftDown = Input.GetMouseButtonDown(0);
        mouseRightDown = Input.GetMouseButtonDown(1);
        mouseMiddleDown = Input.GetMouseButtonDown(2);
        mouseLeftUp = Input.GetMouseButtonUp(0);
        mouseRightUp = Input.GetMouseButtonUp(1);
        mouseMiddleUp = Input.GetMouseButtonUp(0);

        rend = gameObject.GetComponent<Renderer>();

        activeObjectID = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (control == controlOption.mouse)
        {
            rayPointer = Camera.main.ScreenPointToRay(Input.mousePosition);

            mouseLeftDown = Input.GetMouseButtonDown(0);

            mouseLeftDown = Input.GetMouseButtonDown(0);
            mouseRightDown = Input.GetMouseButtonDown(1);
            mouseMiddleDown = Input.GetMouseButtonDown(2);
            mouseLeftUp = Input.GetMouseButtonUp(0);
            mouseRightUp = Input.GetMouseButtonUp(1);
            mouseMiddleUp = Input.GetMouseButtonUp(0);

            mouseWheelValue = Input.mouseScrollDelta.y;

            if (Input.anyKeyDown)
                inputString = Input.inputString;
            else
                inputString = null;
            
        }

        if (Physics.Raycast(new Vector3(0, 0, 0), rayPointer.direction, out hit, 1000.0f))
        {
            this.transform.position = hit.point;
            if (mouseLeftDown || mouseRightDown || mouseMiddleDown)
            {
                activeObjectID = hit.collider.gameObject.GetInstanceID();
                Debug.Log(activeObjectID);
            }

            rend.material.color = sphereColor;
        }
        else
        {
            this.transform.position = rayPointer.direction * radius;
            if (mouseLeftDown || mouseRightDown || mouseMiddleDown)
            {
                activeObjectID = 0;
            }

            rend.material.color = Color.grey;
        }
    }
}
