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
    /// ���ؽ�����ַ��޲��¼�"LoadComplete"
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
    /// ���س���(��ʾLoadingUI����)
    /// ���ع����л�ַ��в��¼�"Loading",����Ϊ���ؽ���
    /// ���ؽ�����ַ��޲��¼�"LoadComplete"�����Զ����ؼ������
    /// </summary>
    /// <param name="name">������</param>
    /// <param name="panelName">���ؽ���PanelԤ������</param>
    /// <param name="callback">������ɵĻص�</param>
    public void LoadSceneAsync<T>(string name, string panelName, UnityAction callback) where T : BasePanel
    {
        //����ʾLoadingUI
        UIMgr.Instance.ShowPanel<T>(panelName, E_UI_Layer.System, (panel) =>
        {
            //��ʾ���ʼ�л�����
            PoolMgr.Instance.Clear();
            MonoMgr.Instance.StartCoroutine(AsyncLoad<T>(name, panelName, callback));
        });
    }

    //�첽���س���Э��
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

    //�첽���س���Э��(��UI�ڵ�)
    IEnumerator AsyncLoad<T>(string name, string panelName, UnityAction callback) where T : BasePanel
    {
        //����toProgress��ʾ�ٵļ��ؽ���
        //��Ϊao.progress�ڼ���С����ʱ�仯̫�죬Ч������
        int toProgress = 0;
        AsyncOperation ao = SceneManager.LoadSceneAsync(name);
        ao.allowSceneActivation = false;

        //��allowSceneActivation = true֮ǰprogressֻ����ص�0.9
        //��Ҫ�ֶ�����
        while (ao.progress < 0.9f)
        {
            //�����ʾ�ļٵĽ��ȱ仯�����С��ao.progres��ÿ֡��1
            while (toProgress < (int)(ao.progress * 100))
            {
                toProgress++;
                EventCenter.Instance.PostEvent("Loading", toProgress);
                yield return toProgress;
            }
        }
        //���ص�0.9���ֶ�����Ϊtrue
        //���������0.9~1����Դ����������
        //��ʱ����Ӧ����ʾ����
        ao.allowSceneActivation = true;
        //isDoneֻ����allowSceneActivationΪtrue���Ҽ�������Ϊtrue
        //����ͨ������ж��Ƿ�������
        while (!ao.isDone)
        {
            toProgress = 95;
            EventCenter.Instance.PostEvent("Loading", toProgress);
            yield return toProgress;
        }
        //������һ֡����ȥ��
        yield return toProgress;
        toProgress = 100;
        callback?.Invoke();
        UIMgr.Instance.HidePanel(panelName);
        EventCenter.Instance.PostEvent("LoadComplete");
    }
}
