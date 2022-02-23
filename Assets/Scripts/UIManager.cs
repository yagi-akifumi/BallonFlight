using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{

    [SerializeField]
    private Text txtScore;                  // txtScore ゲームオブジェクトの持つ Text コンポーネントをインスペクターからアサインする

    [SerializeField]
    private Text txtInfo;

    [SerializeField]
    private CanvasGroup canvasGroupInfo;

    [SerializeField]
    private ResultPopUp resultPopUpPrefab;

    [SerializeField]
    private Transform canvasTran;

    [SerializeField]
    private Button btnInfo;

    [SerializeField]
    private Button btnTitle;

    [SerializeField]
    private Text lblStart;

    [SerializeField]
    private CanvasGroup canvasGroupTitle;

    private Tweener tweener;

    /// <summary>
    /// スコア表示を更新
    /// </summary>
    /// <param name="score"></param>

    public void UpdateDisplayScore(int Score)
    {
        //　この引数でスコアの情報を受け取る
        txtScore.text = Score.ToString();
    }

    /// <summary>
    /// ゲームオーバー表示
    /// </summary>

    public void DisplayGameOverInfo()
    {
        // InfoBackGround ゲームオブジェクトの持つ CanvasGroup コンポーネントの Alpha の値を、1秒かけて 1 に変更して、背景と文字が画面に見えるようにする
        canvasGroupInfo.DOFade(1.0f, 1.0f);

        // 文字列をアニメーションさせて表示
        txtInfo.DOText("Game Over...", 1.0f);

        btnInfo.onClick.AddListener(RestartGame);
    }

    /// <summary>
    /// ResultPopUpの生成
    /// </summary>

    public void GenerateResultPopUp(int score)
    {
        // ResultPopUp を生成
        ResultPopUp resultPopUp = Instantiate(resultPopUpPrefab, canvasTran, false);

        // ResultPopUpの設定を行う
        resultPopUp.SetUpResultPopUp(score);
    }

    /// <summary>
    /// タイトルへ戻る
    /// </summary>

    public void RestartGame()
    {
        // ボタンからメソッドを削除(重複クリック防止)
        btnInfo.onClick.RemoveAllListeners();

        // 現在のシーンの名前を取得
        string sceneName = SceneManager.GetActiveScene().name;

        canvasGroupInfo.DOFade(0f, 1.0f).OnComplete(() =>
        {
            Debug.Log("Restart");
            SceneManager.LoadScene(sceneName);
        });
    }

    private void Start()
    {
        // タイトル表示
        SwitchDisplayTitle(true, 1.0f);

        // ボタンのOnClickイベントにメソッドを登録
        btnTitle.onClick.AddListener(OnClickTitle);
    }

    /// <summary>
    /// タイトル表示（処理をコメントで書いてみましょう）
    /// </summary>

    public void SwitchDisplayTitle(bool isSwitch, float alpha)
    {
        if (isSwitch) canvasGroupTitle.alpha = 0;

        canvasGroupTitle.DOFade(alpha, 1.0f).SetEase(Ease.Linear).OnComplete(() =>
        {
            lblStart.gameObject.SetActive(isSwitch);
        });

        if (tweener == null)
        {
            // Tap Startの文字をゆっくり点滅させる
            tweener = lblStart.gameObject.GetComponent<CanvasGroup>().DOFade(0, 1.0f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
        }

        else
        {
            tweener.Kill();
        }
    }

    /// <summary>
    /// タイトル表示中に画面をクリックした際の処理
    /// </summary>

    private void OnClickTitle()
    {
        // ボタンのメソッドを削除して重複タップ防止
        btnTitle.onClick.RemoveAllListeners();

        // タイトルを徐々に非表示
        SwitchDisplayTitle(false, 0.0f);

        // タイトル表示が消えるのと入れ替わりで、ゲームスタートの文字を表示する
        StartCoroutine(DisplayGameStartInfo());
    }

    /// <summary>
    /// ゲームスタート表示（処理をコメントで書いてみましょう）
    /// </summary>
    /// <returns></returns>

    public IEnumerator DisplayGameStartInfo()
    {
        yield return new WaitForSeconds(0.5f);

        canvasGroupInfo.alpha = 0;

        // InfoBackGround ゲームオブジェクトの持つ CanvasGroup コンポーネントの Alpha の値を、0.5秒かけて 1 に変更して、背景と文字が画面に見えるようにする
        canvasGroupInfo.DOFade(1.0f, 0.5f);

        // 文字列をアニメーションさせて表示
        txtInfo.text = "Game Start!";

        yield return new WaitForSeconds(1.0f);

        // InfoBackGround ゲームオブジェクトの持つ CanvasGroup コンポーネントの Alpha の値を、0.5秒かけて 0 に変更して、背景と文字が画面から消す
        canvasGroupInfo.DOFade(0f, 0.5f);

        canvasGroupTitle.gameObject.SetActive(false);
    }
}