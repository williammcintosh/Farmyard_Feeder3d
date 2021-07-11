using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CamInventoryManagerScript : MonoBehaviour
{
    public GameObject tileFolder;
    public GameObject darkScreenArea;
    public GameObject inventoryManager;
    InventoryManagerScript inventoryManagerScript;
    Camera [] camList;
    public Camera preFabCamera;
    GameObject [] animalList;
    public GameObject prefabAnimalsInventoryTile;
    GameObject [] levelTextList;
    public GameObject prefabLevelText;
    GameObject [] animalInventoryTiles;
    public GameObject prefabFoodToNextLevel;
    GameObject [] foodToNextLevelList;
    public GameObject prefabTotalFoodEaten;
    GameObject [] totalFoodEatenList;
    public GameObject prefabBrushToNextLevel;
    GameObject [] brushToNextLevelList;
    public GameObject prefabHeaderLevel, prefabHeaderFoodToNext, prefabHeaderTotalFood, prefabHeaderBrushToNext;
    GameObject levelHeader, FoodToNextHeader, totalFoodHeader, brushToNextHeader;
    public Vector3 startingPos = new Vector3(6.77f, 2.46f, 12.05f);
    // Start is called before the first frame update
    void Start()
    {
        inventoryManagerScript = inventoryManager.GetComponent<InventoryManagerScript>();
        MakeHeaders();
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void MakeLists(int sizeOfAnimals)
    {
        camList = new Camera [sizeOfAnimals];
        animalList = new GameObject [sizeOfAnimals];
        animalInventoryTiles = new GameObject [sizeOfAnimals];
        levelTextList = new GameObject [sizeOfAnimals];
        foodToNextLevelList = new GameObject [sizeOfAnimals];
        totalFoodEatenList = new GameObject [sizeOfAnimals];
        brushToNextLevelList = new GameObject [sizeOfAnimals];
    }
    public void MakeHeaders()
    {
        int position = 0;
        Vector3 myPos = this.transform.position;
        Quaternion tileRotation = Quaternion.Euler(0, -90, 0);
        levelHeader = Instantiate(prefabHeaderLevel, startingPos+(Vector3.down*0.5f*position)+(Vector3.forward*0.5f), tileRotation) as GameObject;
        levelHeader.transform.SetParent(tileFolder.transform);
        FoodToNextHeader = Instantiate(prefabHeaderFoodToNext, startingPos+(Vector3.down*0.5f*position)+(Vector3.forward*1.0f), tileRotation) as GameObject;
        FoodToNextHeader.transform.SetParent(tileFolder.transform);
        totalFoodHeader = Instantiate(prefabHeaderTotalFood, startingPos+(Vector3.down*0.5f*position)+(Vector3.forward*1.5f), tileRotation) as GameObject;
        totalFoodHeader.transform.SetParent(tileFolder.transform);
        brushToNextHeader = Instantiate(prefabHeaderBrushToNext, startingPos+(Vector3.down*0.5f*position)+(Vector3.forward*2.0f), tileRotation) as GameObject;
        brushToNextHeader.transform.SetParent(tileFolder.transform);
    }
    public void MakeACopy(GameObject prefabAnimal, int position, AnimalStatsScript animalScript)
    {
        Vector3 myPos = this.transform.position;
        Quaternion animalRotation = Quaternion.Euler(0, 120, 0);
        Quaternion tileRotation = Quaternion.Euler(0, -90, 0);
        Vector3 animalPosition = (Vector3.down*0.75f)+(Vector3.forward*3*position);
        //Creates and sets parent to this transform
        float downWard = prefabAnimalsInventoryTile.transform.localScale.y;
        camList[position] = Instantiate(preFabCamera, myPos+(Vector3.down*3*(position+1)), Quaternion.identity) as Camera;
        camList[position].transform.SetParent(this.transform);
        animalList[position] = Instantiate(prefabAnimal, myPos+(Vector3.down*3*(position+1))+animalPosition, animalRotation) as GameObject;
        animalList[position].transform.SetParent(this.transform);
        animalInventoryTiles[position] = Instantiate(prefabAnimalsInventoryTile, startingPos+(Vector3.down*downWard*(position+1)), tileRotation) as GameObject;
        animalInventoryTiles[position].transform.SetParent(tileFolder.transform);
        levelTextList[position] = Instantiate(prefabLevelText, startingPos+(Vector3.down*downWard*(position+1))+(Vector3.forward*0.5f), tileRotation) as GameObject;
        levelTextList[position].transform.SetParent(tileFolder.transform);
        UpdateString(levelTextList[position], animalScript.level.ToString());
        foodToNextLevelList[position] = Instantiate(prefabFoodToNextLevel, startingPos+(Vector3.down*downWard*(position+1))+(Vector3.forward*1f), tileRotation) as GameObject;
        foodToNextLevelList[position].transform.SetParent(tileFolder.transform);
        UpdateString(foodToNextLevelList[position], animalScript.foodToNextHappy.ToString());
        totalFoodEatenList[position] = Instantiate(prefabTotalFoodEaten, startingPos+(Vector3.down*downWard*(position+1))+(Vector3.forward*1.5f), tileRotation) as GameObject;
        totalFoodEatenList[position].transform.SetParent(tileFolder.transform);
        UpdateString(totalFoodEatenList[position], animalScript.foodCount.ToString());
        brushToNextLevelList[position] = Instantiate(prefabFoodToNextLevel, startingPos+(Vector3.down*downWard*(position+1))+(Vector3.forward*2f), tileRotation) as GameObject;
        brushToNextLevelList[position].transform.SetParent(tileFolder.transform);
        UpdateBrushText(brushToNextLevelList[position], animalScript);
        //Remove gravity from the animal
        AnimalScript animScript = animalList[position].GetComponent<AnimalScript>();
        animScript.ForceSetMaterial();
        Destroy(animScript);
        Destroy(animalList[position].GetComponent<CharacterController>());
        //Create a Render Texture for the inventory tiles
        RenderTexture newTileRT = new RenderTexture(256, 256, 16, RenderTextureFormat.ARGB32);
        Material tileMat = animalInventoryTiles[position].GetComponent<Renderer>().material;
        tileMat.mainTexture = newTileRT;
        tileMat.SetFloat("_Metallic", 1.0f);
        tileMat.SetFloat("_Glossiness", 0.0f);
        camList[position].targetTexture = newTileRT;
            if (!animalScript.equipped) {
                animalInventoryTiles[position].SetActive(false);
                levelTextList[position].SetActive(false);
                foodToNextLevelList[position].SetActive(false);
                totalFoodEatenList[position].SetActive(false);
                brushToNextLevelList[position].SetActive(false);
            }
    }
    public void UpdateBrushText(GameObject textObj, AnimalStatsScript animalScript)
    {
        float ratio = (float) inventoryManagerScript.GetNextBrushHappyRatio(animalScript);
        int brushPercent = (int) Mathf.Round(ratio*100f);
        if (brushPercent >= 100) {
            UpdateString(textObj, "100%");
        } else {
            UpdateString(textObj, brushPercent.ToString()+"%");
        }

    }
    public void UpdateString(GameObject textObj, string newText)
    {
        ReferToMyChildScript howTheHeckDoIGetAChildObject = textObj.GetComponent<ReferToMyChildScript>();
        TextMeshProUGUI levelTMPro = howTheHeckDoIGetAChildObject.myChild.GetComponent<TextMeshProUGUI>();
        levelTMPro.text = newText;

    }
    public void UpdateString(int position, string level, string foodToNexthappy, string foodCount, AnimalStatsScript animalStats)
    {
        UpdateString(levelTextList[position], level);
        UpdateString(foodToNextLevelList[position], foodToNexthappy);
        UpdateString(totalFoodEatenList[position], foodCount);
        //UpdateString(brushToNextLevelList[position], brushToNextHappy+"%");
        UpdateBrushText(brushToNextLevelList[position], animalStats);
    }
    public void EnableAnimalHealthForAnimal(int position)
    {
        animalInventoryTiles[position].SetActive(true);
        levelTextList[position].SetActive(true);
        foodToNextLevelList[position].SetActive(true);
        totalFoodEatenList[position].SetActive(true);
        brushToNextLevelList[position].SetActive(true);
    }
}
