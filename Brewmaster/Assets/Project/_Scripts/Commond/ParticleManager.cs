using UnityEngine;
using NOOD.SerializableDictionary;
using System.Linq;
using System.Collections.Generic;
using Game.Extension;
using NOOD;
using UnityEditor;

namespace Game
{
    public enum ParticleType
    {
        Dust
    }

    public class ParticleManager : MonoBehaviorInstance<ParticleManager>
    {
        [SerializeField] private SerializableDictionary<ParticleType, ParticleSystem> _particleTypeDic = new SerializableDictionary<ParticleType, ParticleSystem>();
        private List<System.Collections.Generic.KeyValuePair<ParticleType, ParticleSystem>> _particleSystemPool = new List<System.Collections.Generic.KeyValuePair<ParticleType, ParticleSystem>>();

        public void PlayParticle(ParticleType particleType, Vector3 position)
        {
            ParticleSystem particleSystem = null;
            if(_particleSystemPool.Any(x => x.Key == particleType && x.Value.gameObject.activeInHierarchy == false))
            {
                // Has particle in pool list
                System.Collections.Generic.KeyValuePair<ParticleType, ParticleSystem> pair = _particleSystemPool.First(x => x.Key == particleType && x.Value.gameObject.activeInHierarchy == false);
                particleSystem = pair.Value;
            }
            else
            {
                // Don't have particle in pool list but has in dictionary
                if(_particleTypeDic.ContainsKey(particleType))
                {
                    // Create new particle and add to pool list
                    ParticleSystem ps = Instantiate<ParticleSystem>(_particleTypeDic.Dictionary[particleType]);
                    System.Collections.Generic.KeyValuePair<ParticleType, ParticleSystem> newPair = new System.Collections.Generic.KeyValuePair<ParticleType, ParticleSystem>(particleType, ps);

                    _particleSystemPool.Add(newPair);
                    particleSystem = ps;
                }
            }

            particleSystem.gameObject.SetActive(true);
            particleSystem.transform.position = position;
            particleSystem.Play();
        }
    }
}
