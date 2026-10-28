using UnityEngine;

public class EnemyAttackHitbox : MonoBehaviour
{
    [Header("マウス入力")]
    [SerializeField]
    private MouseInputDetector mouseInputDetector;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        if (mouseInputDetector == null)
        {
            Debug.LogWarning(
                "MouseInputDetector が設定されていません。"
            );
            return;
        }
        // パリィ受付時間内か確認
        if (mouseInputDetector.IsParryWindowActive)
        {
            Debug.Log("パリィ成功！");
        }
        else
        {
            Debug.Log("パリィ失敗！");
        }
    }
}