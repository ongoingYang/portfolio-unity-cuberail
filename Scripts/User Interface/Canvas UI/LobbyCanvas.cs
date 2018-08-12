using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameCookInterface;
using TMPro;
public class LobbyCanvas : CanvasUI
{
    [SerializeField] private Button _playButton;
    [SerializeField] private Button _OptionButton;
    [SerializeField] private Button _ScoresButton;
    [SerializeField] private Button _CreditButton;
    [SerializeField] private Button _backButton;
    public TextMeshProUGUI userEmail;
    private void Start()
    {
        _playButton.onClick.AddListener(() => OnClickPlay());
        _OptionButton.onClick.AddListener(() => OnClickOptions());
        _CreditButton.onClick.AddListener(() => OnClickCredits());
        _ScoresButton.onClick.AddListener(() => OnClickScore());
        _backButton.onClick.AddListener(() => OnClickBack());

        if (GameDesignManager.userName != null)
        {
            userEmail.text = string.Format("{0}", GameDesignManager.userName.Split('@')[0]);  
        }
   
        AudioManager.Instance.SetStartAudioVolume();
    }
    private void OnClickPlay(){
        SoundOnByNext();
        GameDesignManager.Instance.StageLevel = 1;
        LevelSceneManager.Instance.LoadSceneByFade(SceneLevel.Play, CanvasType.Play, BGMSoundType.Play);
        
    }
    private void OnClickOptions(){
        SoundOnByNext();
        CanvasManager.Instance.OpenCanvasUI(CanvasType.Options);
    }
    private void OnClickCredits(){
        SoundOnByNext();
        CanvasManager.Instance.OpenCanvasUI(CanvasType.Credits);
        AudioManager.Instance.PlayBGMSound(BGMSoundType.Credits);
    }
    private void OnClickScore(){
        SoundOnByNext();
        CanvasManager.Instance.OpenCanvasUI(CanvasType.Score);
    }
    protected override void OnClickBack(){
        LevelSceneManager.Instance.SwitchGamePower(false);
    }
    protected override void OnEnable()
    {
        base.OnEnable();
    }


}
