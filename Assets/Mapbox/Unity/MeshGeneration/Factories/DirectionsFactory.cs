namespace Mapbox.Unity.MeshGeneration.Factories
{
	using UnityEngine;
	using Mapbox.Directions;
	using System.Collections.Generic;
	using System.Linq;
	using Mapbox.Unity.Map;
	using Data;
	using Modifiers;
	using Mapbox.Utils;
	using Mapbox.Unity.Utilities;
	using System.Collections;

	public class DirectionsFactory : MonoBehaviour
	{
		[SerializeField]
		AbstractMap _map;

		[SerializeField]
		MeshModifier[] MeshModifiers;
		[SerializeField]
		Material _material;

		[SerializeField]
		public Transform[] _waypoints;
		private List<Vector3> _cachedWaypoints;

		[SerializeField]
		[Range(1,10)]
		private float UpdateFrequency = 2;



		private Directions _directions;
		private int _counter;

		GameObject _directionsGO;
		private bool _recalculateNext;

		double routeDistance;
		double routeDuration;
		double distanceToNextPoi;
		double durationToNextPoi;
		string instruction;

		DirectionsResponse dirResponse;
		
		protected virtual void Awake()
		{
			if (_map == null)
			{
				_map = FindObjectOfType<AbstractMap>();
			}
			_directions = MapboxAccess.Instance.Directions;
			_map.OnInitialized += Query;
			//_map.OnUpdated += Query;


		}

		public void Start()
		{
			dirResponse = null;
			_cachedWaypoints = new List<Vector3>(_waypoints.Length);
			foreach (var item in _waypoints)
			{
				_cachedWaypoints.Add(item.position);
			}
			//print("_cachedWaypoints " + _cachedWaypoints.Count);

			_recalculateNext = false;

			foreach (var modifier in MeshModifiers)
			{
				modifier.Initialize();
			}

			StartCoroutine(QueryTimer());

			//print("ENTREI Start");

		}

		protected virtual void OnDestroy()
		{
			_map.OnInitialized -= Query;
			_map.OnUpdated -= Query;
		}
	
		public void Query()
		{
			var count = _waypoints.Length;
			var wp = new Vector2d[count];
			for (int i = 0; i < count; i++)
			{
				wp[i] = _waypoints[i].GetGeoPosition(_map.CenterMercator, _map.WorldRelativeScale);
			}
			var _directionResource = new DirectionResource(wp, RoutingProfile.Walking);
			_directionResource.Steps = true;
			_directions.Query(_directionResource, HandleDirectionsResponse);

			//print("ENTREI Query");

		}

		public IEnumerator QueryTimer()
		{
			Query();

			while (true)
			{
				yield return new WaitForSeconds(UpdateFrequency);
				for (int i = 0; i < _waypoints.Length; i++)
				{
					if (_waypoints[i].position != _cachedWaypoints[i])
					{
						_recalculateNext = true;
						_cachedWaypoints[i] = _waypoints[i].position;
						
					}
				}

				if (_recalculateNext)
				{
					Query();
					_recalculateNext = false;
				}
			}
		}

		void HandleDirectionsResponse(DirectionsResponse response)
		{
			if (response == null || null == response.Routes || response.Routes.Count < 1)
			{
				return;
			}

			//print(response.Code);
			dirResponse = response;

			routeDistance = response.Routes[0].Distance;
			routeDuration = response.Routes[0].Duration;
			//distanceToNextPoi = response.Routes[0].Legs[0].Distance;
			//durationToNextPoi = response.Routes[0].Legs[0].Duration;
			//instruction = response.Routes[0].Legs[0].Steps[0].Maneuver.Instruction;
			//print("Distance: " + response.Routes[0].Legs[0].Distance);
   //         print("Instruction: " + response.Routes[0].Legs[0].Steps[0].Maneuver.Instruction);


            //for (int i = 0; i < response.Routes[0].Legs.Count; i++)
            //{
            //    print(response.Routes[0].Legs[i].Summary + " -> Distance: " + response.Routes[0].Legs[i].Distance);
            //    for (int j = 0; j < response.Routes[0].Legs[i].Steps.Count; j++)
            //    {
            //        print("Legs[" + i + "] - Step[" + j + "] " +
            //            response.Routes[0].Legs[i].Steps[j].Maneuver.Instruction);
            //    }
            //}
            //print(routeDistance);
            var meshData = new MeshData();
			var dat = new List<Vector3>();
			foreach (var point in response.Routes[0].Geometry)
			{
				dat.Add(Conversions.GeoToWorldPosition(point.x, point.y, _map.CenterMercator, _map.WorldRelativeScale).ToVector3xz());
			}

			var feat = new VectorFeatureUnity();
			feat.Points.Add(dat);

			foreach (MeshModifier mod in MeshModifiers.Where(x => x.Active))
			{
				mod.Run(feat, meshData, _map.WorldRelativeScale);
			}

			CreateGameObject(meshData);
		}

		GameObject CreateGameObject(MeshData data)
		{
			if (_directionsGO != null)
			{
				_directionsGO.Destroy();
			}
			_directionsGO = new GameObject("direction waypoint " + " entity");
			var mesh = _directionsGO.AddComponent<MeshFilter>().mesh;
			mesh.subMeshCount = data.Triangles.Count;

			mesh.SetVertices(data.Vertices);
			_counter = data.Triangles.Count;
			for (int i = 0; i < _counter; i++)
			{
				var triangle = data.Triangles[i];
				mesh.SetTriangles(triangle, i);
			}

			_counter = data.UV.Count;
			for (int i = 0; i < _counter; i++)
			{
				var uv = data.UV[i];
				mesh.SetUVs(i, uv);
			}

			mesh.RecalculateNormals();
			_directionsGO.AddComponent<MeshRenderer>().material = _material;
			return _directionsGO;
		}

		public void CancelDirections()
		{
			_recalculateNext = false;

			if (_directionsGO != null)
			{
				_directionsGO.Destroy();
				
			}
		}

		public void PauseDirections()
        {
			_recalculateNext = false;
		}

		public void ResumeDirections()
		{
			_recalculateNext = true;
		}
		public DirectionsResponse GetDirectionsResponse()
        {
			return dirResponse;

		}

		public double GetRouteDistance()
        {
			return routeDistance;
        }

		public double GetRouteDuration()
		{
			return routeDuration;
		}

		public void SetWaypoints(Transform[] newWaypoints)
        {
		
			_waypoints = new Transform[newWaypoints.Length];
			_waypoints = newWaypoints;

			//for(int i = 0; i < _waypoints.Length; i++)
   //         {
			//	_waypoints[i] = newWaypoints[i];

			//}

		}
	}

	

}
