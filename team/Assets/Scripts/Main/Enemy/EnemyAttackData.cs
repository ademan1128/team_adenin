using UnityEngine;

[CreateAssetMenu(fileName = "EnemyAttackData", menuName = "Game/Enemy Attack Data")]
public class EnemyAttackData : ScriptableObject
{
    [Header("ƒtƒŒ[ƒ€ƒŒ[ƒg")]
    public float frameRate;

    [Header("’ÊíUŒ‚")]
    public int attackParryFrames;

    [Header("”ò‚Ñ‚©‚©‚èUŒ‚")]
    public int pounceAttackParryFrames;

    [Header("‰º’iUŒ‚")]
    public int downAttackParryFrames;
}