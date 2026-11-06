
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioVolumeSettings : MonoBehaviour
{
    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer audioMixer; // 音量を管理するAudioMixer

    [Header("Volume Sliders")]
    [SerializeField] private Slider masterSlider; // マスター音量スライダー
    [SerializeField] private Slider bgmSlider;    // BGM音量スライダー
    [SerializeField] private Slider seSlider;     // SE音量スライダー

    private void Start()
    {
        // ゲーム開始時に各スライダーの音量を100%に設定
        masterSlider.SetValueWithoutNotify(1f);
        bgmSlider.SetValueWithoutNotify(1f);
        seSlider.SetValueWithoutNotify(1f);

        // スライダーの値が変更されたときに呼び出す処理を登録
        masterSlider.onValueChanged.AddListener(SetMasterVolume);
        bgmSlider.onValueChanged.AddListener(SetBGMVolume);
        seSlider.onValueChanged.AddListener(SetSEVolume);

        // 初期音量をAudioMixerに反映
        SetMasterVolume(1f);
        SetBGMVolume(1f);
        SetSEVolume(1f);
    }

    // マスター音量を変更する処理
    public void SetMasterVolume(float volume)
    {
        audioMixer.SetFloat("MasterVolume", ToDecibel(volume));
    }

    // BGM音量を変更する処理
    public void SetBGMVolume(float volume)
    {
        audioMixer.SetFloat("BGMVolume", ToDecibel(volume));
    }

    // SE音量を変更する処理
    public void SetSEVolume(float volume)
    {
        audioMixer.SetFloat("SEVolume", ToDecibel(volume));
    }

    // スライダーの音量値（0～1）をAudioMixer用のデシベル値に変換
    private float ToDecibel(float volume)
    {
        // 音量が0の場合は完全な無音に近い値を設定
        if (volume <= 0.0001f)
        {
            return -80f;
        }

        // 音量をデシベルに変換
        return Mathf.Log10(volume) * 20f;
    }
}
