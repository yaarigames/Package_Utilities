using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SAS.Pool;

[CreateAssetMenu(menuName = "SAS/Pool/Factory/ParticleSystem")]
public class ParticleSystemFactorySO : FactorySO<ParticleSystem>
{
    public override bool Create(out ParticleSystem item)
    {
        item = new GameObject("ParticleSystem").AddComponent<ParticleSystem>();
        return true;
    }
}
