using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroCanvas : CanvasUI
{

    private void Start()
    {
        if (DatabaseManager.Instance != null)
        {
            Debug.Log("데이터 로드");
            DatabaseManager.Instance.GetUserSettingData();
            DatabaseManager.Instance.GetUserScoreData();
            AudioManager.Instance.MusicVolumeChange(AudioManager.Instance.musicVolume);
            AudioManager.Instance.SoundVolumeChange(AudioManager.Instance.soundVolume); 



        }
    }

}
