using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CanvasGroup))]
public class DifficultySelectionController : MonoBehaviour
{
    [Header("難易度選択ボタン")]
    [SerializeField] private Button tutorialButton;
    [SerializeField] private Button easyButton;
    [SerializeField] private Button normalButton;
    [SerializeField] private Button hardButton;

    [Header("難易度説明ウィンドウ")]
    [SerializeField]
    private DifficultyDescriptionController descriptionController;

    [Header("決定演出")]
    [SerializeField, Min(1f)]
    private float selectedScale = 1.06f;

    [SerializeField, Min(0f)]
    private float scaleUpDuration = 0.1f;

    [SerializeField, Min(0f)]
    private float scaleDownDuration = 0.15f;

    [Header("決定SE")]
    [SerializeField] private AudioSource seSource;
    [SerializeField] private AudioClip selectSE;

    [SerializeField, Range(0f, 1f)]
    private float seVolume = 0.8f;

    [Header("遷移先のScene")]
    [SerializeField]
    private string tutorialSceneName = "TutorialScene";

    [SerializeField]
    private string easySceneName = "Test Scene";

    [SerializeField]
    private string normalSceneName = "Test Scene";

    [SerializeField]
    private string hardSceneName = "Test Scene";

    [Header("Scene遷移演出")]
    [SerializeField]
    private CanvasGroup sceneFadeGroup;

    [SerializeField, Min(0f)]
    private float sceneFadeDuration = 0.4f;

    // Sceneをまたいで保持する仮の難易度情報
    public static TitleDifficulty? LastSelectedDifficulty
    {
        get;
        private set;
    }

    private CanvasGroup panelGroup;
    private bool isInputLocked = false;

    private RectTransform animatedButton;
    private Vector3 originalScale;

    // 実際に決定された難易度
    // nullの場合は未決定
    public TitleDifficulty? SelectedDifficulty
    {
        get;
        private set;
    }

    // 決定演出が完了したか
    public bool IsSelectionComplete
    {
        get;
        private set;
    }

    private void Awake()
    {
        panelGroup = GetComponent<CanvasGroup>();
    }

    // 難易度選択画面を開くたびに初期化
    private void OnEnable()
    {
        isInputLocked = false;

        SelectedDifficulty = null;
        LastSelectedDifficulty = null;
        IsSelectionComplete = false;

        if (sceneFadeGroup != null)
        {
            sceneFadeGroup.alpha = 0f;
            sceneFadeGroup.interactable = false;
            sceneFadeGroup.blocksRaycasts = false;
        }

        SetPanelInput(true);
    }

    // 指南
    public void SelectTutorial()
    {
        SelectDifficulty(
            TitleDifficulty.Tutorial,
            tutorialButton
        );
    }

    // 小手調べ
    public void SelectEasy()
    {
        SelectDifficulty(
            TitleDifficulty.Easy,
            easyButton
        );
    }

    // 真剣勝負
    public void SelectNormal()
    {
        SelectDifficulty(
            TitleDifficulty.Normal,
            normalButton
        );
    }

    // 修羅の道
    public void SelectHard()
    {
        SelectDifficulty(
            TitleDifficulty.Hard,
            hardButton
        );
    }

    // 難易度を決定する共通処理
    private void SelectDifficulty(
            TitleDifficulty difficulty,
            Button selectedButton)
    {
        // 多重実行を防止
        if (isInputLocked ||
            selectedButton == null ||
            !selectedButton.IsInteractable())
        {
            return;
        }

        // 遷移先のSceneを取得
        string sceneName = GetSceneName(difficulty);

        // Sceneが読み込めるか事前確認
        if (string.IsNullOrWhiteSpace(sceneName) ||
            !Application.CanStreamedLevelBeLoaded(sceneName))
        {
            Debug.LogError(
                "遷移先Sceneが登録されていません：" +
                sceneName,
                this
            );

            return;
        }

        // フェード用の参照を確認
        if (sceneFadeGroup == null)
        {
            Debug.LogError(
                "Scene Fade Groupが未設定です",
                this
            );

            return;
        }

        // 入力ロック
        isInputLocked = true;

        // 難易度を確定
        SelectedDifficulty = difficulty;

        // 選択した難易度の説明を表示
        if (descriptionController != null)
        {
            descriptionController.ShowDescription(
                difficulty
            );
        }

        // 全ボタンの操作を無効化
        SetPanelInput(false);

        // 決定SEを再生
        PlaySelectSE();

        Debug.Log("難易度決定：" + difficulty);

        // 決定演出とScene遷移を開始
        StartCoroutine(
            PlaySelectionAnimation(
                selectedButton,
                sceneName
            )
        );
    }

    // 選択ボタンの拡大・縮小演出
    private IEnumerator PlaySelectionAnimation(
        Button selectedButton,
        string sceneName)
    {
        animatedButton =
            selectedButton.transform as RectTransform;

        if (animatedButton != null)
        {
            originalScale =
                animatedButton.localScale;

            Vector3 targetScale =
                originalScale * selectedScale;

            // 拡大演出
            yield return AnimateScale(
                animatedButton,
                originalScale,
                targetScale,
                scaleUpDuration
            );

            // 元のサイズに戻す
            yield return AnimateScale(
                animatedButton,
                targetScale,
                originalScale,
                scaleDownDuration
            );

            animatedButton.localScale =
                originalScale;

            animatedButton = null;
        }

        IsSelectionComplete = true;

        Debug.Log(
            "難易度決定演出完了：" +
            SelectedDifficulty
        );

        // Scene遷移処理
        yield return LoadSelectedScene(sceneName);
    }

    // 拡大・縮小の共通処理
    private IEnumerator AnimateScale(
        RectTransform target,
        Vector3 fromScale,
        Vector3 toScale,
        float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float t = Mathf.Clamp01(
                elapsedTime / duration
            );

            t = Mathf.SmoothStep(0f, 1f, t);

            target.localScale = Vector3.Lerp(
                fromScale,
                toScale,
                t
            );

            yield return null;

            elapsedTime += Time.unscaledDeltaTime;
        }

        target.localScale = toScale;
    }

    // パネル全体の入力制御
    private void SetPanelInput(bool enabled)
    {
        if (panelGroup == null)
            return;

        panelGroup.interactable = enabled;
        panelGroup.blocksRaycasts = enabled;
    }

    // 決定SE
    private void PlaySelectSE()
    {
        if (seSource != null && selectSE != null)
        {
            seSource.PlayOneShot(
                selectSE,
                seVolume
            );
        }
    }

    // 画面が閉じられた際の後処理
    private void OnDisable()
    {
        StopAllCoroutines();

        // 演出途中で閉じた場合もサイズを復元
        if (animatedButton != null)
        {
            animatedButton.localScale =
                originalScale;

            animatedButton = null;
        }
    }

    // 難易度に対応するScene名を取得
    private string GetSceneName(
        TitleDifficulty difficulty)
    {
        switch (difficulty)
        {
            case TitleDifficulty.Tutorial:
                return tutorialSceneName;

            case TitleDifficulty.Easy:
                return easySceneName;

            case TitleDifficulty.Normal:
                return normalSceneName;

            case TitleDifficulty.Hard:
                return hardSceneName;

            default:
                return string.Empty;
        }
    }

    // 暗転してSceneを切り替える
    private IEnumerator LoadSelectedScene(
        string sceneName)
    {
        // フェード中の操作を遮断
        sceneFadeGroup.blocksRaycasts = true;

        float elapsedTime = 0f;

        // フェードアウト
        while (elapsedTime < sceneFadeDuration)
        {
            float t = Mathf.Clamp01(
                elapsedTime / sceneFadeDuration
            );

            sceneFadeGroup.alpha =
                Mathf.Lerp(0f, 1f, t);

            yield return null;

            elapsedTime += Time.unscaledDeltaTime;
        }

        // 完全に暗転
        sceneFadeGroup.alpha = 1f;

        // 難易度情報をScene間で保持
        LastSelectedDifficulty = SelectedDifficulty;

        Debug.Log(
            "遷移先Scene：" + sceneName +
            " / 難易度：" + LastSelectedDifficulty
        );

        // Sceneの非同期読み込み
        AsyncOperation loadOperation = null;

        try
        {
            loadOperation = SceneManager.LoadSceneAsync(
                sceneName,
                LoadSceneMode.Single
            );
        }
        catch (System.Exception exception)
        {
            Debug.LogException(exception, this);
        }

        // 読み込み開始に失敗した場合
        if (loadOperation == null)
        {
            sceneFadeGroup.alpha = 0f;
            sceneFadeGroup.blocksRaycasts = false;

            SelectedDifficulty = null;
            LastSelectedDifficulty = null;
            IsSelectionComplete = false;

            isInputLocked = false;
            SetPanelInput(true);

            yield break;
        }

        // 読み込み完了まで待機
        yield return loadOperation;
    }
}