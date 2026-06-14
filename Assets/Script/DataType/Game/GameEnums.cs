using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameEnums
{
    NotStarted,
    Bidding,        // 叫地主
    Playing,        // 出牌阶段
    RoundEnd,       // 回合结束
    GameEnd,        // 游戏结束
}
