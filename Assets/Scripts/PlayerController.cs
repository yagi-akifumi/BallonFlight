using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private string horizontal = "Horizontal";   // キー入力用の文字列指定
    private string jump = "Jump";               // キー入力用の文字列指定
    private string fire = "Fire1";

    private Rigidbody2D rb;                     // コンポーネントの取得用
    private Animator anim;                      // アニメーションの遷移の命令を実行する

    private float limitPosX = 9.5f;             // 横方向の制限値
    private float limitPosY = 4.45f;            // 縦方向の制限値

    private bool isGameOver = false;            // GameOver状態の判定用。true ならゲームオーバー

    public bool isFirstGenerateBallon;          // 初めてバルーンを生成したかを判定するための変数(後程外部スクリプトでも利用するためpublicで宣言する)
    private float scale;                        // 向きの設定に利用する
    public float moveSpeed;                     // 移動速度
    public float jumpPower;                     // ジャンプ・浮遊力

    public GameObject[] ballons;                // GameObject型の配列。インスペクターからヒエラルキーにある Ballon ゲームオブジェクトを２つアサインする

    public int BallonCount;                     // バルーンを生成する最大数
    public int maxBallonCount;                  // バルーンを生成する最大数
    public Transform[] ballonTrans;             // バルーンの生成位置の配列
    public GameObject ballonPrefab;             // バルーンのプレファブ
    public float generateTime;                  // バルーンを生成する時間
    public bool isGenerating;                   // バルーンを生成中かどうかを判定する。false なら生成していない状態。true は生成中の状態

    public float knockbackpower;                // 敵と接触した際に吹き飛ばされる力

    public int coinPoint;                       // コインを獲得すると増えるポイントの総数
    
    public UIManager uiManager;

    [SerializeField, Header("Linecast用 地面判定レイヤー")]
    private LayerMask groundLayer;
    public bool isGrounded;

    [SerializeField]
    private StartChecker startChecker;

    [SerializeField]
    private AudioClip knockbackSE;                  // 敵と接触した際に鳴らすSE用のオーディオファイルをアサインする

    [SerializeField]
    private AudioClip coinSE;                       // コインと接触した際に鳴らすSE用のオーディオファイルをアサインする

    [SerializeField]
    private AudioClip diamondSE;                    // ダイアモンドと接触した際に鳴らすSE用のオーディオファイルをアサインする

    [SerializeField]
    private AudioClip ballonSE;                     // バルーンと接触した際に鳴らすSE用のオーディオファイルをアサインする

    [SerializeField]
    private GameObject knockbackEffectPrefab;       // 敵と接触した際に生成するエフェクト用のプレファブのゲームオブジェクトをアサインする

    [SerializeField]
    private GameObject coinEffectPrefab;            // コインと接触した際に生成するエフェクト用のプレファブのゲームオブジェクトをアサインする

    [SerializeField]
    private GameObject diamondEffectPrefab;         // ダイアモンドと接触した際に生成するエフェクト用のプレファブのゲームオブジェクトをアサインする

    [SerializeField]
    private GameObject ballonEffectPrefab;          // バルーンと接触した際に生成するエフェクト用のプレファブのゲームオブジェクトをアサインする

    public GameObject bullet;                       // バレット
    public int bulletCount;                         // バレットの数
    public int MaxbulletCount;                      // バレットの最大数
    public float bulletSpeed;                       // バレットのスピード
    public bool bulletDirection;                    // バレットの向き
    public bool bulletBombardment;                  // バレットの砲撃



    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();       // 必要なコンポーネントを取得して用意した変数に代入
        anim = GetComponent<Animator>();
        scale = transform.localScale.x;         // 向きをかえる

        ballons = new GameObject[maxBallonCount];
        jumpPower = 0f;
        BallonCount = 0;
        bulletDirection = false;                // バレットの向きは右
}

    void Update()
    {
        // 地面接地  Physics2D.Linecastメソッドを実行して、Ground Layerとキャラのコライダーとが接地している距離かどうかを確認し、接地しているなら true、接地していないなら false を戻す
        isGrounded = Physics2D.Linecast(transform.position + transform.up * 0.4f, transform.position - transform.up * 0.9f, groundLayer);

        // Sceneビューに Physics2D.LinecastメソッドのLineを表示する
        Debug.DrawLine(transform.position + transform.up * 0.4f, transform.position - transform.up * 0.9f, Color.red, 1.0f);

        // Ballons配列変数の最大要素数が 0 以上なら = インスペクターでBallons変数に情報が登録されているなら
        if (ballons[0] != null) 
        {
            if (Input.GetButtonDown(jump))
            {
                // InputManager の Jump の項目に登録されているキー入力を判定する
                Jump();
            }

            // 接地していない(空中にいる)間で、落下中の場合
            if (isGrounded == false && rb.velocity.y < 0.15f)
            {
                anim.SetTrigger("Fall");
            }
        }
        else
        {
            //Debug.Log("バルーンがない。ジャンプできない");
        }

        // Velocity.y の値が 5.0f を超える場合(ジャンプを連続で押した場合)
        if (rb.velocity.y > 5.0f)
        {
            // Velocity.y の値に制限をかける(落下せずに上空で待機できてしまう現象を防ぐため)
            rb.velocity = new Vector2(rb.velocity.x, 5.0f);
        }

        // 地面に接地していて、バルーンが生成中ではない場合
        if (isGrounded == true && isGenerating == false)
        {
            // Qボタンを押したら
            if(Input.GetKeyDown(KeyCode.Q))
            {
                Debug.Log("バルーン作成成功");

                // バルーンを１つ作成する
                StartCoroutine(GenerateBallon()); //StartCoroutine(呼び出すコルーチン・メソッドの名前(引数)) の書式で記述する
            }
        }

        /// <summary>
        /// バレットの管理
        /// </summary>       

        if (MaxbulletCount <= bulletCount)
        {
            if (Input.GetButtonDown(fire))

            {
                Invoke("bullethoju",2f);
                Debug.Log("打てないよ");
                //GameObject bulletSpeed = GameObject.Find("Bullet");
                //bulletSpeed.GetComponent<Bullet>().bulletSpeed();

            }
        }
        if (MaxbulletCount > bulletCount)
        {
            if (Input.GetButtonDown(fire))

            {
                Instantiate(bullet, transform.position, Quaternion.identity);
                bulletCount += 1;
                Debug.Log("バレット数は" + bulletCount + "");
                //GameObject bulletSpeed = GameObject.Find("Bullet");
                //bulletSpeed.GetComponent<Bullet>().bulletSpeed();
            }
        }

        if(Input.GetKeyDown(KeyCode.LeftArrow))
        {
            bulletDirection = true;
            bulletSpeed = -0.1f;
            Debug.Log("左を向いている");
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            bulletDirection = false;
            bulletSpeed = 0.1f;
            Debug.Log("右を向いている");
        }



    }
    void bullethoju()
    {
        this.bulletCount = 0;
    }
    /// <summary>
    /// ジャンプと空中浮遊
    /// </summary>

    private void Jump()
    {
        // キャラの位置を上方向へ移動させる(ジャンプ・浮遊)
        rb.AddForce(transform.up * jumpPower);

        // Jump(Up + Mid) アニメーションを再生する
        anim.SetTrigger("Jump");
    }

    void FixedUpdate()
    {
        if (isGameOver == true)
        {
            return;
        }

        Move();     // 移動
    }

    /// <summary>
    /// 移動
    /// </summary>

    private void Move()
    {
        // 水平(横)方向への入力受付
        float x = Input.GetAxis(horizontal);    // InputManager の Horizontal に登録されているキーの入力があるかどうか確認を行う

        // x の値が 0 ではない場合 = キー入力がある場合
        if (x != 0)
        {
            // velocity(速度)に新しい値を代入して移動
            rb.velocity = new Vector2(x * moveSpeed, rb.velocity.y);

            // temp 変数に現在の localScale 値を代入
            Vector3 temp = transform.localScale;

            // 現在のキー入力値 x を temp.x に代入
            temp.x = x;

            // 向きが変わるときに小数になるとキャラが縮んで見えてしまうので整数値にする
            if (temp.x > 0)
            {
                //  数字が0よりも大きければすべて1にする
                temp.x = scale;
            }
            else
            {
                //  数字が0よりも小さければすべて-1にする
                temp.x = -scale;
            }

            // キャラの向きを移動方向に合わせる
            transform.localScale = temp;

            // 待機状態のアニメの再生を止めて、走るアニメの再生への遷移を行う
            anim.SetBool("Idle", false);    // ☆　追加　Idle アニメーションを false にして、待機アニメーションを停止する
            anim.SetFloat("Run", 0.5f);     // ☆　追加  Run アニメーションに対して、0.5f の値を情報として渡す。遷移条件が greater 0.1 なので、0.1 以上の値を渡すと条件が成立してRun アニメーションが再生される

        }

        else
        {
            //  左右の入力がなかったら横移動の速度を0にしてすぐに停止させる
            rb.velocity = new Vector2(0, rb.velocity.y);

            //  走るアニメの再生を止めて、待機状態のアニメの再生への遷移を行う
            anim.SetFloat("Run", 0.0f);     // ☆　追加  Run アニメーションに対して、0.f の値を情報として渡す。遷移条件が less 0.1 なので、0.1 以下の値を渡すと条件が成立してRun アニメーションが停止される
            anim.SetBool("Idle", true);     // ☆　追加　Idle アニメーションを true にして、待機アニメーションを再生する
        }

        // 現在の位置情報が移動範囲の制限範囲を超えていないか確認する。超えていたら、制限範囲内に収める
        float posX = Mathf.Clamp(transform.position.x, -limitPosX, limitPosX);
        float posY = Mathf.Clamp(transform.position.y, -limitPosY, limitPosY);

        // 現在の位置を更新(制限範囲を超えた場合、ここで移動の範囲を制限する)
        transform.position = new Vector2(posX, posY);
    }

    /// <summary>
    /// バルーン生成
    /// </summary>
    /// <returns></returns>

    private IEnumerator GenerateBallon()
    {
        // すべての配列の要素にバルーンが存在している場合には、バルーンを生成しない
        if(ballons[1]!=null)
        {
            yield break;        //yield による処理。yield break は、コルーチン処理を終了する命令
        }

        // 生成中状態にする
        isGenerating = true;

        // isFirstGenerateBallon 変数の値が false、つまり、ゲームを開始してから、まだバルーンを１回も生成していないなら
        if(isFirstGenerateBallon==false)
        {
            // 初回バルーン生成を行ったと判断し、true に変更する = 次回以降はバルーンを生成しても、if 文の条件を満たさなくなり、この処理には入らない
            isFirstGenerateBallon = true;
            //Debug.Log("初回のバルーン生成");

            // startChecker 変数に代入されている StartChecker スクリプトにアクセスして、SetInitialSpeed メソッドを実行する
            startChecker.SetInitialSpeed();
        }

        // １つめの配列の要素が空なら
        if (ballons[0]==null)
        {
            // 1つ目のバルーン生成を生成して、1番目の配列へ代入
            BallonCount += 1;
            ballons[0] = Instantiate(ballonPrefab, ballonTrans[0]);
            ballons[0].GetComponent<Ballon>().SetUpBallon(this);
            jumpPower += 150f;
        }
        else
        {
            // 2つ目のバルーン生成を生成して、2番目の配列へ代入
            BallonCount += 1;
            ballons[1] = Instantiate(ballonPrefab, ballonTrans[1]);
            ballons[1].GetComponent<Ballon>().SetUpBallon(this);
            jumpPower += 150f;
        }

        // 生成時間分待機
        yield return new WaitForSeconds(generateTime);  // yield による処理。yield return new WaitForSecondsメソッドは、引数で指定した秒数だけ次の処理へ移らずに処理を一時停止する処理
        // 生成中状態終了。再度生成できるようにする
        isGenerating = false;
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        // 接触したコライダーを持つゲームオブジェクトのTagがEnemyなら
        if (col.gameObject.tag == "Enemy")
        {
            // キャラと敵の位置から距離と方向を計算
            Vector3 direction = (transform.position - col.transform.position).normalized;

            // 敵の反対側にキャラを吹き飛ばす
            transform.position += direction * knockbackpower;

            // 敵との接触用のSE(AudioClip)を再生する
            AudioSource.PlayClipAtPoint(knockbackSE, transform.position);

            // 接触した際のエフェクトを、敵の位置に、クローンとして生成する。生成されたゲームオブジェクトを変数へ代入
            GameObject knockbackEffect = Instantiate(knockbackEffectPrefab, col.transform.position, Quaternion.identity);

            // エフェクトを 0.5 秒後に破棄。生成したタイミングで変数に代入しているので、削除の命令が出せる
            Destroy(knockbackEffect, 0.5f);

            DestroyBallon();

        }
    }

    /// <summary>
    /// バルーン破壊
    /// </summary>

    public void DestroyBallon()
    {
        if(ballons[1]!=null)
        {
            Destroy(ballons[1]);
            BallonCount -= 1;
            jumpPower -= 150f;
        }
        else if(ballons[0]!=null)
        {
            Destroy(ballons[0]);
            BallonCount -= 1;
            jumpPower -= 150f;
        }
    }

    // IsTriggerがオンのコライダーを持つゲームオブジェクトを通過した場合に呼び出されるメソッド
    private void OnTriggerEnter2D(Collider2D col)
    {
        // 通過したコライダーを持つゲームオブジェクトの Tag が Coin の場合
        if (col.gameObject.tag == "Coin")
        {

            Debug.Log("コイン獲得");

            // 通過したコインのゲームオブジェクトの持つ Coin スクリプトを取得し、point 変数の値をキャラの持つ coinPoint 変数に加算
            coinPoint += col.gameObject.GetComponent<Coin>().point;

            // コインポイントをUIマネージャーに表示
            uiManager.UpdateDisplayScore(coinPoint);
            
            // 通過したコインのゲームオブジェクトを破壊する
            Destroy(col.gameObject);

            // コインとの接触用のSE(AudioClip)を再生する
            AudioSource.PlayClipAtPoint(coinSE, transform.position);

            // 接触した際のエフェクトを、コインの位置に、クローンとして生成する。生成されたゲームオブジェクトを変数へ代入
            GameObject coinEffect = Instantiate(coinEffectPrefab, col.transform.position, Quaternion.identity);

            // エフェクトを 0.5 秒後に破棄。生成したタイミングで変数に代入しているので、削除の命令が出せる
            Destroy(coinEffect, 0.5f);
        }

        // 通過したコライダーを持つゲームオブジェクトの Tag が Diamond の場合
        if (col.gameObject.tag == "Diamond")
        {

            Debug.Log("ダイアモンド獲得");

            // 通過したコインのゲームオブジェクトの持つ Coin スクリプトを取得し、point 変数の値をキャラの持つ coinPoint 変数に加算
            coinPoint += col.gameObject.GetComponent<Diamond>().diamond;

            // コインポイントをUIマネージャーに表示
            uiManager.UpdateDisplayScore(coinPoint);

            // 通過したダイアモンドのゲームオブジェクトを破壊する
            Destroy(col.gameObject);

            // ダイアモンドとの接触用のSE(AudioClip)を再生する
            AudioSource.PlayClipAtPoint(diamondSE, transform.position);

            // 接触した際のエフェクトを、ダイアモンドの位置に、クローンとして生成する。生成されたゲームオブジェクトを変数へ代入
            GameObject diamondEffect = Instantiate(diamondEffectPrefab, col.transform.position, Quaternion.identity);

            // エフェクトを 0.8 秒後に破棄。生成したタイミングで変数に代入しているので、削除の命令が出せる
            Destroy(diamondEffect, 0.8f);
        }

        // 通過したコライダーを持つゲームオブジェクトの Tag が Ballon の場合
        if (col.gameObject.tag == "Ballon")
        {

            StartCoroutine(GenerateBallon()); //StartCoroutine(呼び出すコルーチン・メソッドの名前(引数)) の書式で記述する
            Debug.Log("バルーン獲得");

            // 通過したバルーンのゲームオブジェクトの持つ Coin スクリプトを取得し、point 変数の値をキャラの持つ coinPoint 変数に加算
            coinPoint += col.gameObject.GetComponent<ItemBallon>().itemBallon;

            // コインポイントをUIマネージャーに表示
            uiManager.UpdateDisplayScore(coinPoint);

            // 通過したバルーンのゲームオブジェクトを破壊する
            Destroy(col.gameObject);

            // バルーンとの接触用のSE(AudioClip)を再生する
            AudioSource.PlayClipAtPoint(diamondSE, transform.position);

            // 接触した際のバルーンを、ダイアモンドの位置に、クローンとして生成する。生成されたゲームオブジェクトを変数へ代入
            GameObject ballonEffect = Instantiate(ballonEffectPrefab, col.transform.position, Quaternion.identity);

            // バルーンを 0.8 秒後に破棄。生成したタイミングで変数に代入しているので、削除の命令が出せる
            Destroy(ballonEffect, 0.8f);
        }

    }

    /// <summary>
    /// ゲームオーバー
    /// </summary>

    public void GameOver()
    {
        isGameOver = true;

        // Console ビューに isGameOver 変数の値を表示する。ここが実行されると true と表示される
        Debug.Log(isGameOver);

        // 画面にゲームオーバー表示を行う
        uiManager.DisplayGameOverInfo();
    }
}

