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
        private bool _isPausePressed;

        #region Unity functions
        private void Start()
        {
            Utility.Socket.SubscribeEvent("playerMove", this.gameObject.name, nameof(PlayerMove), PlayerMove);
        }

        private void Update()
        {
            GetInput();

            if (_isPausePressed)
            {
                GameplayManager.Instance.OnPausePressed?.Invoke();
            }

            if (toMove)
            {
                OnPlayerChangePositionSuccess?.Invoke();

                transform.position = new Vector3(this.transform.position.x, 0, standPosition.z);
                toMove = false;
            }
        }
        #endregion

        #region Event functions
        private void PlayerMove(string data)
        {
            object useData = JsonConvert.DeserializeObject<object[]>(data.ToString())[0];
            try
            {
                standPosition = JsonConvert.DeserializeObject<Vector3>(useData.ToString());
                Debug.Log("PlayerMove: " + standPosition);
                toMove = true;
            }
            catch (Exception e)
            {
                Debug.LogError("An error occurred: " + e.Message);
                throw;
            }
        }
        #endregion

        #region Private functions
        private void GetInput()
        {
            _isPausePressed = false;
            float y = 0;
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                y = 1;
            }
            if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                y = -1;
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                string json = JsonConvert.SerializeObject(new ArrayWrapper { array = new string[] { Time.time.ToString() } });
                Utility.Socket.EmitEvent("serveBeer", json);
            }
            if (Input.GetKeyDown(KeyCode.Escape))
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
        #endregion
    }
}
