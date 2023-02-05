using Hali_Framework;

namespace Example
{
    public class InitProcedure : ProcedureBase
    {
        protected internal override void OnEnter(IFsm<ProcedureMgr> procedureOwner)
        {
            base.OnEnter(procedureOwner);
            
            //初始化UI结构
            UIMgr.Instance.AddUIGroup(GameConst.UIGROUP_BOT, 0);
            UIMgr.Instance.AddUIGroup(GameConst.UIGROUP_MID, 1);
            UIMgr.Instance.AddUIGroup(GameConst.UIGROUP_TOP, 2);
            UIMgr.Instance.AddUIGroup(GameConst.UIGROUP_SYS, 3);
            
            //Test
            SceneMgr.Instance.LoadSceneWithPanel<LoadingPanel>("PlayGround", null);
        }
    }
}