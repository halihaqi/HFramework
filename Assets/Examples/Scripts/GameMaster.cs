using System;
using System.Collections;
using System.Collections.Generic;
using Example;
using Hali_Framework;
using UnityEngine;

public class GameMaster : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(this);
        //注册模块
        FrameworkEntry.Init();
        
        //开始初始化流程
        ProcedureMgr.Instance.Initialize(new InitProcedure());
        ProcedureMgr.Instance.StartProcedure<InitProcedure>();
    }
}
