
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SpecialMoveUI : MonoBehaviour
{
    [Header("参照")]
    [SerializeField] private Slider SPslider;
    [SerializeField] private ParryComboManager parryComboManager;

    [Header("アニメーション")]
    [SerializeField] private float animationDuration = 0.2f;

    private Tween SPTween;
    private float previousSP;

    private void Start()
    {
        previousSP = parryComboManager.SpecialGaugeNormalized;
        SPslider.value = previousSP;
    }

    private void Update()
    {
        UpdateSPUI();
    }

    private void UpdateSPUI()
    {
        float targetValue = parryComboManager.SpecialGaugeNormalized;

        if (Mathf.Approximately(previousSP, targetValue))
        {
            return;
        }

        previousSP = targetValue;

        SPTween?.Kill();

        SPTween = SPslider
            .DOValue(targetValue, animationDuration)
            .SetEase(Ease.OutQuad);
    }

    private void OnDestroy()
    {
        SPTween?.Kill();
    }
}
