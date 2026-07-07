using UnityEngine;

public class SoundService : MonoBehaviour, ISoundService
{
    private static SoundService _instance;
    public static SoundService Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<SoundService>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("SoundService");
                    _instance = go.AddComponent<SoundService>();
                }
            }

            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void PlayWinBgm()
    {
        if (SoundManager.instance == null) return;
        SoundManager.instance.PlayBGM(AudioPath.BGMWin, 1f, false);
    }

    public void PlayLoseBgm()
    {
        if (SoundManager.instance == null) return;
        SoundManager.instance.PlayBGM(AudioPath.BGMLose, 1f, false);
    }

    public void PlayCallLandlord()
    {
        PlaySound(AudioPath.CallLandlord);
    }

    public void PlayStartBgm()
    {
        if (SoundManager.instance == null) return;
        SoundManager.instance.PlayBGM(AudioPath.BGMStart, 1f);
    }

    public void PlayGameBgm()
    {
        if (SoundManager.instance == null) return;
        SoundManager.instance.PlayBGM(AudioPath.BGMGame, 1f);
    }

    /// <summary>ClientRpc 等客户端音效逻辑统一走这个入口，避免 GameServer 直接依赖 SoundManager。</summary>
    public void PlaySound(string path, float volume = 1f)
    {
        if (SoundManager.instance == null) return;
        SoundManager.instance.PlaySound(path, volume);
    }

    public void PlayNotCallLandlord()
    {
        PlaySound(AudioPath.NotCallLandlord);
    }

    public void PlayPassCard()
    {
        PlaySound(AudioPath.Refuse1, 1f);
    }

    /// <summary>报牌（剩 1 张 / 2 张），与 CardConditionDetect 的 soundIndex 约定一致。</summary>
    public void PlayHandCountReport(int soundIndex)
    {
        switch (soundIndex)
        {
            case 1:
                PlaySound(AudioPath.SingleReport, 1f);
                break;
            case 2:
                PlaySound(AudioPath.PairReport, 1f);
                break;
        }
    }

    /// <summary>根据牌型与权重播放出牌音效（原 playSoundByPatternClientRpc 逻辑）。</summary>
    public void PlaySoundForCardPattern(CardPattern pattern, int weight)
    {
        if (pattern == CardPattern.Single)
        {
            switch (weight)
            {
                case 3: PlaySound(AudioPath.Single3, 1f); break;
                case 4: PlaySound(AudioPath.Single4, 1f); break;
                case 5: PlaySound(AudioPath.Single5, 1f); break;
                case 6: PlaySound(AudioPath.Single6, 1f); break;
                case 7: PlaySound(AudioPath.Single7, 1f); break;
                case 8: PlaySound(AudioPath.Single8, 1f); break;
                case 9: PlaySound(AudioPath.Single9, 1f); break;
                case 10: PlaySound(AudioPath.Single10, 1f); break;
                case 11: PlaySound(AudioPath.SingleJ, 1f); break;
                case 12: PlaySound(AudioPath.SingleQ, 1f); break;
                case 13: PlaySound(AudioPath.SingleK, 1f); break;
                case 14: PlaySound(AudioPath.SingleA, 1f); break;
                case 15: PlaySound(AudioPath.Single2, 1f); break;
                case 16: PlaySound(AudioPath.SingleLK, 1f); break;
                case 17: PlaySound(AudioPath.SingleBK, 1f); break;
                default:
                    Debug.LogWarning($"[SoundService] 未知单牌权重 {weight}，无法播放音效");
                    break;
            }
        }
        else if (pattern == CardPattern.pair)
        {
            switch (weight)
            {
                case 3: PlaySound(AudioPath.Pair3, 1f); break;
                case 4: PlaySound(AudioPath.Pair4, 1f); break;
                case 5: PlaySound(AudioPath.Pair5, 1f); break;
                case 6: PlaySound(AudioPath.Pair6, 1f); break;
                case 7: PlaySound(AudioPath.Pair7, 1f); break;
                case 8: PlaySound(AudioPath.Pair8, 1f); break;
                case 9: PlaySound(AudioPath.Pair9, 1f); break;
                case 10: PlaySound(AudioPath.Pair10, 1f); break;
                case 11: PlaySound(AudioPath.PairJ, 1f); break;
                case 12: PlaySound(AudioPath.PairQ, 1f); break;
                case 13: PlaySound(AudioPath.PairK, 1f); break;
                case 14: PlaySound(AudioPath.PairA, 1f); break;
                case 15: PlaySound(AudioPath.Pair2, 1f); break;
                default:
                    Debug.LogWarning($"[SoundService] 未知对子权重 {weight}，无法播放音效");
                    break;
            }
        }
        else if (pattern == CardPattern.Plane)
        {
            PlaySound(AudioPath.Airplane, 1f);
        }
        else if (pattern == CardPattern.StraightPair)
        {
            PlaySound(AudioPath.ConsecutivePairs, 1f);
        }
        else if (pattern == CardPattern.TripleWithSingle)
        {
            PlaySound(AudioPath.ThreeWithOne, 1f);
        }
        else if (pattern == CardPattern.TripleWithPair)
        {
            PlaySound(AudioPath.ThreeWithPair, 1f);
        }
        else if (pattern == CardPattern.Triple)
        {
            switch (weight)
            {
                case 3: PlaySound(AudioPath.Triple3, 1f); break;
                case 4: PlaySound(AudioPath.Triple4, 1f); break;
                case 5: PlaySound(AudioPath.Triple5, 1f); break;
                case 6: PlaySound(AudioPath.Triple6, 1f); break;
                case 7: PlaySound(AudioPath.Triple7, 1f); break;
                case 8: PlaySound(AudioPath.Triple8, 1f); break;
                case 9: PlaySound(AudioPath.Triple9, 1f); break;
                case 10: PlaySound(AudioPath.Triple10, 1f); break;
                case 11: PlaySound(AudioPath.TripleJ, 1f); break;
                case 12: PlaySound(AudioPath.TripleQ, 1f); break;
                case 13: PlaySound(AudioPath.TripleK, 1f); break;
                case 14: PlaySound(AudioPath.TripleA, 1f); break;
                case 15: PlaySound(AudioPath.Triple2, 1f); break;
                default:
                    Debug.LogWarning($"[SoundService] 未知三张权重 {weight}，无法播放音效");
                    break;
            }
        }
        else if (pattern == CardPattern.Straight)
        {
            PlaySound(AudioPath.Straight, 1f);
        }
        else if (pattern == CardPattern.Bomb)
        {
            PlaySound(AudioPath.Bomb, 1f);
        }
        else if (pattern == CardPattern.JokerBomb)
        {
            PlaySound(AudioPath.JokerBomb, 1f);
        }
        else
        {
            Debug.LogWarning($"[SoundService] 未知牌型 {pattern}，无法播放音效");
        }
    }
}
