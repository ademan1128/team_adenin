using UnityEngine;

public class EnemyAttackController : MonoBehaviour
{
    [Header("UŒ‚İ’è")]
    [SerializeField] private float attackInterval;

    [SerializeField] private float attackDuration;

    [Header("QÆ")]
    [SerializeField] private EnemyParryReceiver parryReceiver;

    // UŒ‚‚Ü‚Å‚ÌŒo‰ßŠÔ
    private float timer;

    // Œ»İUŒ‚’†‚©
    private bool isAttacking;

    private void Update()
    {
        // UŒ‚’†‚Ìˆ—
        if (isAttacking)
        {
            timer += Time.deltaTime;

            // UŒ‚ŠÔ‚ªI—¹‚µ‚½‚çó•t‚ğI—¹
            if (timer >= attackDuration)
            {
                EndAttack();
            }

            return;
        }

        // Ÿ‚ÌUŒ‚‚Ü‚Åƒ^ƒCƒ}[‚ği‚ß‚é
        timer += Time.deltaTime;

        // UŒ‚ŠÔŠu‚É’B‚µ‚½‚çUŒ‚ŠJn
        if (timer >= attackInterval)
        {
            StartAttack();
        }
    }

    // UŒ‚ŠJn
    private void StartAttack()
    {
        isAttacking = true;
        timer = 0f;

        Debug.Log("“G‚ÌUŒ‚ŠJnI");

        parryReceiver.StartParryWindow();
    }

    // UŒ‚I—¹
    private void EndAttack()
    {
        isAttacking = false;
        timer = 0f;

        Debug.Log("“G‚ÌUŒ‚I—¹I");

        parryReceiver.EndParryWindow();
    }
}
