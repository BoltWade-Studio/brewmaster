using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Game
{
    public class LoadingUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _loadingText;

        public void SetLoadingText(string message)
        {
            _loadingText.text = message;
        }
    }
}
