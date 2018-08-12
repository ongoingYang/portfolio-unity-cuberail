using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using GameCookInterface;

public class AudioManager : MonoBehaviour
{
    #region 싱글턴 정의
    private static AudioManager _instance;
    public static AudioManager Instance
    { get { return _instance; } }
    private void MakeSingleton()
    {
        if (_instance != null) DestroyImmediate(gameObject);
        else
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    private void Awake()
    {
        MakeSingleton();
        
        /** sound resource initialize 
         * -접근 쉽게 하기 위해 미리 enum 그룹화 */
        InitBGMSoundGroup();
        InitUISoundGroup();
        InitSFXSoundGroup();
    }
    #endregion
    private void Start()
    {
        soundSource.ignoreListenerPause = true;
        StartCoroutine(VolumeFadeIn(0.75f, 3f)); /** 첫 로그인 화면에서 배경음 Fade in */
    }
    public BGMSoundType playOnMusic; /** 현재 플레이 중인 배경음 표시 */

    [Range(0, 1)] public float musicVolume;
    [Range(0, 1)] public float soundVolume;

    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource soundSource;

    [SerializeField] private BGMSound[] bgmSoundGroup = new BGMSound[_bgmIndex];
    [SerializeField] private SFXSound[] sfxSoundGroup = new SFXSound[_sfxIndex];
    [SerializeField] private UISound[] uiSoundGroup = new UISound[_uiIndex];
    
    const int _bgmIndex = 9;
    const int _sfxIndex = 18;
    const int _uiIndex = 10;

    #region volume relevance
    public void SetStartAudioVolume(){
        StartCoroutine(VolumeFadeIn(musicVolume, 1f));
        soundSource.volume = soundVolume;
    }
    public void MusicVolumeChange(float volume)
    {
        musicSource.volume = volume;
        musicVolume = volume;
    }
    public void SoundVolumeChange(float volume)
    {
        soundSource.volume = volume;
        soundVolume = volume;
    }
    #endregion
    #region play Sound Method
    public void PauseSound()
    {
        AudioListener.pause = true;
    }
    public void UnPauseSound()
    {
        if(AudioListener.pause)
            AudioListener.pause = false;
    }
    public void PlayBGMSound(BGMSoundType type){

        if (type == BGMSoundType.None) {
            musicSource.Stop();
            playOnMusic = type;
            return;
        }
        if (bgmSoundGroup[type.BGMSoundTypeToIndex()].isLoop) musicSource.loop = true;
        else musicSource.loop = false;
        musicSource.clip = bgmSoundGroup[type.BGMSoundTypeToIndex()].audioClip;
        musicSource.Play();
        playOnMusic = type;
    }
    public void PlayUISound(UISoundType type){

        if (type == UISoundType.None){
            return;
        }
        soundSource.clip = uiSoundGroup[type.UISoundTypeToIndex()].audioClip;
        soundSource.Play();
    }
    public void PlaySFXSound(SFXSoundType type){

        if (type == SFXSoundType.None){
            return;
        }
        soundSource.PlayOneShot(sfxSoundGroup[type.SFXSoundTypeToIndex()].audioClip);
    }

    public void PlaySound(AudioClip clip)
    {
        soundSource.PlayOneShot(clip);
    }
    #endregion

    #region initializaion_BGMGroup



    private void InitBGMSoundGroup()
    {
        foreach(BGMSound sound in bgmSoundGroup){
            sound.SourceID = sound.bgmType.BGMSoundTypeToIndex();
        }
        int index = 0;
        BGMSound[] temp = bgmSoundGroup;
        foreach (BGMSound sound in temp.OrderBy(t => t.SourceID)){
            bgmSoundGroup[index++] = sound;
        }
    }
    #endregion
    #region initializaion_SoundGroup
    private void InitUISoundGroup()
    {
        foreach (UISound sound in uiSoundGroup){
            sound.SourceID = sound.uiType.UISoundTypeToIndex();
        }
        int index = 0;
        UISound[] temp = uiSoundGroup;
        foreach (UISound sound in temp.OrderBy(t => t.SourceID)){
            uiSoundGroup[index++] = sound;
        }
    }
    #endregion
    #region initializaion_Sound Effect Group
    private void InitSFXSoundGroup(){
        foreach (SFXSound sound in sfxSoundGroup){
            sound.SourceID = sound.sfxType.SFXSoundTypeToIndex();
        }
        int index = 0;
        SFXSound[] temp = sfxSoundGroup;
        foreach (SFXSound sound in temp.OrderBy(t => t.SourceID)){
            sfxSoundGroup[index++] = sound;
        }
    }
    #endregion

    #region Volume fade in/out Coroutine

    IEnumerator VoluemFadeOut(float durationTime){
        float from = musicSource.volume;
        float to = 0;

        float elapsedTime = 0f;
        while (elapsedTime <= 1){
            elapsedTime += Time.deltaTime / durationTime;
            musicSource.volume = Mathf.Lerp(from, to, elapsedTime.Interpolation(SmoothType.EaseOutWithSin));
            yield return null;
        }
    }
    IEnumerator VolumeFadeIn(float targetValue, float durationTime){
        float from = 0;
        float to = targetValue;

        float elapsedTime = 0f;
        while (elapsedTime <= 1){
            elapsedTime += Time.deltaTime / durationTime;
            musicSource.volume = Mathf.Lerp(from, to, elapsedTime.Interpolation(SmoothType.EaseInWithCos));
            yield return null;
        }
    }
    #endregion

    #region Legacy
    IEnumerator VoluemDecrease()
    {
        float voluem = musicSource.volume;
        while (voluem > 0)
        {
            voluem -= 0.1f;
            musicSource.volume = voluem;
            yield return new WaitForSeconds(0.1f);
        }
        musicSource.volume = 0;
    }
    IEnumerator VoluemIncrease(float to)
    {
        float volume = 0;
        while (musicSource.volume < to)
        {
            volume += 0.01f;
            musicSource.volume = volume;
            yield return new WaitForSeconds(0.05f);
        }
        musicSource.volume = to;
    }
    #endregion
}
