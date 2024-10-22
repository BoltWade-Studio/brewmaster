using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Game
{
    [ExecuteInEditMode]
    public class Test : MonoBehaviour
    {
        public bool _isStart;

        void Update()
        {
            if (_isStart)
            {
                _isStart = false;
                ConvertToSocketJson();
            }
        }

        private void ConvertToSocketJson()
        {
            string jsonRaw = JsonConvert.SerializeObject(new string[] { "Normal string" });
            Debug.Log(jsonRaw);
        }
    }
}
