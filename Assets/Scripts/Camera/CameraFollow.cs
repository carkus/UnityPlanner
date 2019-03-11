using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraFollow : MonoBehaviour
{
    private Vector3 dest;
    private RaycastHit _hit;

    public Transform target;            // The position that that camera will be following.
    public float smoothing = 5f;        // The speed with which the camera will be following.

    /*public string floorTag = "floor";
    public string obstacleTag = "obstacle";
    public string doorTag = "door";
    public string alarmTag = "alarm";*/

    Vector3 offset;                     // The initial offset from the target.


    void Start ()
    {
        // Calculate the initial offset.
        offset = transform.position - target.position;
    }

    //float damping = 0.8f;

    /// <summary>
    /// LateUpdate is called every frame, if the Behaviour is enabled.
    /// It is called after all Update functions have been called.
    /// </summary>
    /*void LateUpdate()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out _hit, 100))
        {		

                float X = _hit.point.x;
                float Z = _hit.point.z;
                dest = new Vector3(X, transform.position.y*0.99f, Z);
                //MoveOrder(target);
                transform.position = Vector3.Slerp (transform.position, dest, Time.deltaTime * damping);
        }
        
    }*/

    /// <summary>
    /// This function is called every fixed framerate frame, if the MonoBehaviour is enabled.
    /// </summary>
    void FixedUpdate ()
    {
        // Create a postion the camera is aiming for based on the offset from the target.
        //Vector3 targetCamPos = target.position + offset;

        // Smoothly interpolate between the camera's current position and it's target position.
        //transform.position = Vector3.Lerp (Input.mousePosition.x, transform.position.y, targetCamPos, smoothing * Time.deltaTime);
        //transform.position = Vector3.Lerp (transform.position, targetCamPos, smoothing * Time.deltaTime);

    }
    
}
