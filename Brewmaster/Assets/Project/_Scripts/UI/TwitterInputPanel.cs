using Cysharp.Threading.Tasks;
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
		[SerializeField] private CustomButton _cancelButton;

		[Header("Input Link")]
		[SerializeField] private CustomButton _confirmButton;
		[SerializeField] private TMP_InputField _twitterInputField;
		[SerializeField] private TextMeshProUGUI _messageError;

		[Header("Success Message")]
		[SerializeField] private TextMeshProUGUI _title;
		[SerializeField] private TextMeshProUGUI _SuccessMessage;

		void Awake()
		{
			_confirmButton.OnClick += OnConfirmBtnClick;
			_cancelButton.OnClick += OnCancelBtnClick;
		}
		void OnEnable()
		{
			ReturnToNormal();
		}
		void OnDestroy()
		{
			_confirmButton.OnClick -= OnConfirmBtnClick;
			_cancelButton.OnClick -= OnCancelBtnClick;
		}

		#region Button Events
		private void OnConfirmBtnClick()
		{
			TwitterShareManager.Instance.ConfirmTwitterInput(_twitterInputField.text, OnTwitterCallbackSuccess, OnTwitterCallbackException);
		}

		private void OnTwitterCallbackSuccess(string giftTxHash)
		{
			_title.text = "Congratulations!";
			_confirmButton.gameObject.SetActive(false);
			_twitterInputField.gameObject.SetActive(false);
			_messageError.gameObject.SetActive(false);
			_SuccessMessage.gameObject.SetActive(true);
		}

		private void OnTwitterCallbackException(string exception)
		{
			_messageError.gameObject.SetActive(true);
			_messageError.text = exception.ToString();
			Debug.Log("exception: " + exception.ToString());
		}
		private void ReturnToNormal()
		{
			_title.text = "Input your post link";
			_confirmButton.gameObject.SetActive(true);
			_twitterInputField.gameObject.SetActive(true);
			_SuccessMessage.gameObject.SetActive(false);
			_messageError.gameObject.SetActive(false);
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
