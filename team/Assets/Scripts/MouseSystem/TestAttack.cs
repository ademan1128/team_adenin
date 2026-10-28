using UnityEngine;
using UnityEngine.InputSystem;

public class TestAttack : MonoBehaviour
{
    [SerializeField] private GameObject enemyAttackHitbox;

    private void Update()
    {
        if (Keyboard.current == null)
        {
            return;
        }

        bool isAttacking = Keyboard.current.spaceKey.isPressed;

        enemyAttackHitbox.SetActive(isAttacking);
    }
}