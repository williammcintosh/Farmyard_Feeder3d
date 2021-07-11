using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigBarnSlidingDoorScript : MonoBehaviour
{
    public float travelSpeed = 1.0f;
    public float slideDistance = 2.5f;
    public bool openTheDoors = false, closeTheDoors = false;
    public bool doorsClosed = true, doorsOpen = false;
    public GameObject barnDoorLeft, barnDoorRight;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (openTheDoors) {
            openTheDoors = false;
            StartCoroutine(OpenDoors());
        }
        if (closeTheDoors) {
            closeTheDoors = false;
            StartCoroutine(CloseDoors());
        }

    }
    public IEnumerator OpenDoors()
    {
        //Calls both door's open() but waits until fully open before considered "doorsOpen"
        doorsClosed = false;
        StartCoroutine(OpenLeftBarnDoor());
        yield return StartCoroutine(OpenRightBarnDoor());
        doorsOpen = true;
    }
    //Called from the Back sign game object which uses the Stable Script
    public IEnumerator CloseDoors()
    {
        //Calls both door's closed() but waits until fully closed before considered "doorsClosed"
        doorsOpen = false;
        StartCoroutine(CloseLeftBarnDoor());
        yield return StartCoroutine(CloseRightBarnDoor());
        doorsClosed = true;
    }
    public IEnumerator OpenLeftBarnDoor()
    {
        yield return new WaitForSeconds(0.5f);
        Vector3 doorStartPos = barnDoorLeft.transform.position;
        Vector3 doorEndPos = doorStartPos + barnDoorLeft.transform.TransformDirection(Vector3.left * slideDistance);
        float timeOfTravel = 1f;
        float currentTime = 0;
        float normalizedValue;
        while (currentTime <= timeOfTravel) {
            currentTime += Time.deltaTime * travelSpeed;
            normalizedValue = currentTime / timeOfTravel; // we normalize our time
            barnDoorLeft.transform.position = Vector3.Lerp(doorStartPos, doorEndPos, normalizedValue);
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);
        barnDoorLeft.transform.position = doorEndPos;
        yield return null;
    }
    public IEnumerator OpenRightBarnDoor()
    {
        yield return new WaitForSeconds(0.5f);
        Vector3 doorStartPos = barnDoorRight.transform.position;
        Vector3 doorEndPos = doorStartPos + barnDoorRight.transform.TransformDirection(Vector3.right * slideDistance);
        float timeOfTravel = 1f;
        float currentTime = 0;
        float normalizedValue;
        while (currentTime <= timeOfTravel) {
            currentTime += Time.deltaTime * travelSpeed;
            normalizedValue = currentTime / timeOfTravel; // we normalize our time
            barnDoorRight.transform.position = Vector3.Lerp(doorStartPos, doorEndPos, normalizedValue);
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);
        barnDoorRight.transform.position = doorEndPos;
        yield return null;
    }
    public IEnumerator CloseLeftBarnDoor()
    {
        yield return new WaitForSeconds(0.5f);
        Vector3 doorStartPos = barnDoorLeft.transform.position;
        Vector3 doorEndPos = doorStartPos + barnDoorLeft.transform.TransformDirection(Vector3.right * slideDistance);
        float timeOfTravel = 1f;
        float currentTime = 0;
        float normalizedValue;
        while (currentTime <= timeOfTravel) {
            currentTime += Time.deltaTime * travelSpeed;
            normalizedValue = currentTime / timeOfTravel; // we normalize our time
            barnDoorLeft.transform.position = Vector3.Lerp(doorStartPos, doorEndPos, normalizedValue);
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);
        barnDoorLeft.transform.position = doorEndPos;
        yield return null;
    }
    public IEnumerator CloseRightBarnDoor()
    {
        yield return new WaitForSeconds(0.5f);
        Vector3 doorStartPos = barnDoorRight.transform.position;
        Vector3 doorEndPos = doorStartPos + barnDoorRight.transform.TransformDirection(Vector3.left * slideDistance);
        float timeOfTravel = 1f;
        float currentTime = 0;
        float normalizedValue;
        while (currentTime <= timeOfTravel) {
            currentTime += Time.deltaTime * travelSpeed;
            normalizedValue = currentTime / timeOfTravel; // we normalize our time
            barnDoorRight.transform.position = Vector3.Lerp(doorStartPos, doorEndPos, normalizedValue);
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);
        barnDoorRight.transform.position = doorEndPos;
        yield return null;
    }

}
