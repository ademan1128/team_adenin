using UnityEngine;
using UnityEngine.InputSystem;

public class MouseInputDetector : MonoBehaviour
{
    [Header("マウスの振り判定")]
    [SerializeField] private float swingSpeedThreshold = 50000f;

    [Header("パリィ受付時間")]
    [SerializeField] private float parryWindow = 0.2f;

    //スタミナ管理の参照
    [SerializeField] private StaminaController staminaController;

    // 敵のパリィ判定の参照
    [SerializeField] private EnemyParryReceiver enemyParryReceiver;

    //現在のマウス座標
    private Vector2 currentMousePosition;

    //前フレームのマウス座標
    private Vector2 previousMousePosition;

    //1フレーム間の移動量
    private Vector2 mouseDelta;

    //マウスの移動方向
    private Vector2 mouseDirection;

    //マウスの移動速度
    private float mouseSpeed;

    //前フレームで振っていたか
    private bool wasSwinging;

    //マウスが振られたか
    public bool IsSwinging { get; private set; }


    //最後にマウスを振った時刻
    private float lastSwingTime = float.NegativeInfinity;

    //マウスの振った方向を外部から取得できるようのやつ
    public Vector2 MouseDirection => mouseDirection;

    // マウスの移動速度を外部から取得できる
    public float MouseSpeed => mouseSpeed;

    // マウスを振ってから受付時間内か判定
    public bool IsParryWindowActive =>Time.time - lastSwingTime <= parryWindow;

    private void Start()
    {
        if (Mouse.current == null)
        {
            Debug.LogWarning("マウスが認識されてない");
            return;
        }

        // 初期位置を取得
        previousMousePosition = Mouse.current.position.ReadValue();
    }

    private void Update()
    {
        // マウスが認識されているか確認
        if (Mouse.current == null)
        {
            return;
        }
        // 前フレームのマウス座標を保存
        currentMousePosition = Mouse.current.position.ReadValue();

        // マウスの差を計算
        mouseDelta = currentMousePosition - previousMousePosition;

        //マウスの移動速度を計算
        mouseSpeed = mouseDelta.magnitude / Mathf.Max(Time.deltaTime, 0.0001f);

        //マウスの方向を計算
        if (mouseDelta.sqrMagnitude > 0f)
        {
            mouseDirection = mouseDelta.normalized;
        }
        else
        {
            mouseDirection = Vector2.zero;
        }

        //ここで速度によってマウスが振られたかどうかを判定してる
        IsSwinging = mouseSpeed >= swingSpeedThreshold;

        // 振った瞬間だけログを表示
        if (IsSwinging && !wasSwinging)
        {
            //スタミナを消費出来た場合のみパリィが出来るように
            if (staminaController.ConsumeStamina())
            {
                lastSwingTime = Time.time;

                Debug.Log("マウススイング成功！");

                // 敵にパリィ判定を要求
                if (enemyParryReceiver.TryParry())
                {
                    Debug.Log("敵へのパリィ成功！");
                }
            }
            else
            {
                Debug.Log("スタミナ不足");
            }
        }

        //ここで前フレームのマウス座標と振ったかどうかを更新
        previousMousePosition = currentMousePosition;
        wasSwinging = IsSwinging;
    }
}
