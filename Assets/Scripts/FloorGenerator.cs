using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorGenerator : MonoBehaviour
{
    [SerializeField]
    private GameObject aerialFloorPrefab;   // プレファブにした AerialFloor_Mid ゲームオブジェクトをインスペクターからアサインする

    [SerializeField]
    private Transform generateTran;         // プレファブのクローンを生成する位置の設定

    [Header("生成までの待機時間")]
    public float waitTime;                  // １回生成するまでの待機時間。どの位の間隔で自動生成を行うか設定
    private float timer;                    // 待機時間の計測用

    private GameDirector gameDirector;

    private bool isActivate;                // 生成の状態を設定し、生成を行うかどうかの判定に利用する。trueなら 生成し、false なら生成しない

    void Update()
    {
        // 停止中は生成を行わない
        if(isActivate==false)
        {
            return;
        }

        // 時間を計測する
        timer += Time.deltaTime;

        // 計測している時間が waitTime の値と同じか、超えたら
        if (timer >= waitTime)
        {
            // 次回の計測用に、timer を 0 にする
            timer = 0;

            // クローン生成用のメソッドを呼び出す
            GenerateFloor();
        }
    }

    /// <summary>
    /// プレファブを元にクローンのゲームオブジェクトを生成
    /// </summary>

    private void GenerateFloor()
    {
        // 空中床のプレファブを元にクローンのゲームオブジェクトを生成
        GameObject obj = Instantiate(aerialFloorPrefab, generateTran);

        // ランダムな値を取得
        float randomPosY = Random.Range(-4.0f, 4.0f);

        // 生成されたゲームオブジェクトのY軸にランダムな値を加算して、生成されるたびに高さの位置を変更する
        obj.transform.position = new Vector2(obj.transform.position.x, obj.transform.position.y + randomPosY);

        // 生成数をカウントアップ
        gameDirector.GenerateCount++;
    }

    /// <summary>
    /// FloorGeneratorの準備
    /// </summary>
    /// <param name="gameDirector"></param>

    public void SetUpGenerator(GameDirector gameDirector)
    {
        this.gameDirector = gameDirector;

        // TODO 他にも初期設定したい情報がある場合にはここに処理を追加する
    }

    /// <summary>
    /// 生成状態のオン/オフを切り替え
    /// </summary>
    /// <param name="isSwitch"></param>

    public void SwitchActivation(bool isSwitch)
    {
        isActivate = isSwitch;
    }
}
