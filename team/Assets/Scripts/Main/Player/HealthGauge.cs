using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using DG.Tweening;
using UnityEngine.SceneManagement;
using System.Collections;

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

    [Header("撃破後のシーン設定")]
    [SerializeField] private string nextScene = "GameOverScene";

    [Header("ゲームオーバー設定")]
    [SerializeField] private float gameOverDelay = 3f;

    private bool isGameOver = false;

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
        // ゲームオーバー後は処理しない
        if (isGameOver)
        {
            return;
        }

        SetGauge(currentRate - rate);

        // HPが0になったらゲームオーバー
        if (currentRate <= 0f)
        {
            isGameOver = true;

            // コルーチンを開始
            StartCoroutine(GameOver());
        }
    }

    //ゲームオーバー処理
    private IEnumerator GameOver()
    {
        Debug.Log("プレイヤーが倒れた！");

        // 指定した秒数だけ待機
        yield return new WaitForSeconds(gameOverDelay);

        // ゲームオーバーシーンへ移動
        SceneManager.LoadScene(nextScene);
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
