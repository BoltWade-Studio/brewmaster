using UnityEngine;

namespace Game
{
	public class CoinSpawner : MonoBehaviour
	{
		[SerializeField] private GameObject _coinPref;
		private bool _isSpawnCoin;

		void Start()
		{
			// CustomerSpawner.Instance.onCustomerSpawn += CustomerSpawner_OnCustomerSpawn;
			// Utility.Socket.OnEvent("spawnCoin", this.gameObject.name, nameof(SpawnCoin), SpawnCoin);
		}

		private void SpawnCoin(string data)
		{
			Debug.Log("spawnCoin");
			_isSpawnCoin = true;
		}

		private void CustomerSpawner_OnCustomerSpawn(Customer customer)
		{
			if (_isSpawnCoin)
			{
				GameObject coin = Instantiate(_coinPref, customer.transform);
				coin.transform.localPosition = new Vector3(0, 2.7f, 0);
				customer.SetCoin(coin.transform);
				_isSpawnCoin = false;
			}
		}
	}
}
