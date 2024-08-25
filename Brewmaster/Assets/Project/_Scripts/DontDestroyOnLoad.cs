using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class DontDestroyOnLoad : MonoBehaviour
    {
        void Awake()
        {
            this.transform.SetParent(null);
            DontDestroyOnLoad(this.gameObject);
        }
    }
}
