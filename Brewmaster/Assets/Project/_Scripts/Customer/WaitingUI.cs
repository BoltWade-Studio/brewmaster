using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class WaitingUI : MonoBehaviour
    {
        [SerializeField] private Sprite _happy, _neutral, _sad;
        [SerializeField] private Image _iconImage, _iconBG;
        [SerializeField] private Color _happyColor, _neutralColor, _sadColor;

        public void UpdateWaitingUI(float progress)
        {
            if(progress < 0.3f)
            {
                // happy
                _iconImage.sprite = _happy;
                _iconBG.color = Color.Lerp(_happyColor, _neutralColor, progress / 0.3f);
            }
            if(progress > 0.3f && progress < 0.6f)
            {
                // neutral
                _iconImage.sprite = _neutral;
                _iconBG.color = Color.Lerp(_neutralColor, _sadColor, progress / 0.6f);
            }
            if(progress > 0.6f && progress < 1f)
            {
                // sad
                _iconImage.sprite = _sad;
                _iconBG.color = _sadColor;
            }
        }
    }
}
