using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class SwordAnimation : MonoBehaviour
{
    [SerializeField] private Transform leftSword;
    [SerializeField] private Transform rightSword;

    [SerializeField] private Image leftSwordImage;
    [SerializeField] private Sprite secondSwordSprite;

    void Start()
    {
        Sequence sequence = DOTween.Sequence();

        sequence.Append(leftSword.DORotate(new Vector3(0, 0, -540), 0.5f));
        sequence.Join(rightSword.DORotate(new Vector3(0, 0, 2), 0.5f));

        sequence.AppendInterval(0.3f);

        sequence.Append(leftSword.DOMoveX(leftSword.position.x - 1500f, 0.3f));

        sequence.Join(rightSword.DOMoveX(rightSword.position.x + 1500f, 0.3f));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
