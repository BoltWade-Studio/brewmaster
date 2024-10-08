#if UNITY_EDITOR
using NOOD.NoodCustomEditor;
#endif
using UnityEngine;

namespace NOOD.NoodCamera
{
    public class CustomCamera : MonoBehaviorInstance<CustomCamera>
    {
        #region Stats
        [Header("Stats")]
        [SerializeField] float duration = 0.2f;
        [SerializeField] float magnitude = 0.02f;
        [SerializeField] float explodeMagnitude = 0.1f;
        [SerializeField] float smoothTime = 2;


        [SerializeField] bool isShake;
        [SerializeField] bool isHeavyShake;
        #endregion

        public static CustomCamera InsCustomCamera;

        protected override void ChildAwake()
        {
            if (InsCustomCamera == null) InsCustomCamera = this;
        }

        private void Update()
        {
            if (isShake) Shake();
            if (isHeavyShake) HeaveShake();
            if (Input.GetKeyDown(KeyCode.T))
            {
                Shake();
            }
        }

        public void Shake()
        {
            StartCoroutine(NOOD.NoodyCustomCode.ObjectShake(this.gameObject, duration, magnitude));
            isShake = false;
        }

        public void HeaveShake()
        {
            StartCoroutine(NOOD.NoodyCustomCode.ObjectShake(this.gameObject, duration, explodeMagnitude));
            isHeavyShake = false;
        }
    }

}
