
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using DG.Tweening;

public class HealthGauge : MonoBehaviour
{
    [SerializeField] private Image healthImage;
    [SerializeField] private Image burnImage;

    // 指定した時間で変動
    public float duration = 0.5f;

    // 画面の揺れの強さ
    public float strength = 20f;

    // 画面の揺れの具合
    public int vibrate = 100;

    // デバッグ用のダメージ量
    public float debugDamageRate = 0.1f;

    // 現在のHP割合
    private float currentRate = 1f;

    [Header("パリィ失敗時のダメージ")]
    // 敵のパリィ判定スクリプト
    [SerializeField] private EnemyParryReceiver enemyParryReceiver;

    // パリィを見逃したときのHP減少割合
    [SerializeField] private float missedParryDamageRate;

    private void Start()
    {
        SetGauge(1f);
    }

    // HPゲージの変動
    public void SetGauge(float value)
    {
        value = Mathf.Clamp01(value);

        // DoTweenを連結して動かす
        healthImage.DOFillAmount(value, duration)
            .OnComplete(() =>
            {
                burnImage
                    .DOFillAmount(value, duration / 2f)
                    .SetDelay(0.5f);
            });

        // 画面を揺らす
        transform.DOShakePosition(
            duration / 2f,
            strength,
            vibrate
        );

        currentRate = value;
    }

    // オブジェクトが有効になったとき
    private void OnEnable()
    {
        if (enemyParryReceiver != null)
        {
            enemyParryReceiver.OnParryFailure += OnParryFailure;
        }
    }

    // オブジェクトが無効になったとき
    private void OnDisable()
    {
        if (enemyParryReceiver != null)
        {
            enemyParryReceiver.OnParryFailure -= OnParryFailure;
        }
    }

    // パリィを見逃したとき
    private void OnParryFailure()
    {
        TakeDamage(missedParryDamageRate);

        Debug.Log("パリィ失敗！Playerがダメージを受けた！");
    }

    // ダメージを受ける
    public void TakeDamage(float rate)
    {
        SetGauge(currentRate - rate);
    }

    // デバッグ用のキー入力
    private void Update()
    {
        // Jキーでダメージを受ける
        if (Keyboard.current != null &&
            Keyboard.current.jKey.wasPressedThisFrame)
        {
            TakeDamage(debugDamageRate);
        }
    }
}
