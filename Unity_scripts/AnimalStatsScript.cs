using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AnimalStatsScript : MonoBehaviour
{
    [Header("STORED VALUES")]
    public StatsManagerScript statsManagerScript;
    public string animalName;
    public int foodCount;
    public int brushCount;
    public DateTime dateLastBrushed;
    public string dateLastBrushedS;
    public DateTime dateLastPlay;
    public string dateLastPlayS;
    public DateTime [] dateSinceLastInitiallySetHappy = new DateTime [10];
    public string [] dateLastHappyS;
    public DateTime dateLastHeartEarned;
    public string dateLastHeartS;
    public float metersTraveled;
    public bool equipped;
    public bool unlocked;
    [Header("CALCULATED VALUES")]
    public int level;
    public int foodToNextHappy;
    public int brushToNextHappy;
    public int durationBetweenLastHeartEarnedAndNow;
    public bool scannedForUnlock = false;

    public void Start()
    {

        dateLastHappyS = new string [10];
    }
    public void Update()
    {
        dateLastBrushedS = dateLastBrushed.ToString();
        dateLastPlayS = dateLastPlay.ToString();
        for (int i = 0; i < 10; i++) {
            dateLastHappyS[i] = dateSinceLastInitiallySetHappy[i].ToString();
        }
        dateLastHeartS = dateLastHeartEarned.ToString();
    }
    public AnimalStatsScript(StatsManagerScript newStat, string newName, string newScore, DateTime newDate, DateTime newDateLastHeartEarned, string newMeters, string newEquipped, string newUnlocked)
    {
        statsManagerScript = newStat;
        animalName = String.Copy(newName);
        foodCount = int.Parse(newScore);
        brushCount = int.Parse(newScore);
        dateLastPlay = newDate;
        dateLastHeartEarned = newDateLastHeartEarned;
        metersTraveled = float.Parse(newMeters);
        equipped = bool.Parse(newEquipped);
        unlocked = bool.Parse(newUnlocked);
        CalculateRemainingStats();
    }
    public AnimalStatsScript(AnimalStatsScript animalToCopy)
    {
        statsManagerScript = animalToCopy.statsManagerScript;
        animalName = String.Copy(animalToCopy.animalName);
        foodCount = animalToCopy.foodCount;
        brushCount = animalToCopy.brushCount;
        dateLastPlay = animalToCopy.dateLastPlay;
        for (int i = 0; i < 10; i++) {
            dateSinceLastInitiallySetHappy[i] = animalToCopy.dateSinceLastInitiallySetHappy[i];
        }
        dateLastHeartEarned = animalToCopy.dateLastHeartEarned;
        metersTraveled = animalToCopy.metersTraveled;
        equipped = animalToCopy.equipped;
        unlocked = animalToCopy.unlocked;
        CalculateRemainingStats();
    }
    public void SetAllDateSinceLastInitHappy(DateTime [] newdateSinceLastInitHappy)
    {
        dateSinceLastInitiallySetHappy = new DateTime [10];
        for (int i = 0; i < 10; i++) {
            dateSinceLastInitiallySetHappy[i] = newdateSinceLastInitHappy[i];
        }
    }
    public void CalculateRemainingStats()
    {
        level = statsManagerScript.GetLevel(foodCount);
        foodToNextHappy = statsManagerScript.GetNextFoodHappy(foodCount, level);
        brushToNextHappy = (int) Mathf.Round(statsManagerScript.GetNextBrushHappy(this)*100f);
    }
}
