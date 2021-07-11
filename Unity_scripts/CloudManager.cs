using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudManager : MonoBehaviour
{
    public bool canMakeACloud = true;
    public GameObject prefabCloud;
    public Mesh[] cloudMeshes;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (canMakeACloud)
            StartCoroutine(MakeACloud());
    }
    public IEnumerator MakeACloud()
    {
        canMakeACloud = false;
        yield return new WaitForSeconds(5f+UnityEngine.Random.Range(0,5));
        Vector3 randomPlacement = new Vector3(-150,15,UnityEngine.Random.Range(-10,30));
        GameObject myCloud = Instantiate(prefabCloud, randomPlacement, Quaternion.identity) as GameObject;
        myCloud.transform.SetParent(this.transform);
        myCloud.GetComponent<MeshFilter>().mesh = PickRandomMesh();
        canMakeACloud = true;
        yield return null;
    }
    public Mesh PickRandomMesh()
    {
        int pos = UnityEngine.Random.Range(0,cloudMeshes.Length-1);
        return cloudMeshes[pos];
    }
}
