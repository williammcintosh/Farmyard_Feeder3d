using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrateScript : MonoBehaviour
{
    AudioSource myAudioSource;
    public AudioClip [] pickupSounds;
    public GameObject myPrefabFood;
    public void Start()
    {
        myAudioSource = gameObject.AddComponent<AudioSource>();
        myAudioSource.volume = 0.5f;
    }
    public void PlaySounds()
    {
        StartCoroutine(PlayMySounds());
    }
    public IEnumerator PlayMySounds()
    {
        myAudioSource.clip = pickupSounds[0];
        myAudioSource.Play();
        yield return new WaitForSeconds(myAudioSource.clip.length);
        myAudioSource.clip = pickupSounds[1];
        myAudioSource.Play();
        yield return new WaitForSeconds(myAudioSource.clip.length);
    }
}
