namespace SPGame
{
    /// <summary>
    /// 游戏启动流程，初始化音频设置等
    /// </summary>
    public sealed class LaunchProcedure : BaseProcedure
    {
        public override void OnEnter(IFsm<IProcedureModule> fsm)
        {
            fsm.Owner.SwitchState<SplashProcedure>();
        }
    }
}
