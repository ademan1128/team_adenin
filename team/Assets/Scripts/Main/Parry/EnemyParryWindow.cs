using UnityEngine;

public class EnemyParryWindow : MonoBehaviour
{
    public bool CanBeParried { get; private set; }

    // パリィ受付開始
    public void StartParryWindow()
    {
        CanBeParried = true;
        Debug.Log("パリィ受付開始！");
    }

    // パリィ受付終了
    public void EndParryWindow()
    {
        CanBeParried = false;
        Debug.Log("パリィ受付終了！");
    }
}