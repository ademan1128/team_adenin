using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Button))]
public class DifficultyButtonHover
    : MonoBehaviour, IPointerEnterHandler
{
    [Header("難易度の種類")]
    [SerializeField]
    private TitleDifficulty difficulty
        = TitleDifficulty.Normal;

    [Header("説明ウィンドウ管理")]
    [SerializeField]
    private DifficultyDescriptionController
        descriptionController;

    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    // カーソルがボタンに重なったとき
    public void OnPointerEnter(
        PointerEventData eventData)
    {
        // 操作できない状態では反応しない
        if (button == null ||
            !button.IsInteractable())
        {
            return;
        }

        if (descriptionController == null)
        {
            Debug.LogWarning(
                "Description Controllerが未設定です",
                this
            );

            return;
        }

        // 説明ウィンドウの表示を切り替える
        descriptionController.ShowDescription(
            difficulty
        );
    }
}