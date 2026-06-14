using UnityEngine;

[CreateAssetMenu(fileName = "AudioConfig", menuName = "CardGame/Config/AudioConfig")]
public class AudioConfigSO : ScriptableObject
{
    [Header("背景音乐")]
    public string bgmStart   = "MusicEx/MusicEx_Welcome";
    public string bgmGame    = "MusicEx/MusicEx_Normal";
    public string bgmWin     = "Sound/Fight/MusicEx_Win";
    public string bgmLose    = "Sound/Fight/MusicEx_Lose";

    [Header("单牌音效")]
    public string single3  = "Sound/Fight/Woman_3";
    public string single4  = "Sound/Fight/Woman_4";
    public string single5  = "Sound/Fight/Woman_5";
    public string single6  = "Sound/Fight/Woman_6";
    public string single7  = "Sound/Fight/Woman_7";
    public string single8  = "Sound/Fight/Woman_8";
    public string single9  = "Sound/Fight/Woman_9";
    public string single10 = "Sound/Fight/Woman_10";
    public string singleJ  = "Sound/Fight/Woman_11";
    public string singleQ  = "Sound/Fight/Woman_12";
    public string singleK  = "Sound/Fight/Woman_13";
    public string singleA  = "Sound/Fight/Woman_14";
    public string single2  = "Sound/Fight/Woman_15";
    public string singleLK = "Sound/Fight/Woman_100";
    public string singleBK = "Sound/Fight/Woman_200";

    [Header("对子音效")]
    public string pair3  = "Sound/Fight/Woman_dui3";
    public string pair4  = "Sound/Fight/Woman_dui4";
    public string pair5  = "Sound/Fight/Woman_dui5";
    public string pair6  = "Sound/Fight/Woman_dui6";
    public string pair7  = "Sound/Fight/Woman_dui7";
    public string pair8  = "Sound/Fight/Woman_dui8";
    public string pair9  = "Sound/Fight/Woman_dui9";
    public string pair10 = "Sound/Fight/Woman_dui10";
    public string pairJ  = "Sound/Fight/Woman_dui11";
    public string pairQ  = "Sound/Fight/Woman_dui12";
    public string pairK  = "Sound/Fight/Woman_dui13";
    public string pairA  = "Sound/Fight/Woman_dui14";
    public string pair2  = "Sound/Fight/Woman_dui15";

    [Header("牌型音效")]
    public string airplane         = "Sound/Fight/Woman_Feiji";
    public string consecutivePairs = "Sound/Fight/Woman_liandui";
    public string threeWithOne     = "Sound/Fight/Woman_sandaiyi";
    public string threeWithPair    = "Sound/Fight/Woman_sandaiyidui";
    public string straight         = "Sound/Fight/Woman_shunzi";
    public string triple3          = "Sound/Fight/Woman_tuple3";
    public string triple4          = "Sound/Fight/Woman_tuple4";
    public string triple5          = "Sound/Fight/Woman_tuple5";
    public string triple6          = "Sound/Fight/Woman_tuple6";
    public string triple7          = "Sound/Fight/Woman_tuple7";
    public string triple8          = "Sound/Fight/Woman_tuple8";
    public string triple9          = "Sound/Fight/Woman_tuple9";
    public string triple10         = "Sound/Fight/Woman_tuple10";
    public string tripleJ          = "Sound/Fight/Woman_tuple11";
    public string tripleQ          = "Sound/Fight/Woman_tuple12";
    public string tripleK          = "Sound/Fight/Woman_tuple13";
    public string tripleA          = "Sound/Fight/Woman_tuple14";
    public string triple2          = "Sound/Fight/Woman_tuple15";
    public string jokerBomb        = "Sound/Fight/Woman_wangzha";
    public string bomb             = "Sound/Fight/Woman_zhadan";

    [Header("叫牌 / 不叫音效")]
    public string callLandlord    = "Sound/Fight/Woman_Order";
    public string notCallLandlord = "Sound/Fight/Woman_NoOrder";
    public string refuse1         = "Sound/Fight/Woman_buyao1";
    public string refuse2         = "Sound/Fight/Woman_buyao2";
    public string refuse3         = "Sound/Fight/Woman_buyao3";
    public string refuse4         = "Sound/Fight/Woman_buyao4";
    public string singleReport    = "Sound/Fight/Woman_baojing1";
    public string pairReport      = "Sound/Fight/Woman_baojing2";
}
