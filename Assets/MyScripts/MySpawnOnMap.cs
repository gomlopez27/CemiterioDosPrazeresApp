
using UnityEngine;
using Mapbox.Utils;
using Mapbox.Unity.Map;
using Mapbox.Unity.MeshGeneration.Factories;
using Mapbox.Unity.Utilities;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine.UI;

public class MySpawnOnMap : MonoBehaviour
{
	[SerializeField]
	AbstractMap _map;

	
	Vector2d[] _locations;

	[SerializeField]
	float _spawnScale = 100f;

	[SerializeField]
	GameObject _markerPrefab;

	List<GameObject> _spawnedObjects;

	public GameObject Directions;

	//private JSONNode PoisInMap;

	private void Awake()
	{
		//TextAsset json = Resources.Load<TextAsset>("MapPopularPOI");
		//PoisInMap = JSON.Parse(json.ToString());
		//POIPhotosArray = new Sprite[PoiListData["pois"].Count];
		//LoadPOIPhotos();
	}

	void Start()
	{
		//_locations = new Vector2d[_locationStrings.Length];
		//_spawnedObjects = new List<GameObject>();
		//for (int i = 0; i < _locationStrings.Length; i++)
		//{
		//	var locationString = _locationStrings[i];
		//	_locations[i] = Conversions.StringToLatLon(locationString);
		//	var instance = Instantiate(_markerPrefab);
		//	instance.transform.localPosition = _map.GeoToWorldPosition(_locations[i], true);
		//	//instance.transform.localScale = new Vector3(_spawnScale, _spawnScale, _spawnScale);
		//	_spawnedObjects.Add(instance);
		//}

		//print("COUNT:" + PoisInMap["pois"].Count);
		print("COUNT:" + MainDataHolder.PopularPois.Count);

		_locations = new Vector2d[MainDataHolder.PopularPois.Count];
		_spawnedObjects = new List<GameObject>();
		for (int i = 0; i < MainDataHolder.PopularPois.Count; i++)
		{
			string poiId = MainDataHolder.PopularPois[i].Id;
			string lat = MainDataHolder.PopularPois[i].Latitude;
			string lng = MainDataHolder.PopularPois[i].Longitude;
			string location = lat + "," + lng;
			_locations[i] = Conversions.StringToLatLon(location);

			GameObject POILocation = new GameObject("Poi-" + poiId);
			POIMapSpecifications specifications = POILocation.AddComponent<POIMapSpecifications>();
			specifications.SetVariables(poiId, lat, lng);

			GameObject thisPOI = Instantiate(_markerPrefab, POILocation.transform);


			//LoadInfoOnClick(i);
			//var instance = Instantiate(_markerPrefab);

			thisPOI.transform.localPosition = _map.GeoToWorldPosition(_locations[i], true);
			thisPOI.transform.localPosition = new Vector3(thisPOI.transform.localPosition.x, 1,
				thisPOI.transform.localPosition.z);

			thisPOI.transform.localScale = new Vector3(_spawnScale, _spawnScale, _spawnScale);
			_spawnedObjects.Add(thisPOI);

			//Player.transform.localPosition = new Vector3(Player.transform.localPosition.x, 1,
			//	Player.transform.localPosition.z);
		}
	}

	private void Update()
	{

		int count = _spawnedObjects.Count;
	

		for (int i = 0; i < count; i++)
		{
			var spawnedObject = _spawnedObjects[i];
			
			var location = _locations[i];
			spawnedObject.transform.localPosition = _map.GeoToWorldPosition(location, true);
			spawnedObject.transform.localPosition = new Vector3(spawnedObject.transform.localPosition.x, 1,
				spawnedObject.transform.localPosition.z);
			spawnedObject.transform.localScale = new Vector3(_spawnScale, _spawnScale, _spawnScale);
		}
	}

	public List<GameObject> GetSpawnedPois()
    {
		return _spawnedObjects;
    }
}
