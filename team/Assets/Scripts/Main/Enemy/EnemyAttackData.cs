using UnityEngine;

[CreateAssetMenu(fileName = "EnemyAttackData", menuName = "Game/Enemy Attack Data")]
public class EnemyAttackData : ScriptableObject
{
    [Header("フレームレート")]
    public float frameRate;

    [Header("通常攻撃")]
    public int attackParryFrames;

    [Header("飛びかかり攻撃")]
    public int pounceAttackParryFrames;
}