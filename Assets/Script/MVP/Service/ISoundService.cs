public interface ISoundService
{
    void PlayStartBgm();
    void PlayGameBgm();
    void PlayWinBgm();
    void PlayLoseBgm();
    void PlayCallLandlord();
    void PlayNotCallLandlord();
    void PlayPassCard();
    void PlayHandCountReport(int soundIndex);
    void PlaySoundForCardPattern(CardPattern pattern, int weight);
}
