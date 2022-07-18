using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SceneMgr : Singleton<SceneMgr>
{
    /// <summary>
    /// ���س���(ͬ��)
    /// </summary>
    /// <param name="name">������</param>
    public void LoadScene(string name)
    {
        PoolMgr.Instance.Clear();
        SceneManager.LoadScene(name);
        EventCenter.Instance.Clear();
    }

    /// <summary>
    /// ���س���(�첽)
    /// ���ع����л�ַ��¼�"Loading"
    /// </summary>
    /// <param name="name">������</param>
    /// <param name="action">��ɻص�����</param>
    public void LoadSceneAsync(string name, UnityAction callback)
    {
        //���س���ǰҪ��ն�����ֵ䣬��������
        PoolMgr.Instance.Clear();
        MonoMgr.Instance.StartCoroutine(AsyncLoad(name, callback));
    }

    /// <summary>
    /// ���س���(�첽Pro)
    /// ����ʾ������壬�ټ���
    /// ������ɺ�ȴ�waitTime�����ؼ������
    /// </summary>
    /// <param name="name">������</param>
    /// <param name="callback">������ɵĻص�</param>
    /// <param name="waitTime">������ɺ��������ӳ���ʧ��ʱ��</param>
    public void LoadSceneAsyncPro(string name, UnityAction callback, float waitTime = 0.5f)
    {

    }


    IEnumerator AsyncLoad(string name,UnityAction action)
    {
        AsyncOperation ao = SceneManager.LoadSceneAsync(name);
        while (!ao.isDone)
        {
            yield return ao.progress;
            EventCenter.Instance.PostEvent("Loading");
        }
        EventCenter.Instance.PostEvent("LoadComplete");
        action?.Invoke();
    }

    IEnumerator AsyncLoadPro(string name, UnityAction callback, float waitTime)
    {
        int toProgress = 0;
        AsyncOperation ao = SceneManager.LoadSceneAsync(name);
        //���ص�0.9�ͼ�������
        ao.allowSceneActivation = false;
        yield return new WaitForEndOfFrame();
        while (ao.progress < 0.9f)
        {
            toProgress = (int)(ao.progress * 100);
            EventCenter.Instance.PostEvent("Loading", toProgress);
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForEndOfFrame();
        ao.allowSceneActivation = true;
        while (!ao.isDone)
            yield return new WaitForEndOfFrame();
        callback?.Invoke();
        //������Ϊ���ӳ�����Loading���
        yield return new WaitForSeconds(waitTime);
        while (toProgress < 100)
        {
            toProgress++;
            EventCenter.Instance.PostEvent("Loading", toProgress);
            yield return new WaitForEndOfFrame();
        }
        EventCenter.Instance.PostEvent("LoadComplete");
    }
}
