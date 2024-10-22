using System.Collections;
using System.Collections.Generic;
using NOOD;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class FlashingUI : MonoBehaviorInstance<FlashingUI>
    {
        [SerializeField] private float _flashingSpeed = 5f;
        private Image _flashingImage;
        
        void Start()
        {
            _flashingImage = GetComponent<Image>();
        }

        public void Flash(Color color)
        {
            _flashingImage.color = color;
            NoodyCustomCode.StartUpdater(this, () =>
            {
                Color color = _flashingImage.color;
                if(color.a > 0)
                {
                    color.a -= Time.deltaTime * _flashingSpeed;
                    _flashingImage.color = color;
                    return false;
                }
                else
                {
                    return true;
                }
            });
        }
        void Update()
        {
            if(Input.GetKeyDown(KeyCode.T))
            {
                Flash(Color.red);
            }
        }
    }
}
