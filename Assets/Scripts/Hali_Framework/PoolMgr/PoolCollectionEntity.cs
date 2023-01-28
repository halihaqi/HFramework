using System.Collections.Generic;
using UnityEngine;

namespace Hali_Framework
{
    public class PoolCollectionEntity : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(this);
        }
    }
}