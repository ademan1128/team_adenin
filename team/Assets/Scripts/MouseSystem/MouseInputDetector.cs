using UnityEngine;
using UnityEngine.InputSystem;

public class MouseInputDetector : MonoBehaviour
{
    [Header("マウスの振り判定")]
    [SerializeField] private float swingSpeedThreshold = 50000f;

    [Header("パリィ受付時間")]
    [SerializeField] private float parryWindow = 0.2f;

    [Header("スイング再判定までの時間")]
    [SerializeField] private float swingCooldown = 0.2f;

    private float nextSwingTime;

    [Header("スタミナ管理")]
    [SerializeField] private StaminaController staminaController;

    [Header("フェーズ管理")]
    [SerializeField] private PhaseManager phaseManager;

    private Vector2 currentMousePosition;
    private Vector2 previousMousePosition;
    private Vector2 mouseDelta;
    private Vector2 mouseDirection;

    private float mouseSpeed;

    private bool wasSwinging;

    public bool IsSwinging { get; private set; }

    private float lastSwingTime = float.NegativeInfinity;

    public Vector2 MouseDirection => mouseDirection;

    public float MouseSpeed => mouseSpeed;

    public bool IsParryWindowActive =>
        Time.time - lastSwingTime <= parryWindow;

    [Header("SP管理")]
    [SerializeField] private SpecialMoveController specialMoveController;

    [Header("デバッグ専用")]
    [SerializeField] private GameObject debugbox;


    private void Start()
    {
        if (Mouse.current == null)
        {
            Debug.LogWarning("マウスが認識されていません");
            return;
        }

        if (phaseManager == null)
        {
            phaseManager = FindObjectOfType<PhaseManager>();
        }

        previousMousePosition = Mouse.current.position.ReadValue();
    }


    private void Update()
    {
        if (Mouse.current == null)
            return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            specialMoveController.TryActivateSpecialMove();
        }

        currentMousePosition = Mouse.current.position.ReadValue();

        mouseDelta = currentMousePosition - previousMousePosition;

        mouseSpeed = mouseDelta.magnitude /
            Mathf.Max(Time.deltaTime, 0.0001f);

        if (mouseDelta.sqrMagnitude > 0f)
        {
            mouseDirection = mouseDelta.normalized;
        }
        else
        {
            mouseDirection = Vector2.zero;
        }

        IsSwinging = mouseSpeed >= swingSpeedThreshold;

        if (IsSwinging && !wasSwinging && Time.time >= nextSwingTime)
        {
            nextSwingTime = Time.time + swingCooldown;

            if (staminaController.ConsumeStamina())
            {
                lastSwingTime = Time.time;

                Debug.Log("マウススイング成功！");

                // 現在の敵のReceiverを取得
                EnemyParryReceiver receiver = null;

                if (phaseManager != null &&
                    phaseManager.CurrentEnemy != null)
                {
                    receiver = phaseManager.CurrentEnemy
                        .GetComponent<EnemyParryReceiver>();
                }

                if (receiver == null)
                {
                    Debug.LogWarning(
                        "現在の敵にEnemyParryReceiverがありません！"
                    );
                }
                else
                {
                    if (receiver.TryParry())
                    {
                        Debug.Log("敵へのパリィ成功！");
                    }
                }
            }
            else
            {
                Debug.Log("スタミナ不足");
            }
        }

        previousMousePosition = currentMousePosition;
        wasSwinging = IsSwinging;

        if (debugbox != null)
        {
            debugbox.SetActive(IsParryWindowActive);
        }
    }
}
