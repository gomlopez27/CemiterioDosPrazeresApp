namespace Mapbox.Unity.Map
{
	using System.Collections;
	using Mapbox.Unity.Location;
    using Mapbox.Unity.Utilities;
    using Mapbox.Utils;
    using UnityEngine;

	public class InitializeMapWithLocationProvider : MonoBehaviour
	{
		[SerializeField]
		AbstractMap _map;
		[SerializeField]
		string InitialLatitude;
		[SerializeField]
		string InitialLongitude;
		[SerializeField]
		int InitialMapZoom;
		ILocationProvider _locationProvider;
		Vector2d InitialMapLocation;

		private void Awake()
		{
			// Prevent double initialization of the map. 			

			_map.InitializeOnStart = false;
			InitialMapLocation = Conversions.StringToLatLon(InitialLatitude+","+ InitialLongitude);

		}

		protected virtual IEnumerator Start()
		{
			yield return null;
			_locationProvider = LocationProviderFactory.Instance.DefaultLocationProvider;
			_locationProvider.OnLocationUpdated += LocationProvider_OnLocationUpdated;
			
		}

		void LocationProvider_OnLocationUpdated(Unity.Location.Location location)
		{
			_locationProvider.OnLocationUpdated -= LocationProvider_OnLocationUpdated;
			_map.Initialize(InitialMapLocation, InitialMapZoom);
		}

		public Vector2d GetInitialMapLocation()
        {
			return InitialMapLocation;
        }		
		public int GetInitialMapZoom()
        {
			return InitialMapZoom;
        }
	}
}
