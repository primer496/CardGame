using UnityEngine;

[CreateAssetMenu(fileName = "CardGameRule", menuName = "CardGame/Config/CardGameRule")]
public class CardGameRuleSO : ScriptableObject
{
    [Header("权重阈值")]
    [Tooltip("权重 >= 此值的牌（2、小王、大王）不能出现在顺子/连对/飞机中")]
    public int specialCardWeightThreshold = 15;

    [Tooltip("小王权重，用于王炸判定")]
    public int littleJokerWeight = 16;

    [Tooltip("大王权重，用于王炸判定")]
    public int bigJokerWeight = 17;

    [Header("顺子规则")]
    [Tooltip("顺子最少张数")]
    public int straightMinCount = 5;

    [Tooltip("顺子最多张数")]
    public int straightMaxCount = 12;

    [Header("连对规则")]
    [Tooltip("连对最少对数（最少 6 张）")]
    public int straightPairMinGroups = 3;

    [Header("飞机规则")]
    [Tooltip("飞机最少组数（每组 3 张，最少 6 张）")]
    public int planeMinGroups = 2;

    [Header("过牌规则")]
    [Tooltip("连续过牌次数达到此值时重置上一手牌")]
    public int refuseCountToReset = 2;
}
