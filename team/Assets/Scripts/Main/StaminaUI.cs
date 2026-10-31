using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class StaminaUI : MonoBehaviour
{
    [Header("参照")]
    [SerializeField] private Slider staminaSlider;
    [SerializeField] private StaminaController staminaController;

    [Header("アニメーション")]
    [SerializeField] private float animationDuration = 0.2f;

    private Tween staminaTween;

    private void Start()
    {
        staminaSlider.value = staminaController.StaminaNormalized;
    }

    private void Update()
    {
        UpdateStaminaUI();
    }

    private void UpdateStaminaUI()
    {
        float targetValue = staminaController.StaminaNormalized;

        staminaTween?.Kill();

        staminaTween = staminaSlider
            .DOValue(targetValue, animationDuration)
            .SetEase(Ease.OutQuad);
    }

    private void OnDestroy()
    {
        staminaTween?.Kill();
    }
}
