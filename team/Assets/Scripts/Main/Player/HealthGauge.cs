
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

    public float duration = 0.5f;
    public float strength = 20f;
    public int vibrate = 100;

    public float debugDamageRate = 0.1f;

    private float currentRate = 1f;

    [Header("パリィ失敗時のダメージ")]
    [SerializeField] private PhaseManager phaseManager;

    [SerializeField] private float missedParryDamageRate = 0.1f;

    [Header("撃破後のシーン設定")]
    [SerializeField] private string nextScene = "GameOverScene";

    [Header("ゲームオーバー設定")]
    [SerializeField] private float gameOverDelay = 3f;

    private bool isGameOver = false;

    // 現在イベント登録しているReceiver
    private EnemyParryReceiver currentReceiver;

    private void Start()
    {
        SetGauge(1f);

        if (phaseManager == null)
        {
            phaseManager = FindObjectOfType<PhaseManager>();
        }

        UpdateReceiver();
    }

    private void Update()
    {
        if (Keyboard.current != null &&
            Keyboard.current.jKey.wasPressedThisFrame)
        {
            TakeDamage(debugDamageRate);
        }

        // 現在の敵のReceiverを確認
        UpdateReceiver();
    }

    // Receiverの切り替え
    private void UpdateReceiver()
    {
        if (phaseManager == null)
            return;

        EnemyStaminaController enemy = phaseManager.CurrentEnemy;

        EnemyParryReceiver newReceiver = null;

        if (enemy != null)
        {
            newReceiver = enemy.GetComponent<EnemyParryReceiver>();
        }

        // 同じReceiverなら何もしない
        if (currentReceiver == newReceiver)
            return;

        // 古いイベント登録を解除
        if (currentReceiver != null)
        {
            currentReceiver.OnParryFailure -= OnParryFailure;
        }

        // 新しいReceiverを設定
        currentReceiver = newReceiver;

        // 新しいイベントを登録
        if (currentReceiver != null)
        {
            currentReceiver.OnParryFailure += OnParryFailure;

            Debug.Log(
                "HP管理：イベント登録先を変更しました → "
                + currentReceiver.name
            );
        }
    }

    // パリィを見逃したとき
    private void OnParryFailure()
    {
        TakeDamage(missedParryDamageRate);

        Debug.Log("パリィ失敗！Playerがダメージを受けた！");
    }

    // HPゲージの変動
    public void SetGauge(float value)
    {
        value = Mathf.Clamp01(value);

        healthImage.DOFillAmount(value, duration)
            .OnComplete(() =>
            {
                burnImage
                    .DOFillAmount(value, duration / 2f)
                    .SetDelay(0.5f);
            });

        transform.DOShakePosition(
            duration / 2f,
            strength,
            vibrate
        );

        currentRate = value;
    }

    // ダメージを受ける
    public void TakeDamage(float rate)
    {
        if (isGameOver)
            return;

        SetGauge(currentRate - rate);

        if (currentRate <= 0f)
        {
            isGameOver = true;

            StartCoroutine(GameOver());
        }
    }

    // ゲームオーバー処理
    private IEnumerator GameOver()
    {
        Debug.Log("プレイヤーが倒れた！");

        yield return new WaitForSeconds(gameOverDelay);

        SceneManager.LoadScene(nextScene);
    }

    private void OnDestroy()
    {
        if (currentReceiver != null)
        {
            currentReceiver.OnParryFailure -= OnParryFailure;
        }

        healthImage?.DOKill();
        burnImage?.DOKill();
        transform.DOKill();
    }
}
