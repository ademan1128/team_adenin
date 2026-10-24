using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class SwordAnimation : MonoBehaviour
{
    [SerializeField] private Transform leftSword;
    [SerializeField] private Transform rightSword;

    [SerializeField] private Image leftSwordImage;
    [SerializeField] private Image rightSwordImage;
    [SerializeField] private Sprite secondSwordSprite;

    void Start()
    {
        Sequence sequence = DOTween.Sequence();

        sequence.Append(leftSword.DORotate(new Vector3(0, 0, -532), 0.5f));
        sequence.Join(rightSword.DORotate(new Vector3(0, 0, -1), 0.5f));

        sequence.AppendInterval(0.3f);

        sequence.Append(leftSword.DOMoveX(leftSword.position.x - 1700f, 0.3f));
        sequence.Join(rightSword.DOMoveX(rightSword.position.x + 1700f, 0.3f));

        sequence.AppendInterval(0.3f);
        sequence.AppendCallback(() =>
        {
            leftSwordImage.sprite = secondSwordSprite;
            rightSwordImage.sprite = secondSwordSprite;

            //leftSword.position = new Vector3(0,0,0);
            leftSwordImage.rectTransform.anchoredPosition = new Vector2(-2008, -691);
            leftSword.localEulerAngles = new Vector3(0, 0, 22);

            //rightSword.position = new Vector3(0,0,0);
            rightSwordImage.rectTransform.anchoredPosition = new Vector2(2094, 555);
            rightSword.localEulerAngles = new Vector3(0, 0, -174);
        });

        //sequence.Append(leftSword.DORotate(new Vector3(0, 0, -12), 0.5f));
        //sequence.Join(rightSword.DORotate(new Vector3(0, 0, 32), 0.5f));
    }

    // Update is called once per frame
    void Update()
    {

    }
}
