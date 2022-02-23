using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParentSwitcher : MonoBehaviour
{
    private string player = "Player";       // Tag に設定している文字列を代入

    // このスクリプトがアタッチされているゲームオブジェクトのコライダーと、他のゲームオブジェクトのコライダーが接触している間ずっと接触判定を行うメソッド
    private void OnCollisionStay2D(Collision2D col)
    {
        // 接触判定が発生すると col 変数にコライダーの情報が代入される。そのコライダーを持つゲームオブジェクトのTagが player 変数の値（"Player"）と同じ文字列なら
        if (col.gameObject.tag == player)
        {
            // 接触しているゲームオブジェクト(キャラ)を、このスクリプトがアタッチされているゲームオブジェクト(床)の子オブジェクトにする
            col.transform.SetParent(transform);
        }
    }

    // このスクリプトがアタッチされているゲームオブジェクトのコライダーと、他のゲームオブジェクトのコライダーとが離れた際に判定を行うメソッド
    private void OnCollisionExit2D(Collision2D col)
    {
        // コライダーが離れた判定が発生すると col 変数にコライダーの情報が代入される。そのコライダーを持つゲームオブジェクトのTagが player 変数の値（"Player"）と同じ文字列なら
        if (col.gameObject.tag == player)
        {
            // 接触状態ではなくなった(離れた)ゲームオブジェクト(キャラ)と、このスクリプトがアタッチされているゲームオブジェクト(床)の親子関係を解消する
            col.transform.SetParent(null);
        }
    }


}
