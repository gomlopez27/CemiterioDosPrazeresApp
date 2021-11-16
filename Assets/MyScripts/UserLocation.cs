using Mapbox.Examples;
using Mapbox.Unity.Location;
using Mapbox.Unity.Map;
using Mapbox.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UserLocation : MonoBehaviour
{
	[SerializeField]
	public Camera _referenceCamera;
	[SerializeField]
	GameObject Player;
	[SerializeField]
	AbstractMap _map;
	[SerializeField]
	Text _statusText;

	Vector3 initialCameraPos;
	private AbstractLocationProvider _locationProvider = null;

	private Vector2d latlong;


	private void Awake()
	{
		_map = FindObjectOfType<AbstractMap>();

		if (_map == null)
		{
			Debug.LogError("Error: No Abstract Map component found in scene.");
			return;
		}
	}
	public void Start()
	{
		initialCameraPos = _referenceCamera.transform.position;

		_statusText.gameObject.SetActive(true);

		if (null == _locationProvider)
		{
			_locationProvider = LocationProviderFactory.Instance.DefaultLocationProvider as AbstractLocationProvider;
		}

		print("START USERLOCATION");
	}


	void Update()
	{
		Location currLoc = _locationProvider.CurrentLocation;
		if (currLoc.IsLocationServiceInitializing)
		{
			//_statusText.text = "location services are initializing";
			_statusText.text = "Serviços de localização estão a inicializar.";

		}
		else
		{
			if (!currLoc.IsLocationServiceEnabled)
			{
				//_statusText.text = "location services not enabled";
				_statusText.text = "Serviços de localização não estão ativos";

			}
			else
			{
				_statusText.text = "Serviços de localização ativos";

				if (currLoc.LatitudeLongitude.Equals(Vector2d.zero))
				{

					//_statusText.text = "Waiting for location ....";
					_statusText.text = "A carregar localização...";
				}
				else
				{
					//_statusText.text = string.Format("{0}", currLoc.LatitudeLongitude);
					_statusText.gameObject.SetActive(false);
					latlong = currLoc.LatitudeLongitude;
					//print("currLoc.IsLocationUpdated " + currLoc.IsLocationUpdated + " : " + latlong);
				}
			}
		}

	}


	

// Update is called once per frame
//void Update()
//{
//    //if (Input.GetMouseButtonDown(0))
//    //{
//    //    if (EventSystem.current.IsPointerOverGameObject())
//    //        return;
//    //    //this.gameObject.SetActive(false);
//    //    Player.GetComponent<ImmediatePositionWithLocationProvider>().enabled = false;
//    //    //Player.SetActive(false);

//    //}
//}


    public void CenterMapInUserLoc()
    {
		_referenceCamera.transform.position = new Vector3(Player.transform.position.x,
															 _referenceCamera.transform.position.y,
															 Player.transform.position.z);

		_map.SetCenterLatitudeLongitude(latlong);
		_map.UpdateMap(_map.CenterLatitudeLongitude, _map.InitialZoom);
		//      print("LATLONG: " + string.Format("{0}", _map.CenterLatitudeLongitude));
		//_referenceCamera.transform.position = initialCameraPos;
	}
}
