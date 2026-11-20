using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class EnemyStaminaController : MonoBehaviour
{
    [Header("敵のスタミナ設定")]

    [SerializeField] private float maxStamina = 100f;

    [SerializeField] private float parryDamage = 10f;

    [SerializeField] private int maxPhases = 3;


    [Header("段階移行設定")]

    [SerializeField] private float recoveryDelay = 3f;


    [Header("デバッグ設定")]

    [SerializeField] private float debugDamage = 20f;

    [SerializeField] private Key debugDamageKey = Key.K;

    [SerializeField] private Key debugNextPhaseKey = Key.N;


    [Header("現在の状態")]

    public float CurrentStamina { get; private set; }

    public float StaminaNormalized =>
        maxStamina > 0f ? CurrentStamina / maxStamina : 0f;

    public bool IsBroken { get; private set; }

    public int CurrentPhase { get; private set; } = 1;

    public int RemainingPhases =>
        Mathf.Max(0, maxPhases - CurrentPhase + 1);

    public int MaxPhases => maxPhases;

    // 二重実行防止
    private bool isDead = false;

    // イベント
    public event Action<float> OnStaminaChanged;

    public event Action<int> OnPhaseChanged;

    public event Action OnStanceBroken;

    public event Action OnDeath;

    // 初期化処理
    private void Start()
    {
        maxPhases = Mathf.Max(1, maxPhases);

        CurrentStamina = maxStamina;

        OnStaminaChanged?.Invoke(CurrentStamina);

        OnPhaseChanged?.Invoke(CurrentPhase);

        // PhaseManagerに敵を登録
        PhaseManager manager =
            FindObjectOfType<PhaseManager>();

        if (manager != null)
        {
            manager.RegisterEnemy(this);

            Debug.Log("PhaseManagerに敵を登録しました");
        }
        else
        {
            Debug.LogError("PhaseManagerが見つかりません！");
        }
    }

    // 毎フレーム実行
    private void Update()
    {
        if (Keyboard.current == null)
        {
            return;
        }

        if (Keyboard.current[debugDamageKey].wasPressedThisFrame)
        {
            TakeDebugDamage();
        }

        if (Keyboard.current[debugNextPhaseKey].wasPressedThisFrame)
        {
            NextPhase();
        }
    }

    // デバッグ用ダメージ処理
    private void TakeDebugDamage()
    {
        ApplyDamage(debugDamage);

        Debug.Log("デバッグダメージ：" + debugDamage);
    }

    // パリィ成功時
    public void TakeParryDamage()
    {
        ApplyDamage(parryDamage);
    }

    // スタミナ減少処理
    private void ApplyDamage(float damage)
    {
        if (IsBroken || isDead)
        {
            return;
        }

        CurrentStamina -= damage;

        CurrentStamina = Mathf.Clamp(
            CurrentStamina,
            0f,
            maxStamina
        );

        OnStaminaChanged?.Invoke(CurrentStamina);

        Debug.Log("敵のスタミナ：" + CurrentStamina);

        if (CurrentStamina <= 0f)
        {
            BreakStance();
        }
    }

    // 体勢を崩す処理
    private void BreakStance()
    {
        IsBroken = true;

        Debug.Log("敵の体勢が崩れた！");

        OnStanceBroken?.Invoke();

        if (CurrentPhase < maxPhases)
        {
            StartCoroutine(RecoverStamina());
        }
        else
        {
            Die();
        }
    }

    // スタミナ回復処理
    private IEnumerator RecoverStamina()
    {
        Debug.Log("3秒後に次の段階へ移行します");

        yield return new WaitForSeconds(recoveryDelay);

        NextPhase();
    }

    // 次の段階へ移行
    public void NextPhase()
    {
        if (!IsBroken || isDead)
        {
            return;
        }

        if (CurrentPhase >= maxPhases)
        {
            Die();
            return;
        }

        CurrentPhase++;

        CurrentStamina = maxStamina;

        IsBroken = false;

        OnPhaseChanged?.Invoke(CurrentPhase);

        OnStaminaChanged?.Invoke(CurrentStamina);

        Debug.Log("敵のスタミナ段階：" + CurrentPhase);

        Debug.Log("敵のスタミナが最大まで回復！");
    }

    // 敵の撃破処理
    private void Die()
    {
        if (isDead)
        {
            return;
        }

        isDead = true;

        // バトル管理側に撃破を通知
        OnDeath?.Invoke();

        Debug.Log("敵を撃破！");
    }
}
