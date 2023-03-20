using UnityEngine;
using UnityEngine.Events;

namespace HFramework
{
    public interface IAudioManager
    {
        /// <summary>
        /// 播放背景音乐
        /// </summary>
        /// <param name="path">音乐路径</param>
        void PlayBkMusic(string path);

        /// <summary>
        /// 改变背景音乐大小
        /// </summary>
        /// <param name="volume">音量</param>
        void ChangeBkMusicVolume(float volume);

        /// <summary>
        /// 改变背景音乐开关
        /// </summary>
        /// <param name="isOn">是否开启</param>
        void ChangeBkMusicOn(bool isOn);

        /// <summary>
        /// 暂停背景音乐
        /// </summary>
        void PauseBkMusic();

        /// <summary>
        /// 停止背景音乐
        /// </summary>
        void StopBkMusic();


        /// <summary>
        /// 播放音效
        /// </summary>
        /// <param name="path">音效路径</param>
        /// <param name="isLoop">是否循环播放</param>
        /// <param name="callback">回调函数,获得source</param>
        void PlaySound(string path, bool isLoop, UnityAction<AudioSource> callback = null);

        /// <summary>
        /// 改变单个音效大小
        /// </summary>
        /// <param name="path">音效路径</param>
        /// <param name="volume">音量</param>
        void ChangeSoundVolume(string path, float volume);

        /// <summary>
        /// 改变所有音效大小
        /// </summary>
        /// <param name="volume">音量</param>
        void ChangeAllSoundVolume(float volume);

        /// <summary>
        /// 改变单个音效开关
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="isOn">是否开启</param>
        void ChangeSoundOn(string path, bool isOn);

        /// <summary>
        /// 改变所有音效开关
        /// </summary>
        /// <param name="isOn">是否开启</param>
        void ChangeAllSoundOn(bool isOn);

        /// <summary>
        /// 停止音效
        /// </summary>
        /// <param name="path">路径</param>
        void StopSound(string path);

        /// <summary>
        /// 停止所有音效
        /// </summary>
        void StopAllSound();
    }
}