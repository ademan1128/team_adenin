using UnityEngine;
using System;

public class EnemyParryReceiver : MonoBehaviour
{
    // 敵のスタミナ管理
    [SerializeField] private EnemyStaminaController enemyStamina;

    [Header("フェーズ管理")]
    [SerializeField] private PhaseManager phaseManager;

    // パリィ受付中か
    public bool CanBeParried { get; private set; }

    // 現在の受付でパリィ成功済みか
    private bool hasBeenParried;

    // パリィ成功時の通知
    public event Action OnParrySuccess;

    // パリィ失敗時の通知
    public event Action OnParryFailure;


    private void Awake()
    {
        if (phaseManager == null)
        {
            phaseManager = FindObjectOfType<PhaseManager>();
        }
    }


    // パリィ受付開始
    public void StartParryWindow()
    {
        if (CanBeParried)
        {
            Debug.LogWarning("既にパリィ受付中です！");
            return;
        }

        CanBeParried = true;
        hasBeenParried = false;

        Debug.Log("パリィ受付開始！");
    }


    // パリィ受付終了
    public void EndParryWindow()
    {
        if (!CanBeParried)
            return;

        CanBeParried = false;

        if (!hasBeenParried)
        {
            Debug.Log("パリィ失敗！見逃し！");
            OnParryFailure?.Invoke();
        }

        Debug.Log("パリィ受付終了！");
    }


    // パリィ受付状態のリセット
    public void ResetParry()
    {
        CanBeParried = false;
        hasBeenParried = false;
    }


    // パリィ判定
    public bool TryParry()
    {
        if (!CanBeParried || hasBeenParried)
            return false;

        if (phaseManager == null)
        {
            Debug.LogError("PhaseManagerが設定されていません！");
            return false;
        }

        // 現在の敵を取得
        EnemyStaminaController enemy = phaseManager.CurrentEnemy;

        if (enemy == null)
        {
            Debug.LogWarning("現在の敵が設定されていません！");
            return false;
        }

        if (enemy.IsBroken)
        {
            Debug.LogWarning("敵は現在体勢を崩しています！");
            return false;
        }

        hasBeenParried = true;

        Debug.Log("パリィ成功！対象：" + enemy.name);

        // 現在の敵のスタミナを減らす
        enemy.TakeParryDamage();

        // パリィ成功イベントを通知
        OnParrySuccess?.Invoke();

        return true;
    }


    // デバッグ用
    [ContextMenu("Test/Start Parry Window")]
    private void TestStartParryWindow()
    {
        StartParryWindow();
    }

    [ContextMenu("Test/Try Parry")]
    private void TestTryParry()
    {
        TryParry();
    }

    [ContextMenu("Test/End Parry Window")]
    private void TestEndParryWindow()
    {
        EndParryWindow();
    }
}
