
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class EnemyStaminaController : MonoBehaviour
{
    [Header("敵のスタミナ設定")]

    // 敵のスタミナ最大値
    [SerializeField] private float maxStamina = 100f;

    // パリィ成功時のスタミナ減少量
    [SerializeField] private float parryDamage = 10f;

    // 敵のスタミナ段階数
    [SerializeField] private int maxPhases = 3;


    [Header("段階移行設定")]

    // 次の段階へ移行するまでの待機時間
    [SerializeField] private float recoveryDelay = 3f;


    [Header("デバッグ設定")]

    // デバッグ用ダメージ量
    [SerializeField] private float debugDamage = 20f;

    // デバッグダメージを与えるキー
    [SerializeField] private Key debugDamageKey = Key.K;

    // 次の段階へ進むデバッグキー
    [SerializeField] private Key debugNextPhaseKey = Key.N;


    [Header("現在の状態")]

    // 現在のスタミナ
    public float CurrentStamina { get; private set; }

    // スタミナの割合（UI表示用）
    public float StaminaNormalized =>
        maxStamina > 0f ? CurrentStamina / maxStamina : 0f;

    // 敵のスタミナが0になったか
    public bool IsBroken { get; private set; }

    // 現在の段階数
    public int CurrentPhase { get; private set; } = 1;

    // 残り段階数
    public int RemainingPhases =>
        Mathf.Max(0, maxPhases - CurrentPhase + 1);

    // 最大段階数
    public int MaxPhases => maxPhases;


    // スタミナが変化したときに通知するイベント
    public event Action<float> OnStaminaChanged;

    // 段階が変化したときに通知するイベント
    public event Action<int> OnPhaseChanged;

    // 体勢を崩したときに通知するイベント
    public event Action OnStanceBroken;

    // 最終段階を突破したときに通知するイベント
    public event Action OnDeath;


    // 初期化処理
    private void Start()
    {
        // 最大段階数が1未満にならないようにする
        maxPhases = Mathf.Max(1, maxPhases);

        // スタミナを最大値に設定
        CurrentStamina = maxStamina;

        // 初期スタミナをUIに通知
        OnStaminaChanged?.Invoke(CurrentStamina);

        // 初期段階をUIに通知
        OnPhaseChanged?.Invoke(CurrentPhase);
    }


    // 毎フレーム実行
    private void Update()
    {
        // キーボードが接続されていない場合は処理を終了
        if (Keyboard.current == null)
        {
            return;
        }

        // Kキーを押したらデバッグダメージ
        if (Keyboard.current[debugDamageKey].wasPressedThisFrame)
        {
            TakeDebugDamage();
        }

        // Nキーを押したら段階移行（デバッグ用）
        if (Keyboard.current[debugNextPhaseKey].wasPressedThisFrame)
        {
            NextPhase();
        }
    }


    // デバッグ用ダメージ処理
    private void TakeDebugDamage()
    {
        // デバッグダメージを与える
        ApplyDamage(debugDamage);

        Debug.Log("デバッグダメージ：" + debugDamage);
    }


    // パリィ成功時に呼び出す
    public void TakeParryDamage()
    {
        // パリィ成功時のダメージを与える
        ApplyDamage(parryDamage);
    }


    // スタミナ減少処理
    private void ApplyDamage(float damage)
    {
        // 体勢崩し中はダメージを受けない
        if (IsBroken)
        {
            return;
        }

        // スタミナを減らす
        CurrentStamina -= damage;

        // スタミナが0未満にならないように制限
        CurrentStamina = Mathf.Clamp(
            CurrentStamina,
            0f,
            maxStamina
        );

        // スタミナの変化をUIに通知
        OnStaminaChanged?.Invoke(CurrentStamina);

        Debug.Log("敵のスタミナ：" + CurrentStamina);

        // スタミナが0になったら体勢を崩す
        if (CurrentStamina <= 0f)
        {
            BreakStance();
        }
    }


    // 体勢を崩す処理
    private void BreakStance()
    {
        // 体勢を崩した状態にする
        IsBroken = true;

        Debug.Log("敵の体勢が崩れた！");

        // 体勢崩しイベントを通知
        OnStanceBroken?.Invoke();

        // 次の段階がある場合
        if (CurrentPhase < maxPhases)
        {
            // 3秒後にスタミナを回復する処理を開始
            StartCoroutine(RecoverStamina());
        }
        else
        {
            // 最終段階なら撃破
            Die();
        }
    }


    // スタミナ回復処理
    private IEnumerator RecoverStamina()
    {
        Debug.Log("3秒後に次の段階へ移行します");

        // 指定した秒数だけ待機
        yield return new WaitForSeconds(recoveryDelay);

        // 次の段階へ移行
        NextPhase();
    }


    // 次の段階へ移行する処理
    public void NextPhase()
    {
        // 体勢を崩していなければ移行しない
        if (!IsBroken)
        {
            return;
        }

        // 最終段階なら撃破処理
        if (CurrentPhase >= maxPhases)
        {
            Die();
            return;
        }

        // 現在の段階を1つ進める
        CurrentPhase++;

        // スタミナを最大値まで回復
        CurrentStamina = maxStamina;

        // 体勢崩し状態を解除
        IsBroken = false;

        // 段階変更をUIに通知
        OnPhaseChanged?.Invoke(CurrentPhase);

        // スタミナ変更をUIに通知
        OnStaminaChanged?.Invoke(CurrentStamina);

        Debug.Log("敵のスタミナ段階：" + CurrentPhase);
        Debug.Log("敵のスタミナが最大まで回復！");
    }


    // 敵の撃破処理
    private void Die()
    {
        // 撃破イベントを通知
        OnDeath?.Invoke();

        Debug.Log("敵を撃破！");
    }
}
