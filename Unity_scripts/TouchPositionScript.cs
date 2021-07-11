using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchPositionScript
{
    public int touchID;
    public PickUpScript myObj;
    public Vector3 lastPos;
    //This is the copy constructor for this class
    public TouchPositionScript(int newTouchID, PickUpScript newObj, Vector3 newLastPos)
    {
        touchID = newTouchID;
        myObj = newObj;
        lastPos = newLastPos;
    }
}
