using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPanZoom : MonoBehaviour
{
    Vector3 touchStart;
    [SerializeField]
    public Camera _referenceCamera;
    [SerializeField]
    public float zoomOutMin = 20;
    [SerializeField]
    public float zoomOutMax = 150;

    private Vector3 initialCameraPosition;

    void Awake()
    {
        if (null == _referenceCamera)
        {
            _referenceCamera = GetComponent<Camera>();
            if (null == _referenceCamera) { Debug.LogErrorFormat("{0}: reference camera not set", this.GetType().Name); }
        }

    }

    void Start()
    {
        initialCameraPosition = _referenceCamera.transform.position;

    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            touchStart = _referenceCamera.ScreenToWorldPoint(Input.mousePosition);
        }
        if (Input.touchCount == 2)
        {

            // Store both touches.
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            // Find the position in the previous frame of each touch.
            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            // Find the magnitude of the vector (the distance) between the touches in each frame.
            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float currTouchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            // Find the difference in the distances between each frame.
            float zoomFactor = 0.1f * (currTouchDeltaMag - prevTouchDeltaMag);


            zoom(zoomFactor);
        }
        else if (Input.GetMouseButton(0))
        {
            Vector3 direction = touchStart - _referenceCamera.ScreenToWorldPoint(Input.mousePosition);
            _referenceCamera.transform.position += direction;
        }
    }

    void zoom(float increment)
    {
        _referenceCamera.orthographicSize = Mathf.Clamp(_referenceCamera.orthographicSize - increment, zoomOutMin, zoomOutMax);
    }

    public void SetCameraInitialPosition()
    {
        _referenceCamera.transform.position = initialCameraPosition;
    }
}
