using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FieldManager : MonoBehaviour
{
    public GameObject applesForFillingField;
    public int floorWidth, floorDepth;
    public GameObject prefabTileDirt;
    public GameObject prefabTileGrass;
    GameObject[,] myFloorTiles;
    public GameObject[,] itemsOnBoard;
    [HideInInspector]
    public GameObject[] xCrates, zCrates, animals;
    public AnimalScript selectedAnimal;
    public LayerMask whatIsPickUp, whatIsCrates;
    public bool canTestField = true;
    DateTime oldestPlacementTime = DateTime.MaxValue;
    // Start is called before the first frame update
    void Start()
    {
        MakeFloor();
        UpdateItemsOnBoardList();
        UpdateCratesList();
        UpdateAnimalsList();
    }

    // Update is called once per frame
    void Update()
    {
        //PrintItemsOnBoard();
        UpdateItemsOnBoardList();
        UpdateCratesList();
    }
    public void UpdateAnimalsList()
    {
        animals = new GameObject[7];
        for (int i = 0; i<=6; i++) {
            Collider[] hitColliders = Physics.OverlapSphere(new Vector3(i, 1, 9), 0.05f);
			if (hitColliders.Length > 0)
			{
			    animals[i] = hitColliders[0].gameObject;
			    //Debug.Log("(" + i + ") " + hitColliders[0].gameObject.name.ToString());
			}
		}
    }
    public void UpdateCratesList()
    {
        xCrates = new GameObject[6];
        for (int i = 2; i < 8; i++) {
            Collider[] hitColliders = Physics.OverlapSphere(new Vector3(-2f, 0.1f, i), 0.2f, whatIsCrates);
			if (hitColliders.Length > 0) {
			    xCrates[i-2] = hitColliders[0].gameObject;
			    //Debug.Log("(" + i + ") " + hitColliders[0].gameObject.name.ToString());
			}
		}
        zCrates = new GameObject[6];
        for (int i = 2; i < 8; i++) {
            Collider[] hitColliders = Physics.OverlapSphere(new Vector3(-1f, 0.1f, i), 0.2f, whatIsCrates);
			if (hitColliders.Length > 0) {
			    zCrates[i-2] = hitColliders[0].gameObject;
			    //Debug.Log("(" + i + ") " + hitColliders[0].gameObject.name.ToString());
			}
		}
    }
    public void UpdateItemsOnBoardList()
    {
        //oldestPlacementTime is a global scale within the class
        itemsOnBoard = new GameObject[floorWidth, floorDepth];
        for (int i = 0; i < floorWidth; i++)
        {
            for (int j = 0; j < floorDepth; j++)
            {
                //int old = 0;
                //oldestPlacementTime = DateTime.MaxValue;
                Collider[] hitColliders = Physics.OverlapSphere(new Vector3(i, 0.5f, j), 0.05f, whatIsPickUp);
                if (hitColliders.Length > 0) {
                    itemsOnBoard[i,j] = hitColliders[0].gameObject;
                } else {
                    itemsOnBoard[i,j] = null;
                }
                /*
                if (hitColliders.Length > 0) {
                    old = GetOldestFoodAtPos(hitColliders);
                    if (hitColliders[old] != null) {
                        itemsOnBoard[i,j] = hitColliders[old].gameObject;
                    }
                }
                if (hitColliders.Length > 1) {
                    if (hitColliders[old] != null) {
                        ClearOutAllButOldest(hitColliders, oldestPlacementTime);
                    }
                }
                */
            }
        }
    }
    //Passes in oldestPlacementTime as the second argument to make a copy of the variable
    public void ClearOutAllButOldest(Collider[] hitColliders, DateTime oldest)
    {
        Debug.Log("CURRENT OLDEST = "+oldest.ToString());
        for (int k = 0; k < hitColliders.Length; k++) {
            if (hitColliders[k].gameObject != null) {
                PickUpScript foodScript = hitColliders[k].gameObject.GetComponent<PickUpScript>();
                if (DateTime.Compare(foodScript.placeTime, oldest) > 0) {
                    if (!foodScript.currentlyBeingCarried) {
                        foodScript.GoToCrate();
                    }
                }
            }
        }
    }
    public int GetOldestFoodAtPos(Collider [] hitColliders)
    {
        int oldestFoodAt = 0;
        DateTime oldestPlaced = DateTime.MaxValue;
        for (int k = 0; k < hitColliders.Length; k++) {
            if (hitColliders[k].gameObject != null) {
                PickUpScript foodScript = hitColliders[k].gameObject.GetComponent<PickUpScript>();
                //Only if food isn't being carried and is snapped
                if (!foodScript.currentlyBeingCarried && foodScript.IsSnapped()) {
                    //Keep only oldest pickup
                    if (DateTime.Compare(foodScript.placeTime, oldestPlaced) < 0) {
                        oldestFoodAt = k;
                    }
                }
            }
        }
        return oldestFoodAt;
    }
    public bool TheresSomethingOnTheBoard()
    {

        for (int i = 0; i < floorWidth; i++) {
            for (int j = 0; j < floorDepth; j++) {
                if (itemsOnBoard[i, j] != null) {
                    return true;
                }
            }
        }
        return false;
    }

    public void PrintItemsOnBoard()
    {
        for (int i = 0; i < floorWidth; i++)
            for (int j = 0; j < floorDepth; j++)
                if (itemsOnBoard[i,j] != null)
                    Debug.Log("["+i+","+j+"] = "+itemsOnBoard[i, j].name.ToString());
    }
    public void MakeFloor()
    {
        myFloorTiles = new GameObject[floorWidth, floorDepth];
        int count = 0;
        for (int i = 0; i < floorWidth; i++)
        {
            for (int j = 0; j < floorDepth; j++)
            {
                Vector3 loc = new Vector3(i, -0.1f, j);
                if (count % 2 == 0) {
					myFloorTiles[i, j] = Instantiate(prefabTileDirt, loc, Quaternion.identity) as GameObject;
                } else {
					myFloorTiles[i, j] = Instantiate(prefabTileGrass, loc, Quaternion.identity) as GameObject;
                }
                myFloorTiles[i, j].transform.SetParent(this.transform);
                count++;
            }
            if (floorDepth > 1 && floorWidth % 2 == 0) count++;
        }
    }
    public PickUpScript GameObjectAtCoordinates(int xPos, int yPos)
    {
        if (itemsOnBoard[xPos, yPos] != null)
        {
            if (itemsOnBoard[xPos, yPos].TryGetComponent(out PickUpScript pickUpScript))
            {
                return pickUpScript;
            }
        }
		return null;
    }
    public bool IsVacant(int xPos, int zPos)
    {
        if (itemsOnBoard[xPos, zPos] != null) {
            //Debug.Log("["+xPos+","+zPos+"] IS TAKEN");
            return false;
        }
        if (selectedAnimal && (selectedAnimal.transform.position.x == xPos && selectedAnimal.transform.position.z == zPos)) {
            return false;
        }
        return true;
    }
	//Creates an object (food) from the crateScript and return that food
    public PickUpScript CrateAtCoordinates(int xPos, int zPos)
    {
        //Debug.Log("GRABBIN' AT "+xPos+","+zPos);
        if (xPos == -2) {
            if (xCrates[zPos-2] != null) {
                CrateScript crateScript = xCrates[zPos-2].GetComponent<CrateScript>();
                crateScript.PlaySounds();
                Vector3 upwards = crateScript.transform.TransformDirection(Vector3.up);
                GameObject food = Instantiate(crateScript.myPrefabFood, crateScript.transform.position+upwards, Quaternion.identity) as GameObject;
                return food.GetComponent<PickUpScript>();
            }
        } else if (xPos == -1) {
            if (zCrates[zPos-2] != null) {
                CrateScript crateScript = zCrates[zPos-2].GetComponent<CrateScript>();
                crateScript.PlaySounds();
                Vector3 upwards = crateScript.transform.TransformDirection(Vector3.up);
                GameObject food = Instantiate(crateScript.myPrefabFood, crateScript.transform.position+upwards, Quaternion.identity) as GameObject;
                return food.GetComponent<PickUpScript>();
            }
        }
        return null;
    }
    public GameObject AnimalAtCoordinates(int xPos)
    {
        return animals[xPos];
    }
    public bool IsWithInFieldBounds(int xPos, int yPos)
    {
        if (xPos >= 0 && xPos < floorWidth && yPos >= 0 && yPos < floorDepth)
            return true;
        return false;
    }
    public bool IsWithInStableBounds(int xPos, int yPos)
    {
        if (xPos >= 0 && xPos <=6 && yPos == 9)
            return true;
        return false;
    }
    public bool IsWithInCrateBounds(int xPos, int yPos)
    {
        if (xPos > -3 && xPos < 0 && yPos >= 2 && yPos < 8)
            return true;
        return false;
    }
    public void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        for (int i = 2; i < 8; i++) {
            Vector3 pos = new Vector3(-1f, 0.1f, i);
            Gizmos.DrawSphere(pos, 0.2f);
		}
        for (int i = 2; i < 8; i++) {
            Vector3 pos = new Vector3(-2f, 0.1f, i);
            Gizmos.DrawSphere(pos, 0.2f);
		}
    }
    public void ClearAllFood()
    {
        for (int i = 0; i < floorWidth; i++)
        {
            for (int j = 0; j < floorDepth; j++)
            {
                itemsOnBoard[i, j] = null;
                Collider[] hitColliders = Physics.OverlapSphere(new Vector3(i, 0.5f, j), 0.05f, whatIsPickUp);
                int length = hitColliders.Length;
                for (int k = 0; k < length; k++) {
                    if (hitColliders[k].gameObject != null) {
                        if (!hitColliders[k].gameObject.GetComponent<PickUpScript>().currentlyBeingCarried) {
                            Destroy(hitColliders[k].gameObject);
                        }
                    }
                }
            }
        }
    }
    public void FillTheFieldWithApples()
    {
        if (canTestField) {
            canTestField = false;
            for (int i = 0; i < floorWidth; i++)
            {
                for (int j = 0; j < floorDepth; j++)
                {
                    if (itemsOnBoard[i,j] == null) {
                        itemsOnBoard[i,j] = Instantiate(applesForFillingField, new Vector3(i, 0.5f, j), Quaternion.identity) as GameObject;
                        PickUpScript myPickUpScript = itemsOnBoard[i,j].GetComponent<PickUpScript>();
                        AnimalScript animalScript = selectedAnimal.GetComponent<AnimalScript>();
                        animalScript.foodQueue.Enqueue(myPickUpScript);
                    }
                }
            }
        }
    }
    public void RemoveFromSelectedAnimalsQueue(PickUpScript obj)
    {

    }
}
