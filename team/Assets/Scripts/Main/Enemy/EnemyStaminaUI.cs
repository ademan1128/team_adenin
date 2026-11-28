using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class EnemyStaminaUI : MonoBehaviour
{
    [Header("参照")]

    // スタミナを表示するSlider
    [SerializeField] private Slider staminaSlider;

    // 対応する敵のスタミナ管理スクリプト
    [SerializeField] private EnemyStaminaController staminaController;

    // 段階を表示する〇のImage
    [SerializeField] private Image[] phaseIcons;


    [Header("アニメーション")]

    // ゲージのアニメーション時間
    [SerializeField] private float animationDuration = 0.2f;

    // ゲージのTween
    private Tween staminaTween;

    private void Start()
    {
        if (staminaController == null)
        {
            Debug.LogError(name + " にEnemyStaminaControllerが設定されていません！");
            return;
        }

        // Sliderの最大値を設定
        staminaSlider.minValue = 0f;
        staminaSlider.maxValue = 1f;

        // 現在のスタミナを反映
        staminaSlider.value = staminaController.StaminaNormalized;

        // 現在の段階を反映
        UpdatePhaseUI();

        // イベント登録
        staminaController.OnStaminaChanged += UpdateStaminaUI;
        staminaController.OnPhaseChanged += OnPhaseChanged;
    }

    // スタミナゲージの更新
    private void UpdateStaminaUI(float stamina)
    {
        float targetValue = staminaController.StaminaNormalized;

        staminaTween?.Kill();

        staminaTween = staminaSlider
            .DOValue(targetValue, animationDuration)
            .SetEase(Ease.OutQuad);
    }

    // 段階変更時の処理
    private void OnPhaseChanged(int phase)
    {
        UpdatePhaseUI();
    }

    // 〇の表示更新処理
    private void UpdatePhaseUI()
    {
        int remainingPhases = staminaController.RemainingPhases;

        for (int i = 0; i < phaseIcons.Length; i++)
        {
            phaseIcons[i].enabled = i < remainingPhases;
        }
    }

    private void OnDestroy()
    {
        staminaTween?.Kill();

        if (staminaController != null)
        {
            staminaController.OnStaminaChanged -= UpdateStaminaUI;
            staminaController.OnPhaseChanged -= OnPhaseChanged;
        }
    }
}
