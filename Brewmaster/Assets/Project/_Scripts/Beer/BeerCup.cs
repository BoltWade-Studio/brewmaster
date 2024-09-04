using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class BeerCup : MonoBehaviour
    {
        public int id;
        private bool toMove = false;
        private bool collided = false;
        private BeerDto _beerDto;
        private Customer _customer;
        private Vector3 newPosition;

        private void Update()
        {
	        if (toMove)
			{
				this.transform.position = newPosition;
				toMove = false;
			}

	        if (collided)
	        {
		        if (_beerDto.punished)
		        {
			        Debug.Log("Collider");
			        BeerServeManager.Instance.OnServerFail?.Invoke(this.transform.position, _beerDto.earnedMoney);
		        }
		        else
		        {	if (_customer == null)
			        {
				        Debug.LogError("Customer not found with id: " + _beerDto.customerId);
				        // return;
			        }
			        _customer.Complete(_beerDto.earnedMoney);
		        }
		        Destroy(this.gameObject);
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
    }
}
