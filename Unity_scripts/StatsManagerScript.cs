using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsManagerScript : MonoBehaviour
{
    [HideInInspector]
    public int totalPotentialSizeOfAllAnimals = 100, firstHeartsEquipped = 10;
    public int firstLevelFood = 10;
    public float firstLevelWalk = 50f;
    public int growth = 5;
    public int [] foodToNextHappy, brushToNextHappy, heartsToEarnNextAnimal;
    public float [] walkToNextHappy;
    public int heartCount = 0;
    public int duplicateHeartCounterThatOnlyExistsToTrackChangeInAmount = 0;
    public bool tutorial = true;
    public int happinessDurationMinutes = 5;
    public int heartBeatDurationSeconds = 0;
    public GameObject inventoryManager;
    InventoryManagerScript inventoryManagerScript;
    public GameObject csvReadWriterObj;
    CsvReadWrite CSVWriter;
    // Start is called before the first frame update
    void Start()
    {
        CSVWriter = csvReadWriterObj.GetComponent<CsvReadWrite>();
        inventoryManagerScript = inventoryManager.GetComponent<InventoryManagerScript>();
        CreateArrays();
    }

    // Update is called once per frame
    void Update()
    {
        CheckHeartsWithEquippedList();
        UpdateHeartBeat();
    }
    public void CreateArrays()
    {
        foodToNextHappy = new int[totalPotentialSizeOfAllAnimals];
        foodToNextHappy[0] = 0;
        foodToNextHappy[1] = firstLevelFood; //10
        for (int i = 2; i < totalPotentialSizeOfAllAnimals; i++) {
            foodToNextHappy[i] = foodToNextHappy[i-1] + (firstLevelFood * i);
        }
        brushToNextHappy = new int[totalPotentialSizeOfAllAnimals];
        brushToNextHappy[0] = 0;
        brushToNextHappy[1] = firstLevelFood*40; //400
        for (int i = 2; i < totalPotentialSizeOfAllAnimals; i++) {
            brushToNextHappy[i] = brushToNextHappy[i-1] + (firstLevelFood * i * 40);
        }

        heartsToEarnNextAnimal = new int[totalPotentialSizeOfAllAnimals];
        heartsToEarnNextAnimal[0] = 0; //Horse
        heartsToEarnNextAnimal[1] = 0; //Sheep
        heartsToEarnNextAnimal[2] = 0; //Cow
        heartsToEarnNextAnimal[3] = firstHeartsEquipped; //Pig
        for (int i = 4; i < totalPotentialSizeOfAllAnimals; i++) {
            heartsToEarnNextAnimal[i] = heartsToEarnNextAnimal[i-1] + (firstHeartsEquipped * i * 2);
        }
    }
    public int GetLevel(int foodCount)
    {
        for (int i = 0; i < totalPotentialSizeOfAllAnimals; i++) {
            if (foodCount >= foodToNextHappy[i] && foodCount < foodToNextHappy[i+1]) {
                return i+1;
            }
        }
        return 0;
    }
    public int GetNextFoodHappy(int foodCount, int level)
    {
        return foodToNextHappy[level]-foodCount;
    }
    public int GetNextBrushHappy(AnimalStatsScript animScript)
    {
        return (int) Mathf.Round(inventoryManagerScript.GetNextBrushHappyRatio(animScript));
    }
    public void CheckHeartsWithEquippedList()
    {
        //Do this only when heartCount is initially changed so that it doesn't keep having a single animal become equipped over and over
        int wasHeartCount = heartCount;
        heartCount = duplicateHeartCounterThatOnlyExistsToTrackChangeInAmount;
        if (wasHeartCount != heartCount) {
            //Check to see when heartCount == heartsToEarnNextAnimal[i]
            for (int i = 3; i < 99; i++) {
                if (heartCount >= heartsToEarnNextAnimal[i]) {
                    inventoryManagerScript.MarkAnimalAsUnlockAnimalAtIndex(i);
                }
            }
        }
    }
    //Called by the MenuManagerScript
    public void IncreaseDuplicateHeartCounterByOneWhichExistsOnlyToTrackChangeInHeartCount()
    {
        duplicateHeartCounterThatOnlyExistsToTrackChangeInAmount += 1;
    }
    public void UpdateHeartBeat()
    {
        //This is a problem that each animal is loading and saving to the CSVwriter ever 15 seconds.
        //StatsManager needs to do this and the animal can get that information.
        heartBeatDurationSeconds = CSVWriter.GetApplicationHeartBeatDuration();
        if (heartBeatDurationSeconds <= 15) {
            //System is still running
            CSVWriter.SaveApplicationHeartBeatTime(System.DateTime.Now);
        } else {
            //Some time had passed, we need to get all the animal's earned hearts and make a welcome message
            CSVWriter.SaveApplicationHeartBeatTime(System.DateTime.Now);
        }
    }
}
