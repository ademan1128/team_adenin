using UnityEngine;

[CreateAssetMenu(fileName = "EnemyAttackData", menuName = "Game/Enemy Attack Data")]
public class EnemyAttackData : ScriptableObject
{
    [Header("パリィ受付設定")]
    public int parryWindowFrames_1;
    public int parryWindowFrames_2;

    [Header("フレームレート")]
    public float frameRate;
}