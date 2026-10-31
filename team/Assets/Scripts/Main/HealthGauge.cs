using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using DG.Tweening;

public class HealthGauge : MonoBehaviour
{
    [SerializeField] private Image healthImage;
    [SerializeField] private Image burnImage;

    //指定した時間で変動
    public float duration = 0.5f;
    //画面の揺れの強さ
    public float strength = 20f;
    //画面の揺れの具合
    public int vibrate = 100;

    public float debugDamageRate = 0.1f;

    private float currentRate = 1f;

    private void Start()
    {
        SetGauge(1f);
    }

    public void SetGauge(float value)
    {
        // DoTweenを連結して動かす
        healthImage.DOFillAmount(value, duration)
            .OnComplete(() =>
            {
                burnImage
                    .DOFillAmount(value, duration / 2f)
                    .SetDelay(0.5f);
            });
        transform.DOShakePosition(
            duration / 2f,
            strength, vibrate);

        currentRate = value;
    }

    public void TakeDamage(float rate)
    {
        SetGauge(currentRate - rate);
    }

    private void Update()
    {
        // Jキーでダメージ
        if (Keyboard.current.jKey.wasPressedThisFrame)
        {
            TakeDamage(debugDamageRate);
        }

        //// Aキーで画面を揺らす
        //if (Keyboard.current.aKey.wasPressedThisFrame)
        //{
        //    transform.DOShakePosition(
        //        duration / 2f,
        //        strength,
        //        vibrate);
        //}
    }
}