using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.tag=="Player")
        {
            col.gameObject.GetComponent<PlayerController>().GameOver();   //GameOverのコンポーネントを取得
            Debug.Log("Game Over");
        }
    }
}