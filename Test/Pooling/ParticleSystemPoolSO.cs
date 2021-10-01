using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SAS.Pool;

[CreateAssetMenu(menuName = "SAS/Pool/ParticleSystem")]
public class ParticleSystemPoolSO : ComponentPoolSO<ParticleSystem>
{
	[SerializeField] private ParticleSystemFactorySO _factory;
	protected override IFactory<ParticleSystem> Factory
	{
		get => _factory;
		set => _factory = null;
	}
}
