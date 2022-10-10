using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class SettingsMenu : MonoBehaviour
{
    public static SettingsMenu Instance;

    [SerializeField] private GameObject _setingsTab1;
    

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        LevelLoader.Instance.MusicSource.clip = _audioClips[0];
        //LevelLoader.Instance.MusicSource.Play();
    }

    private void Update()
    {
        if (Input.GetButtonDown("Cancel")) Enable();
        if (!LevelLoader.Instance.MusicSource.isPlaying && !_pause) SkipTrack();
    }

    public void HideSettings()
    {
        _setingsTab1.SetActive(false);
    }
    public void Enable()
    {
        if (Instance._setingsTab1.activeSelf)
        {
            Instance._setingsTab1.SetActive(false);
        } else Instance._setingsTab1.SetActive(true);
    }
    
    #region MusicPlayer
    [Header("MUSIC_PLAYER")]
    
    [SerializeField] private AudioSource _ActionAudioSourceTest;
    [SerializeField] private List<AudioClip> _audioClips;
    [SerializeField] private int _currentClip = 0;
    private bool _pause = false;
    public void StopMusic()
    {
        LevelLoader.Instance.MusicSource.Pause();
        _pause = true;
    }

    public void PlayMusic()
    {
        LevelLoader.Instance.MusicSource.UnPause();
        _pause = false;
    }

    public void SkipTrack()
    {
        if (_currentClip < _audioClips.Count-1)
        {
            _currentClip++;
            LevelLoader.Instance.MusicSource.Stop();
            LevelLoader.Instance.MusicSource.clip = _audioClips[_currentClip];
            LevelLoader.Instance.MusicSource.Play();
        }
        else
        {
            _currentClip = 0;
            LevelLoader.Instance.MusicSource.Stop();
            LevelLoader.Instance.MusicSource.clip = _audioClips[_currentClip];
            LevelLoader.Instance.MusicSource.Play();
        }
    }

    public void PrevTrack()
    {
        if (_currentClip > 0)
        {
            _currentClip--;
            LevelLoader.Instance.MusicSource.Stop();
            LevelLoader.Instance.MusicSource.clip = _audioClips[_currentClip];
            LevelLoader.Instance.MusicSource.Play();
        }
        else
        {
            _currentClip = _audioClips.Count-1;
            LevelLoader.Instance.MusicSource.Stop();
            LevelLoader.Instance.MusicSource.clip = _audioClips[_currentClip];
            LevelLoader.Instance.MusicSource.Play();
        }
    }
    #endregion

    #region AudioSliders
    [Header("AUDIO_SLIDERS")]
    
    public float _musicVolume = 0.5f;
    public float _actionVolume = 0.5f;
    [SerializeField] private Slider _musicSlider;
    [SerializeField] private Slider _actionSlider;
    public void ChangeMusicVoulume()
    {
        _musicVolume = _musicSlider.value/10;
        LevelLoader.Instance.MusicSource.volume = _musicVolume;
    }
    
    public void ChangeActionVoulume()
    {
        _actionVolume = _actionSlider.value/10;
        LevelLoader.Instance.SelectionAudio.volume = _actionVolume;
        if (PlayerController.Instance)
        {
            PlayerController.Instance._audioSourceOthers.volume = _actionVolume;
            PlayerController.Instance._audioSourcePlayerActions.volume = _actionVolume;
        }
        
        _ActionAudioSourceTest.volume = _actionVolume;
        _ActionAudioSourceTest.Play();
    }
    #endregion
    
}
