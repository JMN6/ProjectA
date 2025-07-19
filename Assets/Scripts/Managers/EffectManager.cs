using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoSingleton<EffectManager>
{
    [SerializeField] private List<ParticleSystem> particlePrefabs;

    private Dictionary<int, List<ParticleSystem>> particlePools;

    private void Awake()
    {
        particlePools = new(particlePrefabs.Count);
        for (int i = 0; i < particlePrefabs.Count; i++)
        {
            particlePools.Add(i, new List<ParticleSystem>(10));
        }
    }

    public void PlayParticle(int index, Vector3 position)
    {
        if (particlePrefabs.Count <= index) return;

        var particlePool = particlePools[index];

        foreach (var particle in particlePool)
        {
            if (particle.totalTime > 2f)
            {
                particle.Clear();
                particle.transform.position = position;
                particle.Play();
                return;
            }
        }

        ParticleSystem newParticle = Instantiate(particlePrefabs[index], transform);
        newParticle.transform.position = position;
        newParticle.Play();
        particlePool.Add(newParticle);
    }
}
