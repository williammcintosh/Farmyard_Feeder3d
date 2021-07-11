using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_IPHONE
using UnityEngine.iOS;
#endif

public class TouchManagerScript : MonoBehaviour
{
    /*
    [Header("TESTING VECTORS")]
    public Vector3 screenToWorldPoint;
    public Vector3 worldToScreenPoint;
    public Vector3 touchPosition;
    */
    public GameObject brushRayCastBlocker;
    [Header("BRUSHING ANIMAL")]
    public GameObject horseBrush;
    public ParticleSystem horseBrushHairParticlesPrefab;
    ParticleSystem myHorseBrushHairParticles;
    public bool brushedAnAnimal;
    public bool canIncreaseBrushCount = true;
    public int testStdAnimalBrushCount = 0;
    public int testRteAnimalBrushCount = 0;
    [HideInInspector]
    public Animator animAnim;
    [HideInInspector]
    public Material hairMat;
    public Vector3 restLocation;
    public float swipeAngle, hypoteneousLength, minSwipeLength;
    public Vector3 fingerDownPos, fingerUpPos;
    public LayerMask whatIsBrushBlocker, whatIsAnimal;
    public Ray ray;
    public GameObject touchPosMarker, presciseTouchMarker;
    public Vector3 snappedTouch;
    public Camera fieldCam, menuCam;
    public GameObject fieldManager;
    FieldManager fieldManagerScript;
    public GameObject menuManager;
    MenuManagerScript menuManagerScript;
    public GameObject statsManagerObj;
    StatsManagerScript statsManagerScript;
    public int touchCount = 0;
    public List<TouchPositionScript> touches = new List<TouchPositionScript>();
    public Vector3 [] targetPos;
    public bool brushingAnimal = false;
    public AudioClip [] brushSounds;
    AudioSource myAudioSource;
    public bool canPlayNewBrushSound = true;
    // Start is called before the first frame update
    void Start()
    {
        myAudioSource = gameObject.AddComponent<AudioSource>();
        myAudioSource.volume = 0.2f;
        fieldManagerScript = fieldManager.GetComponent<FieldManager>();
        menuManagerScript = menuManager.GetComponent<MenuManagerScript>();
        statsManagerScript = statsManagerObj.GetComponent<StatsManagerScript>();
        targetPos = new Vector3 [10];
    }
    // Update is called once per frame
    void Update()
    {
        if (brushingAnimal) {
            brushRayCastBlocker.transform.position = menuCam.transform.position + menuCam.transform.TransformDirection(Vector3.forward);
            BrushTouchDetection();
        } else {
            brushRayCastBlocker.transform.position = Vector3.one * 1000;
            horseBrush.transform.position = restLocation;
            if (myHorseBrushHairParticles != null) {
                myHorseBrushHairParticles.Stop();
            }
            horseBrush.transform.localScale = Vector3.one;
            if (menuManagerScript.atTheField && menuManagerScript.tutorialPause == false) {
                FieldTouchDetection();
    		}
        }
        touchCount = Input.touchCount;
    }
    public void FieldTouchDetection()
    {
        if (touchCount == 6) {
            fieldManagerScript.FillTheFieldWithApples();
        }
        if (Input.touchCount > 0)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch touch = Input.GetTouch(i);
                    ray = fieldCam.ScreenPointToRay(touch.position);
                    RaycastHit rayCastHit;
                    if (Physics.Raycast(ray, out rayCastHit, Mathf.Infinity)) {
                        Vector3 touch_ScreenToWorld = fieldCam.ScreenToWorldPoint(touch.position);
                        float y_total = rayCastHit.point.y - touch_ScreenToWorld.y;                          // calculating the total y difference
                        float newY = (y_total - (rayCastHit.point.y - 1));                // calculating the difference between hit.y and player's y position ...
                                                                                                // ... and subtracting it from the total y difference
                        float factor = newY / y_total;                                          // multiplier in order to adjust the original length and reach the target position
                        targetPos[i] = touch_ScreenToWorld + ((rayCastHit.point - touch_ScreenToWorld) * factor); // start of at the starting point and add the adjusted directional vector
                        presciseTouchMarker.transform.position = targetPos[i];
                    }
                    snappedTouch = new Vector3(
                        Mathf.Round(targetPos[i].x),
                        0.2f,
                        Mathf.Round(targetPos[i].z)
                    );
                touchPosMarker.transform.position = snappedTouch;

                if (touch.phase == TouchPhase.Began) {
                    //resets obj
                    PickUpScript obj = null;
                    //Assigns obj
                    if (fieldManagerScript.IsWithInFieldBounds((int)snappedTouch.x, (int)snappedTouch.z)) {
                        obj = fieldManagerScript.GameObjectAtCoordinates((int)snappedTouch.x, (int)snappedTouch.z);
                    } else if (fieldManagerScript.IsWithInCrateBounds((int)snappedTouch.x, (int)snappedTouch.z)) {
                        obj = fieldManagerScript.CrateAtCoordinates((int)snappedTouch.x,(int)snappedTouch.z);
                    }
                    //Initializes obj's placement values
                    if (obj != null) {
                        if (obj.canBeLifted) {
                            obj.myCollider.isTrigger = true;
                            obj.currentlyBeingCarried = true;
                            obj.SetRigidbodyGravity(false);
                            obj.Init(fieldManagerScript, snappedTouch);
                            touches.Add(new TouchPositionScript(touch.fingerId, obj, snappedTouch));
                        }
                    }
                }
                else if (touch.phase == TouchPhase.Ended) {
                    TouchPositionScript thisTouch = touches.Find(touchLocation => touchLocation.touchID == touch.fingerId);
                    if (thisTouch != null) {
                        if (thisTouch.myObj != null) {
                            touches.RemoveAt(touches.IndexOf(thisTouch));
                            SnapObjectToGrid(thisTouch.myObj.gameObject);
                            PickUpScript carryingObj = thisTouch.myObj;
                            if (fieldManagerScript.IsWithInFieldBounds((int)snappedTouch.x, (int)snappedTouch.z)) {
                                if (fieldManagerScript.IsVacant((int)snappedTouch.x, (int)snappedTouch.z)) {
                                    carryingObj.myCollider.isTrigger = false;
                                    carryingObj.currentlyBeingCarried = false;
                                    //TODO Test the code below
                                    carryingObj.SetRigidbodyGravity(true);
                                    AnimalScript animalScript = fieldManagerScript.selectedAnimal.GetComponent<AnimalScript>();
                                    animalScript.foodQueue.Enqueue(thisTouch.myObj);
                                } else {
                                    carryingObj.GoBack();
                                }
                            } else {
                                carryingObj.GoBack();
                            }
                        }
                    }
                } else if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary) {
                    //Assigns the touchScript's object's position to this touch
                    TouchPositionScript thisTouch = touches.Find(touchLocation => touchLocation.touchID == touch.fingerId);
                    if (thisTouch != null) {
                        if (thisTouch.myObj != null) {
                            if (thisTouch.myObj.canBeLifted) {
                                thisTouch.myObj.transform.rotation = Quaternion.identity;
                                thisTouch.myObj.transform.position = targetPos[i];
                                thisTouch.lastPos = snappedTouch;
                            }
                        }
                    }
                } else {
                    Debug.Log("SOMETHING HAPPENED WITH THE TOUCH MANAGER TOUCH PHASES");
                }
            }
        }
    }
    public void BrushTouchDetection()
    {
        if (brushingAnimal) {
            UpdateSwipeAngle();
        }
        if (Input.touchCount > 0)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch touch = Input.GetTouch(i);
                ray = menuCam.ScreenPointToRay(touch.position);
                RaycastHit rayCastHit;
                if (Physics.Raycast(ray, out rayCastHit, Mathf.Infinity, whatIsBrushBlocker)) {
                    targetPos[i] = rayCastHit.point;
                }

                if (touch.phase == TouchPhase.Began) {
                    fingerUpPos = touch.position;
                }
                else if (touch.phase == TouchPhase.Ended) {
                    TouchPositionScript thisTouch = touches.Find(touchLocation => touchLocation.touchID == touch.fingerId);
                    horseBrush.transform.position = restLocation;
                    if (myHorseBrushHairParticles != null) {
                        myHorseBrushHairParticles.Stop();
                    }
                    if (fieldManagerScript.selectedAnimal != null) {
                        fieldManagerScript.selectedAnimal.SaveMyBrushInfo();
                    }
                    horseBrush.transform.localScale = Vector3.one;
                    brushedAnAnimal = false;
                } else if (touch.phase == TouchPhase.Stationary) {
                    fingerUpPos = touch.position;
                    horseBrush.transform.position = targetPos[i];
                    horseBrush.transform.localScale = Vector3.one * 0.5f;
                    hypoteneousLength = 0;
                    if (animAnim != null) {
                        animAnim.SetTrigger("Idle");
                        animAnim = null;
                    }
                    if (myHorseBrushHairParticles != null) {
                        myHorseBrushHairParticles.Stop();
                    }
                } else if (touch.phase == TouchPhase.Moved) {
                    TouchPositionScript thisTouch = touches.Find(touchLocation => touchLocation.touchID == touch.fingerId);
                    fingerDownPos = touch.position;
                    horseBrush.transform.position = targetPos[i];
                    horseBrush.transform.localScale = Vector3.one * 0.5f;
                    hypoteneousLength = Vector3.Distance(fingerDownPos,fingerUpPos);
                    //the 'brushedAnAnimal' bool only gets true if BrushHairParticles is true, and false only when the swipe is done
                    if (BrushHairParticles()) {
                        brushedAnAnimal = true;
                    }
                    //If the swipe was long enough...
                    if (brushedAnAnimal) {
                        if (hypoteneousLength >= minSwipeLength) {
                            //  Get the angle of the swipe
                            swipeAngle = Quaternion.FromToRotation(Vector3.up, fingerDownPos - fingerUpPos).eulerAngles.z;
                            swipeAngle = Mathf.Round(swipeAngle/90)*90;
                            //Get the selectedAnimal's animator
                            if (fieldManagerScript.selectedAnimal != null) {
                                StartCoroutine(PlayBrushSounds());
                                animAnim = fieldManagerScript.selectedAnimal.GetComponent<Animator>();
                                if (swipeAngle == 0 || swipeAngle == 360) {
                                    animAnim.SetTrigger("BrushUp");
                                } else if (swipeAngle == 270) {
                                    animAnim.SetTrigger("BrushRight");
                                } else if (swipeAngle == 90) {
                                    animAnim.SetTrigger("BrushLeft");
                                } else {
                                    animAnim.SetTrigger("Brush");
                                }
                                animAnim.SetTrigger("Idle");
                            }
                            StartCoroutine(IncreaseAnimalsBrushCount());
                        } else {
                            if (animAnim != null) {
                                animAnim.SetTrigger("Idle");
                                animAnim = null;
                            }
                        }
                    } else {
                        if (animAnim != null) {
                            animAnim.SetTrigger("Idle");
                            animAnim = null;
                        }
                    }
                } else {
                    Debug.Log("SOMETHING HAPPENED WITH THE TOUCH MANAGER TOUCH PHASES");
                }
            }
        }
    }
    public void SnapObjectToGrid(GameObject obj)
    {
        int newX = (int)Mathf.Round(obj.transform.position.x);
        int newY = (int)Mathf.Round(obj.transform.position.y);
        int newZ = (int)Mathf.Round(obj.transform.position.z);
        obj.transform.position = new Vector3(newX, newY, newZ);
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(touchPosMarker.transform.position, 0.2f);
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(presciseTouchMarker.transform.position, 0.2f);
        Gizmos.color = Color.white;

    }
    public void UpdateSwipeAngle()
    {
        hypoteneousLength = Vector3.Distance(fingerDownPos,fingerUpPos);
        swipeAngle = Quaternion.FromToRotation(Vector3.up, fingerDownPos - fingerUpPos).eulerAngles.z;
        swipeAngle = Mathf.Round(swipeAngle/90)*90;
    }
    public bool BrushHairParticles()
    {
        RaycastHit rayCastHit;
        if (Physics.Raycast(ray, out rayCastHit, Mathf.Infinity, whatIsAnimal)) {
            //Only creates brush particles one time
            if (myHorseBrushHairParticles == null) {
                myHorseBrushHairParticles = Instantiate(horseBrushHairParticlesPrefab, horseBrush.transform.position, Quaternion.identity) as ParticleSystem;
                myHorseBrushHairParticles.transform.SetParent(horseBrush.transform);
            }
            //Sets the material every successful hit
            /*
            if (myHorseBrushHairParticles != null){
                hairMat = rayCastHit.collider.gameObject.GetComponentInChildren<Renderer>().material;
                if (hairMat != null) {
                    myHorseBrushHairParticles.gameObject.GetComponent<ParticleSystemRenderer>().material = hairMat;
                }
                myHorseBrushHairParticles.Play();
            }
            */
            if (myHorseBrushHairParticles != null){
                //TODO
                if (fieldManagerScript.selectedAnimal != null) {
                    hairMat = fieldManagerScript.selectedAnimal.gameObject.GetComponentInChildren<Renderer>().material;
                }
                if (hairMat != null) {
                    myHorseBrushHairParticles.gameObject.GetComponent<ParticleSystemRenderer>().material = hairMat;
                }
                myHorseBrushHairParticles.Play();
            }
            return true;
        }
        if (myHorseBrushHairParticles != null) {
            myHorseBrushHairParticles.Stop();
        }
        hairMat = null;
        return false;
    }
    public IEnumerator IncreaseAnimalsBrushCount()
    {
        if (canIncreaseBrushCount) {
            canIncreaseBrushCount = false;
            testRteAnimalBrushCount++;
            if (fieldManagerScript.selectedAnimal != null) {
                fieldManagerScript.selectedAnimal.myStats.brushCount++;
            }
            yield return null;
            canIncreaseBrushCount = true;
        }
        yield return null;
    }
    public IEnumerator PlayBrushSounds()
    {
        if (canPlayNewBrushSound) {
            canPlayNewBrushSound = false;
            int randomSound = UnityEngine.Random.Range(0,brushSounds.Length-1);
            myAudioSource.clip = brushSounds[randomSound];
            myAudioSource.Play();
            yield return new WaitForSeconds(myAudioSource.clip.length);
            canPlayNewBrushSound = true;
        }
    }

}
