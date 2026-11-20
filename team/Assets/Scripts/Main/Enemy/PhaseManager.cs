using UnityEngine;
using TMPro;
using System.Collections;

public class PhaseManager : MonoBehaviour
{
    [Header("フェーズ設定")]

    // 最大フェーズ数
    [SerializeField] private int maxPhase = 3;

    [Header("UI設定")]

    // WAVE表示用テキスト
    [SerializeField] private TMP_Text waveText;

    [Header("移行設定")]

    // フェーズ移行までの待機時間
    [SerializeField] private float transitionDelay = 3f;

    // 現在のフェーズ
    private int currentPhase = 1;

    // 残りの敵数
    private int remainingEnemies = 0;

    // フェーズ移行中か
    private bool isChangingPhase = false;

    private void Start()
    {
        waveText.gameObject.SetActive(false);
    }

    // 敵を登録
    public void RegisterEnemy(EnemyStaminaController enemy)
    {
        remainingEnemies++;

        // 敵の撃破イベントを登録
        enemy.OnDeath += EnemyDefeated;

        Debug.Log("敵登録：残り敵数 " + remainingEnemies);
    }

    // 敵が倒されたとき
    private void EnemyDefeated()
    {
        if (isChangingPhase)
        {
            return;
        }

        remainingEnemies--;

        Debug.Log("敵を撃破！残り：" + remainingEnemies);

        if (remainingEnemies <= 0)
        {
            StartCoroutine(ChangePhase());
        }
    }

    // フェーズ移行
    private IEnumerator ChangePhase()
    {
        isChangingPhase = true;

        // 最終フェーズ確認
        if (currentPhase >= maxPhase)
        {
            Debug.Log("全フェーズ終了！");
            yield break;
        }

        // 次のフェーズへ
        currentPhase++;

        // WAVE表示
        waveText.text = "WAVE " + currentPhase;

        waveText.gameObject.SetActive(true);

        Debug.Log("WAVE " + currentPhase);


        // 指定時間待機
        yield return new WaitForSeconds(transitionDelay);


        // 表示を消す
        waveText.gameObject.SetActive(false);

        // 敵数をリセット
        remainingEnemies = 0;

        isChangingPhase = false;
    }
}
