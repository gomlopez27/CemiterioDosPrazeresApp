using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestGPS : MonoBehaviour
{
    public Text coords;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        coords.text = "Lat: " + GPS.Instance.latitude.ToString() + "  Lon:" + GPS.Instance.longitude.ToString();
        Debug.Log("Lat: " + GPS.Instance.latitude.ToString() + "  Lon:" + GPS.Instance.longitude.ToString());
    }
}
