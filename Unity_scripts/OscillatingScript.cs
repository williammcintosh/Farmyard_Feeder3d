using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OscillatingScript : MonoBehaviour
{
    float myTime;
    Vector3 bottom;
    Vector3 top;
    // Update is called once per frame
    void Start()
    {
        myTime = UnityEngine.Random.Range(0.001f,0.009f);
        bottom = this.transform.position;
        top = this.transform.position+(Vector3.right*200);
    }
    void Update()
    {
        if (Vector3.Distance(this.transform.position, top) <= 10) {
            Destroy(this.gameObject);
        }
        this.transform.position = Vector3.Lerp(bottom, top, myTime*Time.time);
    }
}
