using UnityEngine;
using System.Collections;

public class EnemyAttackController : MonoBehaviour
{
    [Header("攻撃設定")]
    [SerializeField] private float attackInterval;

    [Header("参照")]
    [SerializeField] private EnemyParryReceiver parryReceiver;
    [SerializeField] private Animator animator;

    [Header("攻撃データ")]
    [SerializeField] private EnemyAttackData attackData;

    // 現在の攻撃のパリィ受付フレーム数
    private int currentParryFrames;

    // 攻撃までの経過時間
    private float timer;

    // 現在攻撃中か
    private bool isAttacking;

    private void Update()
    {
        // 攻撃中は次の攻撃を開始しない
        if (isAttacking)
        {
            return;
        }

        // 次の攻撃までタイマーを進める
        timer += Time.deltaTime;

        // 攻撃間隔に達したら攻撃開始
        if (timer >= attackInterval)
        {
            StartAttack();
        }
    }

    // 攻撃開始
    private void StartAttack()
    {
        isAttacking = true;
        timer = 0f;

        // 攻撃方法をランダムに選択
        int randomAttack = Random.Range(0, 3);

        switch (randomAttack)
        {
            case 0:
                animator.SetTrigger("Attack");
                currentParryFrames = attackData.attackParryFrames;
                break;

            case 1:
                animator.SetTrigger("PounceAttack");
                currentParryFrames = attackData.pounceAttackParryFrames;
                break;

            case 2:
                animator.SetTrigger("DownAttack");
                currentParryFrames = attackData.downAttackParryFrames;
                break;
        }

        Debug.Log("敵の攻撃開始！");
    }

    // Animation Event：パリィ受付開始
    public void StartParryEvent()
    {
        parryReceiver.StartParryWindow();

        // 指定時間後にパリィ受付を終了する
        StartCoroutine(EndParryAfterTime());
    }

    // パリィ受付時間を管理
    private IEnumerator EndParryAfterTime()
    {
        // フレーム数を秒数に変換
        float duration = currentParryFrames / attackData.frameRate;

        // アニメーション速度に合わせて受付時間も延長
        duration /= animator.speed;

        yield return new WaitForSeconds(duration);

        parryReceiver.EndParryWindow();
    }

    // Animation Event：攻撃アニメーション終了
    public void FinishAttackEvent()
    {
        isAttacking = false;

        Debug.Log("敵の攻撃終了！");
    }

    // 敵のアニメーションをスローにする
    public void SetSlowMotion(bool isSlow)
    {
        if (isSlow)
        {
            animator.speed = 0.5f;
        }
        else
        {
            animator.speed = 1.0f;
        }
    }
    [ContextMenu("Test/Start Slow")]
    private void TestStartSlow()
    {
        SetSlowMotion(true);  // スロー開始
    }
    [ContextMenu("Test/Finish Slow")]
    private void TestFinishSlow()
    {
        SetSlowMotion(false); // スロー終了
    }
}