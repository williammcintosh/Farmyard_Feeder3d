using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PickUpScript : MonoBehaviour
{
    AudioSource myAudioSource;
    public AudioClip dropSound;
    [HideInInspector]
    public FieldManager fieldManagerScript;
    public LayerMask whatIsGround, whatIsPickup;
    public float groundLength = 1.4f;
    public bool isGrounded = false;
    [HideInInspector]
    public Rigidbody myRigidbody;
    public BoxCollider myCollider;
    public bool currentlyBeingCarried = false, canBeLifted = true;
    public Vector3 startingPos;
    public DateTime placeTime;
    public string placeTimeStr;
    public Vector3 cratePos;
    // Start is called before the first frame update
    void Start()
    {
        placeTime = DateTime.MaxValue;
        myRigidbody = gameObject.GetComponent<Rigidbody>();
        myAudioSource = gameObject.AddComponent<AudioSource>();
        myAudioSource.volume = 0.5f;
    }

    // Update is called once per frame
    void Update()
    {
        Gravity();
        if (!currentlyBeingCarried) {
            if (IsSnapped()) {
                int myXPos = (int) Mathf.Round(this.transform.position.x);
                int myZPos = (int) Mathf.Round(this.transform.position.z);
                ChecksIfSelfInBounds(myXPos, myZPos);
            } else {
                SnapToGrid();
            }
        }
    }
    public void Gravity()
    {
        bool wasGrounded = isGrounded;
        isGrounded = IsGrounded();
        //if (isGrounded && Mathf.Abs(myRigidbody.velocity.y) > 0)
        if (!wasGrounded && isGrounded) {
            /*
            if (currentlyBeingCarried) {
                currentlyBeingCarried = false;
            }
            */
            myRigidbody.velocity = Vector3.zero;
            myAudioSource.clip = dropSound;
            myAudioSource.Play();
            placeTime = System.DateTime.Now;
            //jDebug.Log(gameObject.name+" AT "+Mathf.Round(this.transform.position.x)+","+Mathf.Round(this.transform.position.z)+" PLACETIME = "+placeTime);
    //        placeTimeStr = placeTime.ToString();
        }
        if (isGrounded) {
            OrientObject();
            SnapToGrid();
        } else {
            /*
            if (!currentlyBeingCarried) {
                currentlyBeingCarried = true;
            }
            */
            if (DateTime.Compare(placeTime, DateTime.MaxValue) != 0) {
                placeTime = DateTime.MaxValue;
                //Debug.Log(gameObject.name+" AT "+Mathf.Round(this.transform.position.x)+","+Mathf.Round(this.transform.position.z)+" PLACETIME = "+placeTime);
            }
     //       placeTimeStr = placeTime.ToString();
        }
        //FloatDownWard();
        KillIfTooLow();
    }
    public void SnapToGrid()
    {
        //currentlyBeingCarried = false;
        int newX = (int)Mathf.Round(this.transform.position.x);
        float newY = this.transform.position.y;
        //The newY here is commented out because it prevents the item from being picked up once dropped.
            //int newY = (int)Mathf.Round(this.transform.position.y);
        int newZ = (int)Mathf.Round(this.transform.position.z);
        this.transform.position = new Vector3(newX, newY, newZ);
    }
    public void Init(FieldManager theFieldManagerScript, Vector3 theStartPos)
    {
        fieldManagerScript = theFieldManagerScript;
        startingPos = theStartPos;
        cratePos = theStartPos;
    }
    //This is called by the Animal Script, this things about to die.
    public void CantBeLifted()
    {
        canBeLifted = false;
    }
    public bool IsGrounded()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out hit, groundLength, whatIsGround))
        {
            return true;
        }
        if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out hit, groundLength, whatIsPickup))
        {
            ReturnBackToCrate(this.transform.position);
            return false;
        }
        return false;
    }
    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position + Vector3.up, Vector3.down * groundLength);

    }
    public void OrientObject()
    {
        this.transform.rotation = Quaternion.identity;
        Vector3 myRot = Vector3.up * 180;
        this.transform.localRotation = Quaternion.Euler(myRot);
    }
    public void FloatDownWard()
    {
        if (myRigidbody.velocity.y < -5f)
        {
            myRigidbody.velocity = Vector3.down * 5f;
        }
    }
    public void CheckSnap()
    {
        Vector3 pos = this.transform.position;
        Vector3 rndPos = new Vector3(Mathf.Round(pos.x), Mathf.Round(pos.y), Mathf.Round(pos.z));
        if (fieldManagerScript.IsWithInFieldBounds((int)rndPos.x, (int)rndPos.z) && fieldManagerScript.IsVacant((int)rndPos.x, (int)rndPos.z)) {
            this.transform.position = rndPos;
            //placeTime = System.DateTime.Now;
            //placeTimeStr = placeTime.ToString();
        //} else {
        //    GoBack();
        }
    }
    public bool IsSnapped()
    {
        Vector3 pos = this.transform.position;
        if (Mathf.Round(pos.x) == pos.x && Mathf.Round(pos.z) == pos.z) {
            return true;
        }
        //placeTime = DateTime.Parse("10/16/1990 12:00:00 PM");
        //placeTimeStr = placeTime.ToString();
        return false;
    }
    public void GoBack()
    {
        //If startingPos (where we go back to) is outside of field bounds, destroy it
        int roundedX = (int) Mathf.Round(startingPos.x);
        int roundedZ = (int) Mathf.Round(startingPos.z);
        if (fieldManagerScript.IsWithInFieldBounds(roundedX, roundedZ) && fieldManagerScript.IsVacant(roundedX, roundedZ)) {
            StartCoroutine(ReturnBackToPrevLoc(this.transform.position, startingPos));
        } else {
            StartCoroutine(ReturnBackToCrate(this.transform.position));
        }
    }
    public IEnumerator ReturnBackToPrevLoc(Vector3 startPos, Vector3 desPos)
    {
        gameObject.GetComponent<BoxCollider>().isTrigger = false;
        float timeOfTravel = 1f;
        float currentTime = 0;
        float normalizedValue;
        while (currentTime <= timeOfTravel) {
            currentTime += Time.deltaTime * 3f;
            normalizedValue = currentTime / timeOfTravel; // we normalize our time
            this.transform.position = Vector3.Lerp(startPos, desPos, normalizedValue);
            yield return null;
        }
        OrientObject();
        myRigidbody.useGravity = true;
        yield return null;
    }
    public void GoToCrate()
    {
        Vector3 pos = this.transform.position;
        StartCoroutine(ReturnBackToCrate(pos));
    }
    public IEnumerator ReturnBackToCrate(Vector3 startPos)
    {
        Vector3 desPos = cratePos;
        gameObject.GetComponent<BoxCollider>().isTrigger = true;
        float timeOfTravel = 1f;
        float currentTime = 0;
        float normalizedValue;
        while (currentTime <= timeOfTravel) {
            currentTime += Time.deltaTime * 3f;
            normalizedValue = currentTime / timeOfTravel; // we normalize our time
            this.transform.position = Vector3.Lerp(startPos, desPos, normalizedValue);
            yield return null;
        }
        myRigidbody.useGravity = true;
        yield return new WaitForSeconds(0.5f);
        this.transform.position = desPos;
        Destroy(this.gameObject);
        yield return null;
    }
    public void KillIfTooLow()
    {
        if (this.transform.position.y < -5) {
            Destroy(this.gameObject);
        }
    }
    public void SetRigidbodyGravity(bool set)
    {
        if (myRigidbody != null) {
            myRigidbody.useGravity = set;
        }
    }
    public void ChecksIfSelfInBounds(int myXPos, int myZPos)
    {
        if (fieldManagerScript != null) {
            if (!fieldManagerScript.IsWithInFieldBounds(myXPos, myZPos)) {
                Vector3 pos = this.transform.position;
                StartCoroutine(ReturnBackToCrate(pos));
            }
        }
    }
}
