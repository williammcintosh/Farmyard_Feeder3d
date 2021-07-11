using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AnimalScript : MonoBehaviour
{
    public float myCleanlinessPercentage;
    public bool mymaterialSet = false;
    public GameObject myLock;
    UnlockAnimScript lockScript;
    public float eatRadius = 0.8f;
    public Material myMat;
    public SkinnedMeshRenderer myRend;
    public GameObject heartPlacementObj;
    public bool printMyStuff = false;
    public bool displayDateDurationTest = false;
    public bool ImADud = false;
    public TimeSpan [] happyTimer;
    public string [] happyTimerS;
    [HideInInspector]
    public InventoryManagerScript inventoryManagerScript;
    [HideInInspector]
    public CsvReadWrite CSVWriter;
    [HideInInspector]
    public Material foodMat;
    [HideInInspector]
    public GameObject foodToEat;
    [HideInInspector]
    public StatsManagerScript statsManagerScript;
    public ParticleSystem heartParticlePrefab, cleanRainbowGlowPrefab, fliesPrefab, dustPrefab;
    ParticleSystem myCleanRainbowGlow, myFliesCloud, myDustCloud;
    public Material myDirtMat;
    public float particleYPos = 1.3f;
    ParticleSystem [] myHeart = new ParticleSystem [10];
    public Vector3 heartPlacement = Vector3.one;
    public bool happy;
    bool [] heartMade = new bool [10];
    [HideInInspector]
    public AnimalStatsScript myStats;
    public ParticleSystem munchCrumbsPrefab;
    public float distance = 0.0f;
    public float foodEatDistance = 0.4f;
    public bool reachedDestination = true;
    [HideInInspector]
    public CharacterController controller;
    public Vector3 destination;
    float moveSpeed = 1.5f;
    Animator myAnim;
    MenuManagerScript menuManagerScript;
    FieldManager fieldManagerScript;
    public Queue<PickUpScript> foodQueue = new Queue<PickUpScript>();
    PickUpScript nextFood;
    public bool currentlyEatting = false;
    public LayerMask whatIsPickUp;
    public bool currentlyMoving = false;
    public AudioClip [] animalSounds, footsteps, bitingSounds, winSounds;
    public bool playAnimalSounds = false, playFootSteps = false;
    bool loopAnimalSounds = true, loopFootSteps = true, canPlayNewBitingSound = true;
    AudioSource [] myAudioSources;
    int startingXPos = 0;
    bool initialized = false;
    public bool calculatedHeartsWhileSystemWasOff = false;
    float initMoveSpeed;
    float cleanlinessSpeed = 1.0f;

    public void Start()
    {
        for (int i = 0; i < 10; i++) {
            heartMade[i] = false;
        }
        happyTimer = new TimeSpan [10];
        happyTimerS = new string [10];
        myRend = gameObject.GetComponentInChildren<SkinnedMeshRenderer>();
        initMoveSpeed = moveSpeed;
        InitSounds();
        startingXPos = (int) this.transform.position.x;
        controller = gameObject.GetComponent<CharacterController>();
        destination = this.transform.position;
        myAnim = gameObject.GetComponentInChildren<Animator>();
        myAnim.SetTrigger("Idle");
    }
    public void Update()
    {
        if (initialized) {
            if (menuManagerScript.onlyHorse) {
                moveSpeed = initMoveSpeed;
            } else {
                if (menuManagerScript.happyMessageUp || menuManagerScript.messageUp) {
                    moveSpeed = 0f;
                } else {
                    moveSpeed = initMoveSpeed;
                }
            }
            if (myStats != null) {
                //SetUnlocked();
                SetMaterial();
                Happiness();
                Cleanliness();
            }
            if (!ImADud) {
                Walk();
            }
            if (playAnimalSounds && loopAnimalSounds) {
                StartCoroutine(PlayAnimalSounds());
            }
            if (playFootSteps && loopFootSteps) {
                StartCoroutine(PlayFootSteps());
            }
            if (statsManagerScript!= null && inventoryManagerScript != null) {
                if (statsManagerScript.heartBeatDurationSeconds <= 15) {
                    int heartsEarned = CalculateHowManyHeartsWereEarned();
                    StartCoroutine(MakeHearts(heartsEarned));
                } else {
                    inventoryManagerScript.GetAllAnimalsCalculatedHearts();
                }
            }
        }

    }
    public IEnumerator MakeHearts(int heartsEarned)
    {
        for (int i = 0; i < heartsEarned; i++) {
            MakeHeartPlusOne();
            yield return null;
        }
        yield return null;
    }
    public void Init(MenuManagerScript theMenuM, FieldManager FMScript, AnimalStatsScript newStats, CsvReadWrite CSVWrite, StatsManagerScript newStatsManager, InventoryManagerScript newInventoryManager)
    {
        menuManagerScript = theMenuM;
        fieldManagerScript = FMScript;
        myStats = newStats;
        CSVWriter = CSVWrite;
        statsManagerScript = newStatsManager;
        initialized = true;
        inventoryManagerScript = newInventoryManager;
    }
    void OnMouseDown()
    {
        if (!menuManagerScript.messageUp && !menuManagerScript.tutorialPause) {
            if (menuManagerScript.onlyHorse) { //For tutorial
                if (menuManagerScript.horseReady && String.Compare(myStats.animalName, "Horse") == 0) {
                    GoAnimalGo();
                }
            } else {
                if (myStats.equipped) {
                    GoAnimalGo();
                } else {
                    DisplayUnequippedMessage();
                }
            }
        }
    }
    public void SetCalculatedHeartsWhileSystemWasOffToFalse()
    {
        Debug.Log(myStats.animalName+" IS TURNING OFF 'CALCULATEDHEARTS'");
        calculatedHeartsWhileSystemWasOff = false;
    }
    public void MoveOutToField()
    {
        Vector3 myStablePos = new Vector3(startingXPos, 0, 9);
        if (Vector3.Distance(this.transform.position, myStablePos) <= 0.5f) {
            fieldManagerScript.ClearAllFood();
            Vector3 forward = this.transform.TransformDirection(Vector3.forward*2);
            destination = this.transform.position+forward;
            fieldManagerScript.selectedAnimal = this;
            if (menuManagerScript.tourDone) {
                menuManagerScript.MoveCameraToAnimal(this);
            } else {
                menuManagerScript.goToField = true;
            }
        }
    }
    public void GoAnimalGo()
    {
        if (!currentlyEatting && !currentlyMoving) {
            if (menuManagerScript.atTheStables) {
                menuManagerScript.videoAdsButton.SetActive(false);
                menuManagerScript.getATourButton.SetActive(false);
                MoveOutToField();
            } else if (menuManagerScript.atTheField) {
                if (String.Compare(fieldManagerScript.selectedAnimal.name.ToUpper(), this.gameObject.name.ToUpper()) == 0) {
                    if (foodQueue.Count <= 0) {
                        myAnim.SetTrigger("Spin");
                    } else {
                        StartCoroutine(FollowAllFoods());
                        playAnimalSounds = true;
                    }
                }
            }
        }
    }
    public void Walk()
    {
        Vector3 goal = destination - this.transform.position;
        distance = Vector3.Distance(destination, this.transform.position);
        if (!currentlyEatting) {
            controller.Move(goal * Time.deltaTime * moveSpeed * cleanlinessSpeed);
        }
        reachedDestination = ReachedDestination();
        if (reachedDestination)
        {
            currentlyMoving = false;
            myAnim.SetTrigger("Idle");
            nextFood = null;
            destination = this.transform.position;
            playFootSteps = false;
        }
        else
        {
            currentlyMoving = true;
            myAnim.SetTrigger("Walk");
            playFootSteps = true;
            if (nextFood != null) {
                LookAtNextFood(nextFood.transform.position, 20f);
            }
        }
    }
    public void InitSounds()
    {
        for (int i = 0; i < 4; i++) {
            gameObject.AddComponent<AudioSource>();
        }
        myAudioSources = gameObject.GetComponents<AudioSource>();
        myAudioSources[0].volume = 0.75f;
        myAudioSources[1].volume = 0.05f;
        myAudioSources[2].volume = 0.75f;
    }
    public bool ReachedDestination()
    {
        if (distance <= 0.3f)
        {
            Vector3 myPos = this.transform.position;
            Vector3 lockPos = new Vector3(Mathf.Round(myPos.x), Mathf.Round(myPos.y), Mathf.Round(myPos.z));
            this.transform.position = lockPos;
            return true;
        }
        return false;
    }
    public void LookAtNextFood(Vector3 position, float turnSpeed)
    {
        Vector3 targetPoint = new Vector3(position.x, this.transform.position.y, position.z) - this.transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(targetPoint, Vector3.up);
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, targetRotation, Time.deltaTime*turnSpeed);
    }
    public IEnumerator FollowAllFoods()
    {
        for (int i = 0; i < 100000 && foodQueue.Count > 0; i++)
        {
            if (i == 0 || nextFood == null)
                nextFood = foodQueue.Dequeue();
            if (nextFood != null) {
                if (!nextFood.currentlyBeingCarried) {
                    nextFood.GetComponent<PickUpScript>().CantBeLifted();
                    destination = nextFood.transform.position;
                    //Makes the animal wait until it's reached this nextFood before dequeuing the next
                    distance = Vector3.Distance(destination, this.transform.position);
                    UpdateStats(distance);
                    for (int j = 0; j < 400 && distance > 0.2f; j++) {
                        //Vector3 forward = this.transform.TransformDirection(Vector3.forward)*foodEatDistance;
                        //Collider[] hitColliders = Physics.OverlapSphere(transform.position+foodEatDistance, 0.2f, whatIsPickUp);
                        Collider[] hitColliders = Physics.OverlapSphere(transform.position, eatRadius, whatIsPickUp);
                        if (hitColliders.Length > 0) {
                            for (int k = 0; k < hitColliders.Length; k++) {
                                if (hitColliders[k] != null) {
                                    GameObject myFood = hitColliders[k].gameObject;
                                    //Checks if this is a brand new piece of food, so we don't double count
                                    if (foodToEat != myFood) {
                                        myFood.GetComponent<PickUpScript>().CantBeLifted();
                                        this.transform.LookAt(myFood.transform.position);
                                        foodMat = myFood.GetComponentInChildren<Renderer>().material;
                                        foodToEat = myFood;
                                        currentlyEatting = true;
                                        myAnim.SetTrigger("Eat");
                                        for (int l = 0; l < 200 && hitColliders[k] != null; l++)
                                            yield return null;
                                        yield return new WaitForSeconds(0.25f);
                                        currentlyEatting = false;
                                        myStats.foodCount++;
                                        myAnim.SetTrigger("Idle");
                                    }
                                }
                            }
                        } else {
                            currentlyEatting = false;
                            foodMat = null;
                            foodToEat = null;
                        }
                        yield return null;
                    }
                }
            }
            //yield return new WaitForSeconds(0.05f);
            currentlyEatting = false;
            myAnim.SetTrigger("Idle");
            yield return null;
        }
        playAnimalSounds = false;
        yield return new WaitForSeconds(1f);
        myAudioSources[3].clip = winSounds[0];
        myAudioSources[3].Play();
        myAnim.SetTrigger("Roll");
        //SAVE TO FILE
        fieldManagerScript.ClearAllFood();
        CSVWriter.SaveFoodCount(myStats.animalName, myStats.foodCount);
        //CSVWriter.saveShit = true;
        yield return new WaitForSeconds(0.5f);
        myAnim.SetTrigger("Idle");
        yield return null;
    }
    public IEnumerator PlayAnimalSounds()
    {
        loopAnimalSounds = false;
        int randomSound = UnityEngine.Random.Range(0,animalSounds.Length-1);
        myAudioSources[0].clip = animalSounds[randomSound];
        myAudioSources[0].Play();
        yield return new WaitForSeconds(myAudioSources[0].clip.length);
        int randomWait = UnityEngine.Random.Range(0,animalSounds.Length-1);
        yield return new WaitForSeconds(randomWait);
        loopAnimalSounds = true;
    }
    public IEnumerator PlayFootSteps()
    {
        loopFootSteps = false;
        int randomSound = UnityEngine.Random.Range(0,footsteps.Length-1);
        myAudioSources[1].clip = footsteps[randomSound];
        myAudioSources[1].Play();
        yield return new WaitForSeconds(myAudioSources[1].clip.length);
        loopFootSteps = true;
    }
    public void GoBackHome()
    {
        DestroyAllFood();
        StartCoroutine(WalkBack());
    }
    public void DestroyAllFood()
    {
        for (int i = 0; i < foodQueue.Count; i++) {
            PickUpScript nextFood = foodQueue.Dequeue();
            Destroy(nextFood.gameObject);
        }
    }
    public IEnumerator WalkBack()
    {
        destination = new Vector3(startingXPos, 0, 9);
        distance = Vector3.Distance(destination, this.transform.position);
        for (int i = 0; i < 2000 && distance > 0.2f; i++) {
            LookAtNextFood(destination, 20f);
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(1.0f);
        /*
        Vector3 backward = this.transform.TransformDirection(Vector3.back*2);
        destination = this.transform.position+backward;
        distance = Vector3.Distance(destination, this.transform.position);
        for (int i = 0; i < 200 && distance > 0.2f; i++) {
            LookAtNextFood(destination, 10f);
            yield return new WaitForSeconds(0.1f);
        }
        */
        this.transform.rotation = Quaternion.Euler(0,180,0);
    }
    public void PlayEattingSound()
    {
        if (canPlayNewBitingSound)
            StartCoroutine(PlayBitingSound());
        MakeFoodCrumbs();
    }
    public IEnumerator PlayBitingSound()
    {
        canPlayNewBitingSound = false;
        int randomSound = UnityEngine.Random.Range(0,bitingSounds.Length-1);
        myAudioSources[2].clip = bitingSounds[randomSound];
        myAudioSources[2].Play();
        yield return new WaitForSeconds(myAudioSources[2].clip.length);
        canPlayNewBitingSound = true;
    }
    public void MakeFoodCrumbs()
    {
        if (foodMat != null) {
            Vector3 upwards = this.transform.TransformDirection(Vector3.up)*0.5f;
            Vector3 forward = this.transform.TransformDirection(Vector3.forward);
            Quaternion rotationOne = this.gameObject.transform.rotation * Quaternion.Euler(0,270,0);
            Quaternion rotationTwo = this.gameObject.transform.rotation * Quaternion.Euler(0,90,0);
            ParticleSystem crumbsA = Instantiate(munchCrumbsPrefab, this.transform.position+upwards+forward, rotationOne) as ParticleSystem;
            ParticleSystem crumbsB = Instantiate(munchCrumbsPrefab, this.transform.position+upwards+forward, rotationTwo) as ParticleSystem;
            crumbsA.transform.localScale = Vector3.one * 0.3f;
            crumbsB.transform.localScale = Vector3.one * 0.3f;
            crumbsA.gameObject.gameObject.GetComponent<ParticleSystemRenderer>().material = foodMat;
            crumbsB.gameObject.gameObject.GetComponent<ParticleSystemRenderer>().material = foodMat;
            StartCoroutine(KillTimer(crumbsA.gameObject, 2.0f));
            StartCoroutine(KillTimer(crumbsB.gameObject, 2.0f));
        }
        if (foodToEat != null) {
            Destroy(foodToEat);
            nextFood = null;
        }
    }
    public IEnumerator KillTimer(GameObject obj, float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(obj);
    }
    public void CreateGlowingHeart()
    {
        for (int i = 1; i < 10; i++) {
            if (happyTimer[i] != TimeSpan.Zero) {
                if (!heartMade[i]) {
                    heartMade[i] = true;
                    //Vector3 upwards = this.transform.TransformDirection(Vector3.up)*heartPlacement.y;
                    //Vector3 forward = this.transform.TransformDirection(Vector3.forward)*heartPlacement.z;
                    //myHeart = Instantiate(heartParticlePrefab, this.transform.position+forward+upwards, Quaternion.identity) as ParticleSystem;
                    //Vector3 myUp = heartPlacementObj.transform.TransformDirection(Vector3.up*(i-1));
                    Vector3 heartPos = heartPlacementObj.transform.position + (Vector3.up * (i-1) * 0.5f);
                    myHeart[i] = Instantiate(heartParticlePrefab, heartPos, Quaternion.identity) as ParticleSystem;
                    //myHeart.transform.SetParent(this.gameObject.transform);
                    myHeart[i].transform.SetParent(heartPlacementObj.transform);
                    myHeart[i].transform.position = heartPos;
                }
            }
        }
    }
    public void DestroyGlowingHeart()
    {
        for (int i = 0; i < 10; i++) {
            if (happyTimer[i] == TimeSpan.Zero) {
                heartMade[i] = false;
                if (myHeart[i] != null) {
                    Destroy(myHeart[i].gameObject);
                }
            }
        }
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.black;
        Vector3 forward = this.transform.TransformDirection(Vector3.forward)*foodEatDistance;
        //Gizmos.DrawWireSphere(transform.position+forward, 0.1f);
        Gizmos.DrawWireSphere(transform.position, eatRadius);
    }
    public void Happiness()
    {
        if (myStats != null && menuManagerScript != null && statsManagerScript != null) {
            InitiallyBecomeHappy();
            CalculateHappyTimer();
            if (happy) {
                CreateGlowingHeart();
            }
            DestroyGlowingHeart();
        }
    }
    public void UpdateStats(float distance)
    {
        myStats.metersTraveled += distance;
        myStats.dateLastPlay = System.DateTime.Now;
    }
    public void CalculateHappyTimer()
    {
        if (statsManagerScript != null) {
            happy = CalculateIfStillHappy();
            if (happy) {
                for (int i = 0; i < 10; i++) {
                    TimeSpan duration = System.DateTime.Now - myStats.dateSinceLastInitiallySetHappy[i];
                    if (duration.TotalMinutes <= statsManagerScript.happinessDurationMinutes) {
                        happyTimer[i] = duration;
                    } else {
                        happyTimer[i] = TimeSpan.Zero;
                    }
                }
            }
            //Turns off happiness if expired. In other words if (you shouldn't be happy, and you currently are) ...
            if (!CalculateIfStillHappy() && happy) {
                happy = false;
                for (int i = 0; i < 10; i++) {
                    happyTimer[i] = TimeSpan.Zero;
                }
            }
        }
        /*
        for (int i = 0; i < 10; i++) {
            happyTimerS[i] = happyTimer[i].ToString();
        }
        */
    }
    //Goes through all indexes of the timestamp array
    public int CalculateHowManyHeartsWereEarned()
    {
        int heartsEarnedWhileGone = 0;
        for (int i = 0; i < 10; i++) {
            calculatedHeartsWhileSystemWasOff = true;
            //Checks if even eligible to earn hearts while system was off
            int timeBetweenLastHeartEarnedAndInitiallyHappy = (int) Math.Round((myStats.dateLastHeartEarned - myStats.dateSinceLastInitiallySetHappy[i]).TotalMinutes);
            if (timeBetweenLastHeartEarnedAndInitiallyHappy >= statsManagerScript.happinessDurationMinutes) {
                continue;
            }
            //System was turned on before happiness expired
            myStats.durationBetweenLastHeartEarnedAndNow = (int) Math.Round((System.DateTime.Now - myStats.dateLastHeartEarned).TotalMinutes);
            if (myStats.durationBetweenLastHeartEarnedAndNow <= statsManagerScript.happinessDurationMinutes) {
                heartsEarnedWhileGone += myStats.durationBetweenLastHeartEarnedAndNow;
                continue;
            }
            //System was turned on after happiness expired
            int minutesFromInitialHappyToLastEarnedHeart = (int) Math.Round((myStats.dateLastHeartEarned - myStats.dateSinceLastInitiallySetHappy[i]).TotalMinutes);
            heartsEarnedWhileGone += (statsManagerScript.happinessDurationMinutes - minutesFromInitialHappyToLastEarnedHeart);
        }
        return heartsEarnedWhileGone;
    }
    //The actual increase of the heartcount happens as the heartcounter gets a heart in menuManager
    public void MakeHeartPlusOne()
    {
        Vector3 upwards = this.transform.TransformDirection(Vector3.up)*heartPlacement.y;
        Vector3 forward = this.transform.TransformDirection(Vector3.forward)*heartPlacement.z;
        Vector3 heartPos = this.transform.position+forward+upwards;
        myStats.dateLastHeartEarned = System.DateTime.Now;
        menuManagerScript.MakeAHeartPlusOne(heartPos);
        //Update CSV
        CSVWriter.SaveDateLastHeartEarned(myStats.animalName, System.DateTime.Now);
        //CSVWriter.saveShit = true;
    }
    public bool CalculateIfStillHappy()
    {
        for (int i = 0; i < 10; i++) {
            int timeBetweenNowAndInitiallyHappy = ((int) Math.Round((System.DateTime.Now - myStats.dateSinceLastInitiallySetHappy[i]).TotalMinutes));
            if (timeBetweenNowAndInitiallyHappy <= statsManagerScript.happinessDurationMinutes) {
                return true;
            }
        }
        return false;
    }
    public void InitiallyBecomeHappy()
    {
        int wasLevel = myStats.level;
        myStats.CalculateRemainingStats();
        //if the animal's level just changed...
        if (wasLevel != myStats.level) {
            myAudioSources[3].clip = winSounds[1];
            myAudioSources[3].Play();
            //if already happy, move the init happy times upwards in the array
            if (happy) {
                for (int i = 9; i > 0; i--) {
                    myStats.dateSinceLastInitiallySetHappy[i] = myStats.dateSinceLastInitiallySetHappy[i-1];
                }
            }
            //whether currently happy or not, set to happy and initHappy[i] to DateTime.Now
            happy = true;
            myStats.dateSinceLastInitiallySetHappy[1] = System.DateTime.Now;
            myStats.CalculateRemainingStats();
            MakeHeartPlusOne();
            if (inventoryManagerScript.firstTimeHappy) {
                //Make Camera Go To Animal
                Vector3 newPosition = this.transform.position + (Vector3.up) + (Vector3.back*3);
                Quaternion newRotation = Quaternion.Euler(Vector3.forward);
                menuManagerScript.MoveCameraToAnimalFromField(newPosition, newRotation);
                //Make the message
                menuManagerScript.MakeHappyMessageAppear(myStats.level.ToString());
            }
        }
    }
    /*
    public void EarnThisUnlockedAnimal()
    {
        int myPosInLife = inventoryManagerScript.GetAnimalPositionInAllThings(myStats.animalName);
        inventoryManagerScript.EquipAnimalAtPosition(myPosInLife);
    }
    */
    public void DisplayUnequippedMessage()
    {
        int myPosInLife = inventoryManagerScript.GetAnimalPositionInAllThings(myStats.animalName);
        int myHeartsToEarn = statsManagerScript.heartsToEarnNextAnimal[myPosInLife];
        menuManagerScript.MakeEarnToEquipAnimalMessage(myHeartsToEarn, this.transform.position + Vector3.up);
    }
    //This is called if the animal is unlocked and not yet equipped. The lock will look unlocked.
    //Only call this "scan" if the menuManagerScript doesn't have the unlockAnimalMessaeUp
    //Also, only scan if unlocked and not equipped.
/*
    public void SetUnlocked()
    {
        if (menuManagerScript != null && !menuManagerScript.unlockAnimalMessaeUp) {
            Debug.Log(myStats.animalName+" is equipped = "+myStats.equipped+", & unlocked = "+myStats.unlocked);
            if (!myStats.equipped && myStats.unlocked && myLock != null) {
                Animator myLocksAnim = myLock.GetComponentInChildren<Animator>();
                if (lockScript == null) {
                    lockScript = myLock.GetComponentInChildren<UnlockAnimScript>();
                }
                if (myLocksAnim != null) {
                    if (lockScript.locked) {
                        lockScript.locked = false;
                        lockScript.unlocked = true;
                        myLocksAnim.SetTrigger("BypassAnim");
                        Debug.Log("Bypass Being called");
                    }
                }
            }
        }
    }
    */
    public void SetMaterial()
    {
        if (myStats.equipped && myMat != null && !mymaterialSet && myRend != null) {
            myRend.material = myMat;
            mymaterialSet = true;
            if (myLock != null) {
                myLock.SetActive(false);
            }
        }
    }
    public void ForceSetMaterial()
    {
        myRend = gameObject.GetComponentInChildren<SkinnedMeshRenderer>();
        if (myMat != null && !mymaterialSet && myRend != null) {
            myRend.material = myMat;
            mymaterialSet = true;
        }
    }
    public void SaveMyBrushInfo()
    {
        myStats.dateLastBrushed = System.DateTime.Now;
        CSVWriter.SaveDateLastBrushed(myStats.animalName, myStats.dateLastBrushed);
        CSVWriter.SaveBrushCount(myStats.animalName, myStats.brushCount);
        //CSVWriter.saveShit = true;
    }
    public void Cleanliness()
    {
        myCleanlinessPercentage = inventoryManagerScript.GetNextBrushHappyRatio(this.myStats);
        if (myStats.level > 2) {
            int day = 1440;
            TimeSpan durationSinceLastBrush = System.DateTime.Now - myStats.dateLastBrushed;
            if (durationSinceLastBrush.TotalMinutes >= 4 * day && myCleanlinessPercentage <= 0.2f) {
                MakeFliesCloud();
            }
            if (durationSinceLastBrush.TotalMinutes >= 3 * day && myCleanlinessPercentage <= 0.4f) {
                MakeDustCloud();
            }
            if (durationSinceLastBrush.TotalMinutes >= 2 * day && myCleanlinessPercentage <= 0.6f) {
                MakeDirtyColor();
            }
            //TODO confirm the and boolean here
            if (durationSinceLastBrush.TotalMinutes >= 2 * day && myCleanlinessPercentage < 1.0f) {
                TakeRainbowGlowAway();
            }
            if (durationSinceLastBrush.TotalMinutes < 2) {
                if (myCleanlinessPercentage >= 0.2f) {
                    if (myFliesCloud != null) {
                        myFliesCloud.Stop();
                        cleanlinessSpeed = 0.5f;
                    }
                }
                if (myCleanlinessPercentage >= 0.4f) {
                    if (myDustCloud != null) {
                        myDustCloud.Stop();
                        cleanlinessSpeed = 0.75f;
                    }
                }
                if (myCleanlinessPercentage >= 0.8f) {
                        Material[] newMaterialsList = new Material[] {myMat, null};
                        var rend = GetComponentInChildren<Renderer>();
                        //rend.materials[1].mainTexture = null;
                        rend.materials = newMaterialsList;
                        cleanlinessSpeed = 1.0f;
                }
                if (myCleanlinessPercentage >= 1.0f) {
                    MakeRaindbowGlow();
                }
            }
        }
    }
    public void MakeFliesCloud()
    {
        if (myFliesCloud == null) {
            Vector3 pos = this.transform.position + (Vector3.up*particleYPos);
            myFliesCloud = Instantiate(fliesPrefab, pos, Quaternion.identity) as ParticleSystem;
            myFliesCloud.transform.SetParent(this.transform);
            cleanlinessSpeed = 0.25f;
        }
    }
    public void MakeDustCloud()
    {
        if (myDustCloud == null) {
            Vector3 pos = this.transform.position;
            myDustCloud = Instantiate(dustPrefab, pos, Quaternion.identity) as ParticleSystem;
            myDustCloud.transform.SetParent(this.transform);
            cleanlinessSpeed = 0.50f;
        }
    }
    public void MakeDirtyColor()
    {
        Material[] newMaterialsList = new Material[] {myMat, myDirtMat};
        var rend = GetComponentInChildren<Renderer>();
        rend.materials = newMaterialsList;
        cleanlinessSpeed = 0.75f;
    }
    public void MakeRaindbowGlow()
    {
        if (myCleanRainbowGlow == null) {
            Vector3 pos = this.transform.position + (Vector3.up*particleYPos);
            myCleanRainbowGlow = Instantiate(cleanRainbowGlowPrefab, pos, Quaternion.identity) as ParticleSystem;
            myCleanRainbowGlow.transform.SetParent(this.transform);
            cleanlinessSpeed = 1.50f;
        }
    }
    public void TakeRainbowGlowAway()
    {
        if (myCleanRainbowGlow != null) {
            myCleanRainbowGlow.Stop();
            cleanlinessSpeed = 1.0f;
        }
    }
}
