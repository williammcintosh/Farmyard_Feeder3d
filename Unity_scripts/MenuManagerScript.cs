using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Michsky.UI.ModernUIPack;

public class MenuManagerScript : MonoBehaviour
{
    [Header("GAMEOBJECTS")]
    public GameObject rewardedAdObj;
    RewardedAdDisplayScript rewardedAdDisplayScript;
    public GameObject touchManagerObj;
    TouchManagerScript touchManagerScript;
    public GameObject cheatingAlpacaObj;
    CreeperAlpacaScript cheatingAlpacaScript;
    public GameObject fieldManager;
    FieldManager fieldManagerScript;
    public GameObject readWriter;
    CsvReadWrite readWriteScript;
    public GameObject statsManager;
    StatsManagerScript statsManagerScript;
    public GameObject theCanvas;
    public GameObject inventoryManager;
    InventoryManagerScript inventoryManagerScript;
    public float travelSpeed = 1.0f;
    public Camera menuCam, fieldCam;
    public Quaternion getQuaternionValue;
    public GameObject barnDoorBackSign, stablesBackSign;
    [Header("'At' Booleans")]
    public bool atTheMainMenu = true;
    public bool atTheBarnDoors = false;
    public bool atTheStables = false;
    public bool atTheField = false;
    public bool atTheBrushing = false;
    Vector3 titlePos = new Vector3(11.4f, 2.08f, 7f);
    Quaternion titleRot = new Quaternion(-0.2714352f, -0.4104354f, -0.1294098f, 0.8608826f);
    Vector3 mainMenuPos = new Vector3(11.4f, 2.08f, 7f);
    Quaternion mainMenuRot = new Quaternion(-0.01276556f, -0.4823141f, -0.007029534f, 0.8758771f);
    Vector3 stablePos = new Vector3(3, 2, 3);
    Quaternion stableRot = new Quaternion(0.08715578f, 0, 0, 0.9961947f);
    Vector3 fieldPos = new Vector3(0, 4.5f, 2);
    Quaternion fieldRot = new Quaternion(0.3535534f, 0.3535534f, -0.1464466f, 0.8535535f);
    Vector3 animalAdjPos = new Vector3(-1, -0.5f, 0);
    public Vector3 animalBrushRot;
    [HideInInspector]
    public bool goToStart = false, goToStables = false, goToField = false, goFromFieldToStables = false, goToBarnDoor = false, goToAnimal = false;
    Vector3 barnDoorPos = new Vector3(10.14f, 1.46f, 12.9f);
    Quaternion barnDoorRot = new Quaternion(0.0f, -0.7071068f, 0.0f, 0.7071068f);
    //public GameObject happyMessage;
    //public TextMeshProUGUI happyMessageLevelText;
    public Toggle happyMessageToggle;
    public GameObject toggleParent;
    public GameObject titleBox;
    public GameObject startButtonObj;
    public Button startButton;
    public GameObject cockATourGuideObj;
    CockATourGuide cockATourGuide;
    public NotificationManager royMessage;
    public ButtonManagerBasicIcon royButton, royNoButton, royYesButton, royBrushButton, royFeedButton;
    public Animator royAnim;
    AudioSource [] myAudioSources;
    public AudioClip clickSound, blingSound, ambientSound, newlyEquippedAnimalHorn, newlyEquippedSuperHorn, newEquippedAnimalApplause, collectHeartSound;
    public GameObject barnDoor, stableSign, horse, carrotCrate, horseFieldMarker;
    public GameObject glowPrefab;
    public GameObject barnDoorObj;
    BarnDoorScript barnDoorScript;
    public GameObject handTapGesture;
    AnimalScript horseScript;
    public int heartsEarnedWhileGone = 0;
    [Header("UI Objects")]
    public GameObject getATourButton;
    public GameObject videoAdsButton;
    public GameObject goBackFromBrushingButton;
    public bool makingAHeart = false;
    public GameObject mainHeartCounter;
    TextMeshProUGUI heartText;
    public GameObject prefabHeartPlusOne;
    public GameObject foodToNextHappyPieObj, foodToNextHappyTextObj;
    public GameObject brushToNextHappyPieObj, brushToNextHappyTextObj;
    [HideInInspector]
    public ProgressBar foodToNextHappyPie, brushToNextHappyPie;
    TextMeshProUGUI foodToNextHappyText, brushToNextHappyText;
    public ParticleSystem confettiPrefab, newSkinPoofPrefab;
    [Header("Sound Booleans")]
    public bool canPlayAmbientSound = true;
    [Header("Message Booleans")]
    public bool messageUp = false;
    public bool happyMessageUp = false;
    public bool welcomeMessageUp = false;
    public bool unlockAnimalMessaeUp = false;
    public bool wasAtTheFieldBeforeUnlockMessage = false;
    public bool royIsRoosted = false;
    public bool cheatingAlpaca = false;
    public bool rewardedAdVideo = false;
    public bool confirmationMessageUp = false;
    [Header("Tutorial Booleans")]
    public bool tutorialPause = false;
    public bool doingTutorial = false;
    public bool tourDone = false;
    [Header("Tutorial Feed Horse Variables")]
    public int horseFoodCountForTutorial = 0;
    public bool waitForCarrotOnField = false;
    public bool waitForAnimalToEat = false;
    public bool waitForAnimalToBeHappy = false;
    public bool horseHappiness = false;
    public bool onlyHorse = false;
    public bool horseReady = false;
    // Start is called before the first frame update
    void Start()
    {
        rewardedAdDisplayScript = rewardedAdObj.GetComponent<RewardedAdDisplayScript>();
        touchManagerScript = touchManagerObj.GetComponent<TouchManagerScript>();
        cheatingAlpacaScript = cheatingAlpacaObj.GetComponent<CreeperAlpacaScript>();
        fieldManagerScript = fieldManager.GetComponent<FieldManager>();
        readWriteScript = readWriter.GetComponent<CsvReadWrite>();
        foodToNextHappyPie = foodToNextHappyPieObj.GetComponent<ProgressBar>();
        foodToNextHappyText = foodToNextHappyTextObj.GetComponentInChildren<TextMeshProUGUI>();
        brushToNextHappyPie = brushToNextHappyPieObj.GetComponent<ProgressBar>();
        brushToNextHappyText = brushToNextHappyTextObj.GetComponentInChildren<TextMeshProUGUI>();
        inventoryManagerScript = inventoryManager.GetComponent<InventoryManagerScript>();
        statsManagerScript = statsManager.GetComponent<StatsManagerScript>();
        heartText = mainHeartCounter.GetComponentInChildren<TextMeshProUGUI>();
        cockATourGuide = cockATourGuideObj.GetComponent<CockATourGuide>();
        handTapGesture.SetActive(false);
        foodToNextHappyPieObj.SetActive(false);
        brushToNextHappyPieObj.SetActive(false);
        barnDoorBackSign.SetActive(false);
        stablesBackSign.SetActive(false);
        //happyMessage.SetActive(false);
        toggleParent.gameObject.SetActive(false);
        mainHeartCounter.SetActive(false);
        videoAdsButton.SetActive(false);
        goBackFromBrushingButton.SetActive(false);
        getATourButton.SetActive(false);
        startButtonObj.SetActive(true);
        royButton.gameObject.SetActive(false);
        royNoButton.gameObject.SetActive(false);
        royYesButton.gameObject.SetActive(false);
        royBrushButton.gameObject.SetActive(false);
        royFeedButton.gameObject.SetActive(false);
        royMessage.gameObject.SetActive(true);
        royAnim = royMessage.GetComponent<Animator>();
        for (int i = 0; i < 5; i++) {
            gameObject.AddComponent<AudioSource>();
        }
        myAudioSources = gameObject.GetComponents<AudioSource>();
        myAudioSources[0].volume = 0.5f;
        myAudioSources[0].clip = clickSound;
        myAudioSources[1].volume = 0.5f;
        myAudioSources[1].clip = blingSound;
        myAudioSources[2].volume = 0.5f;
        myAudioSources[2].clip = ambientSound;
        myAudioSources[3].volume = 0.5f;
        myAudioSources[3].clip = newlyEquippedAnimalHorn;
        myAudioSources[4].volume = 0.5f;
        myAudioSources[4].clip = newEquippedAnimalApplause;
        glowPrefab.SetActive(false);
        barnDoorScript = barnDoorObj.GetComponent<BarnDoorScript>();
        SetInitialMessageUP();
    }
    public IEnumerator PerpetuallyPlayAmbientSound()
    {
        canPlayAmbientSound = false;
        myAudioSources[2].Play();
        yield return new WaitForSeconds(myAudioSources[2].clip.length);
        canPlayAmbientSound = true;
    }
    // Update is called once per frame
    void Update()
    {
        if (fieldManagerScript.selectedAnimal == null) {
            if (foodToNextHappyPieObj.activeSelf)
                foodToNextHappyPieObj.SetActive(false);
            if (brushToNextHappyPieObj.activeSelf)
                brushToNextHappyPieObj.SetActive(false);
        } else {
            if (atTheField) {
                SetFoodToNextHappyPie();
            }
            if (atTheBrushing) {
                SetBrushToNextHappyPie();
            }
        }
        if (canPlayAmbientSound)
            StartCoroutine(PerpetuallyPlayAmbientSound());
        heartText.text = statsManagerScript.heartCount.ToString();
        getQuaternionValue = menuCam.transform.rotation;
        LocateSelfPos();
        if (goToStart) {
            if (atTheBarnDoors) {
                StartCoroutine(MoveCameraToStartFromBarndoor(mainMenuPos, mainMenuRot));
            } else if (atTheStables) {
                stablesBackSign.SetActive(false);
                goToStart = false;
                StartCoroutine(MoveCameraToStartFromStables(mainMenuPos, mainMenuRot));
            }
        }
        if (goToStables) {
            StartCoroutine(MoveCameraToStablesFromStart(stablePos, stableRot));
		}
        if (goToField) {
            StartCoroutine(MoveCameraToFieldFromStables(fieldPos, fieldRot));
		}
        if (goToBarnDoor) {
            inventoryManagerScript.cameraInventoryManager.SetActive(true);
            goToBarnDoor = false;
            StartCoroutine(MoveAndRotateCameraTo(barnDoorPos, barnDoorRot));
            barnDoorBackSign.SetActive(true);
        }
        if (atTheField && goFromFieldToStables) {
            StartCoroutine(MoveCameraToStablesFromField(stablePos, stableRot));
            inventoryManagerScript.ShutOffCharacterControllers();
        }
        if (atTheMainMenu && royIsRoosted) {
            if (tourDone) {
                getATourButton.SetActive(true);
                getATourButton.GetComponentInChildren<Button>().interactable = true;
            }
        } else {
            getATourButton.SetActive(false);
        }
        if (tourDone && royIsRoosted) {
            videoAdsButton.SetActive(true);
        } else {
            videoAdsButton.SetActive(false);
        }

        //Tutorial Waiters
        if (waitForCarrotOnField) {
            WaitForPLayerToDragACarrotInTheField();
        }
        if (waitForAnimalToEat) {
            WaitForAnimalInTutorialToEatRoutine();
        }
        if (waitForAnimalToBeHappy) {
            WaitForAnimalToBeHappyInTutorial();
        }
        if (cockATourGuide.roosted) {
            royIsRoosted = true;
        } else {
            royIsRoosted = false;
        }
    }
    public void SetInitialMessageUP()
    {
        if (!readWriteScript.GetFirstTimeRoyTutorial()){
            messageUp = true;
        } else {
            messageUp = false;
        }
    }
    public void SetAllBoolsFalse()
    {
        atTheStables = false;
        atTheMainMenu = false;
        atTheField = false;
        atTheBarnDoors = false;
    }
    public void LocateSelfPos()
    {
        SetAllBoolsFalse();
        if (Vector3.Distance(menuCam.transform.position, mainMenuPos) < 0.1f) {
            atTheMainMenu = true;
        }
        if (Vector3.Distance(menuCam.transform.position, barnDoorPos) < 0.1f) {
            atTheBarnDoors = true;
        }
        if (Vector3.Distance(menuCam.transform.position, stablePos) < 0.1f) {
            atTheStables = true;
        }
        if (Vector3.Distance(menuCam.transform.position, fieldPos) < 0.1f) {
            atTheField = true;
        }
    }
    public void PassAlongHorseForTheTutorial(AnimalScript newHorseScript)
    {
        horseScript = newHorseScript;
    }
    public IEnumerator MoveCameraToStablesFromStart(Vector3 cameraDesPos, Quaternion cameraDesRot)
    {
        goToStables = false;
	    yield return new WaitForSeconds(0.5f);
        Vector3 cameraStartPos = menuCam.transform.position;
        Quaternion cameraStartRot = menuCam.transform.rotation;
	    float timeOfTravel = 1f;
	    float currentTime = 0;
	    float normalizedValue;
	    while (currentTime <= timeOfTravel) {
			currentTime += Time.deltaTime * travelSpeed;
			normalizedValue = currentTime / timeOfTravel; // we normalize our time
			menuCam.transform.position = Vector3.Lerp(cameraStartPos, cameraDesPos, normalizedValue);
            menuCam.transform.rotation = Quaternion.Lerp(cameraStartRot, cameraDesRot, normalizedValue);
			yield return null;
	    }
	    yield return new WaitForSeconds(0.5f);
		menuCam.transform.position = cameraDesPos;
        stablesBackSign.SetActive(true);
        yield return null;
    }
    public IEnumerator MoveCameraToStartFromBarndoor(Vector3 cameraDesPos, Quaternion cameraDesRot)
    {
        barnDoorBackSign.SetActive(false);
        goToStart = false;
        yield return new WaitForSeconds(0.5f);
        Vector3 cameraStartPos = menuCam.transform.position;
        Quaternion cameraStartRot = menuCam.transform.rotation;
        float timeOfTravel = 1f;
        float currentTime = 0;
        float normalizedValue;
        while (currentTime <= timeOfTravel) {
            currentTime += Time.deltaTime * travelSpeed;
            normalizedValue = currentTime / timeOfTravel; // we normalize our time
            menuCam.transform.position = Vector3.Lerp(cameraStartPos, cameraDesPos, normalizedValue);
            menuCam.transform.rotation = Quaternion.Lerp(cameraStartRot, cameraDesRot, normalizedValue);
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);
        menuCam.transform.position = cameraDesPos;
        yield return null;
    }
    public IEnumerator MoveCameraToStartFromStables(Vector3 cameraDesPos, Quaternion cameraDesRot)
    {
        yield return new WaitForSeconds(0.5f);
        Vector3 cameraStartPos = menuCam.transform.position;
        Quaternion cameraStartRot = menuCam.transform.rotation;
        float timeOfTravel = 1f;
        float currentTime = 0;
        float normalizedValue;
        while (currentTime <= timeOfTravel) {
            currentTime += Time.deltaTime * travelSpeed;
            normalizedValue = currentTime / timeOfTravel; // we normalize our time
            menuCam.transform.position = Vector3.Lerp(cameraStartPos, cameraDesPos, normalizedValue);
            menuCam.transform.rotation = Quaternion.Lerp(cameraStartRot, cameraDesRot, normalizedValue);
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);
        menuCam.transform.position = cameraDesPos;
        yield return null;
    }

    public IEnumerator MoveAndRotateCameraTo(Vector3 cameraDesPos, Quaternion cameraDesRot)
    {
	    yield return new WaitForSeconds(0.5f);
        Vector3 cameraStartPos = menuCam.transform.position;
        Quaternion cameraStartRot = menuCam.transform.rotation;
	    float timeOfTravel = 1f;
	    float currentTime = 0;
	    float normalizedValue;
	    while (currentTime <= timeOfTravel) {
			currentTime += Time.deltaTime * travelSpeed;
			normalizedValue = currentTime / timeOfTravel; // we normalize our time
			menuCam.transform.position = Vector3.Lerp(cameraStartPos, cameraDesPos, normalizedValue);
            menuCam.transform.rotation = Quaternion.Lerp(cameraStartRot, cameraDesRot, normalizedValue);
			yield return null;
	    }
	    yield return new WaitForSeconds(0.5f);
		menuCam.transform.position = cameraDesPos;
        yield return null;
    }
    public IEnumerator MoveCameraToFieldFromStables(Vector3 cameraDesPos, Quaternion cameraDesRot)
    {
        stablesBackSign.SetActive(false);
        goToField = false;
	    yield return new WaitForSeconds(0.5f);
        Vector3 cameraStartPos = menuCam.transform.position;
        Quaternion cameraStartRot = menuCam.transform.rotation;
	    float timeOfTravel = 1f;
	    float currentTime = 0;
	    float normalizedValue;
	    while (currentTime <= timeOfTravel) {
			currentTime += Time.deltaTime * travelSpeed;
			normalizedValue = currentTime / timeOfTravel; // we normalize our time
			menuCam.transform.position = Vector3.Lerp(cameraStartPos, cameraDesPos, normalizedValue);
            menuCam.transform.rotation = Quaternion.Lerp(cameraStartRot, cameraDesRot, normalizedValue);
			yield return null;
	    }
        yield return null;
		menuCam.transform.position = cameraDesPos;
        fieldCam.enabled = true;
        menuCam.enabled = false;
        goBackFromBrushingButton.SetActive(true);
        goBackFromBrushingButton.GetComponentInChildren<Button>().interactable = true;
        StartCoroutine(Zoom(3.0f, 5.0f, true));
        yield return null;
    }
    public IEnumerator MoveCameraToStablesFromField(Vector3 cameraDesPos, Quaternion cameraDesRot)
    {
        goBackFromBrushingButton.GetComponentInChildren<Button>().interactable = false;
        goFromFieldToStables = false;
        StartCoroutine(Zoom(5.0f, 3.0f, false));
        yield return new WaitForSeconds(1.75f);
        Vector3 cameraStartPos = menuCam.transform.position;
        Quaternion cameraStartRot = menuCam.transform.rotation;
	    float timeOfTravel = 1f;
	    float currentTime = 0;
	    float normalizedValue;
	    while (currentTime <= timeOfTravel) {
			currentTime += Time.deltaTime * travelSpeed;
			normalizedValue = currentTime / timeOfTravel; // we normalize our time
			menuCam.transform.position = Vector3.Lerp(cameraStartPos, cameraDesPos, normalizedValue);
            menuCam.transform.rotation = Quaternion.Lerp(cameraStartRot, cameraDesRot, normalizedValue);
			yield return null;
	    }
        yield return null;
		menuCam.transform.position = cameraDesPos;
        stablesBackSign.SetActive(true);
        goBackFromBrushingButton.SetActive(false);
        yield return null;
    }

    public IEnumerator Zoom(float startPos, float desPos, bool zoomOut)
    {
        float timeOfTravel = 1f;
        float currentTime = 0;
        float normalizedValue;
        while (currentTime <= timeOfTravel) {
            currentTime += Time.deltaTime * travelSpeed;
            normalizedValue = currentTime / timeOfTravel; // we normalize our time
            fieldCam.orthographicSize = Mathf.Lerp(startPos, desPos, normalizedValue);
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);
        if (fieldCam.orthographicSize-desPos < 0.1f) {
            fieldCam.orthographicSize = desPos;
            if (!zoomOut) {
                menuCam.enabled = true;
                fieldCam.enabled = false;
            }
        }
        yield return null;
    }
    public void MakeAHeartPlusOne(Vector3 startPos)
    {
        Vector3 screenPos = startPos;
        if (menuCam.enabled)
            screenPos = menuCam.WorldToScreenPoint(startPos);
        else
            screenPos = fieldCam.WorldToScreenPoint(startPos);
        StartCoroutine(MakinAHeart(screenPos));
    }
    public IEnumerator MakinAHeart(Vector3 startPos)
    {
        makingAHeart = true;
        GameObject theHeart = Instantiate(prefabHeartPlusOne, startPos, Quaternion.identity) as GameObject;
        theHeart.transform.SetParent(theCanvas.transform);
        yield return new WaitForSeconds(0.5f);
        Vector3 desPos = mainHeartCounter.transform.position;
        float timeOfTravel = 1f;
        float currentTime = 0;
        float normalizedValue;
        while (currentTime <= timeOfTravel) {
            currentTime += Time.deltaTime * 2.0f;
            normalizedValue = currentTime / timeOfTravel; // we normalize our time
            theHeart.transform.position = Vector3.Lerp(startPos, desPos, normalizedValue);
            yield return null;
        }
        yield return null;
        theHeart.transform.position = desPos;
        makingAHeart = false;
        myAudioSources[4].clip = collectHeartSound;
        myAudioSources[4].Play();
        StartCoroutine(MakeMainHeartBigger());
        yield return null;
        Destroy(theHeart);
        yield return null;
    }
    public IEnumerator MakeMainHeartBigger()
    {
        Vector3 startSize = Vector3.one;
        Vector3 desSize = Vector3.one * 1.5f;
        float timeOfTravel = 1f;
        float currentTime = 0;
        float normalizedValue;
        while (currentTime <= timeOfTravel) {
            currentTime += Time.deltaTime * 6.0f;
            normalizedValue = currentTime / timeOfTravel; // we normalize our time
            mainHeartCounter.transform.localScale = Vector3.Lerp(startSize, desSize, normalizedValue);
            yield return null;
        }
        statsManagerScript.IncreaseDuplicateHeartCounterByOneWhichExistsOnlyToTrackChangeInHeartCount();
        //readWriteScript.saveShit = true;
        yield return null;
        timeOfTravel = 1f;
        currentTime = 0;
        while (currentTime <= timeOfTravel) {
            currentTime += Time.deltaTime * 6.0f;
            normalizedValue = currentTime / timeOfTravel; // we normalize our time
            mainHeartCounter.transform.localScale = Vector3.Lerp(desSize, startSize, normalizedValue);
            yield return null;
        }
        yield return null;
    }
    public void MakeWelcomeMessageAppear(int hearts)
    {
        messageUp = true;
        welcomeMessageUp = true;
        heartsEarnedWhileGone = hearts;
        StartCoroutine(MakeWelcomeMessageAppearRoutine(hearts));
    }
    public IEnumerator MakeWelcomeMessageAppearRoutine(int hearts)
    {
        DisplayCustomRoyMessage("Welcome back!\nYou've earned "+hearts+" hearts while away!");
        yield return new WaitForSeconds(2f);
        cockATourGuide.PlayRoosterSound(2);
        mainHeartCounter.SetActive(true);
        royButton.gameObject.SetActive(true);
        yield return null;
    }
    //This is called by the animal who made the message
    public void MakeHappyMessageAppear(string level)
    {
        happyMessageUp = true;
        //Gives a different message when this happens during the tutorial
        if (!doingTutorial) {
            if (inventoryManagerScript.firstTimeHappy) {
                StartCoroutine(MakeHappyMessageRoutine(level));
            }
        }
    }
    public IEnumerator MakeHappyMessageRoutine(string level)
    {
        yield return null;
        DisplayCustomRoyMessage("Congrats! This animal is now at level "+level+"! ");
        toggleParent.gameObject.SetActive(true);
        messageUp = true;
        yield return new WaitForSeconds(2f);
        royButton.gameObject.SetActive(true);

    }
    //Called from the ButtonPressed() function
    public void UnlockAnimationAndMakeMessageDisappear()
    {
        StartCoroutine(UnlockAnimationAndMakeMessageDisappearRoutine());
    }
    public IEnumerator UnlockAnimationAndMakeMessageDisappearRoutine()
    {
        //Button is pressed
        yield return new WaitForSeconds(0.5f);
        CloseRoyMessage();
        royButton.gameObject.SetActive(false);
        //Play Unlock animation
        int pos = (int) Mathf.Round(cockATourGuide.transform.position.x);
        yield return StartCoroutine(inventoryManagerScript.UnlockAnimation(pos));
        //yield return new WaitForSeconds(1f);
        //Make horn sound and confetti
        yield return StartCoroutine(MakeHornSoundAndConfetti(pos));
        //Change skin of animal
        Quaternion poofRot = Quaternion.Euler(90,0,0);
        ParticleSystem cartoonyPoof = Instantiate(newSkinPoofPrefab, cockATourGuide.transform.position+Vector3.down, poofRot) as ParticleSystem;
        cartoonyPoof.transform.localScale = Vector3.one*2.0f;
        KillTimer(cartoonyPoof.gameObject, 5f);
        inventoryManagerScript.DisableLockAndChangeAnimalSkin(pos);
        //Make Roy go back
        yield return StartCoroutine(cockATourGuide.CloseUpTour());
        //Make Camera go back to field only if we were at the field when this message came up.
        if (wasAtTheFieldBeforeUnlockMessage) {
            wasAtTheFieldBeforeUnlockMessage = false;
            yield return StartCoroutine(MoveCameraToFieldFromAnimal(fieldPos, fieldRot));
        }
        //Delay to allow for unlocking animation
        unlockAnimalMessaeUp = false;
        //Unlock animation
        yield return null;
    }
    public void MakeCheatingAlpacaMessageDisappear()
    {
        cheatingAlpaca = false;
        for (int i = 0; i < 500; i++)
            statsManagerScript.IncreaseDuplicateHeartCounterByOneWhichExistsOnlyToTrackChangeInHeartCount();
        readWriteScript.saveShit = true;
        cheatingAlpacaScript.bobUpAndDown = true;
        happyMessageUp = false;
        messageUp = false;
        CloseRoyMessage();
        royButton.gameObject.SetActive(false);
        StartCoroutine(cockATourGuide.CloseUpTour());
    }
    public void MakeConfirmationMessageDisappear()
    {
        confirmationMessageUp = false;
        happyMessageUp = false;
        messageUp = false;
        CloseRoyMessage();
        royButton.gameObject.SetActive(false);
        royNoButton.gameObject.SetActive(false);
        royBrushButton.gameObject.SetActive(false);
        royFeedButton.gameObject.SetActive(false);
        StartCoroutine(cockATourGuide.CloseUpTour());
    }
    public void MakeHappyMessageDisappear()
    {
        CloseRoyMessage();
        happyMessageUp = false;
        messageUp = false;
        toggleParent.gameObject.SetActive(false);
        royButton.gameObject.SetActive(false);
        StartCoroutine(MoveCameraToFieldFromAnimal(fieldPos, fieldRot));
    }
    public void MoveCameraToAnimalFromField(Vector3 cameraDesPos, Quaternion cameraDesRot)
    {
        StartCoroutine(MoveCameraToAnimalFromFieldRoutine(cameraDesPos, cameraDesRot));
    }
    public IEnumerator MoveCameraToAnimalFromFieldRoutine(Vector3 cameraDesPos, Quaternion cameraDesRot)
    {
        fieldCam.enabled = false;
        menuCam.enabled = true;
        Vector3 cameraStartPos = menuCam.transform.position;
        Quaternion cameraStartRot = menuCam.transform.rotation;
        float timeOfTravel = 1f;
        float currentTime = 0;
        float normalizedValue;
        while (currentTime <= timeOfTravel) {
            currentTime += Time.deltaTime * travelSpeed *2f;
            normalizedValue = currentTime / timeOfTravel; // we normalize our time
            menuCam.transform.position = Vector3.Lerp(cameraStartPos, cameraDesPos, normalizedValue);
            menuCam.transform.rotation = Quaternion.Lerp(cameraStartRot, cameraDesRot, normalizedValue);
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);
        if (Vector3.Distance(menuCam.transform.position, cameraDesPos) < 0.1f) {
            menuCam.transform.position = cameraDesPos;
            menuCam.transform.rotation = cameraDesRot;
        }
        yield return null;
    }
    public IEnumerator MoveCameraToFieldFromAnimal(Vector3 cameraDesPos, Quaternion cameraDesRot)
    {

        Vector3 cameraStartPos = menuCam.transform.position;
        Quaternion cameraStartRot = menuCam.transform.rotation;
        float timeOfTravel = 1f;
        float currentTime = 0;
        float normalizedValue;
        while (currentTime <= timeOfTravel) {
            currentTime += Time.deltaTime * travelSpeed *2f;
            normalizedValue = currentTime / timeOfTravel; // we normalize our time
            menuCam.transform.position = Vector3.Lerp(cameraStartPos, cameraDesPos, normalizedValue);
            menuCam.transform.rotation = Quaternion.Lerp(cameraStartRot, cameraDesRot, normalizedValue);
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);
        if (Vector3.Distance(menuCam.transform.position, cameraDesPos) < 0.1f) {
            menuCam.transform.position = cameraDesPos;
            menuCam.transform.rotation = cameraDesRot;
        }
        fieldCam.enabled = true;
        menuCam.enabled = false;
        yield return null;
    }
    public void SaveHappyMessageTutorialToggle()
    {
        bool toggleOn = happyMessageToggle.isOn;
        readWriteScript.SaveHappyMessageTutorialToggle(toggleOn);
    }
    public void RotateMainCameraToMenu()
    {
        cockATourGuide.tourDone = readWriteScript.GetFirstTimeRoyTutorial();
        tourDone = cockATourGuide.tourDone;
        myAudioSources[1].Play();
        cockATourGuide.FollowFlyDownPath();
        StartCoroutine(RotateMainCameraToMenuRoutine(mainMenuPos, mainMenuRot));
    }
    public IEnumerator RotateMainCameraToMenuRoutine(Vector3 cameraDesPos, Quaternion cameraDesRot)
    {
        startButton.interactable = false;
        yield return null;
        ShrinkObject(startButtonObj);
        Quaternion cameraStartRot = menuCam.transform.rotation;
        float timeOfTravel = 1f;
        float currentTime = 0;
        float normalizedValue;
        while (currentTime <= timeOfTravel) {
            currentTime += Time.deltaTime * 0.7f;
            normalizedValue = currentTime / timeOfTravel; // we normalize our time
            menuCam.transform.rotation = Quaternion.Slerp(cameraStartRot, cameraDesRot, normalizedValue);
            yield return null;
        }
        //Defaults to preventing the stable buttons from working, then is set to false in the routine below
        messageUp = true;
        yield return new WaitForSeconds(1f);
        menuCam.transform.rotation = cameraDesRot;
        titleBox.SetActive(false);
        mainHeartCounter.SetActive(true);
        StartCoroutine(BeginRoyTutorial());
        yield return null;
    }
    public void ShrinkObject(GameObject obj)
    {
        StartCoroutine(ShrinkObjectRoutine(obj));
    }
    public IEnumerator ShrinkObjectRoutine(GameObject obj)
    {
        yield return new WaitForSeconds(0.5f);
        Vector3 startSize = obj.transform.localScale;
        float timeOfTravel = 1f;
        float currentTime = 0;
        float normalizedValue;
        while (currentTime <= timeOfTravel) {
            currentTime += Time.deltaTime * 2f;
            normalizedValue = currentTime / timeOfTravel; // we normalize our time
            obj.transform.localScale = Vector3.Lerp(startSize, Vector3.zero, normalizedValue);
            yield return null;
        }
        yield return new WaitForSeconds(3f);
        obj.transform.localScale = Vector3.zero;
        obj.SetActive(false);
        yield return null;
    }
    public void RestartTour()
    {
        messageUp = true;
        StartCoroutine(RestartRoyTutorial());
    }
    public IEnumerator BeginRoyTutorial()
    {
        if (!readWriteScript.GetFirstTimeRoyTutorial()) {
            messageUp = true;
            royMessage.description = cockATourGuide.dialogues[0];
            cockATourGuide.dialoguePos = 0;
            royMessage.UpdateUI();
            royAnim.SetBool("Active", true);
            cockATourGuide.GoToPlayer();
            yield return new WaitForSeconds(3f);
            cockATourGuide.PlayRoosterSound(1);
            royButton.gameObject.SetActive(true);
            yield return null;
        } else {
            messageUp = false;
        }
        yield return null;
    }
    public IEnumerator RestartRoyTutorial()
    {
        messageUp = true;
        royMessage.description = cockATourGuide.dialogues[1];
        cockATourGuide.dialoguePos = 1;
        royMessage.UpdateUI();
        royAnim.SetBool("Active", true);
        cockATourGuide.GoToPlayer();
        yield return new WaitForSeconds(3f);
        cockATourGuide.PlayRoosterSound(1);
        royNoButton.GetComponent<Button>().interactable = true;
        royYesButton.GetComponent<Button>().interactable = true;
        cockATourGuide.PlayRoosterSound(1);
        royNoButton.gameObject.SetActive(true);
        royYesButton.gameObject.SetActive(true);
        yield return null;
    }

    public void ButtonPressed()
    {
        if (inventoryManagerScript.AreAnyAnimalsUnequippedAndUnlocked()) {
            unlockAnimalMessaeUp = true;
        }
        cockATourGuide.tourDone = readWriteScript.GetFirstTimeRoyTutorial();
        tourDone = cockATourGuide.tourDone;
        myAudioSources[0].Play();
        royButton.GetComponent<Button>().interactable = false;
        //Debug.Log("MESSAGE UP = "+welcomeMessageUp);
        if (!tourDone) {
            StartCoroutine(ButtonPressedRoutine());
        } else if (welcomeMessageUp) {
            AddWelcomeHeartsToTotalHearts(heartsEarnedWhileGone);
        } else if (happyMessageUp) {
            MakeHappyMessageDisappear();
        } else if (unlockAnimalMessaeUp) {
            UnlockAnimationAndMakeMessageDisappear();
        } else if (cheatingAlpaca) {
            MakeCheatingAlpacaMessageDisappear();
        } else if (rewardedAdVideo) {
            MakeRewardedAdsMessageDisappear(false);
        } else {
            StartCoroutine(CloseUpTour());
        }
    }
    public void AddWelcomeHeartsToTotalHearts(int hearts)
    {
        StartCoroutine(AddWelcomeHeartsRoutine(hearts));
    }
    public IEnumerator AddWelcomeHeartsRoutine(int hearts)
    {
        Vector3 pos = menuCam.ScreenToWorldPoint(royButton.transform.position);
        //int length = inventoryManagerScript.totalHeartsEarnedWhileSystemWasOff;
        inventoryManagerScript.totalHeartsEarnedWhileSystemWasOff = heartsEarnedWhileGone = 0;
        for (int i = 0; i < hearts; i++) {
            MakeAHeartPlusOne(pos);
            yield return null;
        }
        yield return new WaitForSeconds(1.5f);
        readWriteScript.AddTotalHearts(hearts);
        readWriteScript.SaveTotalHeartsWhileSystemWasOff(0);
        messageUp = false;
        welcomeMessageUp = false;
        CloseRoyMessage();
        yield return null;
    }
    public IEnumerator ButtonPressedRoutine()
    {
        yield return new WaitForSeconds(0.5f);
        CloseRoyMessage();

        yield return new WaitForSeconds(0.5f);
        int pos = cockATourGuide.dialoguePos;
        if (pos == 19) {
            tourDone = cockATourGuide.tourDone = true;
            readWriteScript.SaveNoMoreTour();
        }
        if (pos == 17) {
            int heartDuration = statsManagerScript.happinessDurationMinutes;
            DisplayCustomRoyMessage("While happy, he will make one heart\nper minute for "+heartDuration.ToString()+" minutes,\neven if your device is off!");
            cockATourGuide.dialoguePos += 1;
            StartCoroutine(MoveCameraToFieldFromAnimal(fieldPos, fieldRot));
            happyMessageUp = false;
        } else if (pos == 14) {
            string foodToNextHappyString = horseScript.myStats.foodToNextHappy.ToString();
            DisplayCustomRoyMessage("Feed the horse by placing "+foodToNextHappyString+"\npieces of food in the field!");
            cockATourGuide.dialoguePos += 1;
            horseReady = true;
        } else {
            DisplayNextRoyMessage();
        }
        pos = cockATourGuide.dialoguePos;
        //Debug.Log("POS = "+pos);
        if (pos == 16) {
            messageUp = false;
            tutorialPause = false;
            waitForAnimalToBeHappy = true;
            onlyHorse = true;
            CloseRoyMessage();
        } else if (pos == 10) {
            cockATourGuide.PlayRoosterSound(1);
            SetGlowMarker(horse);
        } else if (pos == 7) {
            cockATourGuide.PlayRoosterSound(1);
            GoBackToStart();
        } else if (pos == 3) {
            cockATourGuide.PlayRoosterSound(1);
            SetGlowMarker(barnDoor);
        } else if (pos == 1) {
            royNoButton.GetComponent<Button>().interactable = true;
            royYesButton.GetComponent<Button>().interactable = true;
            yield return new WaitForSeconds(2f);
            cockATourGuide.PlayRoosterSound(1);
            royNoButton.gameObject.SetActive(true);
            royYesButton.gameObject.SetActive(true);
        } else {
            royButton.GetComponent<Button>().interactable = true;
            yield return new WaitForSeconds(2f);
            cockATourGuide.PlayRoosterSound(2);
            royButton.gameObject.SetActive(true);
        }
        yield return null;
    }
    public void NoButtonPressed()
    {
        tourDone = cockATourGuide.tourDone = true;
        readWriteScript.SaveNoMoreTour();
        doingTutorial = false;
        onlyHorse = false;
        if (waitForAnimalToBeHappy) {
            waitForAnimalToEat = false;
            waitForAnimalToBeHappy = false;
            horseReady = false;
            //Tell horse to go back to the stabl
            goFromFieldToStables = true;
            fieldManagerScript.selectedAnimal.GetComponent<AnimalScript>().GoBackHome();
            fieldManagerScript.ClearAllFood();
            fieldManagerScript.selectedAnimal = null;
        }
        if (confirmationMessageUp) {
            waitForAnimalToEat = false;
            waitForAnimalToBeHappy = false;
            horseReady = false;
            //Tell horse to go back to the stabl
            StartCoroutine(MoveCameraToStablesFromField(stablePos, stableRot));
            fieldManagerScript.selectedAnimal.GetComponent<AnimalScript>().GoBackHome();
            fieldManagerScript.ClearAllFood();
            fieldManagerScript.selectedAnimal = null;
            royNoButton.GetComponent<Button>().interactable = false;
            royYesButton.GetComponent<Button>().interactable = false;
            royBrushButton.GetComponent<Button>().interactable = false;
            royFeedButton.GetComponent<Button>().interactable = false;
            myAudioSources[0].Play();
            MakeConfirmationMessageDisappear();
            return;
        }
        if (rewardedAdVideo) {
            waitForAnimalToEat = false;
            waitForAnimalToBeHappy = false;
            horseReady = false;
            royNoButton.GetComponent<Button>().interactable = false;
            royYesButton.GetComponent<Button>().interactable = false;
            royBrushButton.GetComponent<Button>().interactable = false;
            royFeedButton.GetComponent<Button>().interactable = false;
            myAudioSources[0].Play();
            MakeRewardedAdsMessageDisappear(false);
            return;
        }
        //Saves to the CSV object that we don't want the tour anymore
        readWriteScript.SaveNoMoreTour();
        //TODO make everything clear out!
        royNoButton.GetComponent<Button>().interactable = false;
        royYesButton.GetComponent<Button>().interactable = false;
        myAudioSources[0].Play();
        if (!atTheMainMenu) {
            StartCoroutine(GoBackFromBrushingRoutine());
        } else {
            StartCoroutine(DoneWithTour());
        }
    }
    public void YesButtonPressed()
    {
        messageUp = true;
        royNoButton.GetComponent<Button>().interactable = false;
        royYesButton.GetComponent<Button>().interactable = false;
        myAudioSources[1].Play();
        if (rewardedAdVideo) {
            rewardedAdDisplayScript.ShowRewardedVideo();
            return;
        }
        if (!waitForAnimalToEat && !waitForAnimalToBeHappy) {
            tourDone = false;
            doingTutorial = true;
            onlyHorse = true;
            cockATourGuide.dialoguePos += 1;
            readWriteScript.SaveRestartTour();
            StartCoroutine(StartTour());
        }
        if (waitForAnimalToEat || waitForAnimalToBeHappy) {
            StartCoroutine(WaitToClose(0.5f));
        }
    }
    public IEnumerator WaitToClose(float num)
    {
        yield return new WaitForSeconds(num);
        CloseRoyMessage();
    }
    public IEnumerator DoneWithTour()
    {
        handTapGesture.SetActive(false);
        glowPrefab.SetActive(false);
        foodToNextHappyPieObj.SetActive(false);
        brushToNextHappyPieObj.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        CloseRoyMessage();
        yield return new WaitForSeconds(0.5f);
        DisplayCustomRoyMessage("Okay! Just tap me anytime if you need me!");
        yield return new WaitForSeconds(2f);
        cockATourGuide.PlayRoosterSound(2);
        royButton.gameObject.SetActive(true);
        yield return null;
    }
    public IEnumerator CloseUpTour()
    {
        yield return new WaitForSeconds(0.5f);
        CloseRoyMessage();
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(cockATourGuide.CloseUpTour());
        tutorialPause = false;
        doingTutorial = false;
        messageUp = false;
        readWriteScript.SaveNoMoreTour();
        yield return null;
    }
    public IEnumerator StartTour()
    {
        yield return StartCoroutine(ButtonPressedRoutine());
        yield return null;
    }
    //This function is called when the glow button is clicked
    public void GlowButtonTapped()
    {
        glowPrefab.SetActive(false);
        myAudioSources[0].Play();
        int pos = cockATourGuide.dialoguePos;
        //When we're at the field, feeding the horse
        if (pos == 12) {
            tutorialPause = false;
            waitForAnimalToEat = true;
            onlyHorse = true;
            CloseRoyMessage();
            horseScript.GoAnimalGo();
        //When we're going to the field (selecting the Horse)
        } else if (pos == 10) {
            onlyHorse = true;
            cockATourGuide.PlayRoosterSound(2);
            StartCoroutine(SetGestureMarker(carrotCrate));
            waitForCarrotOnField = true;
            StartCoroutine(MoveToFieldTutorialRoutine());
            horseScript.MoveOutToField();
        //When we're going to the stables
        } else if (pos == 8) {
            StartCoroutine(MoveToStablesTutorialRoutine());
        //When we're going to the barnDoor
        } else if (pos == 3) {
            StartCoroutine(MoveToBarnDoorsTutorialRoutine());
        }
    }
    public IEnumerator MoveToBarnDoorsTutorialRoutine()
    {
        yield return new WaitForSeconds(0.5f);
        CloseRoyMessage();
        yield return new WaitForSeconds(0.5f);
        cockATourGuide.MoveToBarnDoorsTutorial();
        yield return StartCoroutine(MoveAndRotateCameraTo(barnDoorPos, barnDoorRot));
        inventoryManagerScript.UpdateAllInventoryCameraStrings();
        barnDoorScript.OpenDoors();
        DisplayNextRoyMessage();
        yield return new WaitForSeconds(2f);
        cockATourGuide.PlayRoosterSound(2);
        royButton.gameObject.SetActive(true);
        yield return null;
    }
    public void GoBackToStart()
    {
        StartCoroutine(GoBackToStartRoutine());
    }
    public IEnumerator GoBackToStartRoutine()
    {
        yield return new WaitForSeconds(0.5f);
        CloseRoyMessage();
        yield return new WaitForSeconds(0.5f);
        cockATourGuide.MoveToStartTutorial();
        barnDoorScript.CloseDoors();
        yield return StartCoroutine(MoveCameraToStartFromBarndoor(mainMenuPos, mainMenuRot));
        DisplayNextRoyMessage();
        yield return new WaitForSeconds(2f);
        cockATourGuide.PlayRoosterSound(1);
        SetGlowMarker(stableSign);
        yield return null;
    }
    public IEnumerator MoveToStablesTutorialRoutine()
    {
        yield return new WaitForSeconds(0.5f);
        CloseRoyMessage();
        yield return new WaitForSeconds(0.5f);
        cockATourGuide.MoveToStablesTutorial();
        yield return StartCoroutine(MoveCameraToStablesFromStart(stablePos, stableRot));
        DisplayNextRoyMessage();
        yield return new WaitForSeconds(2f);
        cockATourGuide.PlayRoosterSound(2);
        royButton.gameObject.SetActive(true);
        yield return null;
    }
    public IEnumerator MoveToFieldTutorialRoutine()
    {
        yield return new WaitForSeconds(0.5f);
        CloseRoyMessage();
        yield return new WaitForSeconds(0.5f);
        cockATourGuide.MoveToFieldTutorial();
        DisplayNextRoyMessage();
        yield return new WaitForSeconds(2f);
        cockATourGuide.PlayRoosterSound(2);
        //royButton.gameObject.SetActive(true);
        yield return null;
    }

    public void SetGlowMarker(GameObject obj)
    {
        glowPrefab.SetActive(true);
        Vector3 pos = menuCam.WorldToScreenPoint(obj.transform.position);
        glowPrefab.transform.position = pos;
    }
    public IEnumerator SetGestureMarker(GameObject obj)
    {
        yield return new WaitForSeconds(2f);
        handTapGesture.SetActive(true);
        Vector3 pos = fieldCam.WorldToScreenPoint(obj.transform.position);
        handTapGesture.transform.position = pos;
        yield return null;
    }
    public void DisplayNextRoyMessage()
    {
        royAnim.SetBool("Active", true);
        cockATourGuide.dialoguePos += 1;
        royMessage.description = cockATourGuide.dialogues[cockATourGuide.dialoguePos];
        royMessage.UpdateUI();
        royButton.GetComponent<Button>().interactable = true;
    }
    public void DisplayCustomRoyMessage(string message)
    {
        royAnim.SetBool("Active", true);
        royMessage.description = message;
        royMessage.UpdateUI();
        royButton.GetComponent<Button>().interactable = true;
    }
    public void CloseRoyMessage()
    {
        royButton.gameObject.SetActive(false);
        royNoButton.gameObject.SetActive(false);
        royYesButton.gameObject.SetActive(false);
        royAnim.SetBool("Active", false);
    }
    public void WaitForPLayerToDragACarrotInTheField()
    {
        if (fieldManagerScript.TheresSomethingOnTheBoard()) {
            //We have something on the board.
            waitForCarrotOnField = false;
            //Temporarily prevent all taps from taking place
            tutorialPause = true;
            StartCoroutine(FeedHorseTutorial());
        }
    }
    public IEnumerator FeedHorseTutorial()
    {
        myAudioSources[1].Play();
        CloseRoyMessage();
        handTapGesture.SetActive(false);
        yield return null;
        SetGlowMarker(horseFieldMarker);
        yield return new WaitForSeconds(0.5f);
        //Make the notification appear over the horse
        DisplayNextRoyMessage();
        yield return new WaitForSeconds(0.5f);
        cockATourGuide.PlayRoosterSound(2);
    }
    public void WaitForAnimalInTutorialToEatRoutine()
    {
        //if, while you're waiting for food, the food from the field for some reason goes away...
        /*
        if (!fieldManagerScript.TheresSomethingOnTheBoard()) {
            waitForAnimalToEat = false;
            waitForCarrotOnField = true;
            cockATourGuide.dialoguePos = 10;
            tutorialPause = false;
            CloseRoyMessage();
            GlowButtonTapped();
        }
        */
        int wasFoodCount = horseFoodCountForTutorial;
        horseFoodCountForTutorial = horseScript.myStats.foodCount;
        if (wasFoodCount != horseFoodCountForTutorial) {
            waitForAnimalToEat = false;
            //TODO Check if the tutorialPause fixed the issue
            tutorialPause = true;
            StartCoroutine(ButtonPressedRoutine());
        }
    }
    public void WaitForAnimalToBeHappyInTutorial()
    {
        bool wasHappy = horseHappiness;
        horseHappiness = horseScript.happy;
        if (!wasHappy && horseHappiness) {
            messageUp = true;
            tutorialPause = true;
            waitForAnimalToBeHappy = false;
            onlyHorse = false;
            StartCoroutine(ButtonPressedRoutine());
        }
    }
    public void BackButtonTappedDuringTutorialSoTellPlayerToFeedAnimal()
    {
        StartCoroutine(MakeFeedAnimalMessageAppear());
    }
    public IEnumerator MakeFeedAnimalMessageAppear()
    {
        int foodToNextHappy = fieldManagerScript.selectedAnimal.myStats.foodToNextHappy;
        DisplayCustomRoyMessage("Feed the horse "+foodToNextHappy.ToString()+" more food?                     ");
        cockATourGuide.PlayRoosterSound(2);
        yield return new WaitForSeconds(3f);
        cockATourGuide.PlayRoosterSound(1);
        royNoButton.GetComponent<Button>().interactable = true;
        royYesButton.GetComponent<Button>().interactable = true;
        cockATourGuide.PlayRoosterSound(1);
        royNoButton.gameObject.SetActive(true);
        royYesButton.gameObject.SetActive(true);
    }
    public IEnumerator MakeContinueTourQuestionlMessageAppear()
    {
        DisplayCustomRoyMessage("Continue the tour?");
        cockATourGuide.PlayRoosterSound(2);
        yield return new WaitForSeconds(3f);
        cockATourGuide.PlayRoosterSound(1);
        royNoButton.GetComponent<Button>().interactable = true;
        royYesButton.GetComponent<Button>().interactable = true;
        cockATourGuide.PlayRoosterSound(1);
        royNoButton.gameObject.SetActive(true);
        royYesButton.gameObject.SetActive(true);
    }
    public void MakeEarnToEquipAnimalMessage(int hearts, Vector3 aboveAnimalPos)
    {
        messageUp = true;
        StartCoroutine(MakeEarnToEquipAnimalMessageRoutine(hearts, aboveAnimalPos));
    }
    public IEnumerator MakeEarnToEquipAnimalMessageRoutine(int hearts, Vector3 aboveAnimalPos)
    {
        DisplayCustomRoyMessage("This animal is locked!\nEarn "+hearts+" hearts total to gain it!");
        cockATourGuide.GoToAnimal(aboveAnimalPos);
        yield return new WaitForSeconds(2f);
        cockATourGuide.PlayRoosterSound(2);
        royButton.gameObject.SetActive(true);
        yield return null;
    }
    public IEnumerator MakeHornSoundAndConfetti(int pos)
    {
        //TODO consider a new noise. The horn was played when Roy arrived to the new animal
        //Play horn sound
        myAudioSources[3].clip = newlyEquippedSuperHorn;
        myAudioSources[3].Play();
        yield return new WaitForSeconds(myAudioSources[3].clip.length);
        //Make confetti
        Vector3 royPos = cockATourGuide.transform.position-Vector3.down;
        ParticleSystem rightConfetti = Instantiate(confettiPrefab, royPos+Vector3.right, Quaternion.identity) as ParticleSystem;
        ParticleSystem leftConfetti = Instantiate(confettiPrefab, royPos+Vector3.left, Quaternion.identity) as ParticleSystem;
        KillTimer(rightConfetti.gameObject, 10f);
        KillTimer(leftConfetti.gameObject, 10f);
        myAudioSources[4].clip = newEquippedAnimalApplause;
        myAudioSources[4].Play();
        yield return new WaitForSeconds(myAudioSources[4].clip.length);
    }
    //Wrapper called from the inventoryManagerScript
    public void MakeUnlockedNewAnimalMessage(string name, Vector3 aboveAnimalPos)
    {
        StartCoroutine(MakeUnlockedNewAnimalMessageRoutine(name, aboveAnimalPos));
    }
    public IEnumerator MakeUnlockedNewAnimalMessageRoutine(string name, Vector3 aboveAnimalPos)
    {
        unlockAnimalMessaeUp = true;
        //Make Roy message
        DisplayCustomRoyMessage(name+" has now joined the farm! Congrats!");
        //Move Roy
        cockATourGuide.GoToAnimal(aboveAnimalPos);
        //Make horn sound
        myAudioSources[3].clip = newlyEquippedAnimalHorn;
        myAudioSources[3].Play();
        yield return new WaitForSeconds(myAudioSources[3].clip.length+2f);
        //Make button appear
        cockATourGuide.PlayRoosterSound(2);
        royButton.gameObject.SetActive(true);
        yield return null;
        //Waits for the ButtonPressed() function
    }
    public void MakeCheaterAlpacaMessageAppear(Vector3 aboveAnimalPos)
    {
        StartCoroutine(MakeCheaterAlpacaMessageAppearRoutine(aboveAnimalPos));
    }
    public IEnumerator MakeCheaterAlpacaMessageAppearRoutine(Vector3 aboveAnimalPos)
    {
        cheatingAlpaca = true;
        //Make Roy message
        DisplayCustomRoyMessage("You found the cheating alpaca! +500 hearts!");
        //Move Roy
        cockATourGuide.GoToAnimal(aboveAnimalPos);
        //Make horn sound
        myAudioSources[3].clip = newlyEquippedAnimalHorn;
        myAudioSources[3].Play();
        yield return new WaitForSeconds(myAudioSources[3].clip.length+2f);
        //Make button appear
        cockATourGuide.PlayRoosterSound(2);
        royButton.gameObject.SetActive(true);
        yield return null;
        //Waits for the ButtonPressed() function
    }

    public IEnumerator KillTimer(GameObject obj, float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(obj);
    }
    public void SetFoodToNextHappyPie()
    {
        foodToNextHappyPieObj.SetActive(true);
        AnimalStatsScript selectStat = fieldManagerScript.selectedAnimal.gameObject.GetComponent<AnimalStatsScript>();
        float ratio = (float) inventoryManagerScript.GetNextFoodHappyRatio(selectStat);
        foodToNextHappyText.text = selectStat.foodToNextHappy.ToString();
        foodToNextHappyPie.currentPercent = (float) ratio*100f;
    }
    public void SetBrushToNextHappyPie()
    {
        brushToNextHappyPieObj.SetActive(true);
        AnimalStatsScript selectStat = fieldManagerScript.selectedAnimal.gameObject.GetComponent<AnimalStatsScript>();
        float ratio = (float) inventoryManagerScript.GetNextBrushHappyRatio(selectStat);
        int brushPercent = (int) Mathf.Round(ratio*100f);
        if (brushPercent >= 100) {
            brushToNextHappyText.text = "100%";
        } else {
            brushToNextHappyText.text = brushPercent.ToString()+"%";
        }
        brushToNextHappyPie.currentPercent = (float) ratio*100f;
    }

    //Called from the AnimalScript
    public void MoveCameraToAnimal(AnimalScript animScript)
    {
        Vector3 pos = animScript.gameObject.transform.position+(Vector3.back*3)+(Vector3.up*1.5f);
        Quaternion animalRot = Quaternion.Euler(animalBrushRot);
        cockATourGuide.GoToAnimal(pos+(Vector3.forward*0.5f));
        StartCoroutine(MakeBrushOrFeedMessageAppear(animScript));
        StartCoroutine(MoveCameraToAnimalRoutine(pos+animalAdjPos, animalRot));
    }
    public IEnumerator MoveCameraToAnimalRoutine(Vector3 cameraDesPos, Quaternion cameraDesRot)
    {
        stablesBackSign.SetActive(false);
        goToField = false;
	    yield return new WaitForSeconds(0.5f);
        Vector3 cameraStartPos = menuCam.transform.position;
        Quaternion cameraStartRot = menuCam.transform.rotation;
	    float timeOfTravel = 1f;
	    float currentTime = 0;
	    float normalizedValue;
	    while (currentTime <= timeOfTravel) {
			currentTime += Time.deltaTime * travelSpeed;
			normalizedValue = currentTime / timeOfTravel; // we normalize our time
			menuCam.transform.position = Vector3.Lerp(cameraStartPos, cameraDesPos, normalizedValue);
            menuCam.transform.rotation = Quaternion.Lerp(cameraStartRot, cameraDesRot, normalizedValue);
			yield return null;
	    }
        yield return null;
		menuCam.transform.position = cameraDesPos;
        yield return null;
    }
    public IEnumerator MakeBrushOrFeedMessageAppear(AnimalScript animScript)
    {
        confirmationMessageUp = true;
        string name = animScript.myStats.animalName;
        DisplayCustomRoyMessage("Would you like to brush\nor feed the "+name+"?");
        cockATourGuide.PlayRoosterSound(2);
        yield return new WaitForSeconds(3f);
        messageUp = true;
        cockATourGuide.PlayRoosterSound(1);
        royBrushButton.GetComponent<Button>().interactable = true;
        royFeedButton.GetComponent<Button>().interactable = true;
        royNoButton.GetComponent<Button>().interactable = true;
        cockATourGuide.PlayRoosterSound(1);
        royBrushButton.gameObject.SetActive(true);
        royFeedButton.gameObject.SetActive(true);
        royNoButton.gameObject.SetActive(true);
    }
    public void BrushButtonPressed()
    {
        StartCoroutine(BrushButtonPressedRoutine());
    }
    public IEnumerator BrushButtonPressedRoutine()
    {
        confirmationMessageUp = false;
        royNoButton.GetComponent<Button>().interactable = false;
        royYesButton.GetComponent<Button>().interactable = false;
        royBrushButton.GetComponent<Button>().interactable = false;
        royFeedButton.GetComponent<Button>().interactable = false;
        SetBrushToNextHappyPie();
        myAudioSources[0].Play();
        happyMessageUp = false;
        messageUp = false;
        yield return new WaitForSeconds(1f);
        CloseRoyMessage();
        royButton.gameObject.SetActive(false);
        royNoButton.gameObject.SetActive(false);
        royBrushButton.gameObject.SetActive(false);
        royFeedButton.gameObject.SetActive(false);
        StartCoroutine(cockATourGuide.CloseUpTour());
        touchManagerScript.brushingAnimal = true;
        goBackFromBrushingButton.SetActive(true);
        goBackFromBrushingButton.GetComponentInChildren<Button>().interactable = true;
        atTheBrushing = true;
    }

    public void FeedButtonPressed()
    {
        confirmationMessageUp = false;
        goToField = true;
        royNoButton.GetComponent<Button>().interactable = false;
        royYesButton.GetComponent<Button>().interactable = false;
        royBrushButton.GetComponent<Button>().interactable = false;
        royFeedButton.GetComponent<Button>().interactable = false;
        myAudioSources[0].Play();
        happyMessageUp = false;
        messageUp = false;
        CloseRoyMessage();
        royButton.gameObject.SetActive(false);
        royNoButton.gameObject.SetActive(false);
        royBrushButton.gameObject.SetActive(false);
        royFeedButton.gameObject.SetActive(false);
        StartCoroutine(cockATourGuide.CloseUpTour());
    }
    public void GoBackFromBrushing()
    {
        if (!tourDone) {
            if (waitForAnimalToEat || waitForAnimalToBeHappy) {
                StartCoroutine(MakeFeedAnimalMessageAppear());
                return;
            }
            StartCoroutine(MakeContinueTourQuestionlMessageAppear());
        } else {
            if (!atTheMainMenu) {
                StartCoroutine(GoBackFromBrushingRoutine());
            }
        }
    }
    public IEnumerator GoBackFromBrushingRoutine()
    {
        atTheBrushing = false;
        waitForAnimalToEat = false;
        waitForAnimalToBeHappy = false;
        horseReady = false;
        //Tell horse to go back to the stabl
        StartCoroutine(MoveCameraToStablesFromField(stablePos, stableRot));
        if (fieldManagerScript.selectedAnimal != null) {
            fieldManagerScript.selectedAnimal.GetComponent<AnimalScript>().GoBackHome();
        }
        fieldManagerScript.ClearAllFood();
        fieldManagerScript.selectedAnimal = null;
        royNoButton.GetComponent<Button>().interactable = false;
        royYesButton.GetComponent<Button>().interactable = false;
        royBrushButton.GetComponent<Button>().interactable = false;
        royFeedButton.GetComponent<Button>().interactable = false;
        goBackFromBrushingButton.GetComponentInChildren<Button>().interactable = false;
        myAudioSources[0].Play();
        yield return new WaitForSeconds(2f);
        goBackFromBrushingButton.SetActive(false);
        touchManagerScript.brushingAnimal = false;
        yield return null;

    }
    public void MakeRewardedAdsMessageAppear()
    {
        StartCoroutine(MakeRewardedAdsMessageAppearRoutine());
    }
    public IEnumerator MakeRewardedAdsMessageAppearRoutine()
    {
        rewardedAdVideo = true;
        //Make Roy message
        DisplayCustomRoyMessage("Would you like to watch\nan ad for +5 hearts?");
        //Move Roy
        //TODO need to move Roy
        cockATourGuide.GoToAnimal(cockATourGuide.transform.position+Vector3.up);
        yield return new WaitForSeconds(2f);
        //Make button appear
        cockATourGuide.PlayRoosterSound(2);
        royButton.gameObject.SetActive(true);
        royNoButton.gameObject.SetActive(true);
        royNoButton.GetComponent<Button>().interactable = true;
        royYesButton.gameObject.SetActive(true);
        royYesButton.GetComponent<Button>().interactable = true;
        yield return null;
        //Waits for the ButtonPressed() function
    }
    public void MakeRewardedAdsMessageDisappear(bool watchedResult)
    {
        StartCoroutine(MakeRewardedAdsMessageDisappearRoutine(watchedResult));
        /*
        rewardedAdVideo = false;
        if (watchedResult) {
            for (int i = 0; i < 5; i++) {
                //TODO make a heart come from Roy
                MakeAHeartPlusOne(cockATourGuide.transform.position);
                statsManagerScript.IncreaseDuplicateHeartCounterByOneWhichExistsOnlyToTrackChangeInHeartCount();
                //yield return null;
            }
        }
        readWriteScript.saveShit = true;
        happyMessageUp = false;
        messageUp = false;
        CloseRoyMessage();
        royButton.gameObject.SetActive(false);
        StartCoroutine(cockATourGuide.CloseUpTour());
        royNoButton.GetComponent<Button>().interactable = false;
        royYesButton.GetComponent<Button>().interactable = false;
        //yield return null;
        */
    }
    public IEnumerator MakeRewardedAdsMessageDisappearRoutine(bool watchedResult)
    {
        rewardedAdVideo = false;
        if (watchedResult) {
            for (int i = 0; i < 5; i++) {
                //TODO make a heart come from Roy
                MakeAHeartPlusOne(cockATourGuide.transform.position);
                statsManagerScript.IncreaseDuplicateHeartCounterByOneWhichExistsOnlyToTrackChangeInHeartCount();
                yield return null;
            }
        }
        readWriteScript.saveShit = true;
        happyMessageUp = false;
        messageUp = false;
        CloseRoyMessage();
        royButton.gameObject.SetActive(false);
        StartCoroutine(cockATourGuide.CloseUpTour());
        royNoButton.GetComponent<Button>().interactable = false;
        royYesButton.GetComponent<Button>().interactable = false;
        yield return null;
    }
}
