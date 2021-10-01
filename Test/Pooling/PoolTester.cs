using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolTester : MonoBehaviour
{
	[SerializeField]
	private ParticleSystemPoolSO _pool = default;

	private void Start()
	{
		_pool.Initialize(100);
		List<ParticleSystem> particles = new List<ParticleSystem>();
		for(int i =0; i< 10; i ++)
        {
			particles.Add(_pool.Spawn());
		}

		foreach (ParticleSystem particle in particles)
		{
			StartCoroutine(DoParticleBehaviour(particle));
		}
	}

	private IEnumerator DoParticleBehaviour(ParticleSystem particle)
	{
		particle.transform.position = Random.insideUnitSphere * 5f;
		particle.Play();
		yield return new WaitForSeconds(particle.main.duration);
		particle.Stop(true, ParticleSystemStopBehavior.StopEmitting);
		yield return new WaitUntil(() => particle.particleCount == 0);
		_pool.Despawn(particle);
	}
}
