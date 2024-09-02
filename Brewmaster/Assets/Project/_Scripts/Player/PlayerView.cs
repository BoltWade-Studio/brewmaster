using NOOD;
using NOOD.Sound;
using UnityEngine;

namespace Game
{
    public class PlayerView : MonoBehaviour
    {
        [SerializeField] private Animator _anim;
        [SerializeField] private ParticleSystem _dustPS;

        void Start()
        {
            Player.OnPlayerChangePositionSuccess += Move;
            BeerServeManager.OnPlayerPressDeliverBeer += DeliverBeer;
        }
        void OnDestroy()
        {
            NoodyCustomCode.UnSubscribeFromStatic(typeof(Player), this);
            NoodyCustomCode.UnSubscribeFromStatic(typeof(BeerServeManager), this);
        }

        private void Move()
        {
            _anim.SetTrigger("Move");
            ParticleManager.Instance.PlayParticle(ParticleType.Dust, this.transform.position);
            SoundManager.PlaySound(SoundEnum.Teleport);
        }
        private void DeliverBeer()
        {
            _anim.SetTrigger("DeliverBeer");
        }
    }
}
