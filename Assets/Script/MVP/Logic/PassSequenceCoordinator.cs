/// <summary>服务端连续过牌计数与“两家过牌清空桌面”规则（原 PassServerRpc 内联逻辑）。</summary>
public static class PassSequenceCoordinator
{
    public struct StepResult
    {
        public bool IgnoreDuplicatePass;
        public int RefuseCountAfter;
        public int LastPassPlayerAfter;
        public bool ShouldResetPlayArea;
    }

    /// <param name="playerIndex">本次请求过牌的玩家 1..3</param>
    /// <param name="refuseCount">当前连续过牌计数（服务器字段）</param>
    /// <param name="lastPassPlayerIndex">上一手过牌玩家，-1 表示无</param>
    public static StepResult ApplyPass(int playerIndex, int refuseCount, int lastPassPlayerIndex)
    {
        if (lastPassPlayerIndex == playerIndex)
        {
            return new StepResult
            {
                IgnoreDuplicatePass = true,
                RefuseCountAfter = refuseCount,
                LastPassPlayerAfter = lastPassPlayerIndex,
                ShouldResetPlayArea = false
            };
        }

        int nextCount = refuseCount + 1;
        if (nextCount >= 2)
        {
            return new StepResult
            {
                IgnoreDuplicatePass = false,
                RefuseCountAfter = 0,
                LastPassPlayerAfter = -1,
                ShouldResetPlayArea = true
            };
        }

        return new StepResult
        {
            IgnoreDuplicatePass = false,
            RefuseCountAfter = nextCount,
            LastPassPlayerAfter = playerIndex,
            ShouldResetPlayArea = false
        };
    }
}
