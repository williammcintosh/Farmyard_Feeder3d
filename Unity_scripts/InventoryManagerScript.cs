using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class InventoryManagerScript : MonoBehaviour
{
    public GameObject happyMessageToggleObj;
    Toggle happyMessageToggle;
    public float selectedAnimalRatio;
    public Camera mainCam;
    public GameObject statsManager;
    StatsManagerScript statsManagerScript;
    public GameObject cameraInventoryManager;
    CamInventoryManagerScript camInvScript;
    public GameObject csvReadWriter;
    [HideInInspector]
    public CsvReadWrite readWriteScript;
    public GameObject menuManager;
    public GameObject fieldManager;
    [HideInInspector]
    public MenuManagerScript menuManagerScript;
    [HideInInspector]
    public FieldManager fieldManagerScript;
    public GameObject [] allAvailableAnimalPrefabs;
    public bool [] equippedAnimals;
    public List <GameObject> currentAnimals = new List<GameObject>();
    public int totalHeartsEarnedWhileSystemWasOff = 0;
    public List<AnimalStatsScript> animalStatsList = new List<AnimalStatsScript>();
    bool camerasAllMade = false;
    public bool okayToLoadEquipped = false;
    public bool allAnimalsExamined = false;
    //public GameObject welcomeSign;
    public GameObject welcomeHeart;
    public TextMeshProUGUI welcomeHeartsCount;
    public Button welcomeButton;
    public bool firstTimeHappy = true;
    public bool allScannedForUnlockAndUnequipped = false;
    public Dictionary <int,int> indexToXPosition, xPositionToIndex;
    // Start is called before the first frame update
    void Start()
    {
        //welcomeSign.SetActive(false);
    }
    //MakeCameras() is only called once, yet only when it's ready to do so
    //UpdateEquippedList() is called everyframe, and only when it's ready to do so
    void Update()
    {
        if (camInvScript != null && !camerasAllMade)
            MakeCameras();
        if (okayToLoadEquipped) {
            UpdateEquippedList();
            ScanForUnequippedAndUnlockedAnimalsForLoop();
        }
    }
    public void SetEverything()
    {
        indexToXPosition = new Dictionary<int,int>()
        {
            {0, 6},  //Horse
            {1, 5},  //Sheep
            {2, 4},  //Cow
            {3, 3},  //Pig
            {4, 2},  //Hen
            {5, 1},  //
            {6, 0},  //
            {7,-1}   //
        };
        xPositionToIndex = new Dictionary<int,int>()
        {
            { 6, 0},  //Horse
            { 5, 1},  //Sheep
            { 4, 2},  //Cow
            { 3, 3},  //Pig
            { 2, 4},  //Hen
            { 1, 5},  //
            { 0, 6},  //
            {-1, 7}   //
        };
        happyMessageToggle = happyMessageToggleObj.GetComponent<Toggle>();
        readWriteScript = csvReadWriter.GetComponent<CsvReadWrite>();
        menuManagerScript = menuManager.GetComponent<MenuManagerScript>();
        fieldManagerScript = fieldManager.GetComponent<FieldManager>();
        camInvScript = cameraInventoryManager.GetComponent<CamInventoryManagerScript>();
        statsManagerScript = statsManager.GetComponent<StatsManagerScript>();
        //MakeAnimals();
        firstTimeHappy = readWriteScript.GetFirstTimeHappy();
    }
    //Called by the CsvReadWrite.cs Script
    public AnimalStatsScript MakeAnimal(int i, StatsManagerScript newStat, string newName, int newScore, int newBrush, DateTime newDateLastBrushed, DateTime newPlayDate, DateTime newDateLastHeartEarned, float newMeters, bool newEquipped, bool newUnlocked)
    {
        SetEverything();
        Vector3 pos = new Vector3(indexToXPosition[i], 0, 9);
        Vector3 myRot = Vector3.up * 180;
        GameObject myAnimal = Instantiate(allAvailableAnimalPrefabs[i], pos, Quaternion.Euler(myRot)) as GameObject;
        AnimalScript myAnimalScript = myAnimal.GetComponent<AnimalScript>();
        AnimalStatsScript animalStats = myAnimal.AddComponent<AnimalStatsScript>();
        animalStats.statsManagerScript = newStat;
        animalStats.animalName = String.Copy(newName);
        animalStats.foodCount = newScore;
        animalStats.brushCount = newBrush;
        animalStats.dateLastPlay = newPlayDate;
        animalStats.dateLastBrushed = newDateLastBrushed;
        animalStats.dateLastHeartEarned = newDateLastHeartEarned;
        animalStats.metersTraveled = newMeters;
        animalStats.equipped = newEquipped;
        animalStats.unlocked = newUnlocked;
        animalStats.CalculateRemainingStats();
        animalStatsList.Add(animalStats);
        currentAnimals.Add(myAnimal);
        myAnimalScript.Init(menuManagerScript, fieldManagerScript, animalStatsList[i],readWriteScript, statsManagerScript, this);
        if (i == 0) {
            menuManagerScript.PassAlongHorseForTheTutorial(myAnimalScript);
        }
        return animalStats;
    }
    public void MakeCameras()
    {
        camerasAllMade = true;
        camInvScript.MakeLists(allAvailableAnimalPrefabs.Length);
        for (int i = 0; i < allAvailableAnimalPrefabs.Length; i++) {
            camInvScript.MakeACopy(allAvailableAnimalPrefabs[i], i, animalStatsList[i]);
        }
        cameraInventoryManager.SetActive(false);
    }
    public void ShutOffCharacterControllers()
    {
        StartCoroutine(ShutEmOff());
    }
    public IEnumerator ShutEmOff()
    {
        for (int i = 0; i < currentAnimals.Count; i++) {
            if (fieldManagerScript.selectedAnimal != null) {
                if (currentAnimals[i].name != fieldManagerScript.selectedAnimal.gameObject.name) {
                    currentAnimals[i].GetComponent<CharacterController>().enabled = false;
                }
            }
        }
        yield return new WaitForSeconds(9f);
        for (int i = 0; i < currentAnimals.Count; i++) {
            currentAnimals[i].GetComponent<CharacterController>().enabled = true;
        }
        yield return null;
    }
    public void UpdateEquippedList()
    {
        int listLength = allAvailableAnimalPrefabs.Length;
        equippedAnimals = new bool [listLength];
        for (int i = 0; i < listLength; i++) {
            equippedAnimals[i] = animalStatsList[i].equipped;
        }
    } //Getting an out of bounds error here? Check the names in the CSVReadWrite.cs and make sure all names are in animalNames[] array
    public void UpdateAllInventoryCameraStrings() {
        int length = equippedAnimals.Length;
        for (int i = 0; i < length; i++) {
            camInvScript.UpdateString(i, animalStatsList[i].level.ToString(), animalStatsList[i].foodToNextHappy.ToString(), animalStatsList[i].foodCount.ToString(), animalStatsList[i]);
        }
    }
    public bool AllAnimalsHaveGatheredHeartsWhileSystemWasOff()
    {
        for (int i = 0; i < currentAnimals.Count; i++) {
            bool allCalculated = currentAnimals[i].GetComponent<AnimalScript>().calculatedHeartsWhileSystemWasOff;
            if (!allCalculated) {
                return false;
            }
        }
        return true;
    }
    public void MakeWelcomeBackSign()
    {
        //currentWelcomeHearts is any saved hearts, just incase the system was turned off before actually collecting welcomeHearts
        int currentWelcomeHearts = readWriteScript.GetHeartsEarnedWhileSystemWasOff();
        totalHeartsEarnedWhileSystemWasOff += currentWelcomeHearts;
        if (totalHeartsEarnedWhileSystemWasOff > 0) {
            menuManagerScript.MakeWelcomeMessageAppear(totalHeartsEarnedWhileSystemWasOff);
            //Saves it just incase the system was turned off mid-welcome-message
            readWriteScript.SaveTotalHeartsWhileSystemWasOff(currentWelcomeHearts);
        }
    }
    //Just the visuals, called when the welcome heart is clicked
    public void AddWelcomeHeartsToTotalHearts()
    {
        StartCoroutine(AddWelcomeHeartsRoutine());
    }
    public IEnumerator AddWelcomeHeartsRoutine()
    {
        //welcomeButton.interactable = false;
        Vector3 pos = mainCam.ScreenToWorldPoint(welcomeHeart.transform.position);
        for (int i = 0; i < totalHeartsEarnedWhileSystemWasOff; i++) {
            menuManagerScript.MakeAHeartPlusOne(pos);
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(1.5f);
        //welcomeSign.SetActive(false);
        menuManagerScript.messageUp = false;
        welcomeHeartsCount.text = "0";
        totalHeartsEarnedWhileSystemWasOff = 0;
        readWriteScript.SaveTotalHeartsWhileSystemWasOff(0);
        yield return null;
    }
    public float GetNextFoodHappyRatio(AnimalStatsScript anim)
    {
        /*
        for (int i = 0; i < animalStatsList.Count; i ++) {
            if (String.Compare(animalStatsList[i].animalName, anim.animalName) == 0) {
                int animLevel = animalStatsList[i].level;
                int totalFoodForNextLevel = statsManagerScript.foodToNextHappy[animLevel];
                int totalFoodForPrevLevel = 0;
                if (animalStatsList[i].level > 0) {
                    totalFoodForPrevLevel = statsManagerScript.foodToNextHappy[animLevel-1];
                }
                int foodCount = animalStatsList[i].foodCount - totalFoodForPrevLevel;
                int totalFoodThisLevel = totalFoodForNextLevel - totalFoodForPrevLevel;
                selectedAnimalRatio = (float) foodCount / totalFoodThisLevel;
                //Debug.Log("RATIO = "+totalFoodThisLevel+" / "+foodCount+" = "+selectedAnimalRatio);
                return selectedAnimalRatio;
            }
        }
        return 0f;
        */

        int animLevel = anim.level;
        int totalFoodForNextLevel = statsManagerScript.foodToNextHappy[animLevel];
        int totalFoodForPrevLevel = 0;
        if (anim.level > 0) {
            totalFoodForPrevLevel = statsManagerScript.foodToNextHappy[animLevel-1];
        }
        float foodCount = (float) anim.foodCount - totalFoodForPrevLevel;
        float totalFoodThisLevel = (float) (totalFoodForNextLevel - totalFoodForPrevLevel);
        //Debug.Log(anim.animalName+" FOOD PERCENTAGE = "+((float) foodCount / totalFoodThisLevel).ToString());
        return (float) foodCount / totalFoodThisLevel;
    }
    public float GetNextBrushHappyRatio(AnimalStatsScript anim)
    {
        int animLevel = anim.level;
        int totalBrushForNextLevel = statsManagerScript.brushToNextHappy[animLevel];
        int totalBrushForPrevLevel = 0;
        if (anim.level > 0) {
            totalBrushForPrevLevel = statsManagerScript.brushToNextHappy[animLevel-1];
        }
        float brushCount = (float) anim.brushCount;
        float totalBrushThisLevel = (float) (totalBrushForNextLevel - totalBrushForPrevLevel);
        return (float) brushCount / totalBrushThisLevel;
    }
    public void GetAllAnimalsCalculatedHearts()
    {
        int gatheredHearts = 0;
        for (int i = 0; i < currentAnimals.Count; i++) {
            AnimalScript myAnim = currentAnimals[i].GetComponent<AnimalScript>();
            int animHearts = myAnim.CalculateHowManyHeartsWereEarned();
            if (animHearts > 0) {
                myAnim.myStats.dateLastHeartEarned = System.DateTime.Now;
            }
            gatheredHearts += animHearts;
        }
        totalHeartsEarnedWhileSystemWasOff = gatheredHearts;
        MakeWelcomeBackSign();
    }
    public void SetFirstTimeHappyMessage()
    {
        firstTimeHappy = happyMessageToggle.isOn;
        readWriteScript.SaveHappyMessageTutorialToggle(firstTimeHappy);
    }
    public int GetAnimalPositionInAllThings(string myName)
    {
        for (int i = 0; i < allAvailableAnimalPrefabs.Length; i++) {
            if (String.Compare(animalStatsList[i].animalName, myName) == 0) {
                return i;
            }
        }
        return -1;
    }
    //Called by the StatsManagerScript
    public void MarkAnimalAsUnlockAnimalAtIndex(int index)
    {
        if (index < animalStatsList.Count) {
            if (animalStatsList[index].equipped == false && animalStatsList[index].unlocked == false) {
                animalStatsList[index].unlocked = true;
            }
        }
    }
    //Called from the MenuManagerScript
    public IEnumerator UnlockAnimation(int royPos)
    {
        int index = xPositionToIndex[royPos];
        AnimalScript myAnimalScript = currentAnimals[index].GetComponent<AnimalScript>();
        if (myAnimalScript.myLock != null) {
            Animator lockAnim = myAnimalScript.myLock.GetComponentInChildren<Animator>();
            if (lockAnim != null) {
                yield return null;
                lockAnim.SetTrigger("Unlocked");
            }
        }
        yield return null;
    }
    //Called from the menuManagerScript
    public void DisableLockAndChangeAnimalSkin(int royPos)
    {
        int index = xPositionToIndex[royPos];
        if (animalStatsList[index].equipped == false) {
            //Turns off the animal's lock
            AnimalScript myAnimalScript = currentAnimals[index].GetComponent<AnimalScript>();
            if (myAnimalScript.myLock != null) {
                myAnimalScript.myLock.SetActive(false);
            }
            //Change Material from the black to normal, animal is constantly scanning for being equipped
            myAnimalScript.myStats.equipped = true;
            readWriteScript.UpdateAnimalEquipped(myAnimalScript.myStats.animalName);
            //TODO tell the CamInventoryManagerScript that we need to enable the tiles for this new animal
            camInvScript.EnableAnimalHealthForAnimal(index);
        }
    }
    public void TellAllAnimalsToGoHome()
    {
        for (int i = 0; i < currentAnimals.Count; i++) {
            currentAnimals[i].GetComponent<AnimalScript>().GoBackHome();
        }
        /*
        if (fieldManagerScript.selectedAnimal != null) {
            fieldManagerScript.selectedAnimal.GetComponent<AnimalScript>().GoBackHome();
        }
        */
    }
    //Constantly scans all animals looking for those marked as unequipped and unlocked
    public void ScanForUnequippedAndUnlockedAnimalsForLoop()
    {
        for (int i = currentAnimals.Count-1; i >= 0; i--) {
            AnimalStatsScript animStats = currentAnimals[i].GetComponent<AnimalStatsScript>();
            //if (!animStats.equipped && animStats.unlocked && !animStats.scannedForUnlock) {
            if (!animStats.equipped && animStats.unlocked) {
                //boolean makes sure that the scan only happens once per animal
                ScanForUnequippedAndUnlockedAnimals(i, animStats);
            }
        }
    }
    public void ScanForUnequippedAndUnlockedAnimals(int pos, AnimalStatsScript animStats)
    {
        if (menuManagerScript.atTheField || menuManagerScript.atTheStables) {
            if (menuManagerScript.royIsRoosted) {
                animStats.scannedForUnlock = true;
                //Make the "you earned" message appear & get position for Roy to go
                string name = animalStatsList[pos].animalName;
                Vector3 royPos = animalStatsList[pos].gameObject.transform.position + Vector3.up;
                menuManagerScript.MakeUnlockedNewAnimalMessage(name, royPos);
                //Pan Camera to the new Animal
                Vector3 newPosition = royPos + (Vector3.back*3);
                Quaternion newRotation = Quaternion.Euler(Vector3.forward);
                if (menuManagerScript.atTheField) {
                    menuManagerScript.wasAtTheFieldBeforeUnlockMessage = true;
                    menuManagerScript.MoveCameraToAnimalFromField(newPosition, newRotation);
                }
            }
        }
    }
    public bool AreAnyAnimalsUnequippedAndUnlocked()
    {
        for (int i = currentAnimals.Count-1; i >= 0; i--) {
            AnimalStatsScript animStats = currentAnimals[i].GetComponent<AnimalStatsScript>();
            if (!animStats.equipped && animStats.unlocked) {
                return true;
            }
        }
        return false;
    }
}
