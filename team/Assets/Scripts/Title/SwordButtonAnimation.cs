using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SwordButtonAnimation : MonoBehaviour
{
    [Header("刀のパーツ")]
    [SerializeField] private RectTransform swordBlade;

    [Header("納刀アニメーション設定")]
    [SerializeField] private float startX = -340f;
    [SerializeField] private float endX = -220f;
    [SerializeField, Min(0f)]
    private float duration = 0.5f;

    [Header("納刀完了SE")]
    [SerializeField] private AudioSource seSource;
    [SerializeField] private AudioClip sheatheSE;

    [SerializeField, Range(0f, 1f)]
    private float seVolume = 1f;

    [Tooltip("納刀完了を基準としたSE再生時間。マイナスで早く、プラスで遅く再生する")]
    [SerializeField] private float seTimingOffset = -0.5f;

    [Header("タイトルメニュー管理")]
    [SerializeField] private TitleMenuController menuController;

    // 重複再生の防止
    private bool hasPlayed = false;

    // SEの再生終了予定時刻
    private float seEndTime = 0f;

    // SEの残り再生時間
    public float RemainingSeTime
    {
        get
        {
            return Mathf.Max(
                0f,
                seEndTime - Time.unscaledTime
            );
        }
    }

    // 初期化
    private void Awake()
    {
        if (swordBlade != null)
        {
            SetBladePosition(startX);
        }
    }

    // ボタンがクリックされたときに呼び出す
    public void PlaySheathe()
    {
        if (hasPlayed || swordBlade == null)
            return;

        if (menuController == null)
        {
            Debug.LogWarning(
                "Menu Controllerが未設定です",
                this
            );
            return;
        }

        // タイトルメニュー全体の入力ロック
        if (!menuController.TrySelect(
                GetComponent<Button>()))
        {
            return;
        }

        hasPlayed = true;

        // 選択したボタンの発光を維持
        ButtonHoverController hover =
            GetComponent<ButtonHoverController>();

        if (hover != null)
        {
            hover.LockHover();
        }

        // 納刀アニメーション開始
        StartCoroutine(SheatheAnimation());
    }

    // 納刀アニメーション
    private IEnumerator SheatheAnimation()
    {
        float elapsedTime = 0f;

        float animationDuration =
            Mathf.Max(0f, duration);

        // SEの再生開始時間
        float sePlayTime = Mathf.Max(
            0f,
            animationDuration + seTimingOffset
        );

        bool sePlayed = false;
        bool animationCompleted = false;

        while (true)
        {
            // 刀を右方向へ移動
            if (!animationCompleted)
            {
                float t = animationDuration > 0f
                    ? Mathf.Clamp01(
                        elapsedTime / animationDuration)
                    : 1f;

                float x = Mathf.Lerp(
                    startX,
                    endX,
                    t
                );

                SetBladePosition(x);

                // 納刀完了
                if (t >= 1f)
                {
                    SetBladePosition(endX);

                    animationCompleted = true;

                    Debug.Log("納刀完了");
                }
            }

            // 指定されたタイミングでSEを再生
            if (!sePlayed &&
                elapsedTime >= sePlayTime)
            {
                PlaySheatheSE();

                sePlayed = true;
            }

            // 納刀とSE再生開始処理が完了
            if (animationCompleted && sePlayed)
                break;

            yield return null;

            elapsedTime += Time.unscaledDeltaTime;
        }

        // タイトルメニュー管理側へ完了を通知
        if (menuController != null)
        {
            menuController.NotifySheatheFinished(
                GetComponent<Button>()
            );
        }
    }

    // 納刀SEの再生
    private void PlaySheatheSE()
    {
        if (seSource != null && sheatheSE != null)
        {
            seSource.PlayOneShot(
                sheatheSE,
                seVolume
            );

            // Pitchを考慮したSEの再生時間
            float pitch = Mathf.Max(
                0.01f,
                Mathf.Abs(seSource.pitch)
            );

            // SEの再生終了予定時刻を記録
            seEndTime = Time.unscaledTime
                + sheatheSE.length / pitch;
        }
    }

    // 刀のX座標だけを変更する
    private void SetBladePosition(float x)
    {
        Vector2 position =
            swordBlade.anchoredPosition;

        position.x = x;

        swordBlade.anchoredPosition = position;
    }

    // タイトルメニューに戻る際のリセット
    public void ResetForMenu()
    {
        StopAllCoroutines();

        hasPlayed = false;
        seEndTime = 0f;

        // 刀を通常位置に戻す
        if (swordBlade != null)
        {
            SetBladePosition(startX);
        }

        // 発光状態をリセット
        ButtonHoverController hover =
            GetComponent<ButtonHoverController>();

        if (hover != null)
        {
            hover.ResetHover();
        }
    }
}