using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Example;
using Hali_Framework;
using UnityEngine;

public class GameMaster : MonoBehaviour
{
    private List<int> _ids = new List<int>();
    
    private void Awake()
    {
        DontDestroyOnLoad(this);
        //注册模块
        FrameworkEntry.Init();
        
        //开始初始化流程
        ProcedureMgr.Instance.Initialize(new InitProcedure());
        ProcedureMgr.Instance.StartProcedure<InitProcedure>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(2))
        {
            SceneMgr.Instance.LoadSceneWithPanel<LoadingPanel>("PlayGround", null);
        }
        if (Input.GetMouseButtonDown(0))
        {
            _ids.Add(UIMgr.Instance.ShowPanel<TestPanel>());
        }
        
        if (Input.GetMouseButtonDown(1))
        {
            if(_ids.Count <= 0) return;
            var last = _ids[_ids.Count - 1];
            _ids.Remove(last);
            UIMgr.Instance.HidePanel(last);
        }
    }
}
