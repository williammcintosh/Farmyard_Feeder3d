using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class RewardedAdDisplayScript : MonoBehaviour, IUnityAdsListener
{
    public GameObject mainMenuObj;
    MenuManagerScript menuManagerScript;
    public string myGameIDApple = "4174060";
    public string myGameIDAndroid = "4174061";
    public string myVideoPlacement = "rewardedVideo";
    public string myAdStatus = "";
    public bool adStarted;
    public bool adComplete;
    public bool testMode = true;
    ShowOptions options = new ShowOptions();

    // Start is called before the first frame update
    void Start()
    {
        menuManagerScript = mainMenuObj.GetComponent<MenuManagerScript>();
        Advertisement.AddListener (this);
        Advertisement.Initialize(myGameIDApple, testMode);
    }

    public void ShowRewardedVideo() {
        // Check if UnityAds ready before calling Show method:
        if (Advertisement.IsReady(myVideoPlacement)) {
            Advertisement.Show(myVideoPlacement);
        }
        else {
            Debug.Log("Rewarded video is not ready at the moment! Please try again later!");
        }
    }

    // Implement IUnityAdsListener interface methods:
    public void OnUnityAdsDidFinish (string surfacingId, ShowResult showResult) {
        bool watchResult = false;
        // Define conditional logic for each ad completion status:
        if (showResult == ShowResult.Finished) {
            // Reward the user for watching the ad to completion.
            Debug.LogWarning ("You get five hearts.");
            watchResult = true;
        } else if (showResult == ShowResult.Skipped) {
            // Do not reward the user for skipping the ad.
            Debug.LogWarning ("No skipping! No rewards for you!");
        } else if (showResult == ShowResult.Failed) {
            Debug.LogWarning ("The ad did not finish due to an error.");
        }
        menuManagerScript.MakeRewardedAdsMessageDisappear(watchResult);
    }

    public void OnUnityAdsReady (string surfacingId) {
        // If the ready Ad Unit or legacy Placement is rewarded, show the ad:
        if (surfacingId == myVideoPlacement) {
            // Optional actions to take when theAd Unit or legacy Placement becomes ready (for example, enable the rewarded ads button)
        }
    }

    public void OnUnityAdsDidError (string message) {
        // Log the error.
    }

    public void OnUnityAdsDidStart (string surfacingId) {
        // Optional actions to take when the end-users triggers an ad.
    }

    // When the object that subscribes to ad events is destroyed, remove the listener:
    public void OnDestroy() {
        Advertisement.RemoveListener(this);
    }
}
