using NOOD.SerializableDictionary;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public enum PlayerRank
    {
        Gold,
        Silver,
        Bronze,
    }

    public class PlayerRowUI : MonoBehaviour
    {
        [SerializeField] private Sprite[] _playerRankIcons;
        [SerializeField] private Image _playerIcon;
        [SerializeField] private Image _playerRankIcon;
        [SerializeField] private TextMeshProUGUI _playerRank;
        [SerializeField] private TextMeshProUGUI _playerAddress;
        [SerializeField] private TextMeshProUGUI _playerScore;

        public void SetPlayerRank(int rank)
        {
            if (rank < _playerRankIcons.Length && rank != -1)
                _playerRankIcon.sprite = _playerRankIcons[rank];
            else
                _playerRankIcon.gameObject.SetActive(false);

            if (rank == -1)
            {
                _playerRank.text = "?.";
            }
            else
            {
                _playerRank.text = (rank + 1).ToString() + ".";
            }
        }

        public void SetPlayerAddress(string address)
        {
            _playerAddress.text = address;
        }

        public void SetPlayerScore(string score)
        {
            _playerScore.text = score;
        }
    }
}
