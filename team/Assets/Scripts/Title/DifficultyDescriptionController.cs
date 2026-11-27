using System;
using UnityEngine;
using TMPro;

// 難易度の種類
public enum TitleDifficulty
{
    Tutorial,
    Easy,
    Normal,
    Hard
}

// 難易度ごとの説明データ
[Serializable]
public class DifficultyDescriptionData
{
    public string difficultyName;

    [TextArea(3, 8)]
    public string description;

    [TextArea(2, 5)]
    public string recommendedPlayer;
}

public class DifficultyDescriptionController : MonoBehaviour
{
    [Header("説明ウィンドウのテキスト")]
    [SerializeField] private TMP_Text difficultyNameText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private TMP_Text recommendedPlayerText;

    [Header("初期表示")]
    [SerializeField]
    private TitleDifficulty defaultDifficulty
        = TitleDifficulty.Normal;

    [Header("指南")]
    [SerializeField]
    private DifficultyDescriptionData tutorial
        = new DifficultyDescriptionData();

    [Header("小手調べ")]
    [SerializeField]
    private DifficultyDescriptionData easy
        = new DifficultyDescriptionData();

    [Header("真剣勝負")]
    [SerializeField]
    private DifficultyDescriptionData normal
        = new DifficultyDescriptionData();

    [Header("修羅の道")]
    [SerializeField]
    private DifficultyDescriptionData hard
        = new DifficultyDescriptionData();

    // 現在表示している難易度
    public TitleDifficulty CurrentDisplayedDifficulty
    {
        get;
        private set;
    }

    // 画面を開くたびに初期表示へ戻す
    private void OnEnable()
    {
        ShowDescription(defaultDifficulty);
    }

    // 難易度の説明を切り替える
    public void ShowDescription(
        TitleDifficulty difficulty)
    {
        DifficultyDescriptionData data;

        switch (difficulty)
        {
            case TitleDifficulty.Tutorial:
                data = tutorial;
                break;

            case TitleDifficulty.Easy:
                data = easy;
                break;

            case TitleDifficulty.Normal:
                data = normal;
                break;

            case TitleDifficulty.Hard:
                data = hard;
                break;

            default:
                return;
        }

        if (data == null)
            return;

        CurrentDisplayedDifficulty = difficulty;

        if (difficultyNameText != null)
        {
            difficultyNameText.text =
                data.difficultyName;
        }

        if (descriptionText != null)
        {
            descriptionText.text =
                data.description;
        }

        if (recommendedPlayerText != null)
        {
            recommendedPlayerText.text =
                data.recommendedPlayer;
        }
    }
}