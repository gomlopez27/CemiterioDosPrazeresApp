using Mapbox.Examples;
using Mapbox.Unity.Location;
using Mapbox.Unity.Map;
using Mapbox.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UserInRoute : MonoBehaviour
{
  
    [SerializeField]
    GameObject Player;
    [SerializeField]
    AbstractMap _map;

    private AbstractLocationProvider _locationProvider = null;
    private Vector2d latlong;
    private POIMapSpecifications[] poisInRoute;
    private bool hasInitializedPois;
    private void Awake()
    {
        _map = FindObjectOfType<AbstractMap>();

        if (_map == null)
        {
            Debug.LogError("Error: No Abstract Map component found in scene.");
            return;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        hasInitializedPois = false;

        if (null == _locationProvider)
        {
            _locationProvider = LocationProviderFactory.Instance.DefaultLocationProvider as AbstractLocationProvider;
        }
    }

    // Update is called once per frame
    void Update()
    {
        poisInRoute = FindObjectsOfType<POIMapSpecifications>(true);
  

        Location currLoc = _locationProvider.CurrentLocation;

		if (currLoc.IsLocationServiceInitializing)
		{
            Debug.Log("location services are initializing");

		}
		else
		{
			if (!currLoc.IsLocationServiceEnabled)
			{
                Debug.Log("location services not enabled");

			}
			else
			{
				if (currLoc.LatitudeLongitude.Equals(Vector2d.zero))
				{

					Debug.Log("Waiting for location ....");
				}
				else
				{

					latlong = currLoc.LatitudeLongitude;
                    UserReachedPoi();

                }
			}
		}

	}

    //Raio de 10 m
    void UserReachedPoi()
    {
        foreach (POIMapSpecifications poi in poisInRoute)
        {
            double poiLat = double.Parse(poi.latitude, System.Globalization.CultureInfo.InvariantCulture);
            double poiLong = double.Parse(poi.longitude, System.Globalization.CultureInfo.InvariantCulture);

            print(poiLat);
            if (GetDistance(latlong.x, latlong.y, poiLat, poiLong) < 0.01) { 
                print("REACHED POI");
                poi.transform.GetChild(0).GetComponent<MeshRenderer>().material.color = Color.green;
                

            }

        }
    }

    //calculate the distance between two coordinates, in KM
    double GetDistance(double lat1, double lon1, double lat2, double lon2)
    {
        var R = 6371; // Radius of the earth in km
        var dLat = ToRadians(lat2 - lat1);
        var dLon = ToRadians(lon2 - lon1);
        var a =
            Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
            Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
            Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        var d = R * c; // Distance in km
        return d;
    }

    double ToRadians(double deg)
    {
        return deg * (Math.PI / 180);
    }
}
