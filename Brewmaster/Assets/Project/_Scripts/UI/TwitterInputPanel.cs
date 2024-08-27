using DG.Tweening;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using Utils;

namespace Game
{
	public class TwitterInputPanel : MonoBehaviour
	{
		[SerializeField] private GameObject _panel;
		[SerializeField] private CustomButton _confirmButton;
		[SerializeField] private CustomButton _cancelButton;
		[SerializeField] private TMP_InputField _twitterInputField;

		void Awake()
		{
			_confirmButton.OnClick += OnConfirmBtnClick;
			_cancelButton.OnClick += OnCancelBtnClick;
		}

		#region Button Events
		private void OnConfirmBtnClick()
		{
			string json = JsonConvert.SerializeObject(new ArrayWrapper { array = new string[] { _twitterInputField.text } });
			Utility.Socket.EmitEvent(SocketEnum.playerInputLink.ToString(), json);
		}
		private void OnCancelBtnClick()
		{
			Hide();
		}
		#endregion

		#region Show Hide
		public void Show()
		{
			this.gameObject.SetActive(true);
			_panel.transform.DOScale(1, 0.3f).SetEase(Ease.Flash);
		}
		public void Hide()
		{
			_panel.transform.DOScale(0, 0.5f).SetEase(Ease.InQuad).OnComplete(() =>
			{
				this.gameObject.SetActive(false);
			});
		}
		#endregion
	}
}
