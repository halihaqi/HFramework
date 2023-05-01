using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

namespace HFramework
{
    internal class AudioManager : HModule, IAudioManager
    {
        public class ChannelInfo
        {
            private AudioSource _audioSource;
            //优先级，优先级低的先被挤掉
            public int order;
            //最后一次播放开始的时间，如果优先级相同，最早播放的被挤掉
            public float lastPlayTime;

            public AudioSource AudioSource => _audioSource;

            public bool IsPlaying => _audioSource.isPlaying;

            public float Volume
            {
                get => _audioSource.volume;
                set => _audioSource.volume = value;
            }

            public bool Mute
            {
                get => _audioSource.mute;
                set => _audioSource.mute = value;
            }

            public void Stop()
            {
                _audioSource.Stop();
            }

            public ChannelInfo(AudioSource audioSource, int order = 0)
            {
                this._audioSource = audioSource;
                this.order = order;
                lastPlayTime = -1;
            }
        }
        
        private const int MAX_SOUND_NUM = 8;
        //背景音乐
        private AudioSource _bkMusic = null;
        private float _bkMusicVolume = 1;
        private bool _bkMusicOn = true;

        //音效
        private SoundMaster _soundMaster = null;//管理AudioSource实体
        private List<ChannelInfo> _channels;
        private float _soundVolume = 1;
        private bool _soundOn = true;

        internal override int Priority => 0;
        
        internal override void Init()
        {
            _channels = new List<ChannelInfo>(MAX_SOUND_NUM);
        }

        internal override void Update(float elapseSeconds, float realElapseSeconds)
        {
        }

        internal override void Shutdown()
        {
            _channels.Clear();
            _channels = null;
        }


        #region 背景音乐公共方法
        /// <summary>
        /// 播放背景音乐
        /// </summary>
        /// <param name="path">音乐路径</param>
        public void PlayBkMusic(string path)
        {
            //如果为空，在场景中新建并添加AudioSource
            if (_bkMusic == null)
            {
                GameObject obj = new GameObject("BkMusic");
                Object.DontDestroyOnLoad(obj);
                _bkMusic = obj.AddComponent<AudioSource>();
            }

            HEntry.ResMgr.LoadAsync<AudioClip>(path, (clip) =>
            {
                _bkMusic.clip = clip;
                _bkMusic.loop = true;
                _bkMusic.mute = !_bkMusicOn;
                _bkMusic.volume = _bkMusicVolume;
                _bkMusic.Play();
            });
        }

        /// <summary>
        /// 改变背景音乐大小
        /// </summary>
        /// <param name="volume">音量</param>
        public void ChangeBkMusicVolume(float volume)
        {
            _bkMusicVolume = volume;
            if (_bkMusic != null)
                _bkMusic.volume = _bkMusicVolume;
        }

        /// <summary>
        /// 改变背景音乐开关
        /// </summary>
        /// <param name="isOn">是否开启</param>
        public void ChangeBkMusicOn(bool isOn)
        {
            _bkMusicOn = isOn;
            if(_bkMusic != null)
                _bkMusic.mute = !_bkMusicOn;
        }

        /// <summary>
        /// 暂停背景音乐
        /// </summary>
        public void PauseBkMusic()
        {
            if (_bkMusic == null)
            {
                Debug.Log("No BkMusic in Scene");
                return;
            }
            _bkMusic.Pause();
        }

        /// <summary>
        /// 停止背景音乐
        /// </summary>
        public void StopBkMusic()
        {
            if (_bkMusic == null)
            {
                Debug.Log("No BkMusic in Scene");
                return;
            }
            _bkMusic.Stop();
        }
        #endregion

        #region 音效公共方法

        /// <summary>
        /// 播放音效
        /// </summary>
        /// <param name="path">音效路径</param>
        /// <param name="isLoop">是否循环播放</param>
        /// <param name="order">音效优先级，优先级低的先被挤掉</param>
        /// <param name="callback">回调函数,获得source</param>
        public int PlaySound(string path, bool isLoop, int order = 0, UnityAction<AudioSource> callback = null)
        {
            //如果场景中没有就新建Sound空物体
            if (_soundMaster == null)
            {
                _soundMaster = new GameObject().AddComponent<SoundMaster>();
                _soundMaster.Init(MAX_SOUND_NUM);
                foreach (var source in _soundMaster.AudioList)
                    _channels.Add(new ChannelInfo(source));
            }

            int index = GetEmptyChannelIndex();
            _channels[index].lastPlayTime = Time.time;
            _channels[index].order = order;
            //异步加载音效，加载完成后播放并加入soundList
            HEntry.ResMgr.LoadAsync<AudioClip>(path, (clip) =>
            {
                AudioSource audio = _channels[index].AudioSource;
                audio.clip = clip;
                audio.volume = _soundVolume;
                audio.mute = !_soundOn;
                audio.loop = isLoop;
                audio.Play();
                callback?.Invoke(audio);
            });
            return index;
        }

        public int GetEmptyChannelCount()
        {
            int count = 0;
            for (int i = 0; i < _channels.Count; i++)
            {
                if (!_channels[i].IsPlaying)
                    count++;
            }

            return count;
        }

        private int GetEmptyChannelIndex()
        {
            int lowestOrder = int.MaxValue;
            int occupyIndex = 0;
            for (int i = 0; i < _channels.Count; i++)
            {
                if (!_channels[i].IsPlaying)
                    return i;
                if (_channels[i].order < lowestOrder)//比较优先级
                {
                    lowestOrder = _channels[i].order;
                    occupyIndex = i;
                }
                else if (_channels[i].order == lowestOrder &&
                         _channels[i].lastPlayTime < _channels[occupyIndex].lastPlayTime) //比较播放时间
                {
                    occupyIndex = i;
                }
            }
            
            return occupyIndex;//返回挤掉的index
        }

        /// <summary>
        /// 改变单个音效大小
        /// </summary>
        /// <param name="channelIndex">频道index</param>
        /// <param name="volume">音量</param>
        public void ChangeSoundVolume(int channelIndex, float volume)
        {
            if (channelIndex < 0 || channelIndex >= MAX_SOUND_NUM)
            {
                Debug.Log("Invalid channel index.");
                return;
            }

            _channels[channelIndex].Volume = volume;
        }

        /// <summary>
        /// 改变所有音效大小
        /// </summary>
        /// <param name="volume">音量</param>
        public void ChangeAllSoundVolume(float volume)
        {
            _soundVolume = volume;
            foreach (var channel in _channels)
            {
                channel.Volume = volume;
            }
        }

        /// <summary>
        /// 改变单个音效开关
        /// </summary>
        /// <param name="channelIndex">频道index</param>
        /// <param name="isOn">是否开启</param>
        public void ChangeSoundOn(int channelIndex, bool isOn)
        {
            if (channelIndex < 0 || channelIndex >= MAX_SOUND_NUM)
            {
                Debug.Log("Invalid channel index.");
                return;
            }

            _channels[channelIndex].Mute = !isOn;
        }

        /// <summary>
        /// 改变所有音效开关
        /// </summary>
        /// <param name="isOn">是否开启</param>
        public void ChangeAllSoundOn(bool isOn)
        {
            _soundOn = isOn;
            foreach (var channel in _channels)
            {
                channel.Mute = !isOn;
            }
        }

        /// <summary>
        /// 停止音效
        /// </summary>
        /// <param name="channelIndex">频道index</param>
        public void StopSound(int channelIndex)
        {
            if (channelIndex < 0 || channelIndex >= MAX_SOUND_NUM)
            {
                Debug.Log("Invalid channel index.");
                return;
            }

            _channels[channelIndex].Stop();
        }

        /// <summary>
        /// 停止所有音效
        /// </summary>
        public void StopAllSound()
        {
            foreach (var channel in _channels)
            {
                channel.Stop();
            }
        }

        #endregion
    }
}
