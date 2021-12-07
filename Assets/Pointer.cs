using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pointer : MonoBehaviour
{
    public Ray rayPointer;
    public bool vision2D;
    

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(vision2D)
            rayPointer = Camera.main.ScreenPointToRay(Input.mousePosition);
    }
}
