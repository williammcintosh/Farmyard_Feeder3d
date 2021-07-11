using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreeperAlpacaScript : MonoBehaviour
{
    public GameObject menuManager;
    MenuManagerScript menuManagerScript;
    public float myTime = 5.0f;
    public Vector3 bottom;
    public Vector3 top;
    public Vector3 standUpPos;
    public bool bobUpAndDown = true;
    // Start is called before the first frame update
    void Start()
    {
        menuManagerScript = menuManager.GetComponent<MenuManagerScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if (bobUpAndDown) {
            float t = Mathf.PingPong(Time.time * myTime, 1.0f);
            this.transform.position = Vector3.Lerp(bottom, top, t);
        } else {
            this.transform.position = standUpPos;
        }
        //BeinACreeper();
    }
    public void OnMouseDown()
    {
        if (menuManagerScript.royIsRoosted) {
            bobUpAndDown = false;
            menuManagerScript.MakeCheaterAlpacaMessageAppear(standUpPos+(Vector3.up*2));
        }
    }
    /*
    public void BeinACreeper()
    {
        if (Vector3.Distance(this.transform.position, top) <= 0.1f)
            StartCoroutine(Move(Vector3.down));
        else if (Vector3.Distance(this.transform.position, bottom) <= 0.1f)
            StartCoroutine(Move(Vector3.up));
    }
    public IEnumerator Move(Vector3 dir)
    {
        Vector3 startPos = this.transform.position;
        Vector3 destPos = this.transform.position + dir;
        for (float i = 0; i < 1.0f; i += Time.deltaTime / myTime) {
            this.transform.position = Vector3.Lerp(startPos, destPos, i);
            yield return null;
        }
        yield return null;
    }
    */
}
