using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using DG.Tweening;

public class ButtonHoverGlow : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image _glowImage;
    [SerializeField] private TMP_Text _buttonText;
    [SerializeField] private Color _hoverTextColor = Color.white;
    [SerializeField] private float _fadeDuration = 0.2f;

    private Color _normalTextColor;
    private Tween _glowTween;
    private Tween _textTween;

    private void Awake()
    {
        _normalTextColor = _buttonText.color;

        Color glowColor = _glowImage.color;
        glowColor.a = 0f;
        _glowImage.color = glowColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _glowTween?.Kill();
        _textTween?.Kill();

        _glowTween = _glowImage
            .DOFade(1f, _fadeDuration)
            .SetEase(Ease.OutQuad);

        _textTween = _buttonText
            .DOColor(_hoverTextColor, _fadeDuration)
            .SetEase(Ease.OutQuad);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _glowTween?.Kill();
        _textTween?.Kill();

        _glowTween = _glowImage
            .DOFade(0f, _fadeDuration)
            .SetEase(Ease.OutQuad);

        _textTween = _buttonText
            .DOColor(_normalTextColor, _fadeDuration)
            .SetEase(Ease.OutQuad);
    }

    private void OnDisable()
    {
        _glowTween?.Kill();
        _textTween?.Kill();

        if (_glowImage != null)
        {
            _glowImage.DOFade(0f, 0f);
        }

        if (_buttonText != null)
        {
            _buttonText.color = _normalTextColor;
        }
    }
}