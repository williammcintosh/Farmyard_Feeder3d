using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System;

public class CsvReadWrite : MonoBehaviour
{
    public GameObject inventoryManager;
    InventoryManagerScript inventoryManagerScript;
    public GameObject statsManagerObj;
    StatsManagerScript statsManager;
    public string [] animalNames;
    //private List<string[]> rowData = new List<string[]>();
    //public bool printList = false;
    //public int animalDataHeight, animalDataWidth, userDataHeight, userDataWidth;
    public bool saveShit = false;
    // Use this for initialization
    void Start () {
        CheckForNewValuesInAnUpdate();
        inventoryManagerScript = inventoryManager.GetComponent<InventoryManagerScript>();
        BuildAnimalNamesArray();
        SetupInitialSaveFileCreation();
        statsManager = statsManagerObj.GetComponent<StatsManagerScript>();
        Read();
    }
    void Update() {
        if (saveShit) {
            saveShit = false;
            Save();
        }
    }

    public void Read()
    {
        LoadAnimalList();
        LoadUserStatsAndSettings();
    }
    public void Save()
    {
        StoreAnimalData();
        StoreUserStatsAndSettings();
    }
    public void LoadAnimalList()
    {
        for (int i = 0; i < animalNames.Length; i++) {
            string newName = animalNames[i];

            int newScore = ES3.Load<int>((newName+"FoodCount").ToString());
            int newBrush = ES3.Load<int>((newName+"BrushCount").ToString());
            DateTime newBrushedDate = ES3.Load<DateTime>((newName+"DateLastBrushed").ToString());
            DateTime newPlayDate = ES3.Load<DateTime>((newName+"DateLastPlay").ToString());
            DateTime newDateLastHeartEarned = ES3.Load<DateTime>((newName+"DateLastHeartEarned").ToString());
            float newMeters = ES3.Load<float>((newName+"MetersTraveled").ToString());
            bool newEquipped = ES3.Load<bool>((newName+"IsEquipped").ToString());
            bool newUnlocked = ES3.Load<bool>((newName+"IsUnlocked").ToString());
            AnimalStatsScript animalStats = inventoryManagerScript.MakeAnimal(i, statsManager, newName, newScore, newBrush, newBrushedDate, newPlayDate, newDateLastHeartEarned, newMeters, newEquipped, newUnlocked);
            //Setting the array of initially set happy times
            DateTime [] newdateSinceLastInitHappy = new DateTime [100];
            for (int j = 0; j < 10; j++) {
                newdateSinceLastInitHappy[j] = ES3.Load<DateTime>((newName+"DateSinceLastInitHappy"+j).ToString());
            }
            animalStats.SetAllDateSinceLastInitHappy(newdateSinceLastInitHappy);
        }
        inventoryManagerScript.okayToLoadEquipped = true;
    }
    public void LoadUserStatsAndSettings()
    {
        statsManager.duplicateHeartCounterThatOnlyExistsToTrackChangeInAmount = ES3.Load<int>("UserHeartCount");
        statsManager.tutorial = ES3.Load<bool>("UserWantsTutorial");
    }
    public void StoreAnimalData()
    {
        for(int i = 0; i < animalNames.Length; i++){
            string newName = animalNames[i];
            AnimalStatsScript animal = inventoryManagerScript.animalStatsList[i];
            ES3.Save((newName+"FoodCount").ToString(), animal.foodCount);
            ES3.Save((newName+"BrushCount").ToString(), animal.brushCount);
            ES3.Save((newName+"DateLastBrushed").ToString(), animal.dateLastBrushed);
            ES3.Save((newName+"DateLastPlay").ToString(), animal.dateLastPlay);
            for (int j = 0; j < 10; j++) {
                ES3.Save((newName+"DateSinceLastInitHappy"+j).ToString(), animal.dateSinceLastInitiallySetHappy[j]);
            }
            ES3.Save((newName+"DateLastHeartEarned").ToString(), animal.dateLastHeartEarned);
            ES3.Save((newName+"MetersTraveled").ToString(), animal.metersTraveled);
            ES3.Save((newName+"IsEquipped").ToString(), animal.equipped);
            ES3.Save((newName+"IsUnlocked").ToString(), animal.unlocked);
        }
    }
    public void StoreUserStatsAndSettings()
    {
        ES3.Save("UserHeartCount", statsManager.heartCount);
        ES3.Save("UserWantsTutorial", statsManager.tutorial);
    }
    public void BuildAnimalNamesArray()
    {
        animalNames = new string [] {"Horse", "Sheep", "Cow", "Pig", "Hen", "Donkey", "Duck"};
    }
    // The purpose of this function is to create the files that don't yet exist
    // Because the animals haven't been earned yet in the game.
    public void SetupInitialSaveFileCreation()
    {
        string myVersion = (float.Parse(Application.version)*10f).ToString();
        //Debug.Log("SaveFile"+myVersion+".es3");
        if(!ES3.FileExists("SaveFile"+myVersion+".es3")) {
            CreateUserStatsAndSettings();
            CreateAnimalFiles();
        }
    }
    public void CreateUserStatsAndSettings()
    {
        ES3.Save("UserHeartCount", (int) 0);
        ES3.Save("UserWantsTutorial", (bool) true);
        ES3.Save("ApplicationHeartBeat", (DateTime) System.DateTime.Now);
    }
    public void CreateAnimalFiles()
    {
        statsManager = statsManagerObj.GetComponent<StatsManagerScript>();
        for (int i = 0; i < animalNames.Length; i++) {
            string newName = animalNames[i];
            DateTime lastPlay = (DateTime) DateTime.Parse("10/16/1990 12:00:00 PM");
            ES3.Save((newName+"FoodCount").ToString(), (int) 0);
            ES3.Save((newName+"BrushCount").ToString(), (int) 960);
            ES3.Save((newName+"DateLastBrushed").ToString(), (DateTime) System.DateTime.Now);
            ES3.Save((newName+"DateLastPlay").ToString(), (DateTime) lastPlay);
            for (int j = 0; j < 10; j++) {
                ES3.Save((newName+"DateSinceLastInitHappy"+j).ToString(), (DateTime) lastPlay);
            }
            ES3.Save((newName+"DateLastHeartEarned").ToString(), lastPlay.AddMinutes(statsManager.happinessDurationMinutes));
            ES3.Save((newName+"MetersTraveled").ToString(), (float) 0.0f);
            if (i < 3) {
                ES3.Save((newName+"IsEquipped").ToString(), (bool) true);
                ES3.Save((newName+"IsUnlocked").ToString(), (bool) true);
            } else {
                ES3.Save((newName+"IsEquipped").ToString(), (bool) false);
                ES3.Save((newName+"IsUnlocked").ToString(), (bool) false);
            }
        }
    }
    public void AddTotalHearts(int additionHearts)
    {
        int totalHearts = statsManager.heartCount + additionHearts;
        ES3.Save("UserHeartCount", (int) totalHearts);
    }
    public void SaveTotalHeartsWhileSystemWasOff(int total)
    {
        ES3.Save("totalHeartsEarnedWhileSystemWasOff", total);
    }
    public int LoadHearts()
    {
        return ES3.Load<int>("totalHeartsEarnedWhileSystemWasOff");
    }
    public void ResetEverything()
    {
        CreateUserStatsAndSettings();
        CreateAnimalFiles();
        ES3.Save(("HorseFoodCount").ToString(), (int) 0);
        ES3.Save(("HorseBrushCount").ToString(), (int) 960);
        ES3.Save("firstTimeHappyTutorial", false);
        ES3.Save("firstTimeRoyTutorial", false);
        statsManager.duplicateHeartCounterThatOnlyExistsToTrackChangeInAmount = 0;
    }
    public bool GetFirstTimeHappy()
    {
        if (ES3.KeyExists("firstTimeHappyTutorial")) {
            return ES3.Load<bool>("firstTimeHappyTutorial");
        }
        //If the file doesn't yet exist, then it's true
        ES3.Save("firstTimeHappyTutorial", true);
        return true;
    }
    public void SaveHappyMessageTutorialToggle(bool toggle)
    {
        ES3.Save("firstTimeHappyTutorial", toggle);
    }
    public bool GetFirstTimeRoyTutorial()
    {
        string myVersion = (float.Parse(Application.version)*10f).ToString();
        //Debug.Log("SaveFile"+myVersion+".es3");

        if(!ES3.FileExists("SaveFile"+myVersion+".es3")) {
            CreateUserStatsAndSettings();
            CreateAnimalFiles();
        } else if (ES3.KeyExists("firstTimeRoyTutorial")) {
            return ES3.Load<bool>("firstTimeRoyTutorial");
        }
        //If the file doesn't yet exist, then it's false
        ES3.Save("firstTimeRoyTutorial", false);
        return false;
    }
    public void SaveNoMoreTour()
    {
        ES3.Save("firstTimeRoyTutorial", true);
    }
    public void SaveRestartTour()
    {
        ES3.Save("firstTimeRoyTutorial", false);
    }
    public void SaveDateLastHeartEarned(string newName, DateTime newDate)
    {
        ES3.Save((newName+"DateLastHeartEarned").ToString(), (DateTime) newDate);
    }
    public void SaveFoodCount(string newName, int newFoodCount)
    {
        ES3.Save((newName+"FoodCount").ToString(), (int) newFoodCount);
    }
    public void SaveBrushCount(string newName, int newBrushCount)
    {
        ES3.Save((newName+"BrushCount").ToString(), (int) newBrushCount);
    }
    public void SaveDateLastBrushed(string newName, DateTime timeStamp)
    {
        ES3.Save((newName+"DateLastBrushed").ToString(), (DateTime) timeStamp);
    }
    public void SaveApplicationHeartBeatTime(DateTime timeStamp)
    {
        ES3.Save("ApplicationHeartBeat", (DateTime) timeStamp);
    }
    public int GetApplicationHeartBeatDuration()
    {
        if (ES3.KeyExists("ApplicationHeartBeat")) {
            DateTime lastHeartBeat = ES3.Load<DateTime>("ApplicationHeartBeat");
            int duration = (int) Math.Round((System.DateTime.Now - lastHeartBeat).TotalSeconds);
            return duration;
        }
        //If the value doesn't yet exist, then it's 0 seconds
        ES3.Save("ApplicationHeartBeat", (DateTime) System.DateTime.Now);
        return 0;
    }
    public int GetHeartsEarnedWhileSystemWasOff()
    {
        if (ES3.KeyExists("totalHeartsEarnedWhileSystemWasOff")) {
            return ES3.Load<int>("totalHeartsEarnedWhileSystemWasOff");
        }
        ES3.Save("totalHeartsEarnedWhileSystemWasOff", (int) 0);
        return 0;
    }
    public void UpdateAnimalEquipped(string newName)
    {
        ES3.Save((newName+"IsEquipped").ToString(), true);
    }
    public void CheckForNewValuesInAnUpdate()
    {
        return;
    }
}
