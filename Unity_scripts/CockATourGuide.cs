using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CockATourGuide : MonoBehaviour
{
    public GameObject menuManager;
    MenuManagerScript menuManagerScript;
    public float groundCheckLaserLength = 1.0f;
    public GameObject [] flyDownPaths, flyToBarnDoorPaths, flyToStablePaths, flyToFieldRoutine;
    public bool isGrounded;
    public LayerMask whatIsGround;
    public Animator myAnim;
    public ParticleSystem makeItRainPrefab;
    ParticleSystem makeItRainLeft, makeItRainRight;
    Vector3 roostPos = new Vector3(6.7f, 0.4662497f, 7.483749f);
    Vector3 talkPos = new Vector3(9, 1.2f, 7.5f);
    Quaternion talkRot = Quaternion.Euler(0, 60, 0);
    Quaternion barnDoorRot = Quaternion.Euler(22, 30, 0);
    [HideInInspector]
    public string [] dialogues;
    public int dialoguePos = 0;
    public bool tourDone = false;
    AudioSource [] myAudioSources;
    public AudioClip [] animalSounds, wingFlaps;
    public bool playWingFlaps = false, canPlayNewAnimalSound = true;
    bool loopWingFlaps = true;
    public bool roosted = false;
    // Start is called before the first frame update
    void Start()
    {
        BuildDialogueArray();
        menuManagerScript = menuManager.GetComponent<MenuManagerScript>();
        myAnim = gameObject.GetComponent<Animator>();
        dialoguePos = 0;
        InitSounds();
    }

    // Update is called once per frame
    void Update()
    {
        roosted = CheckIfRoosted();
        if (roosted) {
            this.transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        isGrounded = ImGrounded();
        MakeItRain();
        myAnim.SetBool("Grounded", isGrounded);
        if (playWingFlaps && loopWingFlaps) {
            StartCoroutine(PlayWingFlapSounds());
        }
    }
    void OnMouseDown()
    {
        CheckToRestartTour();
    }
    public void CheckToRestartTour()
    {
        if (tourDone && roosted && menuManagerScript.atTheMainMenu) {
            menuManagerScript.RestartTour();
        }
    }
    public bool CheckIfRoosted()
    {
        if (Vector3.Distance(this.transform.position, roostPos) < 0.1f) {
            return true;
        }
        return false;
    }
    public void InitSounds()
    {
        for (int i = 0; i < 2; i++) {
            gameObject.AddComponent<AudioSource>();
        }
        myAudioSources = gameObject.GetComponents<AudioSource>();
        myAudioSources[0].volume = 0.05f;
        myAudioSources[1].volume = 0.15f;
    }

    public void MakeItRain()
    {
        if (isGrounded) {
            if (makeItRainRight != null || makeItRainLeft != null) {
                playWingFlaps = false;
                makeItRainRight.Stop();
                makeItRainLeft.Stop();
                KillTimer(makeItRainRight.gameObject);
                KillTimer(makeItRainLeft.gameObject);
                makeItRainRight = null;
                makeItRainLeft = null;
            }
        } else {
            if (makeItRainRight == null) {
                playWingFlaps = true;
                Quaternion rotation = Quaternion.Euler(0, 180, 0);
                Vector3 left = this.transform.TransformDirection(Vector3.left)*0.25f;
                Vector3 right = this.transform.TransformDirection(Vector3.right)*0.25f;
                Vector3 up = this.transform.TransformDirection(Vector3.up)*0.4f;
                makeItRainRight = Instantiate(makeItRainPrefab, this.transform.position+right+up, rotation) as ParticleSystem;
                makeItRainLeft = Instantiate(makeItRainPrefab, this.transform.position+left+up, rotation) as ParticleSystem;
                makeItRainRight.gameObject.transform.localScale = Vector3.one * 0.3f;
                makeItRainLeft.gameObject.transform.localScale = Vector3.one * 0.3f;
                makeItRainRight.Play();
                makeItRainLeft.Play();
                makeItRainRight.transform.SetParent(this.transform);
                makeItRainLeft.transform.SetParent(this.transform);
            }
        }

    }
    public void FollowFlyDownPath()
    {
        StartCoroutine(FollowFlyDownPathRoutine());
    }
    public IEnumerator FollowFlyDownPathRoutine()
    {
        for (int i = 0; i < flyDownPaths.Length; i++) {
            yield return StartCoroutine(FollowPathRoutine(flyDownPaths[i].transform.position));
            if (i % 2 == 0) {
                this.transform.rotation = Quaternion.Euler(0, 0, 0);
            } else {
                //this.transform.rotation = Quaternion.Euler(0, 180, 0);
                this.transform.rotation = talkRot;
            }
        }
        yield return null;
    }
    public IEnumerator FollowPathRoutine(Vector3 desPos)
    {
        Vector3 startPos = this.transform.position;
        float timeOfTravel = 1f;
        float currentTime = 0;
        float normalizedValue;
        while (currentTime <= timeOfTravel) {
            currentTime += Time.deltaTime * 1.5f;
            normalizedValue = currentTime / timeOfTravel; // we normalize our time
            this.transform.position = Vector3.Slerp(startPos, desPos, normalizedValue);
            yield return null;
        }
        yield return null;
        this.transform.position = desPos;
        yield return null;
    }
    public bool ImGrounded()
    {
        if (Physics.Raycast(this.transform.position+Vector3.up, transform.TransformDirection(Vector3.down), groundCheckLaserLength, whatIsGround)) {
            return true;
        }
        return false;
    }
    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 direction = transform.TransformDirection(Vector3.down) * groundCheckLaserLength;
        Gizmos.DrawRay(transform.position+Vector3.up, direction);
    }
    public void KillTimer(GameObject obj)
    {
        StartCoroutine(KillTimerRoutine(obj));
    }
    public IEnumerator KillTimerRoutine(GameObject obj)
    {
        yield return new WaitForSeconds(10f);
        Destroy(obj);
    }
    public void GoToPlayer()
    {
        this.transform.rotation = talkRot;
        StartCoroutine(GoToPlayerRoutine(talkPos));
    }
    public IEnumerator GoToPlayerRoutine(Vector3 desPos)
    {
        this.transform.rotation = talkRot;
        yield return StartCoroutine(FollowPathRoutine(desPos));
        this.transform.rotation = talkRot;
    }
    public IEnumerator CloseUpTour()
    {
        this.transform.rotation = Quaternion.Euler(0, 0, 0);
        int lastPos = flyDownPaths.Length -1;
        yield return StartCoroutine(FollowPathRoutine(flyDownPaths[lastPos-1].transform.position));
        yield return null;
        this.transform.rotation = Quaternion.Euler(0, 180, 0);
        yield return StartCoroutine(FollowPathRoutine(flyDownPaths[lastPos].transform.position));
        yield return new WaitForSeconds(1f);
        tourDone = true;
    }
    public IEnumerator PlayWingFlapSounds()
    {
        loopWingFlaps = false;
        int randomSound = UnityEngine.Random.Range(0,wingFlaps.Length-1);
        myAudioSources[0].clip = wingFlaps[randomSound];
        myAudioSources[0].Play();
        yield return new WaitForSeconds(myAudioSources[0].clip.length);
        loopWingFlaps = true;
        yield return null;
    }
    public void PlayRoosterSound(int num)
    {
        if (canPlayNewAnimalSound)
            StartCoroutine(PlayerRoosterSoundRoutine(num));
    }
    public IEnumerator PlayerRoosterSoundRoutine(int num)
    {
        canPlayNewAnimalSound = false;
        myAudioSources[1].clip = animalSounds[num];
        myAudioSources[1].Play();
        yield return new WaitForSeconds(myAudioSources[1].clip.length);
        canPlayNewAnimalSound = true;
    }
    public void MoveToBarnDoorsTutorial()
    {
        StartCoroutine(FlyToBarnDoorsRoutine());
    }
    public IEnumerator FlyToBarnDoorsRoutine()
    {
        for (int i = 0; i < flyToBarnDoorPaths.Length; i++) {
            yield return StartCoroutine(FollowPathRoutine(flyToBarnDoorPaths[i].transform.position));
            if (i % 2 == 0) {
                this.transform.rotation = Quaternion.Euler(0, 90, 0);
            } else {
                this.transform.rotation = Quaternion.Euler(0, 270, 0);
            }
        }
        this.transform.rotation = barnDoorRot;
        yield return null;
    }
    public void MoveToStartTutorial()
    {
        StartCoroutine(FlyToStartRoutine());
    }
    public IEnumerator FlyToStartRoutine()
    {
        for (int i = flyToBarnDoorPaths.Length-1; i >= 0; i--) {
            yield return StartCoroutine(FollowPathRoutine(flyToBarnDoorPaths[i].transform.position));
            if (i % 2 == 0) {
                this.transform.rotation = Quaternion.Euler(0, 270, 0);
            } else {
                this.transform.rotation = Quaternion.Euler(0, 90, 0);
            }
        }
        yield return StartCoroutine(FollowPathRoutine(talkPos));
        this.transform.rotation = talkRot;
        yield return null;
    }
    public void MoveToStablesTutorial()
    {
        StartCoroutine(FlyToStablesRoutine());
    }
    public IEnumerator FlyToStablesRoutine()
    {
        for (int i = 0; i < flyToStablePaths.Length; i++) {
            yield return StartCoroutine(FollowPathRoutine(flyToStablePaths[i].transform.position));
            if (i % 2 == 0) {
                this.transform.rotation = Quaternion.Euler(0, 90, 0);
            } else {
                this.transform.rotation = Quaternion.Euler(0, 270, 0);
            }
        }
        this.transform.rotation = Quaternion.Euler(0, 133, 0);
        yield return null;
    }
    public void MoveToFieldTutorial()
    {
        StartCoroutine(FlyToFieldRoutine());
    }
    public IEnumerator FlyToFieldRoutine()
    {
        for (int i = 0; i < flyToFieldRoutine.Length; i++) {
            yield return StartCoroutine(FollowPathRoutine(flyToFieldRoutine[i].transform.position));
            if (i % 2 == 0) {
                this.transform.rotation = Quaternion.Euler(0, 90, 0);
            } else {
                this.transform.rotation = Quaternion.Euler(0, 270, 0);
            }
        }
        this.transform.rotation = Quaternion.Euler(0, 133, 0);
        yield return null;
    }
    public void GoToAnimal(Vector3 aboveAnimalPos)
    {
        this.transform.rotation = Quaternion.Euler(0,180,0);
        StartCoroutine(FollowPathRoutine(aboveAnimalPos));
    }
    public void BuildDialogueArray()
    {
        dialogues = new string [] {
            /* 0  */ "Hello! My name is Roy.",
            /* 1  */ "Would you like a tour?",
            /* 2  */ "...",
            /* 3  */ "Let's start with animal health.\n<b>Please tap on the barn doors.</b>",
            /* 4  */ "This is a list of your animals.\nEach with their own level.",
            /* 5  */ "Once it eats enough food\nit will level up and become happy.",
            /* 6  */ "Let's go feed an animal!",
            /* 7  */ "Let's go to the Stables.",
            /* 8   */ "Please tap on the Stables sign",
            /* 9   */ "These are your animals!\nAren't they adorable!?",
            /* 10  */ "Tap on the horse!",
            /* 11  */ "That horse looks hungry!\n<b>Place a carrot in the field.</b>",
            /* 12  */ "Great! Now, tap on the horse!",
            /* 13  */ "Look, the horse are it! Great job!",
            /* 14  */ "Do you see the pie chart in the top left?",
            /* 15  */ "...",
            /* 16  */ "...",
            /* 17  */ "Yay! Theres a heart on the horses head!",
            /* 18  */ "...",
            /* 19  */ "Hearts let you earn more animals!",
            /* 20  */ "That's all for the tour!",
            /* 21  */ "..."
        };
    }
}
