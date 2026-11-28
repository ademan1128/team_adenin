using UnityEngine;
using TMPro;
using System.Collections;

public class PhaseManager : MonoBehaviour
{
    [System.Serializable]
    public class PhaseData
    {
        [Header("フェーズ名")]
        public string phaseName;

        [Header("出現する敵")]
        public GameObject[] enemies;

        [Header("背景")]
        public GameObject background;

        [Header("敵専用UI")]
        public GameObject enemyUI;
    }

    [Header("フェーズ設定")]
    [SerializeField] private PhaseData[] phases;

    [Header("UI設定")]
    [SerializeField] private TMP_Text waveText;

    [Header("移行設定")]
    [SerializeField] private float transitionDelay = 3f;

    // 現在のフェーズ
    private int currentPhase = 1;

    // 残りの敵数
    private int remainingEnemies = 0;

    // フェーズ移行中か
    private bool isChangingPhase = false;

    // ゲームクリア状態
    public bool IsGameClear { get; private set; }

    // 現在戦闘中の敵
    public EnemyStaminaController CurrentEnemy { get; private set; }

    private void Start()
    {
        waveText.gameObject.SetActive(false);

        StartPhase(currentPhase);
    }

    // フェーズ開始
    private void StartPhase(int phase)
    {
        if (phase < 1 || phase > phases.Length)
            return;

        PhaseData data = phases[phase - 1];

        // 背景をすべて非表示
        foreach (PhaseData p in phases)
        {
            if (p.background != null)
                p.background.SetActive(false);
        }

        // 現在の背景を表示
        if (data.background != null)
            data.background.SetActive(true);


        // 全敵を非表示
        foreach (PhaseData p in phases)
        {
            foreach (GameObject enemy in p.enemies)
            {
                if (enemy != null)
                    enemy.SetActive(false);
            }
        }

        // 全敵UIを非表示
        foreach (PhaseData p in phases)
        {
            if (p.enemyUI != null)
                p.enemyUI.SetActive(false);
        }

        // 現在の敵をリセット
        CurrentEnemy = null;

        remainingEnemies = 0;

        // 現在のフェーズの敵を有効化
        foreach (GameObject enemy in data.enemies)
        {
            if (enemy == null)
                continue;

            enemy.SetActive(true);

            EnemyStaminaController controller =
                enemy.GetComponent<EnemyStaminaController>();

            if (controller != null)
            {
                // 敵のステータス初期化
                controller.Initialize();

                // 敵を登録
                RegisterEnemy(controller);

                // 現在の敵を設定
                if (CurrentEnemy == null)
                {
                    CurrentEnemy = controller;
                }
            }
            else
            {
                Debug.LogWarning(
                    enemy.name + " にEnemyStaminaControllerがありません"
                );
            }
        }

        // 対応する敵UIを表示
        if (data.enemyUI != null)
        {
            data.enemyUI.SetActive(true);
        }


        Debug.Log(data.phaseName + " 開始！");

        if (remainingEnemies == 0)
        {
            Debug.LogWarning(
                "このフェーズには有効な敵が登録されていません！"
            );
        }
    }

    // 敵を登録
    private void RegisterEnemy(EnemyStaminaController enemy)
    {
        // イベントの二重登録防止
        enemy.OnDeath -= EnemyDefeated;
        enemy.OnDeath += EnemyDefeated;

        remainingEnemies++;

        Debug.Log("敵登録：残り敵数 " + remainingEnemies);
    }

    // 敵が倒されたとき
    private void EnemyDefeated()
    {
        if (isChangingPhase || IsGameClear)
            return;

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
        if (currentPhase >= phases.Length)
        {
            Debug.Log("全フェーズ終了！ゲームクリア！");

            IsGameClear = true;

            yield break;
        }

        // 次のフェーズへ
        currentPhase++;

        PhaseData data = phases[currentPhase - 1];

        // WAVE表示
        waveText.text = data.phaseName;
        waveText.gameObject.SetActive(true);

        Debug.Log(data.phaseName);

        // 指定時間待機
        yield return new WaitForSeconds(transitionDelay);

        // WAVE表示を消す
        waveText.gameObject.SetActive(false);

        // 次のフェーズ開始
        StartPhase(currentPhase);

        isChangingPhase = false;
    }

    private void OnDestroy()
    {
        // 敵のイベント登録解除
        foreach (PhaseData phase in phases)
        {
            foreach (GameObject enemy in phase.enemies)
            {
                if (enemy == null)
                    continue;

                EnemyStaminaController controller =
                    enemy.GetComponent<EnemyStaminaController>();

                if (controller != null)
                {
                    controller.OnDeath -= EnemyDefeated;
                }
            }
        }
    }
}
