using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class BeerCup : MonoBehaviour
    {
        private string PUNISH_TAG = "PunishCollider";

        void Awake()
        {
            GameplayManager.Instance.OnEndDay += OnEndDayHandler;
        }
        private void OnTriggerEnter(Collider other)
        {
            if(other.gameObject.CompareTag(PUNISH_TAG))
            {
                Debug.Log("Collider");
                Destroy(this.gameObject);
                BeerServeManager.Instance.OnServerFail?.Invoke(this.transform.position);            
            }
        }
        void OnDisable()
        {
            GameplayManager.Instance.OnEndDay -= OnEndDayHandler;
        }

        private void OnEndDayHandler()
        {
            Destroy(this.gameObject);
        }
    }
}
