
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHoverController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Animator _animator;

    private static readonly int IsHoverHash = Animator.StringToHash("IsHover");

    public void OnPointerEnter(PointerEventData eventData)
    {
        _animator.SetBool(IsHoverHash, true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _animator.SetBool(IsHoverHash, false);
    }
}