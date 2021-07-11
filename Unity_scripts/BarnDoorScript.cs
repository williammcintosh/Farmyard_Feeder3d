using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarnDoorScript : MonoBehaviour
{
    public float travelSpeed = 1.0f;
    public GameObject menuManager;
    MenuManagerScript menuManagerScript;
    public GameObject fieldManager;
    FieldManager fieldManagerScript;
    public GameObject barnDoorLeft, barnDoorRight;
    public GameObject inventoryManager;
    InventoryManagerScript inventoryManagerScript;
    Vector3 rightDoorOpenPos, rightDoorClosePos, leftDoorOpenPos, leftDoorClosePos;
    // Start is called before the first frame update
    void Start()
    {
        rightDoorClosePos = barnDoorRight.transform.position;
        rightDoorOpenPos = rightDoorClosePos + barnDoorRight.transform.TransformDirection(Vector3.back * 1.5f);
        leftDoorClosePos = barnDoorLeft.transform.position;
        leftDoorOpenPos = leftDoorClosePos + barnDoorLeft.transform.TransformDirection(Vector3.forward * 1.5f);
        menuManagerScript = menuManager.GetComponent<MenuManagerScript>();
        fieldManagerScript = fieldManager.GetComponent<FieldManager>();
        inventoryManagerScript = inventoryManager.GetComponent<InventoryManagerScript>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    void OnMouseDown()
    {
        if (!menuManagerScript.messageUp) {
            if (menuManagerScript.atTheMainMenu) {
                if (menuManagerScript.royIsRoosted) {
                    OpenDoors();
                }
            }
        }
    }
    public void OpenDoors()
    {
        menuManagerScript.goToBarnDoor = true;
        inventoryManagerScript.UpdateAllInventoryCameraStrings();
        float leftDrDist = Vector3.Distance(barnDoorLeft.transform.position, leftDoorClosePos);
        float rigtDrDist = Vector3.Distance(barnDoorRight.transform.position, rightDoorClosePos);
        if (leftDrDist <= 0.1f && rigtDrDist <= 0.1f) {
            StartCoroutine(OpenLeftBarnDoor());
            StartCoroutine(OpenRightBarnDoor());
        }
    }
    //Called from the Back sign game object which uses the Stable Script
    public void CloseDoors()
    {
        float leftDrDist = Vector3.Distance(barnDoorLeft.transform.position, leftDoorOpenPos);
        float rigtDrDist = Vector3.Distance(barnDoorRight.transform.position, rightDoorOpenPos);

        if (leftDrDist <= 0.1f && rigtDrDist <= 0.1f) {
            StartCoroutine(CloseLeftBarnDoor());
            StartCoroutine(CloseRightBarnDoor());
        }
    }
    public IEnumerator OpenLeftBarnDoor()
    {
	    yield return new WaitForSeconds(0.5f);
        Vector3 doorStartPos = barnDoorLeft.transform.position;
	    float timeOfTravel = 1f;
	    float currentTime = 0;
	    float normalizedValue;
	    while (currentTime <= timeOfTravel) {
			currentTime += Time.deltaTime * travelSpeed;
			normalizedValue = currentTime / timeOfTravel; // we normalize our time
			barnDoorLeft.transform.position = Vector3.Lerp(doorStartPos, leftDoorOpenPos, normalizedValue);
			yield return null;
	    }
	    yield return new WaitForSeconds(0.5f);
		barnDoorLeft.transform.position = leftDoorOpenPos;
        yield return null;
    }
    public IEnumerator OpenRightBarnDoor()
    {
	    yield return new WaitForSeconds(0.5f);
        Vector3 doorStartPos = barnDoorRight.transform.position;
	    float timeOfTravel = 1f;
	    float currentTime = 0;
	    float normalizedValue;
	    while (currentTime <= timeOfTravel) {
			currentTime += Time.deltaTime * travelSpeed;
			normalizedValue = currentTime / timeOfTravel; // we normalize our time
			barnDoorRight.transform.position = Vector3.Lerp(doorStartPos, rightDoorOpenPos, normalizedValue);
			yield return null;
	    }
	    yield return new WaitForSeconds(0.5f);
		barnDoorRight.transform.position = rightDoorOpenPos;
        yield return null;
    }
    public IEnumerator CloseLeftBarnDoor()
    {
	    yield return new WaitForSeconds(0.5f);
        Vector3 doorStartPos = barnDoorLeft.transform.position;
	    float timeOfTravel = 1f;
	    float currentTime = 0;
	    float normalizedValue;
	    while (currentTime <= timeOfTravel) {
			currentTime += Time.deltaTime * travelSpeed;
			normalizedValue = currentTime / timeOfTravel; // we normalize our time
			barnDoorLeft.transform.position = Vector3.Lerp(doorStartPos, leftDoorClosePos, normalizedValue);
			yield return null;
	    }
	    yield return new WaitForSeconds(0.5f);
		barnDoorLeft.transform.position = leftDoorClosePos;
        yield return null;
    }
    public IEnumerator CloseRightBarnDoor()
    {
	    yield return new WaitForSeconds(0.5f);
        Vector3 doorStartPos = barnDoorRight.transform.position;
	    float timeOfTravel = 1f;
	    float currentTime = 0;
	    float normalizedValue;
	    while (currentTime <= timeOfTravel) {
			currentTime += Time.deltaTime * travelSpeed;
			normalizedValue = currentTime / timeOfTravel; // we normalize our time
			barnDoorRight.transform.position = Vector3.Lerp(doorStartPos, rightDoorClosePos, normalizedValue);
			yield return null;
	    }
	    yield return new WaitForSeconds(0.5f);
		barnDoorRight.transform.position = rightDoorClosePos;
        yield return null;
    }
}
