
using UnityEngine;

public class EnemyStaminaController : MonoBehaviour
{
    [Header("敵のスタミナ設定")]

    // 敵のスタミナ最大値
    [SerializeField] private float maxStamina = 100f;

    // パリィ成功時のスタミナ減少量
    [SerializeField] private float parryDamage = 10f;

    // 現在のスタミナ
    public float CurrentStamina { get; private set; }

    // スタミナの割合（UI表示用）
    public float StaminaNormalized => CurrentStamina / maxStamina;

    // 敵のスタミナが0になったか
    public bool IsBroken { get; private set; }

    private void Start()
    {
        // スタミナを最大値に設定
        CurrentStamina = maxStamina;
    }

    // パリィ成功時に呼び出す
    public void TakeParryDamage()
    {
        // すでに体勢を崩していたら何もしない
        if (IsBroken)
        {
            return;
        }

        // パリィ成功によってスタミナを減らす
        CurrentStamina -= parryDamage;

        // スタミナが0未満にならないように制限
        CurrentStamina = Mathf.Clamp(
            CurrentStamina,
            0f,
            maxStamina
        );

        Debug.Log("敵のスタミナ：" + CurrentStamina);

        // スタミナが0になったら体勢を崩す
        if (CurrentStamina <= 0f)
        {
            BreakStance();
        }
    }

    // 体勢を崩す動き
    private void BreakStance()
    {
        // 体勢を崩した状態にする
        IsBroken = true;

        Debug.Log("敵の体勢が崩れた！");
    }
}
