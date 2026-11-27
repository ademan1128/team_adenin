using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class ButtonHoverController : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler
{
    [Header("発光用パーツ")]
    [SerializeField] private CanvasGroup sheathGlow;
    [SerializeField] private CanvasGroup bladeGlow;
    [SerializeField] private TMP_Text buttonText;

    [Header("発光設定")]
    [SerializeField] private float fadeDuration = 0.12f;

    [SerializeField]
    private Color hoverTextColor =
        new Color(1f, 0.8f, 0.4f, 1f);

    [SerializeField, Range(0f, 1f)]
    private float glowIntensity = 0.8f;

    private Color normalTextColor;
    private bool isHovered = false;
    private bool isLocked = false;

    // ホバー入力の受付状態
    private bool inputEnabled = true;

    private float currentGlow = 0f;

    private void Awake()
    {
        if (buttonText != null)
            normalTextColor = buttonText.color;

        ApplyGlow(0f);
    }

    private void Update()
    {
        float target =
            (isLocked || (inputEnabled && isHovered))
            ? 1f : 0f;

        currentGlow = Mathf.MoveTowards(
            currentGlow,
            target,
            Time.unscaledDeltaTime /
            Mathf.Max(fadeDuration, 0.001f)
        );

        ApplyGlow(currentGlow);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!inputEnabled || isLocked)
            return;

        isHovered = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!inputEnabled || isLocked)
            return;

        isHovered = false;
    }

    // ホバー入力の有効・無効
    public void SetInputEnabled(bool enabled)
    {
        inputEnabled = enabled;

        if (!enabled)
            isHovered = false;
    }

    // 選択されたボタンの発光を維持
    public void LockHover()
    {
        isLocked = true;
    }

    // 発光とロックを解除
    public void ResetHover()
    {
        isLocked = false;
        isHovered = false;
        currentGlow = 0f;

        ApplyGlow(0f);
    }

    private void ApplyGlow(float value)
    {
        if (sheathGlow != null)
            sheathGlow.alpha = value * glowIntensity;

        if (bladeGlow != null)
            bladeGlow.alpha = value * glowIntensity;

        if (buttonText != null)
        {
            buttonText.color = Color.Lerp(
                normalTextColor,
                hoverTextColor,
                value
            );
        }
    }
}