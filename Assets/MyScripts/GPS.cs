using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;


public class GPS : MonoBehaviour
{
    public static GPS Instance { set; get; }
    public float latitude;
    public float longitude;

    void Start()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
        StartCoroutine(StartLocationService());

    }

    private IEnumerator StartLocationService()
    {
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Permission.RequestUserPermission(Permission.FineLocation);
            Permission.RequestUserPermission(Permission.CoarseLocation);
        }


        yield return new WaitForSeconds(2);
        // First, check if user has location service enabled
        if (!Input.location.isEnabledByUser)
        {
            Debug.Log("User has not enabled GPS");
            //yield return new WaitForSeconds(3);
            yield break;
        }


        // Start service before querying location
        Input.location.Start();

        yield return new WaitForSeconds(2);

        // Wait until service initializes
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        // Service didn't initialize in 20 seconds
        if (maxWait < 1)
        {
            Debug.Log("Timed out");
            yield break;
        }

        // Connection has failed
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.Log("Unable to determine device location");
            yield break;
        }
        else
        {
            latitude = Input.location.lastData.latitude;
            longitude = Input.location.lastData.longitude;
            // Access granted and location value could be retrieved
            Debug.Log("MY Latitude : " + latitude);
            Debug.Log("MY Longitude : " + longitude);
        }

        // Stop service if there is no need to query location updates continuously
        Input.location.Stop();

    }
}
