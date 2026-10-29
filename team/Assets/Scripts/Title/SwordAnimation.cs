using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[Serializable]
public class SwordPose
{
    public Vector2 position;
    public float rotationZ;

    public SwordPose(float x, float y, float angle)
    {
        position = new Vector2(x, y);
        rotationZ = angle;
    }
}

[Serializable]
public class SlashSettings
{
    public SwordPose leftStart = new SwordPose(0, 0, 0);
    public SwordPose rightStart = new SwordPose(0, 0, 0);
    public SwordPose leftHit = new SwordPose(0, 0, 0);
    public SwordPose rightHit = new SwordPose(0, 0, 0);
    public Vector2 sparkPosition = Vector2.zero;

    [Min(0f)] public float readyTime = 0.08f;
    [Min(0.01f)] public float attackTime = 0.35f;
    [Min(0f)] public float hitStop = 0.05f;
    [Min(0.01f)] public float reboundTime = 0.22f;
    [Min(0f)] public float nextDelay = 0.12f;
}

public class SwordAnimation : MonoBehaviour
{
    [Header("刀の回転・移動用の親")]
    [SerializeField] private RectTransform leftSword;
    [SerializeField] private RectTransform rightSword;

    [Header("1回目に表示する子Image")]
    [SerializeField] private Image leftSwordImage;
    [SerializeField] private Image rightSwordImage;

    [Header("2回目以降に表示する子Image")]
    [SerializeField] private Image secondLeftSwordImage;
    [SerializeField] private Image secondRightSwordImage;
    [SerializeField] private bool useSecondImagesForThird = true;

    [Header("衝突エフェクト")]
    [SerializeField] private CanvasGroup sparkGroup;
    [SerializeField] private RectTransform sparkTransform;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip clashSE;

    [Header("1回目：左下と右上")]
    [SerializeField] private SlashSettings firstSlash = new SlashSettings
    {
        leftStart = new SwordPose(-1200, -720, -20),
        rightStart = new SwordPose(1200, 720, 20),
        leftHit = new SwordPose(-350, -180, -45),
        rightHit = new SwordPose(350, 180, 65),
        sparkPosition = new Vector2(0, 0)
    };

    [Header("2回目：左上と右下")]
    [SerializeField] private SlashSettings secondSlash = new SlashSettings
    {
        leftStart = new SwordPose(-1200, 720, 90),
        rightStart = new SwordPose(1200, -720, -90),
        leftHit = new SwordPose(-350, 180, 15),
        rightHit = new SwordPose(350, -180, -15),
        sparkPosition = new Vector2(0, 0)
    };

    [Header("3回目：X字の衝突")]
    [SerializeField] private SlashSettings thirdSlash = new SlashSettings
    {
        leftStart = new SwordPose(-1200, -650, 0),
        rightStart = new SwordPose(1200, -650, 0),
        leftHit = new SwordPose(-420, -280, 0),
        rightHit = new SwordPose(420, -280, 0),
        sparkPosition = new Vector2(0, 120),
        hitStop = 0.08f
    };

    private Sequence sequence;
    private Sequence sparkSequence;

    private void Start()
    {
        PlayAnimation();
    }

    public void PlayAnimation()
    {
        if (!ValidateReferences()) return;

        sequence?.Kill();
        sparkSequence?.Kill();

        // 再生のたびに最初の画像と姿勢に戻す
        ChangeSwordImages(false);
        SetSwordPose(leftSword, firstSlash.leftStart);
        SetSwordPose(rightSword, firstSlash.rightStart);
        if (sparkGroup != null) sparkGroup.alpha = 0f;

        sequence = DOTween.Sequence();
        sequence.Pause();
        sequence.SetLink(gameObject);

        AddSlash(sequence, firstSlash, 1);
        AddSlash(sequence, secondSlash, 2);
        AddSlash(sequence, thirdSlash, 3);

        sequence.Play();
    }

    private void AddSlash(Sequence seq, SlashSettings slash, int number)
    {
        // 左右の画像を切り替える。Pivotは各子Imageの設定をそのまま使う
        seq.AppendCallback(() =>
        {
            bool useSecond = number == 2 || (number == 3 && useSecondImagesForThird);
            ChangeSwordImages(useSecond);
            SetSwordPose(leftSword, slash.leftStart);
            SetSwordPose(rightSword, slash.rightStart);
            Debug.Log($"{number}回目の斬撃開始");
        });

        seq.AppendInterval(slash.readyTime);

        // 親のRectTransformだけを移動・回転させる
        seq.Append(leftSword.DOAnchorPos(slash.leftHit.position, slash.attackTime).SetEase(Ease.InCubic));
        seq.Join(rightSword.DOAnchorPos(slash.rightHit.position, slash.attackTime).SetEase(Ease.InCubic));
        seq.Join(leftSword.DOLocalRotate(new Vector3(0, 0, slash.leftHit.rotationZ), slash.attackTime).SetEase(Ease.InCubic));
        seq.Join(rightSword.DOLocalRotate(new Vector3(0, 0, slash.rightHit.rotationZ), slash.attackTime).SetEase(Ease.InCubic));

        seq.AppendCallback(() => PlayImpact(slash.sparkPosition, number));
        seq.AppendInterval(slash.hitStop);

        seq.Append(leftSword.DOAnchorPos(slash.leftStart.position, slash.reboundTime).SetEase(Ease.OutCubic));
        seq.Join(rightSword.DOAnchorPos(slash.rightStart.position, slash.reboundTime).SetEase(Ease.OutCubic));
        seq.Join(leftSword.DOLocalRotate(new Vector3(0, 0, slash.leftStart.rotationZ), slash.reboundTime).SetEase(Ease.OutCubic));
        seq.Join(rightSword.DOLocalRotate(new Vector3(0, 0, slash.rightStart.rotationZ), slash.reboundTime).SetEase(Ease.OutCubic));
        seq.AppendInterval(slash.nextDelay);
    }

    private void ChangeSwordImages(bool useSecond)
    {
        // Imageオブジェクトだけを切り替え、動かす親は常にActiveにしておく
        leftSwordImage.gameObject.SetActive(!useSecond);
        rightSwordImage.gameObject.SetActive(!useSecond);
        secondLeftSwordImage.gameObject.SetActive(useSecond);
        secondRightSwordImage.gameObject.SetActive(useSecond);
    }

    private void SetSwordPose(RectTransform sword, SwordPose pose)
    {
        sword.anchoredPosition = pose.position;
        sword.localRotation = Quaternion.Euler(0, 0, pose.rotationZ);
    }

    private void PlayImpact(Vector2 position, int number)
    {
        Debug.Log($"{number}回目の衝突！");
        if (audioSource != null && clashSE != null)
            audioSource.PlayOneShot(clashSE);

        if (sparkGroup == null || sparkTransform == null) return;

        sparkSequence?.Kill();
        sparkTransform.anchoredPosition = position;
        sparkTransform.localScale = Vector3.one * 0.3f;
        sparkGroup.alpha = 0f;

        float scale = number == 3 ? 1.6f : 1.2f;
        sparkSequence = DOTween.Sequence();
        sparkSequence.SetLink(sparkGroup.gameObject);
        sparkSequence.Append(sparkGroup.DOFade(1f, 0.03f));
        sparkSequence.Join(sparkTransform.DOScale(scale, 0.1f));
        sparkSequence.Append(sparkGroup.DOFade(0f, 0.15f));
    }

    private bool ValidateReferences()
    {
        if (leftSword == null || rightSword == null ||
            leftSwordImage == null || rightSwordImage == null ||
            secondLeftSwordImage == null || secondRightSwordImage == null)
        {
            Debug.LogError("刀のRectTransformと4つの子ImageをInspectorに設定してください。", this);
            return false;
        }

        // 親自身をImageとして指定するとSetActive(false)で親まで消えるため禁止
        if (!IsChildImage(leftSwordImage, leftSword) ||
            !IsChildImage(secondLeftSwordImage, leftSword) ||
            !IsChildImage(rightSwordImage, rightSword) ||
            !IsChildImage(secondRightSwordImage, rightSword))
        {
            Debug.LogError("4つのImageは、左右それぞれの刀の親RectTransformの『子』にしてください。", this);
            return false;
        }
        return true;
    }

    private bool IsChildImage(Image image, RectTransform root)
    {
        return image.transform != root && image.transform.IsChildOf(root);
    }

    private void OnDestroy()
    {
        sequence?.Kill();
        sparkSequence?.Kill();
    }
}
