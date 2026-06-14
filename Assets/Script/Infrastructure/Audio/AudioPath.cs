using UnityEngine;

/// <summary>
/// 音频路径外观类。路径数据存储在 Resources/Config/AudioConfig.asset (AudioConfigSO) 中。
/// 保持原有静态属性接口，方便已有代码无缝切换。
/// </summary>
public static class AudioPath
{
    private static AudioConfigSO _config;
    private static AudioConfigSO Config
    {
        get
        {
            if (_config == null)
                _config = Resources.Load<AudioConfigSO>("Config/AudioConfig");
            return _config;
        }
    }

    // 背景音乐
    public static string BGMStart  => Config?.bgmStart  ?? "MusicEx/MusicEx_Welcome";
    public static string BGMGame   => Config?.bgmGame   ?? "MusicEx/MusicEx_Normal";
    public static string BGMWin    => Config?.bgmWin    ?? "Sound/Fight/MusicEx_Win";
    public static string BGMLose   => Config?.bgmLose   ?? "Sound/Fight/MusicEx_Lose";

    // 单牌
    public static string Single3   => Config?.single3   ?? "Sound/Fight/Woman_3";
    public static string Single4   => Config?.single4   ?? "Sound/Fight/Woman_4";
    public static string Single5   => Config?.single5   ?? "Sound/Fight/Woman_5";
    public static string Single6   => Config?.single6   ?? "Sound/Fight/Woman_6";
    public static string Single7   => Config?.single7   ?? "Sound/Fight/Woman_7";
    public static string Single8   => Config?.single8   ?? "Sound/Fight/Woman_8";
    public static string Single9   => Config?.single9   ?? "Sound/Fight/Woman_9";
    public static string Single10  => Config?.single10  ?? "Sound/Fight/Woman_10";
    public static string SingleJ   => Config?.singleJ   ?? "Sound/Fight/Woman_11";
    public static string SingleQ   => Config?.singleQ   ?? "Sound/Fight/Woman_12";
    public static string SingleK   => Config?.singleK   ?? "Sound/Fight/Woman_13";
    public static string SingleA   => Config?.singleA   ?? "Sound/Fight/Woman_14";
    public static string Single2   => Config?.single2   ?? "Sound/Fight/Woman_15";
    public static string SingleLK  => Config?.singleLK  ?? "Sound/Fight/Woman_100";
    public static string SingleBK  => Config?.singleBK  ?? "Sound/Fight/Woman_200";

    // 对子
    public static string Pair3     => Config?.pair3     ?? "Sound/Fight/Woman_dui3";
    public static string Pair4     => Config?.pair4     ?? "Sound/Fight/Woman_dui4";
    public static string Pair5     => Config?.pair5     ?? "Sound/Fight/Woman_dui5";
    public static string Pair6     => Config?.pair6     ?? "Sound/Fight/Woman_dui6";
    public static string Pair7     => Config?.pair7     ?? "Sound/Fight/Woman_dui7";
    public static string Pair8     => Config?.pair8     ?? "Sound/Fight/Woman_dui8";
    public static string Pair9     => Config?.pair9     ?? "Sound/Fight/Woman_dui9";
    public static string Pair10    => Config?.pair10    ?? "Sound/Fight/Woman_dui10";
    public static string PairJ     => Config?.pairJ     ?? "Sound/Fight/Woman_dui11";
    public static string PairQ     => Config?.pairQ     ?? "Sound/Fight/Woman_dui12";
    public static string PairK     => Config?.pairK     ?? "Sound/Fight/Woman_dui13";
    public static string PairA     => Config?.pairA     ?? "Sound/Fight/Woman_dui14";
    public static string Pair2     => Config?.pair2     ?? "Sound/Fight/Woman_dui15";

    // 牌型
    public static string Airplane         => Config?.airplane         ?? "Sound/Fight/Woman_Feiji";
    public static string ConsecutivePairs => Config?.consecutivePairs ?? "Sound/Fight/Woman_liandui";
    public static string ThreeWithOne     => Config?.threeWithOne     ?? "Sound/Fight/Woman_sandaiyi";
    public static string ThreeWithPair    => Config?.threeWithPair    ?? "Sound/Fight/Woman_sandaiyidui";
    public static string Straight         => Config?.straight         ?? "Sound/Fight/Woman_shunzi";
    public static string Triple3          => Config?.triple3          ?? "Sound/Fight/Woman_tuple3";
    public static string Triple4          => Config?.triple4          ?? "Sound/Fight/Woman_tuple4";
    public static string Triple5          => Config?.triple5          ?? "Sound/Fight/Woman_tuple5";
    public static string Triple6          => Config?.triple6          ?? "Sound/Fight/Woman_tuple6";
    public static string Triple7          => Config?.triple7          ?? "Sound/Fight/Woman_tuple7";
    public static string Triple8          => Config?.triple8          ?? "Sound/Fight/Woman_tuple8";
    public static string Triple9          => Config?.triple9          ?? "Sound/Fight/Woman_tuple9";
    public static string Triple10         => Config?.triple10         ?? "Sound/Fight/Woman_tuple10";
    public static string TripleJ          => Config?.tripleJ          ?? "Sound/Fight/Woman_tuple11";
    public static string TripleQ          => Config?.tripleQ          ?? "Sound/Fight/Woman_tuple12";
    public static string TripleK          => Config?.tripleK          ?? "Sound/Fight/Woman_tuple13";
    public static string TripleA          => Config?.tripleA          ?? "Sound/Fight/Woman_tuple14";
    public static string Triple2          => Config?.triple2          ?? "Sound/Fight/Woman_tuple15";
    public static string JokerBomb        => Config?.jokerBomb        ?? "Sound/Fight/Woman_wangzha";
    public static string Bomb             => Config?.bomb             ?? "Sound/Fight/Woman_zhadan";

    // 叫牌 / 不叫
    public static string CallLandlord    => Config?.callLandlord    ?? "Sound/Fight/Woman_Order";
    public static string NotCallLandlord => Config?.notCallLandlord ?? "Sound/Fight/Woman_NoOrder";
    public static string Refuse1         => Config?.refuse1         ?? "Sound/Fight/Woman_buyao1";
    public static string Refuse2         => Config?.refuse2         ?? "Sound/Fight/Woman_buyao2";
    public static string Refuse3         => Config?.refuse3         ?? "Sound/Fight/Woman_buyao3";
    public static string Refuse4         => Config?.refuse4         ?? "Sound/Fight/Woman_buyao4";
    public static string SingleReport    => Config?.singleReport    ?? "Sound/Fight/Woman_baojing1";
    public static string PairReport      => Config?.pairReport      ?? "Sound/Fight/Woman_baojing2";
}
