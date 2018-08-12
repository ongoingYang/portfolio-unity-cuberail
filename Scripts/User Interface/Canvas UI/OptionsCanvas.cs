using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using GameCookInterface;

public class OptionsCanvas : CanvasUI
{
    [SerializeField] private Slider _musicVolumeSlider;
    [SerializeField] private Slider _soundVolumeSlider;
    [SerializeField] private Button _gameTypeToggle;
    [SerializeField] private Button _backButton;

    private void Start()
    {
        _musicVolumeSlider.onValueChanged.AddListener(delegate { OnMusicVolumeChanged(); });
        _soundVolumeSlider.onValueChanged.AddListener(delegate { OnSoundVolumeChanged(); });
        _gameTypeToggle.onClick.AddListener(() => OnClickGameType());
        _backButton.onClick.AddListener(() => OnClickBack());
    }
    public void OnMusicVolumeChanged() {
        if (AudioManager.Instance != null)
            AudioManager.Instance.MusicVolumeChange(_musicVolumeSlider.value);
    }
    public void OnSoundVolumeChanged() {
        if (AudioManager.Instance != null)
            AudioManager.Instance.SoundVolumeChange(_soundVolumeSlider.value);
    }
    private void OnClickGameType() {

        if (_gameTypeToggle.transform.GetChild(1).gameObject.activeSelf)
        {
            _gameTypeToggle.transform.GetChild(1).gameObject.SetActive(false);
            _gameTypeToggle.transform.GetChild(0).gameObject.SetActive(true);
            if (GameDesignManager.Instance != null) GameDesignManager.Instance.gameType = 0;

        }
        else if (_gameTypeToggle.transform.GetChild(0).gameObject.activeSelf) {
            _gameTypeToggle.transform.GetChild(0).gameObject.SetActive(false);
            _gameTypeToggle.transform.GetChild(1).gameObject.SetActive(true);
            StartCoroutine(RoutineInterrupt());
            if (GameDesignManager.Instance != null) GameDesignManager.Instance.gameType = 0;
        }
    }
    public void Interrupt()
    {

    }
   IEnumerator RoutineInterrupt()
    {
        AudioManager.Instance.PlayUISound(UISoundType.Interrupt);
        float elapsedTime = 0f;
      
        while (elapsedTime <= 0.75f)
        {
      
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        _gameTypeToggle.transform.GetChild(0).gameObject.SetActive(true);
        _gameTypeToggle.transform.GetChild(1).gameObject.SetActive(false);
        AudioManager.Instance.PlayUISound(UISoundType.Switch);
    }

    public void DisplayGameType(int index)
    {
        switch (index)
        {
            case 0:
                _gameTypeToggle.transform.GetChild(1).gameObject.SetActive(false);
                _gameTypeToggle.transform.GetChild(0).gameObject.SetActive(true);
                if (GameDesignManager.Instance != null) GameDesignManager.Instance.gameType = 0;
                break;
            case 1:
                _gameTypeToggle.transform.GetChild(0).gameObject.SetActive(false);
                _gameTypeToggle.transform.GetChild(1).gameObject.SetActive(true);
                if (GameDesignManager.Instance != null) GameDesignManager.Instance.gameType = 1;
                break;
        }
    }
    protected override void OnClickBack()
    {
        if (DatabaseManager.Instance != null&&
            GameDesignManager.userName!=null) {
            DatabaseManager.Instance.UpdateUserSetting(_musicVolumeSlider.value, _soundVolumeSlider.value, GameDesignManager.Instance.gameType);
        }
        base.OnClickBack();
    }
    public void LoadOptionsData()
    {
        if (AudioManager.Instance != null) {
            _musicVolumeSlider.value = AudioManager.Instance.musicVolume;
            _soundVolumeSlider.value = AudioManager.Instance.soundVolume;
        } 
        if(GameDesignManager.Instance != null) DisplayGameType(GameDesignManager.Instance.gameType);
    }
    protected override void OnEnable()
    {
        if(LoginCanvas.user!=null) LoadOptionsData();
        base.OnEnable();
        StopAllCoroutines();
    }
}
