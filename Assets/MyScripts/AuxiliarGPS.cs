using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AuxiliarGPS : MonoBehaviour
{
    public static AuxiliarGPS Instance { set; get; }

    private void Start()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    //calculate the distance between two coordinates, in KM
    public double GetDistance(double lat1, double lon1, double lat2, double lon2)
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
