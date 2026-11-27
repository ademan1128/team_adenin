using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleMenuController : MonoBehaviour
{
    [Header("タイトルメニューボタン")]
    [SerializeField] private Button[] menuButtons;

    [Header("メニュー出現演出")]
    [SerializeField, Min(0f)]
    private float menuFadeDuration = 0.3f;

    [Header("フェードアウト対象")]
    [SerializeField] private CanvasGroup titleLogoGroup;
    [SerializeField] private Button exitButton;

    [Header("フェードアウト演出")]
    [SerializeField, Min(0f)]
    private float otherFadeDuration = 0.3f;

    [SerializeField, Min(0f)]
    private float selectedFadeDelay = 0.15f;

    [SerializeField, Min(0f)]
    private float selectedFadeDuration = 0.3f;

    [Header("各ボタンの登録")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button settingsButton;

    [Header("難易度選択画面")]
    [SerializeField] private GameObject difficultyPanel;
    [SerializeField] private string difficultySceneName;

    [Header("難易度選択画面の演出対象")]
    [SerializeField] private RectTransform difficultyButtons;
    [SerializeField] private RectTransform difficultyDescriptionPanel;

    [Header("難易度選択画面の演出設定")]
    [SerializeField, Min(0f)]
    private float difficultySlideDuration = 0.7f;

    [SerializeField, Min(0f)]
    private float difficultySlideDistance = 1200f;

    [SerializeField, Min(0f)]
    private float difficultyTransitionDelay = 0.2f;

    [Header("設定ウィンドウ")]
    [SerializeField] private GameObject settingsPanel;

    [Header("設定ウィンドウの演出")]
    [SerializeField, Min(0f)]
    private float settingsSlideDuration = 0.7f;

    [SerializeField, Min(0f)]
    private float settingsSlideDistance = 2200f;

    [SerializeField, Min(0f)]
    private float settingsTransitionDelay = 0.2f;

    [Header("終了処理")]
    [SerializeField] private GameObject exitConfirmPanel;
    [SerializeField] private bool quitWithoutConfirmation = false;

    [Header("SEの余韻")]
    [SerializeField] private bool waitForSEBeforeLeaving = true;

    private bool isInputLocked = true;
    private bool hasStartedShowing = false;
    private bool selectionCompleted = false;
    private bool canReturnToMenu = false;

    private Button currentSelection;

    private void Awake()
    {
        SetMenuAlpha(0f);
        SetMenuInput(false);

        SetPanelActive(difficultyPanel, false);
        SetPanelActive(settingsPanel, false);
        SetPanelActive(exitConfirmPanel, false);
    }

    // タイトルロゴの出現完了後に呼び出す
    public void ShowMenu()
    {
        if (hasStartedShowing)
            return;

        hasStartedShowing = true;

        StartCoroutine(FadeInMenu());
    }

    // メニューのフェードイン
    private IEnumerator FadeInMenu(
        bool restoreLogo = false)
    {
        float elapsedTime = 0f;

        isInputLocked = true;
        SetMenuInput(false);

        while (elapsedTime < menuFadeDuration)
        {
            yield return null;

            elapsedTime += Time.unscaledDeltaTime;

            float t = Mathf.Clamp01(
                elapsedTime / menuFadeDuration
            );

            float alpha = Mathf.SmoothStep(0f, 1f, t);

            SetMenuAlpha(alpha);

            if (restoreLogo && titleLogoGroup != null)
                titleLogoGroup.alpha = alpha;
        }

        SetMenuAlpha(1f);

        if (restoreLogo && titleLogoGroup != null)
            titleLogoGroup.alpha = 1f;

        isInputLocked = false;

        SetMenuInput(true);

        Debug.Log("タイトルメニュー操作受付開始");
    }

    // ボタンの選択を受け付ける
    public bool TrySelect(Button selectedButton)
    {
        if (isInputLocked || selectedButton == null)
            return false;

        if (menuButtons == null ||
            System.Array.IndexOf(
                menuButtons, selectedButton) < 0)
        {
            Debug.LogWarning("メニューボタンが未登録です");
            return false;
        }

        // 遷移先の設定を確認
        if (!ValidateDestination(selectedButton))
            return false;

        isInputLocked = true;
        currentSelection = selectedButton;
        selectionCompleted = false;
        canReturnToMenu = false;

        SetMenuInput(false);

        Debug.Log("タイトルメニュー入力ロック");

        return true;
    }

    // 遷移先の登録漏れを防止する
    private bool ValidateDestination(Button button)
    {
        if (button == startButton)
        {
            if (difficultyPanel != null)
                return true;

            if (!string.IsNullOrWhiteSpace(
                    difficultySceneName) &&
                Application.CanStreamedLevelBeLoaded(
                    difficultySceneName))
            {
                return true;
            }

            Debug.LogWarning(
                "難易度選択画面が未設定です"
            );

            return false;
        }

        if (button == settingsButton)
        {
            if (settingsPanel != null)
                return true;

            Debug.LogWarning("SettingPanelが未設定です");
            return false;
        }

        if (button == exitButton)
        {
            if (exitConfirmPanel != null ||
                quitWithoutConfirmation)
            {
                return true;
            }

            Debug.LogWarning("終了方法が未設定です");
            return false;
        }

        Debug.LogWarning("ボタンの役割が未設定です");
        return false;
    }

    // 納刀完了時に呼び出す
    public void NotifySheatheFinished(
        Button selectedButton)
    {
        if (!isInputLocked ||
            selectionCompleted ||
            selectedButton == null ||
            selectedButton != currentSelection)
        {
            return;
        }

        selectionCompleted = true;

        // EXITはフェードアウトしない
        if (selectedButton == exitButton)
        {
            HandleExit();
            return;
        }

        StartCoroutine(FadeOutMenu(selectedButton));
    }

    // 決定後のフェードアウト
    private IEnumerator FadeOutMenu(
        Button selectedButton)
    {
        float elapsedTime = 0f;

        float totalDuration = Mathf.Max(
            otherFadeDuration,
            selectedFadeDelay + selectedFadeDuration
        );

        while (elapsedTime < totalDuration)
        {
            float otherAlpha = GetFadeAlpha(
                elapsedTime,
                0f,
                otherFadeDuration
            );

            float selectedAlpha = GetFadeAlpha(
                elapsedTime,
                selectedFadeDelay,
                selectedFadeDuration
            );

            if (titleLogoGroup != null)
                titleLogoGroup.alpha = otherAlpha;

            foreach (Button button in menuButtons)
            {
                if (button == null)
                    continue;

                CanvasGroup group =
                    button.GetComponent<CanvasGroup>();

                if (group == null)
                    continue;

                group.alpha = button == selectedButton
                    ? selectedAlpha
                    : otherAlpha;
            }

            yield return null;

            elapsedTime += Time.unscaledDeltaTime;
        }

        SetMenuAlpha(0f);

        if (titleLogoGroup != null)
            titleLogoGroup.alpha = 0f;

        Debug.Log(
            selectedButton.name + "：フェードアウト完了"
        );

        // STARTの処理
        if (selectedButton == startButton)
        {
            if (difficultyPanel != null)
            {
                OpenPanel(difficultyPanel);
            }
            else
            {
                yield return WaitForSelectedSE(
                    selectedButton
                );

                SceneManager.LoadScene(
                    difficultySceneName
                );
            }
        }
        // SETTINGSの処理
        else if (selectedButton == settingsButton)
        {
            OpenPanel(settingsPanel);
        }
    }

    // パネルを表示する
    private void OpenPanel(GameObject panel)
    {
        // 設定画面の出現演出
        if (panel != null && panel == settingsPanel)
        {
            StartCoroutine(
                AnimateSettingsPanel(true)
            );
            return;
        }

        // 難易度選択画面の出現演出
        if (panel != null && panel == difficultyPanel)
        {
            StartCoroutine(
                AnimateDifficultyPanel(true)
            );
            return;
        }

        // その他の画面は従来どおり表示
        canReturnToMenu = true;

        SetPanelActive(panel, true);
    }

    // EXIT選択後の処理
    private void HandleExit()
    {
        if (exitConfirmPanel != null)
        {
            OpenPanel(exitConfirmPanel);
        }
        else if (quitWithoutConfirmation)
        {
            StartCoroutine(QuitGame());
        }
    }

    // 終了確認の「はい」から呼び出す
    public void ConfirmExit()
    {
        if (currentSelection != exitButton ||
            !selectionCompleted ||
            !canReturnToMenu)
        {
            return;
        }

        canReturnToMenu = false;

        StartCoroutine(QuitGame());
    }

    // ゲーム終了
    private IEnumerator QuitGame()
    {
        yield return WaitForSelectedSE(
            currentSelection
        );

        Debug.Log("ゲーム終了");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // 戻るボタンから呼び出す
    public void ReturnToMenu()
    {
        if (!canReturnToMenu)
            return;

        // 難易度決定後はタイトルに戻れない
        if (currentSelection == startButton &&
            difficultyPanel != null)
        {
            DifficultySelectionController selection =
                difficultyPanel.GetComponent<
                    DifficultySelectionController>();

            if (selection != null &&
                selection.SelectedDifficulty.HasValue)
            {
                return;
            }
        }

        // 連続クリックを防止
        canReturnToMenu = false;

        // 設定画面の退場演出
        if (currentSelection == settingsButton &&
            settingsPanel != null)
        {
            StartCoroutine(
                AnimateSettingsPanel(false)
            );
            return;
        }

        // 難易度選択画面の退場演出
        if (currentSelection == startButton &&
            difficultyPanel != null)
        {
            StartCoroutine(
                AnimateDifficultyPanel(false)
            );
            return;
        }

        // その他の画面は従来どおり復帰
        RestoreMenu();
    }

    // タイトルメニューの復元
    private void RestoreMenu()
    {
        canReturnToMenu = false;

        bool wasExit = currentSelection == exitButton;

        SetMenuInput(false);

        SetPanelActive(difficultyPanel, false);
        SetPanelActive(settingsPanel, false);
        SetPanelActive(exitConfirmPanel, false);

        // すべての刀・発光状態をリセット
        foreach (Button button in menuButtons)
        {
            if (button == null)
                continue;

            SwordButtonAnimation sword =
                button.GetComponent<SwordButtonAnimation>();

            if (sword != null)
            {
                sword.ResetForMenu();
            }
        }

        currentSelection = null;
        selectionCompleted = false;

        if (wasExit)
        {
            // EXITのキャンセル時
            SetMenuAlpha(1f);

            if (titleLogoGroup != null)
                titleLogoGroup.alpha = 1f;

            isInputLocked = false;

            SetMenuInput(true);
        }
        else
        {
            // タイトルUIの再表示
            SetMenuAlpha(0f);

            if (titleLogoGroup != null)
                titleLogoGroup.alpha = 0f;

            StartCoroutine(FadeInMenu(true));
        }
    }

    // 必要に応じてSEの余韻が終わるまで待つ
    private IEnumerator WaitForSelectedSE(
        Button button)
    {
        if (!waitForSEBeforeLeaving || button == null)
            yield break;

        SwordButtonAnimation sword =
            button.GetComponent<SwordButtonAnimation>();

        if (sword == null)
            yield break;

        float remainingTime = sword.RemainingSeTime;

        if (remainingTime > 0f)
        {
            yield return new WaitForSecondsRealtime(
                remainingTime
            );
        }
    }

    // パネルの表示・非表示
    private void SetPanelActive(
        GameObject panel, bool active)
    {
        if (panel != null)
            panel.SetActive(active);
    }

    // フェードアウトの透明度を計算する
    private float GetFadeAlpha(
        float elapsed,
        float delay,
        float duration)
    {
        if (elapsed < delay)
            return 1f;

        if (duration <= 0f)
            return 0f;

        float t = Mathf.Clamp01(
            (elapsed - delay) / duration
        );

        return Mathf.SmoothStep(1f, 0f, t);
    }

    // 全ボタンの透明度を変更
    private void SetMenuAlpha(float alpha)
    {
        if (menuButtons == null)
            return;

        foreach (Button button in menuButtons)
        {
            if (button == null)
                continue;

            CanvasGroup group =
                button.GetComponent<CanvasGroup>();

            if (group != null)
                group.alpha = alpha;
        }
    }

    // 全ボタンの入力受付を制御
    private void SetMenuInput(bool enabled)
    {
        if (menuButtons == null)
            return;

        foreach (Button button in menuButtons)
        {
            if (button == null)
                continue;

            button.interactable = enabled;

            CanvasGroup group =
                button.GetComponent<CanvasGroup>();

            if (group != null)
            {
                group.interactable = enabled;
                group.blocksRaycasts = enabled;
            }

            ButtonHoverController hover =
                button.GetComponent<ButtonHoverController>();

            if (hover != null)
                hover.SetInputEnabled(enabled);
        }
    }
    private IEnumerator AnimateSettingsPanel(bool opening)
    {
        canReturnToMenu = false;

        RectTransform rect =
            settingsPanel.GetComponent<RectTransform>();

        CanvasGroup group =
            settingsPanel.GetComponent<CanvasGroup>();

        // 必要なコンポーネントがない場合の対策
        if (rect == null || group == null)
        {
            Debug.LogWarning(
                "SettingPanelにRectTransformとCanvasGroupが必要です"
            );

            if (opening)
            {
                SetPanelActive(settingsPanel, true);
                canReturnToMenu = true;
            }
            else
            {
                RestoreMenu();
            }

            yield break;
        }

        // 現在の通常表示位置を記録
        Vector2 homePosition = rect.anchoredPosition;

        // 画面左側の待機位置
        Vector2 hiddenPosition =
            homePosition +
            Vector2.left * settingsSlideDistance;

        // 演出中の操作を無効化
        group.interactable = false;
        group.blocksRaycasts = false;

        Vector2 fromPosition;
        Vector2 toPosition;

        float fromAlpha;
        float toAlpha;

        if (opening)
        {
            fromPosition = hiddenPosition;
            toPosition = homePosition;

            fromAlpha = 0f;
            toAlpha = 1f;

            rect.anchoredPosition = hiddenPosition;
            group.alpha = 0f;

            // タイトルUI消失後の待機時間
            if (settingsTransitionDelay > 0f)
            {
                yield return new WaitForSecondsRealtime(
                    settingsTransitionDelay
                );
            }

            SetPanelActive(settingsPanel, true);
        }
        else
        {
            fromPosition = homePosition;
            toPosition = hiddenPosition;

            fromAlpha = 1f;
            toAlpha = 0f;
        }

        float elapsedTime = 0f;

        float animationDuration =
            Mathf.Max(0f, settingsSlideDuration);

        while (elapsedTime < animationDuration)
        {
            float t = Mathf.Clamp01(
                elapsedTime / animationDuration
            );

            // 出現時は減速、退場時は加速
            float easedT = opening
                ? 1f - Mathf.Pow(1f - t, 3f)
                : Mathf.Pow(t, 3f);

            rect.anchoredPosition = Vector2.Lerp(
                fromPosition,
                toPosition,
                easedT
            );

            group.alpha = Mathf.Lerp(
                fromAlpha,
                toAlpha,
                t
            );

            yield return null;

            elapsedTime += Time.unscaledDeltaTime;
        }

        rect.anchoredPosition = toPosition;
        group.alpha = toAlpha;

        if (opening)
        {
            // 表示完了後に操作受付開始
            group.interactable = true;
            group.blocksRaycasts = true;

            canReturnToMenu = true;

            Debug.Log("設定画面の表示完了");
        }
        else
        {
            // 完全に消えてから非表示にする
            SetPanelActive(settingsPanel, false);

            // 次回表示のために元の座標へ戻す
            rect.anchoredPosition = homePosition;

            // タイトル復帰前の待機時間
            if (settingsTransitionDelay > 0f)
            {
                yield return new WaitForSecondsRealtime(
                    settingsTransitionDelay
                );
            }

            Debug.Log("設定画面の退場完了");

            RestoreMenu();
        }
    }

    private IEnumerator AnimateDifficultyPanel(bool opening)
    {
        canReturnToMenu = false;

        CanvasGroup group =
            difficultyPanel.GetComponent<CanvasGroup>();

        // 必要な参照が設定されているか確認
        if (group == null ||
            difficultyButtons == null ||
            difficultyDescriptionPanel == null)
        {
            Debug.LogWarning(
                "難易度選択画面の演出対象が未設定です",
                this
            );

            // 設定漏れがあっても操作不能にしない
            if (opening)
            {
                SetPanelActive(difficultyPanel, true);

                if (group != null)
                {
                    group.alpha = 1f;
                    group.interactable = true;
                    group.blocksRaycasts = true;
                }

                canReturnToMenu = true;
            }
            else
            {
                RestoreMenu();
            }

            yield break;
        }

        // 通常表示位置を記録
        Vector2 buttonsHome =
            difficultyButtons.anchoredPosition;

        Vector2 descriptionHome =
            difficultyDescriptionPanel.anchoredPosition;

        // 左右の画面外待機位置
        Vector2 buttonsHidden =
            buttonsHome +
            Vector2.left * difficultySlideDistance;

        Vector2 descriptionHidden =
            descriptionHome +
            Vector2.right * difficultySlideDistance;

        // 演出中は操作不可
        group.interactable = false;
        group.blocksRaycasts = false;

        if (opening)
        {
            // 出現前の初期状態
            difficultyButtons.anchoredPosition =
                buttonsHidden;

            difficultyDescriptionPanel.anchoredPosition =
                descriptionHidden;

            group.alpha = 0f;

            // タイトルUI消失後の待機
            if (difficultyTransitionDelay > 0f)
            {
                yield return new WaitForSecondsRealtime(
                    difficultyTransitionDelay
                );
            }

            // 難易度選択画面を有効化
            SetPanelActive(difficultyPanel, true);

            // OnEnableによる入力有効化を上書き
            group.interactable = false;
            group.blocksRaycasts = false;
        }

        // アニメーション開始・終了位置
        Vector2 buttonsFrom = opening
            ? buttonsHidden
            : buttonsHome;

        Vector2 buttonsTo = opening
            ? buttonsHome
            : buttonsHidden;

        Vector2 descriptionFrom = opening
            ? descriptionHidden
            : descriptionHome;

        Vector2 descriptionTo = opening
            ? descriptionHome
            : descriptionHidden;

        float fromAlpha = opening ? 0f : 1f;
        float toAlpha = opening ? 1f : 0f;

        float elapsedTime = 0f;

        float animationDuration =
            Mathf.Max(0f, difficultySlideDuration);

        // スライドとフェードを同時に再生
        while (elapsedTime < animationDuration)
        {
            float t = Mathf.Clamp01(
                elapsedTime / animationDuration
            );

            // 出現時は減速、退場時は加速
            float easedT = opening
                ? 1f - Mathf.Pow(1f - t, 3f)
                : Mathf.Pow(t, 3f);

            // 難易度ボタンの移動
            difficultyButtons.anchoredPosition =
                Vector2.Lerp(
                    buttonsFrom,
                    buttonsTo,
                    easedT
                );

            // 説明ウィンドウの移動
            difficultyDescriptionPanel.anchoredPosition =
                Vector2.Lerp(
                    descriptionFrom,
                    descriptionTo,
                    easedT
                );

            // 画面全体のフェード
            group.alpha = Mathf.Lerp(
                fromAlpha,
                toAlpha,
                t
            );

            yield return null;

            elapsedTime += Time.unscaledDeltaTime;
        }

        // 最終位置と透明度を確定
        difficultyButtons.anchoredPosition =
            buttonsTo;

        difficultyDescriptionPanel.anchoredPosition =
            descriptionTo;

        group.alpha = toAlpha;

        if (opening)
        {
            // 出現完了後に操作受付開始
            group.interactable = true;
            group.blocksRaycasts = true;

            canReturnToMenu = true;

            Debug.Log("難易度選択画面の表示完了");
        }
        else
        {
            // 退場完了後に画面を無効化
            SetPanelActive(difficultyPanel, false);

            // 次回表示に備えて元の位置へ戻す
            difficultyButtons.anchoredPosition =
                buttonsHome;

            difficultyDescriptionPanel.anchoredPosition =
                descriptionHome;

            if (difficultyTransitionDelay > 0f)
            {
                yield return new WaitForSecondsRealtime(
                    difficultyTransitionDelay
                );
            }

            Debug.Log("難易度選択画面の退場完了");

            // タイトル画面を復元
            RestoreMenu();
        }
    }
}