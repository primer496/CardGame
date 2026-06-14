using UnityEngine;

[CreateAssetMenu(fileName = "DeckConfig", menuName = "CardGame/Config/DeckConfig")]
public class DeckConfigSO : ScriptableObject
{
    [Header("牌组基础参数")]
    [Tooltip("标准牌组总张数（含大小王）")]
    public int totalCardCount = 54;

    [Tooltip("普通牌数量（不含大小王）")]
    public int normalCardCount = 52;

    [Header("特殊牌权重")]
    [Tooltip("A 的权重")]
    public int weightAce = 14;

    [Tooltip("2 的权重")]
    public int weightTwo = 15;

    [Tooltip("小王的权重")]
    public int weightLittleJoker = 16;

    [Tooltip("大王的权重")]
    public int weightBigJoker = 17;

    [Header("发牌参数")]
    [Tooltip("底牌数量")]
    public int lordCardCount = 3;

    [Tooltip("玩家数量")]
    public int playerCount = 3;
}
