using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DinoMovement : MonoBehaviour
{
    public Transform target;
    public float t;
    public float speed;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Lerp
        //Vector3 a = transform.position;
        //Vector3 b = target.position;
        //transform.position = Vector3.Lerp(a, b, t);

        //Move towards
        Vector3 a = transform.position;
        Vector3 b = target.position;
        transform.position = Vector3.MoveTowards(a, b, speed);
    }

}
