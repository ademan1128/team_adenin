
using UnityEngine;

public class ParryComboManager : MonoBehaviour
{
    [Header("参照")]

    // パリィ判定を管理しているスクリプト
    [SerializeField] private EnemyParryReceiver enemyParryReceiver;


    [Header("コンボ設定")]

    // 現在の連続成功回数
    public int ComboCount { get; private set; }


    [Header("必殺ゲージ設定")]

    // 必殺ゲージの最大値
    [SerializeField] private float maxSpecialGauge = 100f;

    // パリィ成功時のゲージ上昇量
    [SerializeField] private float specialGaugeIncrease = 10f;

    // 現在の必殺ゲージ
    public float SpecialGauge { get; private set; }

    // 必殺ゲージの割合（UI表示用）
    public float SpecialGaugeNormalized => SpecialGauge / maxSpecialGauge;


    // オブジェクトが有効になったとき
    private void OnEnable()
    {
        // パリィ成功・失敗イベントを登録
        if (enemyParryReceiver != null)
        {
            enemyParryReceiver.OnParrySuccess += OnParrySuccess;
            enemyParryReceiver.OnParryFailure += OnParryFailure;
        }
    }


    // オブジェクトが無効になったとき
    private void OnDisable()
    {
        // イベントの登録を解除
        if (enemyParryReceiver != null)
        {
            enemyParryReceiver.OnParrySuccess -= OnParrySuccess;
            enemyParryReceiver.OnParryFailure -= OnParryFailure;
        }
    }


    // パリィ成功時
    private void OnParrySuccess()
    {
        // コンボ数を増やす
        ComboCount++;

        Debug.Log("パリィコンボ：" + ComboCount);

        // 2連続成功から必殺ゲージを増やす
        if (ComboCount >= 2)
        {
            AddSpecialGauge();
        }
    }


    // 必殺ゲージを増やす
    private void AddSpecialGauge()
    {
        // ゲージが最大値を超えないようにする
        SpecialGauge = Mathf.Clamp(
            SpecialGauge + specialGaugeIncrease,
            0f,
            maxSpecialGauge
        );

        Debug.Log("必殺ゲージ：" + SpecialGauge);
    }


    // パリィ失敗時
    private void OnParryFailure()
    {
        ResetCombo();
    }


    // コンボリセット
    public void ResetCombo()
    {
        ComboCount = 0;

        Debug.Log("コンボリセット！");
    }
}
