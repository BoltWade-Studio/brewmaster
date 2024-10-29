using NOOD;
using TMPro;
using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

namespace Game
{
    public class TextPopup : MonoBehaviour
    {
        private static float _disappearSpeed = 2f;
        private static List<TextMeshPro> _textMeshProPool;
        private static List<TextMeshProUGUI> _textMeshProUGUIPool;
        private static GameObject _textPopupInit;

        private static void InitIfNeed()
        {
            if(_textPopupInit == null || _textPopupInit.gameObject == null )
            {
                _textPopupInit = new GameObject("TextPopUpInitGO");
                _textMeshProPool = new List<TextMeshPro>();
                _textMeshProUGUIPool = new List<TextMeshProUGUI>();
            }
        }

        public static void Show(string text, Vector3 position, Color color)
        {
            InitIfNeed();
            TextMeshPro textMeshPro;
            if(_textMeshProPool.Count == 0)
            {
                textMeshPro = Instantiate(Resources.Load<TextMeshPro>("TextPopup/TextPopup"), position, Quaternion.identity);
                textMeshPro.transform.SetParent(_textPopupInit.transform);
            }
            else
            {
                textMeshPro = _textMeshProPool[0];
                _textMeshProPool.RemoveAt(0);
                textMeshPro.gameObject.SetActive(true);
            }

            textMeshPro.text = text;
            textMeshPro.color = color;
            textMeshPro.transform.position = position;
            textMeshPro.transform.localScale = Vector3.zero;
            textMeshPro.transform.DOScale(1, 0.2f).SetEase(Ease.OutBounce);
            textMeshPro.transform.DOLocalMoveY(textMeshPro.transform.position.y + 1, 0.2f).SetEase(Ease.OutElastic);

            NoodyCustomCode.StartUpdater(textMeshPro, () =>
            {
                Color c = textMeshPro.color;
                if(color.a > 0)
                {
                    color.a -= Time.deltaTime * _disappearSpeed;
                    textMeshPro.color = c;
                    return false;
                }
                else
                {
                    textMeshPro.transform.DOScale(0, 0.2f).SetEase(Ease.InBounce);
                    textMeshPro.transform.DOLocalMoveY(textMeshPro.transform.position.y + 0.5f, 0.2f).SetEase(Ease.InElastic);
                    textMeshPro.gameObject.SetActive(false);
                    _textMeshProPool.Add(textMeshPro);
                    return true;
                }
            });
            ShowInUI(text, color);
        }
         
        private static void ShowInUI(string text, Color color)
        {
            InitIfNeed();
            TextMeshProUGUI textMeshProUGUI;
            if(_textMeshProUGUIPool.Count == 0)
            {
                GameObject textUI = Instantiate(Resources.Load<GameObject>("TextPopup/MoneyText"), null);
                textMeshProUGUI = textUI.GetComponentInChildren<TextMeshProUGUI>();
                textUI.transform.SetParent(_textPopupInit.transform);
            }
            else
            {
                textMeshProUGUI = _textMeshProUGUIPool[0];
                _textMeshProUGUIPool.RemoveAt(0);
                textMeshProUGUI.transform.parent.gameObject.SetActive(true);
            }


            textMeshProUGUI.text = text;
            textMeshProUGUI.color = color;

            NoodyCustomCode.StartUpdater(textMeshProUGUI, () =>
            {
                Color c = textMeshProUGUI.color;
                if(c.a > 0)
                {
                    c.a -= Time.deltaTime * _disappearSpeed;
                    textMeshProUGUI.color = c;
                    return false;
                }
                else
                {
                    textMeshProUGUI.transform.parent.gameObject.SetActive(false);
                    _textMeshProUGUIPool.Add(textMeshProUGUI);
                    return true;
                }
            });
        }
    }
}
