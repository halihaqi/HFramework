using System;
using UnityEngine;

namespace HFramework
{
    public class ObjectPoolEntity : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}