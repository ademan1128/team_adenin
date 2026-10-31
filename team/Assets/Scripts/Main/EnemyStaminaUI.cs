
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class EnemyStaminaUI : MonoBehaviour
{
    [Header("参照")]

    // スタミナを表示するSlider
    [SerializeField] private Slider staminaSlider;

    // 敵のスタミナを管理しているスクリプト
    [SerializeField] private EnemyStaminaController staminaController;


    [Header("アニメーション")]

    // スタミナゲージが変動するときにかかる時間
    [SerializeField] private float animationDuration = 0.2f;

    // スタミナゲージのアニメーションを管理するTween
    private Tween staminaTween;


    // 初期化処理
    private void Start()
    {
        // 現在のスタミナ割合をSliderに反映する
        staminaSlider.value = staminaController.StaminaNormalized;
    }


    // 毎フレーム実行
    private void Update()
    {
        // スタミナゲージの更新処理を呼び出す
        UpdateStaminaUI();
    }


    // スタミナゲージの更新
    private void UpdateStaminaUI()
    {
        // 現在のスタミナ割合を取得
        float targetValue = staminaController.StaminaNormalized;

        // Sliderの値と現在のスタミナ割合がほぼ同じなら何もしない
        if (Mathf.Approximately(staminaSlider.value, targetValue))
        {
            return;
        }

        // 前回のアニメーションが残っていたら停止する
        staminaTween?.Kill();

        // DOTweenを使ってSliderの値を滑らかに変化させる
        staminaTween = staminaSlider
            .DOValue(targetValue, animationDuration)
            .SetEase(Ease.OutQuad);
    }


    // このオブジェクトが破棄されるときに実行
    private void OnDestroy()
    {
        // 再生中のアニメーションを停止する
        staminaTween?.Kill();
    }
}
