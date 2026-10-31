using System;
using UnityEngine;
using DG.Tweening;

// 刀1本分の位置と回転角度
[Serializable]
public struct SlashPoseData
{
    public Vector2 position;
    public float rotationZ;

    public static SlashPoseData FromRectTransform(RectTransform target)
    {
        return new SlashPoseData
        {
            position = target.anchoredPosition,
            rotationZ = target.localEulerAngles.z
        };
    }
}

// 斬撃1回分の設定（左右の刀は画像を切り替えず、別々のUIを動かす）
[Serializable]
public class SlashRoundData
{
    [Header("この斬撃で使用する刀（子ImageのRectTransform）")]
    public RectTransform leftImage;
    public RectTransform rightImage;

    [Header("開始位置")]
    [Tooltip("オン：Sceneで事前に配置した位置と角度を使用。オフ：下の数値を使用")]
    public bool useSceneStart = true;
    public SlashPoseData leftStart;
    public SlashPoseData rightStart;

    [Header("衝突時の位置と角度")]
    public SlashPoseData leftHit;
    public SlashPoseData rightHit;

    [Header("離脱位置")]
    [Tooltip("オン：開始位置へ戻る。オフ：下の離脱位置を使用")]
    public bool returnToStart = true;
    public SlashPoseData leftExit;
    public SlashPoseData rightExit;

    [Header("時間（秒）")]
    [Min(0f)] public float readyTime = 0.08f;
    [Min(0.01f)] public float attackTime = 0.35f;
    [Min(0f)] public float hitStop = 0.05f;
    [Min(0.01f)] public float reboundTime = 0.22f;
    [Min(0f)] public float nextDelay = 0.12f;

    [Header("衝突エフェクト（任意）")]
    public Vector2 sparkPosition;
    [Min(0.1f)] public float sparkScale = 1.2f;

    // 再生中に書き換わったSceneの配置値ではなく、起動時の配置値を保持する
    [NonSerialized] public SlashPoseData sceneLeftStart;
    [NonSerialized] public SlashPoseData sceneRightStart;
}

public class SwordAnimation : MonoBehaviour
{
    [Header("1回目：左下と右上")]
    [SerializeField] private SlashRoundData firstSlash = new SlashRoundData();

    [Header("2回目：左上と右下")]
    [SerializeField] private SlashRoundData secondSlash = new SlashRoundData();

    [Header("3回目：刀を交差させる")]
    [SerializeField] private SlashRoundData thirdSlash = new SlashRoundData();

    [Header("衝突時の演出（後から設定できる・空欄でも動作）")]
    [SerializeField] private CanvasGroup sparkGroup;
    [SerializeField] private AudioSource clashAudioSource;
    [SerializeField] private AudioClip clashSound;

    private Sequence mainSequence;
    private Sequence sparkSequence;

    private void Awake()
    {
        // Scene上の位置を記録（子ImageのPivotは変更しない）
        CacheScenePose(firstSlash);
        CacheScenePose(secondSlash);
        CacheScenePose(thirdSlash);
    }

    private void Start()
    {
        PlayAnimation();
    }

    public void PlayAnimation()
    {
        if (!HasRequiredImages())
        {
            Debug.LogError("各斬撃のLeft Image / Right Imageに子Imageを設定してください。", this);
            return;
        }

        mainSequence?.Kill();
        sparkSequence?.Kill();
        HideAllSwords();
        if (sparkGroup != null) sparkGroup.alpha = 0f;

        mainSequence = DOTween.Sequence();
        mainSequence.Pause();
        AddSlash(mainSequence, firstSlash, 1);
        AddSlash(mainSequence, secondSlash, 2);
        AddSlash(mainSequence, thirdSlash, 3);
        mainSequence.AppendCallback(HideAllSwords);
        mainSequence.Play();
    }

    private void AddSlash(Sequence seq, SlashRoundData slash, int number)
    {
        // この回に使う2本だけを表示して、指定した開始位置へ
        seq.AppendCallback(() =>
        {
            HideAllSwords();
            SlashPoseData leftStart = GetLeftStart(slash);
            SlashPoseData rightStart = GetRightStart(slash);
            ApplyPose(slash.leftImage, leftStart);
            ApplyPose(slash.rightImage, rightStart);
            slash.leftImage.gameObject.SetActive(true);
            slash.rightImage.gameObject.SetActive(true);
            Debug.Log($"{number}回目の斬撃開始");
        });

        seq.AppendInterval(slash.readyTime);

        // 移動と回転を同時に再生
        float attack = Mathf.Max(0.01f, slash.attackTime);
        seq.Append(slash.leftImage.DOAnchorPos(slash.leftHit.position, attack)
            .SetEase(Ease.InCubic));
        seq.Join(slash.rightImage.DOAnchorPos(slash.rightHit.position, attack)
            .SetEase(Ease.InCubic));
        seq.Join(slash.leftImage.DOLocalRotate(
            new Vector3(0f, 0f, slash.leftHit.rotationZ), attack)
            .SetEase(Ease.InCubic));
        seq.Join(slash.rightImage.DOLocalRotate(
            new Vector3(0f, 0f, slash.rightHit.rotationZ), attack)
            .SetEase(Ease.InCubic));

        // 衝突時の火花・効果音（未設定ならログのみ）
        seq.AppendCallback(() => PlayImpact(slash, number));
        seq.AppendInterval(slash.hitStop);

        SlashPoseData leftExit = slash.returnToStart ? GetLeftStart(slash) : slash.leftExit;
        SlashPoseData rightExit = slash.returnToStart ? GetRightStart(slash) : slash.rightExit;
        float rebound = Mathf.Max(0.01f, slash.reboundTime);
        seq.Append(slash.leftImage.DOAnchorPos(leftExit.position, rebound)
            .SetEase(Ease.OutCubic));
        seq.Join(slash.rightImage.DOAnchorPos(rightExit.position, rebound)
            .SetEase(Ease.OutCubic));
        seq.Join(slash.leftImage.DOLocalRotate(
            new Vector3(0f, 0f, leftExit.rotationZ), rebound)
            .SetEase(Ease.OutCubic));
        seq.Join(slash.rightImage.DOLocalRotate(
            new Vector3(0f, 0f, rightExit.rotationZ), rebound)
            .SetEase(Ease.OutCubic));

        seq.AppendCallback(HideAllSwords);
        seq.AppendInterval(slash.nextDelay);
    }

    private static void CacheScenePose(SlashRoundData slash)
    {
        if (slash == null) return;
        if (slash.leftImage != null)
            slash.sceneLeftStart = SlashPoseData.FromRectTransform(slash.leftImage);
        if (slash.rightImage != null)
            slash.sceneRightStart = SlashPoseData.FromRectTransform(slash.rightImage);
    }

    private static SlashPoseData GetLeftStart(SlashRoundData slash)
    {
        return slash.useSceneStart ? slash.sceneLeftStart : slash.leftStart;
    }

    private static SlashPoseData GetRightStart(SlashRoundData slash)
    {
        return slash.useSceneStart ? slash.sceneRightStart : slash.rightStart;
    }

    private static void ApplyPose(RectTransform target, SlashPoseData pose)
    {
        target.anchoredPosition = pose.position;
        target.localRotation = Quaternion.Euler(0f, 0f, pose.rotationZ);
    }

    private void HideAllSwords()
    {
        SetSwordActive(firstSlash, false);
        SetSwordActive(secondSlash, false);
        SetSwordActive(thirdSlash, false);
    }

    private static void SetSwordActive(SlashRoundData slash, bool active)
    {
        if (slash == null) return;
        if (slash.leftImage != null) slash.leftImage.gameObject.SetActive(active);
        if (slash.rightImage != null) slash.rightImage.gameObject.SetActive(active);
    }

    private bool HasRequiredImages()
    {
        return firstSlash != null && secondSlash != null && thirdSlash != null
            && firstSlash.leftImage != null && firstSlash.rightImage != null
            && secondSlash.leftImage != null && secondSlash.rightImage != null
            && thirdSlash.leftImage != null && thirdSlash.rightImage != null;
    }

    private void PlayImpact(SlashRoundData slash, int number)
    {
        Debug.Log($"{number}回目：衝突！");
        if (clashAudioSource != null && clashSound != null)
            clashAudioSource.PlayOneShot(clashSound);

        if (sparkGroup == null) return;
        RectTransform sparkRect = sparkGroup.transform as RectTransform;
        if (sparkRect == null) return;

        sparkSequence?.Kill();
        sparkRect.anchoredPosition = slash.sparkPosition;
        sparkRect.localScale = Vector3.one * 0.3f;
        sparkGroup.alpha = 0f;

        sparkSequence = DOTween.Sequence();
        sparkSequence.Append(sparkGroup.DOFade(1f, 0.03f));
        sparkSequence.Join(sparkRect.DOScale(slash.sparkScale, 0.10f));
        sparkSequence.Append(sparkGroup.DOFade(0f, 0.15f));
    }

    private void OnDestroy()
    {
        mainSequence?.Kill();
        sparkSequence?.Kill();
    }
}
