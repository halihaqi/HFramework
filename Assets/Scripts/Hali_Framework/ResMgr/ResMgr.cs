using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ResMgr : Singleton<ResMgr>
{
    //��Դ����
    //�ŵ㣺�����ظ����أ���������Ч��
    //ȱ�㣺�ڴ�ռ�ã���Ҫ�ں���ʱ���ͷ�
    private Dictionary<string, object> resDic = new Dictionary<string, object>();

    /// <summary>
    /// �����Դ�������ͷ��ڴ�
    /// </summary>
    public void ClearResDic()
    {
        resDic.Clear();
    }

    /// <summary>
    /// ������Դ(ͬ��)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="path">��Դ·��</param>
    /// <returns></returns>
    public T Load<T>(string path) where T : Object
    {
        T res = null;
        //����ֵ����У��Ͳ��ü�����
        if (resDic.ContainsKey(path))
            res = resDic[path] as T;
        else
            res = Resources.Load<T>(path);
        //�����GameObject����ʵ�����ٷ���
        if (res is GameObject)
            return GameObject.Instantiate(res);
        else
            return res;
    }

    /// <summary>
    /// ������Դ(�첽)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="path">��Դ·��</param>
    /// <param name="callback">�ص�����</param>
    public void LoadAsync<T>(string path, UnityAction<T> callback = null) where T : Object
    {
        MonoMgr.Instance.StartCoroutine(AsyncLoad(path, callback));
    }

    IEnumerator AsyncLoad<T>(string path, UnityAction<T> callback) where T : Object
    {
        T res = null;
        //����ֵ����У��Ͳ��ü�����
        if (resDic.ContainsKey(path))
            res = resDic[path] as T;
        else
        {
            ResourceRequest rr = Resources.LoadAsync<T>(path);
            res = rr.asset as T;
        }
        yield return null;
        //�����GameObject��������Ԥ������ִ�лص�
        if (res is GameObject)
            callback?.Invoke(GameObject.Instantiate(res));
        else
            callback?.Invoke(res);
    }
}
