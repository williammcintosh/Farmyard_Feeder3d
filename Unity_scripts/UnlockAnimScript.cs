using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockAnimScript : MonoBehaviour
{
    public ParticleSystem prefabUnlockSpark;
    public bool locked = true;
    public bool unlocked = false;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void MakeUnlockSpark()
    {
        Quaternion rot = Quaternion.Euler(90,0,0);
        ParticleSystem myUnlockSpark = Instantiate(prefabUnlockSpark, this.transform.position, rot) as ParticleSystem;
        KillTimer(myUnlockSpark.gameObject, 2.0f);
    }
    public IEnumerator KillTimer(GameObject obj, float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(obj);
    }
}
