using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SA.iOS.AppTrackingTransparency;
using Balaso;
using System;


public class AppTrackingScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
# if UNITY_IOS
        //AppTrackingTransparency.OnAuthorizationRequestDone += OnAuthorizationRequestDone;
        AppTrackingTransparency.RequestTrackingAuthorization();
# endif
    }
}
