using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

namespace SAS.Utilities
{
    [System.Serializable]
    public struct Parameter
    {
        [SerializeField] private string m_Name;
        [SerializeField] private ParameterType m_Type;
        [SerializeField] private bool m_BoolValue;
        [SerializeField] private int m_IntValue;
        [SerializeField] private float m_FloatValue;

        public string Name => m_Name;
        public ParameterType Type => m_Type;
        public bool BoolValue => m_BoolValue;
        public int IntValue => m_IntValue;
        public float FloatValue => m_FloatValue;

    }

    public enum ParameterType
    {
        Float = 1,
        Int = 3,
        Bool = 4,
        Trigger = 9
    }

    public static class Extensions
    {
        private static Random rng = new Random();

        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static void Apply(this Animator animator, in Parameter parameter)
        {
            switch (parameter.Type)
            {
                case ParameterType.Bool:
                    animator.SetBool(parameter.Name, parameter.BoolValue);
                    break;
                case ParameterType.Int:
                    animator.SetInteger(parameter.Name, parameter.IntValue);
                    break;
                case ParameterType.Float:
                    animator.SetFloat(parameter.Name, parameter.FloatValue);
                    break;
                case ParameterType.Trigger:
                    animator.SetTrigger(parameter.Name);
                    break;
            }
        }

    }
}
