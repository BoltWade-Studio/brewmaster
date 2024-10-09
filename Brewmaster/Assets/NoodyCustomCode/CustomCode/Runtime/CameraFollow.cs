using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NOOD.NoodCamera
{
    public class CameraFollow : MonoBehaviorInstance<CameraFollow>
    {
        [SerializeField] float smoothTime = 2;
        [SerializeField] string targetTag = "Player";
        [SerializeField] Vector3 offset = new Vector3(0, 0, -10);
        [SerializeField] bool isFollow = true;
        Transform targetTransform;
        Vector3 targetPosition;
        public Transform TopBorder;

        private void LateUpdate()
        {
            if (isFollow) FollowPlayer();
        }

        void FollowPlayer()
        {
            GameObject target = GameObject.FindGameObjectWithTag(targetTag);
            if (!target)
            {
                return;
            }

            targetPosition = target.transform.position;

            if (TopBorder)
            {
                targetPosition = new Vector3(targetPosition.x, targetPosition.y,
                    Mathf.Min(targetPosition.z, TopBorder.position.z));
            }

            NOOD.NoodyCustomCode.LerpSmoothCameraFollow(this.gameObject, smoothTime, targetPosition, offset);
        }
    }
}
