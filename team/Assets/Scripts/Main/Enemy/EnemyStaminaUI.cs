
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class EnemyStaminaUI : MonoBehaviour
{
    [Header("参照")]

    // スタミナを表示するSlider
    [SerializeField] private Slider staminaSlider;

    // 敵のスタミナを管理するスクリプト
    [SerializeField] private EnemyStaminaController staminaController;

    // 段階を表示する〇のImage
    [SerializeField] private Image[] phaseIcons;


    [Header("アニメーション")]

    // ゲージのアニメーション時間
    [SerializeField] private float animationDuration = 0.2f;

    // ゲージのアニメーションを管理するTween
    private Tween staminaTween;


    private void Start()
    {
        // 現在のスタミナ割合をSliderに反映
        staminaSlider.value = staminaController.StaminaNormalized;

        // 現在の段階に合わせて〇を表示
        UpdatePhaseUI();

        // スタミナ変更イベントを登録
        staminaController.OnStaminaChanged += UpdateStaminaUI;

        // 段階変更イベントを登録
        staminaController.OnPhaseChanged += OnPhaseChanged;
    }


    // スタミナゲージの更新
    private void UpdateStaminaUI(float stamina)
    {
        // 現在のスタミナ割合を取得
        float targetValue = staminaController.StaminaNormalized;

        // 前回のTweenを停止
        staminaTween?.Kill();

        // Sliderを滑らかに変化させる
        staminaTween = staminaSlider
            .DOValue(targetValue, animationDuration)
            .SetEase(Ease.OutQuad);
    }


    // 段階変更時の処理
    private void OnPhaseChanged(int phase)
    {
        // 〇の表示を更新
        UpdatePhaseUI();
    }



    // 〇の表示更新処理
    private void UpdatePhaseUI()
    {
        // 残り段階数を取得
        int remainingPhases = staminaController.RemainingPhases;

        // すべての〇を順番に確認
        for (int i = 0; i < phaseIcons.Length; i++)
        {
            // 残り段階数に応じて表示を切り替える
            phaseIcons[i].enabled = i < remainingPhases;
        }
    }

    private void OnDestroy()
    {
        // Tweenを停止
        staminaTween?.Kill();

        // イベント登録を解除
        if (staminaController != null)
        {
            staminaController.OnStaminaChanged -= UpdateStaminaUI;

            staminaController.OnPhaseChanged -= OnPhaseChanged;
        }
    }
}
