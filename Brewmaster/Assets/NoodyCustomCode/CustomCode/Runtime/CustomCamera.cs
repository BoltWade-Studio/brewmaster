#if UNITY_EDITOR
using NOOD.NoodCustomEditor;
#endif
using UnityEngine;

namespace NOOD.NoodCamera
{
    public class CustomCamera : MonoBehaviorInstance<CustomCamera>
    {
        #region Components
        #if UNITY_EDITOR
        [Header("Tool tip"), ShowOnly]
        [SerializeField] string TOOL_TIP = "Press T to test";
        #endif
        #endregion

        #region Stats
        [Header("Stats")]
        [SerializeField] float duration = 0.2f;
        [SerializeField] float magnitude = 0.02f;
        [SerializeField] float explodeMagnitude = 0.1f;
        [SerializeField] float smoothTime = 2;
        [SerializeField] string targetTag = "Player";
        Vector3 targetPosition;

        [SerializeField] Vector3 offset;
        [SerializeField] public Transform topBorder;

        [SerializeField] bool isFollow;
        [SerializeField] bool isShake;
        [SerializeField] bool isHeavyShake;
        #endregion

        public static CustomCamera InsCustomCamera;

        protected override void ChildAwake()
        {
            if(InsCustomCamera == null) InsCustomCamera = this;
        }

        private void Update()
        {
            if (isShake) Shake();
            if (isHeavyShake) HeaveShake();
            if(Input.GetKeyDown(KeyCode.T))
            {
                Shake();
	        }
        }

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

	        if (topBorder)
	        {
		        targetPosition = new Vector3(targetPosition.x, targetPosition.y,
			        Mathf.Min(targetPosition.z, topBorder.position.z));
	        }

            NOOD.NoodyCustomCode.LerpSmoothCameraFollow(Camera.main.gameObject, smoothTime, targetPosition,
		            offset);
            // this.transform.LookAt(targetTransform);
        }

        public void Shake(){
            StartCoroutine(NOOD.NoodyCustomCode.ObjectShake(this.gameObject, duration, magnitude));
            isShake = false;
        }

        public void HeaveShake(){
            StartCoroutine(NOOD.NoodyCustomCode.ObjectShake(this.gameObject, duration, explodeMagnitude));
            isHeavyShake = false;
        }
    }

}
