using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class SerifController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text serifText;
    [SerializeField] private Image characterImage;

    [Header("Settings")]
    [SerializeField] private float characterInterval;
    [System.Serializable]
    public class CharacterSpriteData
    {
        public string characterName;
        public Sprite sprite;
    }
    [SerializeField] private CharacterSpriteData[] charaSprites;

    private DialogueData dialogueData;
    private int currentIndex;

    private bool isTyping;
    private bool skipRequested;
    private bool isFinished;

    private CancellationTokenSource typingCts;

    private void Start()
    {
        LoadDialogue();

        if (dialogueData?.dialogues == null ||
            dialogueData.dialogues.Length == 0)
        {
            Debug.LogError("セリフデータがありません");
            return;
        }

        ShowDialogue();
    }

    private void Update()
    {
        // マウス左クリック
        if (Mouse.current != null &&
            Mouse.current.leftButton.wasPressedThisFrame)
        {
            OnNextClicked();
        }
    }

    private void LoadDialogue()
    {
        TextAsset json =
            Resources.Load<TextAsset>("Serif");

        if (json == null)
        {
            Debug.LogError("Serif.json が見つかりません");
            return;
        }

        dialogueData =
            JsonUtility.FromJson<DialogueData>(json.text);
    }

    private void OnNextClicked()
    {
        if (isFinished)
            return;

        if (isTyping)
        {
            // 文字送り中なら全文を表示する
            skipRequested = true;
        }
        else
        {
            // 全文表示済みなら次のセリフへ
            currentIndex++;

            if (currentIndex >= dialogueData.dialogues.Length)
            {
                isFinished = true;
                nameText.text = "";
                serifText.text = "";
                Debug.Log("シナリオ終了");
                return;
            }

            ShowDialogue();
        }
    }

    private void ShowDialogue()
{
    Dialogue dialogue =
        dialogueData.dialogues[currentIndex];

    nameText.text = dialogue.name;

    // 名前に対応する立ち絵を設定
    UpdateCharacterSprite(dialogue.name);

    TypeSerifAsync(
        dialogue.serif,
        this.GetCancellationTokenOnDestroy()
    ).Forget();
}

private void UpdateCharacterSprite(string characterName)
{
    foreach (var character in charaSprites)
    {
        if (character.characterName == characterName)
        {
            characterImage.sprite = character.sprite;
            characterImage.enabled = character.sprite != null;
            return;
        }
    }

    // 対応するキャラクターが見つからない場合
    characterImage.sprite = null;
    characterImage.enabled = false;

    Debug.LogWarning(
        $"立ち絵が登録されていません: {characterName}"
    );
}

    private async UniTaskVoid TypeSerifAsync(
        string text,
        CancellationToken destroyToken)
    {
        // 前の文字送りを停止
        typingCts?.Cancel();
        typingCts?.Dispose();

        typingCts = new CancellationTokenSource();

        using var linkedCts =
            CancellationTokenSource.CreateLinkedTokenSource(
                destroyToken,
                typingCts.Token
            );

        var token = linkedCts.Token;

        isTyping = true;
        skipRequested = false;

        try
        {
            serifText.text = text;
            serifText.maxVisibleCharacters = 0;
            serifText.ForceMeshUpdate();

            int count = serifText.textInfo.characterCount;

            for (int i = 1; i <= count; i++)
            {
                token.ThrowIfCancellationRequested();

                if (skipRequested)
                    break;

                serifText.maxVisibleCharacters = i;

                await UniTask.Delay(
                    TimeSpan.FromSeconds(characterInterval),
                    cancellationToken: token
                );
            }

            // 全文を表示
            serifText.maxVisibleCharacters = count;
        }
        catch (OperationCanceledException)
        {
            // オブジェクト破棄などによるキャンセル
        }
        finally
        {
            isTyping = false;
        }
    }
}
