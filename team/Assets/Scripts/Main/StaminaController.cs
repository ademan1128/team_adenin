using UnityEngine;

public class StaminaController : MonoBehaviour
{
    [Header("スタミナ設定")]
    //スタミナ最大値
    [SerializeField] private float maxStamina = 100f;
    //スタミナ消費量
    [SerializeField] private float swingCost = 10f;
    //スタミナ回復量
    [SerializeField] private float recoverySpeed = 5f;

    public float CurrentStamina { get; private set; }

    public float StaminaNormalized => CurrentStamina / maxStamina;

    private void Start()
    {
        CurrentStamina = maxStamina;
    }

    private void Update()
    {
        // スタミナ回復
        CurrentStamina += recoverySpeed * Time.deltaTime;

        CurrentStamina = Mathf.Clamp(
            CurrentStamina,
            0f,
            maxStamina
        );
    }

    public bool ConsumeStamina()
    {
        if (CurrentStamina < swingCost)
        {
            return false;
        }

        CurrentStamina -= swingCost;

        return true;
    }
}
