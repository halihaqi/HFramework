using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MonoMgr : Singleton<MonoMgr>
{
    private GlobalMono globalMono;

    public MonoMgr()
    {
        GameObject obj = new GameObject("GlobalMono");
        //GlobalMono��Start()���й�����������
        globalMono = obj.AddComponent<GlobalMono>();
    }

    #region Update��ع�������
    /// <summary>
    /// ���Update�¼�����
    /// </summary>
    /// <param name="action">�¼�</param>
    public void AddUpdateListener(UnityAction action)
    {
        globalMono.AddUpdateListener(action);
    }

    /// <summary>
    /// �Ƴ�Update�¼�����
    /// </summary>
    /// <param name="action">�¼�</param>
    public void RemoveUpdateListener(UnityAction action)
    {
        globalMono.RemoveUpdateListener(action);
    }

    /// <summary>
    /// ���Update�¼�����
    /// </summary>
    public void Clear()
    {
        globalMono.Clear();
    }
    #endregion

    #region Coroutine��ع�������
    //����Э��
    public Coroutine StartCoroutine(IEnumerator routine)
    {
        return globalMono.StartCoroutine(routine);
    }

    public Coroutine StartCoroutine(string methodName)
    {
        return globalMono.StartCoroutine(methodName);
    }

    //ֹͣЭ��
    public void StopCoroutine(IEnumerator routine)
    {
        globalMono.StopCoroutine(routine);
    }

    public void StopCoroutine(string methodName)
    {
        globalMono.StopCoroutine(methodName);
    }

    //ֹͣ����Э��
    public void StopAllCoroutines()
    {
        globalMono.StopAllCoroutines();
    }
    #endregion


}
