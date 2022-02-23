using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHP : MonoBehaviour
{
    public int EnemysHP;
    public GameObject DestroyEaglePrefab;       //イーグル爆発エフェクトのPrefab

    [SerializeField]
    private AudioClip DestroyEagleSE;                  // イーグルと弾が接触した際に鳴らすSE用のオーディオファイルをアサインする


    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D col)
    {

        if (col.gameObject.tag == "Bullet")
        {
            Debug.Log("敵とぶつかった1。");
            EnemysHP -= 1;
            Destroy(col.gameObject);
            AudioSource.PlayClipAtPoint(DestroyEagleSE, transform.position);

            if (EnemysHP==0)
            {
                Instantiate(DestroyEaglePrefab, transform.position, Quaternion.identity);
                Destroy(gameObject);
            }

        }
    }
}
