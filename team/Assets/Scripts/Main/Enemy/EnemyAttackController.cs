using UnityEngine;

public class EnemyAttackController : MonoBehaviour
{
    [Header("攻撃設定")]
    [SerializeField] private float attackInterval = 3f;

    [Header("参照")]
    [SerializeField] private EnemyParryReceiver parryReceiver;
    [SerializeField] private Animator animator;

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

        Debug.Log("敵の攻撃開始！");

        // 攻撃アニメーションを再生
        animator.SetTrigger("Attack");
    }

    // Animation Event：パリィ受付開始
    public void StartParryEvent()
    {
        parryReceiver.StartParryWindow();
    }

    // Animation Event：パリィ受付終了
    public void EndParryEvent()
    {
        parryReceiver.EndParryWindow();
    }

    // Animation Event：攻撃アニメーション終了
    public void FinishAttackEvent()
    {
        isAttacking = false;

        Debug.Log("敵の攻撃終了！");
    }
}
