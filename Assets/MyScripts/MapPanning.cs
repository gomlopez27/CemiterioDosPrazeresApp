using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MapPanning : MonoBehaviour
{
    [SerializeField]
    public Camera _referenceCamera;
    //[SerializeField]
    //RouteDataHolder dataHolder;
    public float groundY = 0;
    Vector3 touchStart;
    private Vector3 initialCameraPosition;
    private Vector3 _mousePosition;

    void Awake()
    {
        if (null == _referenceCamera)
        {
            _referenceCamera = GetComponent<Camera>();
            if (null == _referenceCamera) { Debug.LogErrorFormat("{0}: reference camera not set", this.GetType().Name); }
        }
      
    }

    // Start is called before the first frame update
    void Start()
    {
        initialCameraPosition = _referenceCamera.transform.position;
        //dataHolder.InitialCameraTransform = _referenceCamera.transform;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject() /* && Input.touchSupported && Input.touchCount == 1*/)
        {

            touchStart = GetWorldPosition(groundY);
            //print("touchStart " + touchStart);

            //var mousePosScreen = Input.mousePosition;
            ////assign distance of camera to ground plane to z, otherwise ScreenToWorldPoint() will always return the position of the camera
            ////http://answers.unity3d.com/answers/599100/view.html
            //mousePosScreen.z = _referenceCamera.transform.localPosition.y;
            //_mousePosition = _referenceCamera.ScreenToWorldPoint(mousePosScreen);
            //print("_mousePosition: " + _mousePosition);


        }
        if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject() /*&& Input.touchSupported && Input.touchCount == 1*/)
        {
            Vector3 directionCamera = touchStart - GetWorldPosition(groundY);
            _referenceCamera.transform.position += directionCamera;

            //var mousePosScreen = Input.mousePosition;
            //mousePosScreen.z = _referenceCamera.transform.localPosition.y;

            //Vector3 directionCamera = _mousePosition - _referenceCamera.ScreenToWorldPoint(mousePosScreen);
            ////print(_referenceCamera.ScreenToWorldPoint(Input.mousePosition));
            ////print("directionCamera " + directionCamera);
            //print("directionCamera: " + directionCamera);

        }
    }

    //Returns a vector3 position that has the same z pos as the ground, which is basically the same
    //as using ScreenToWorldPoint on  ortographic camera
    private Vector3 GetWorldPosition(float y)
    {
        Ray mousePos = _referenceCamera.ScreenPointToRay(Input.mousePosition);
        Plane ground = new Plane(Vector3.up, new Vector3(0,y,0));
        float distance;
        ground.Raycast(mousePos, out distance);
        return mousePos.GetPoint(distance);
    }

    public void SetCameraInitialPosition()
    {
        _referenceCamera.transform.position = initialCameraPosition;
    }
}
