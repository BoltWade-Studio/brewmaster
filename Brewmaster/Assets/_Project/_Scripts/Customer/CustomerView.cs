using System.Collections;
using System.Collections.Generic;
using Game.Extension;
using NOOD;
using UnityEngine;

namespace Game
{
    public class CustomerView : MonoBehaviour
    {
        [SerializeField] private bool _isUseSkinGO = false;

        [Header("Component")]
        [SerializeField] private Customer _customer;

        [Header("Customize List")]
        [SerializeField] private List<SkinnedMeshRenderer> _head = new List<SkinnedMeshRenderer>();
        [SerializeField] private List<SkinnedMeshRenderer> _chest = new List<SkinnedMeshRenderer>();
        [SerializeField] private List<SkinnedMeshRenderer> _pant = new List<SkinnedMeshRenderer>();
        [SerializeField] private List<SkinnedMeshRenderer> _feet = new List<SkinnedMeshRenderer>();

        [Header("Customer skin")]
        [SerializeField] private List<GameObject> _customerSkins = new List<GameObject>();

        [Header("Animator")]
        [SerializeField] private Animator _anim;

        private float _sitDownSpeed = 2;
        private float _sitStage = 0;
        private UpdateObject _sitDownUpdater, _moveUpdater;

        void Awake()
        {
            _customerSkins.ForEach(skin => skin.SetActive(false));
        }

        void Start()
        {
            GetRandomCloth();
            _customer.OnCustomerEnterSeat += SitDown;
            _customer.OnCustomerReturn += Move;
            TimeManager.OnTimePause += PauseAnimation;
            TimeManager.OnTimeResume += ResumeAnimation;
        }
        void OnDestroy()
        {
            _customer.OnCustomerEnterSeat -= SitDown;
            _customer.OnCustomerReturn -= Move;
            TimeManager.OnTimePause -= PauseAnimation;
            TimeManager.OnTimeResume -= ResumeAnimation;
        }

        private void GetRandomCloth()
        {
            if (_isUseSkinGO)
            {
                GameObject skin = _customerSkins.GetRandom();
                skin.SetActive(true);
                _anim = skin.GetComponent<Animator>();
            }
            else
            {
                _head.ForEach(renderer => renderer.enabled = false);
                _chest.ForEach(renderer => renderer.enabled = false);
                _pant.ForEach(renderer => renderer.enabled = false);
                _feet.ForEach(renderer => renderer.enabled = false);

                _head.GetRandom().enabled = true;
                _chest.GetRandom().enabled = true;
                _pant.GetRandom().enabled = true;
                _feet.GetRandom().enabled = true;
            }
        }

        public void StopAllAnimation()
        {
            if (_moveUpdater) _moveUpdater.Stop();
            if (_sitDownUpdater) _sitDownUpdater.Stop();
        }
        public void PauseAnimation()
        {
            _anim.speed = 0;
        }
        public void ResumeAnimation()
        {
            _anim.speed = 1;
        }

        private void SitDown()
        {
            if (_moveUpdater)
            {
                _moveUpdater.Stop();
            }
            _sitDownUpdater = NoodyCustomCode.StartUpdater(_anim, () =>
            {
                if (_sitStage < 1)
                {
                    _sitStage += Time.deltaTime * _sitDownSpeed;
                    _anim.SetFloat("SitStage", _sitStage);
                    return false;
                }
                else
                    return true;
            }, "SitDown");
        }
        private void Move()
        {
            if (_sitDownUpdater)
            {
                _sitDownUpdater.Stop();
            }
            _moveUpdater = NoodyCustomCode.StartUpdater(_anim, () =>
            {
                if (_sitStage > 0)
                {
                    _sitStage -= Time.deltaTime * _sitDownSpeed;
                    _anim.SetFloat("SitStage", _sitStage);
                    return false;
                }
                else
                    return true;
            }, "Move");
        }
    }
}
