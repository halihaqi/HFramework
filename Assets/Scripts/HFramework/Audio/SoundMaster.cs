using System.Collections.Generic;
using UnityEngine;

namespace HFramework
{
    public class SoundMaster : MonoBehaviour
    {
        private List<AudioSource> _audioList;

        public List<AudioSource> AudioList => _audioList;

        private void Awake()
        {
            gameObject.name = "SoundMaster";
            DontDestroyOnLoad(gameObject);
        }

        public void Init(int maxAudioNum)
        {
            _audioList = new List<AudioSource>(maxAudioNum);
            for (int i = 0; i < maxAudioNum; i++)
            {
                var source = gameObject.AddComponent<AudioSource>();
                _audioList.Add(source);
            }
        }
    }
}