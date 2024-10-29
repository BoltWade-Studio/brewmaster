using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class FaceToCamera : MonoBehaviour
    {
        Camera _camera;

        void Start()
        {
            _camera = Camera.main;
        }

        // Update is called once per frame
        void Update()
        {
            transform.LookAt(transform.position + _camera.transform.rotation * Vector3.forward, _camera.transform.rotation * Vector3.up);
        }
    }

}
