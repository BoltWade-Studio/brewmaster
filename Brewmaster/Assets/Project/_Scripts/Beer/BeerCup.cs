using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class BeerCup : MonoBehaviour
    {
        // private string PUNISH_TAG = "PunishCollider";
        public int id;
        private bool toMove = false;
        private bool collided = false;
        private BeerDto _beerDto;
        private Customer _customer;
        private Vector3 newPosition;

        void Awake()
        {
            GameplayManager.Instance.OnEndDay += OnEndDayHandler;
        }

        private void Update()
        {
	        if (toMove)
			{
				this.transform.position = newPosition;
				toMove = false;
			}

	        if (collided)
	        {
		        Destroy(this.gameObject);
		        if (_beerDto.punished)
		        {
			        Debug.Log("Collider");
			        BeerServeManager.Instance.OnServerFail?.Invoke(this.transform.position);
		        }
		        else
		        {
			        _customer.Complete(_beerDto.earnedMoney);

			        if (_customer.HaveCoin())
			        {
			        	// Trigger event to get reward coin
			        	Utility.Socket.EmitEvent("coinCollect");
			        	// if (Application.isEditor)
			        	// 	SocketConnectManager.Instance.EmitEvent("coinCollect");
			        	// else
			        	// 	JsSocketConnect.EmitEvent("coinCollect");

			        	_customer.CollectCoin();
			        }

		        }
	        }
        }

        public void MoveTo(Vector3 position)
		{
			newPosition = position;
			toMove = true;
		}

		public void Collide(BeerDto beerDto, Customer customer)
		{
			collided = true;
			_beerDto = beerDto;
			_customer = customer;
		}

        // private void OnTriggerEnter(Collider other)
        // {
        //     if(other.gameObject.CompareTag(PUNISH_TAG))
        //     {
        //         Debug.Log("Collider");
        //         Destroy(this.gameObject);
        //         BeerServeManager.Instance.OnServerFail?.Invoke(this.transform.position);
        //     }
        // }
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
