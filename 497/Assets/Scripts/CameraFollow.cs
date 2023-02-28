using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public class CameraFollow : MonoBehaviour
    {
        public Transform target;
        public float smoothSpeed = 0.125f;
        public Vector3 locationOffset;
        //public Vector3 rotationOffset;

        //private void Start()
        //{
        //    if(target.gameObject.GetComponent<Player>().playerTeam == 0) //left
        //    {
        //        locationOffset = new Vector3(-locationOffset.x, locationOffset.y, locationOffset.z);
        //        transform.transform.eulerAngles = new Vector3(30, 90, 0);
        //        GetComponent<Camera>().rect = new Rect(0f, 0f, 0.5f, 1f);
        //    }
        //}

        void FixedUpdate()
        {
            Vector3 desiredPosition = target.position + locationOffset; // + target.rotation * locationOffset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;
        }

        public void setTarget(Transform playerTransform)
        {
            target = playerTransform;
        }
    }