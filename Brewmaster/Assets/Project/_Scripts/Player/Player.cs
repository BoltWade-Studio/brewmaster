using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using NOOD;
using NOOD.Sound;
using UnityEngine;
using Utils;

namespace Game
{
    public class Player : MonoBehaviorInstance<Player>
    {
        public static Action<int> OnPlayerChangePosition;
        public static Action OnPlayerChangePositionSuccess;
        private int _index = 0;
        private Vector3 standPosition = new Vector3(0, 0, 0);
        private bool toMove = false;
        private Vector2 _input;
        private bool _isServePressed;
        private bool _isPausePressed;

        private void Start()
        {
	        Utility.Socket.OnEvent("playerMove", this.gameObject.name, nameof(PlayerMove), PlayerMove);
        }

        private void Update()
        {
            GetInput();
            // if (_index != -1)
            // {
	           //  OnPlayerChangePosition?.Invoke(_index);
	           //  OnPlayerChangePositionSuccess?.Invoke();
            //
	           //  Vector3 standPosition = new Vector3(this.transform.position.x, 0, TableManager.Instance.GetPlayerTablePosition().z);
	           //  transform.position = standPosition;
	           //  _index = -1;
            // }

            if (toMove)
			{
				OnPlayerChangePositionSuccess?.Invoke();

				transform.position = new Vector3(this.transform.position.x, 0, standPosition.z);
				toMove = false;
			}
        }

        private void PlayerMove(string data)
        {
	        try
	        {
		        List<Vector3> playerPosition = JsonConvert.DeserializeObject<List<Vector3>>(data);
		        standPosition = playerPosition[0];
		        // Debug.Log("PlayerMove: " + standPosition);
		        toMove = true;
	        }
	        catch (Exception e)
	        {
		        Debug.LogError("An error occurred: " + e.Message);
		        throw;
	        }
		}

        private void Move()
        {
            if(_isServePressed)
            {
                // BeerServeManager.Instance.ServeBeer();
            }
            if(_isPausePressed)
            {
                GameplayManager.Instance.OnPausePressed?.Invoke();
            }

            Vector3 newPosition = new Vector3(this.transform.position.x, 0, TableManager.Instance.GetPlayerTablePosition().z);
            this.transform.position = newPosition;

        }

        private void GetInput()
        {
            _isServePressed = false;
            _isPausePressed = false;
            float y = 0;
            if(Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                y = 1;
            }
            if(Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                y = -1;
            }
            if(Input.GetKeyDown(KeyCode.Space))
            {
                // _isServePressed = true;
                string json = JsonConvert.SerializeObject(new ArrayWrapper { array = new string[] { Time.time.ToString() } });
                Utility.Socket.EmitEvent("serveBeer", json);
            }
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                _isPausePressed = true;
            }

            _input = new Vector2(0, y);
            if (y != 0)
            {
	            string json = JsonConvert.SerializeObject(new ArrayWrapper { array = new string[] { JsonUtility.ToJson(_input) } });
	            Utility.Socket.EmitEvent("playerMove", json);
            }
        }
    }
}
