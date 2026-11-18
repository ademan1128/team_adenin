using Cysharp.Threading.Tasks;
using UnityEngine;

public class SpecialMoveController : MonoBehaviour
{
    [Header("参照")]
    [SerializeField] private ParryComboManager parryComboManager;
    [SerializeField] private EnemyAttackController enemyAttackController;

    [Header("必殺技設定")]
    [SerializeField] private float consumeDuration = 3f;

    [Header("スロー設定")]
    [SerializeField] private float slowSpeed = 0.5f;

    // 必殺技発動
    public bool TryActivateSpecialMove()
    {
        // ゲージが足りなければ発動しない
        if (!parryComboManager.TryStartGaugeConsumption())
        {
            return false;
        }

        Debug.Log("必殺技発動！");

        // 敵をスローにする
        enemyAttackController.SetSlowMotion(true, slowSpeed);

        // ゲージ消費開始
        ConsumeSPOverTime().Forget();

        return true;
    }

    // 時間をかけてゲージを減らす
    private async UniTaskVoid ConsumeSPOverTime()
    {
        float startGauge = parryComboManager.SpecialGauge;
        float elapsedTime = 0f;

        while (elapsedTime < consumeDuration)
        {
            elapsedTime += Time.deltaTime;

            float t = Mathf.Clamp01(
                elapsedTime / consumeDuration
            );

            float currentGauge = Mathf.Lerp(
                startGauge,
                0f,
                t
            );

            parryComboManager.SetSpecialGauge(currentGauge);

            await UniTask.Yield(PlayerLoopTiming.Update);
        }

        // ゲージを0にする
        parryComboManager.FinishGaugeConsumption();

        // 敵の速度を元に戻す
        enemyAttackController.SetSlowMotion(false);

        Debug.Log("必殺技終了！");
    }
}