using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartChecker : MonoBehaviour
{
    private MoveObject moveObject;          // MoveObject スクリプトを取得した際に代入するための準備

    // Start is called before the first frame update
    void Start()
    {
        moveObject = GetComponent<MoveObject>();    // このスクリプトがアタッチされているゲームオブジェクトの持つ、MoveObject スクリプトを探して取得し、moveObject 変数に代入
    }

    /// <summary>
    /// 空中床に移動速度を与える
    /// </summary>

    public void SetInitialSpeed()
    {
        // アサインしているゲームオブジェクトの持つ MoveObject スクリプトの moveSpeed 変数にアクセスして、右辺の値を代入する
        moveObject.moveSpeed = 0.005f;
    }
}