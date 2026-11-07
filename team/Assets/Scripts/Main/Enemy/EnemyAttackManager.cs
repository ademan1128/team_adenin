using UnityEngine;

[CreateAssetMenu(fileName = "EnemyAttackData", menuName = "Enemy/Enemy Attack")]
public class EnemyAttackManager : ScriptableObject
{
    public string AttackName;
    [TextArea]
    public string description;

    public Sprite icon;

    public RuntimeAnimatorController animatorController;
    //public Animator animator;

    public float weak;
    public float medium;
    public float strength;
}
