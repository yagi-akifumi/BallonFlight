using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int BulletPoint;

    public GameObject DestroyBallonPrefab;      //バルーン爆発エフェクトのPrefab
    public GameObject DestroyEaglePrefab;       //イーグル爆発エフェクトのPrefab

    [SerializeField]
    private AudioClip DestroyBallonSE;                  // バルーンと弾が接触した際に鳴らすSE用のオーディオファイルをアサインする

    [SerializeField]
    private AudioClip DestroyEagleSE;                  // イーグルと弾が接触した際に鳴らすSE用のオーディオファイルをアサインする

    public float Speed;
    public bool Bombar;
    public float balletSpeeds;

    // Start is called before the first frame update
    void Start()
    {

        Bombar = false;
        Speed = GameObject.Find("Yuko_Player").GetComponent<PlayerController>().bulletSpeed;
        transform.Translate(Speed, 0, 0);
        if (Speed == 0)
        {
            this.Speed = 0.1f;
        }

        

        

    }

    // Update is called once per frame
    void Update()
    {
        Bombar = true;
        if (Bombar == true)
        {
            this.balletSpeeds= Speed;
            transform.Translate(balletSpeeds, 0, 0);
        }

        if (transform.position.x > 10.0f)
        {
            Destroy(gameObject);
        }

        if (transform.position.x < -10.0f)
        {
            Destroy(gameObject);
        }

        if (balletSpeeds < 0)
        {
            int balletDir = -4;
            transform.localScale = new Vector3(balletDir, 4, 4);
        }

    }

    private void OnTriggerEnter2D(Collider2D col)
    {

        if (col.gameObject.tag == "Enemy")
        {
        Debug.Log("tama当たった。");
        //    Instantiate(DestroyEaglePrefab, transform.position, Quaternion.identity);
        //AudioSource.PlayClipAtPoint(DestroyEagleSE, transform.position);
        //Destroy(col.gameObject);
        //Destroy(gameObject);
        }
        if (col.gameObject.tag == "Ballon")
        {
            Debug.Log("バルーンとぶつかった。");
            Instantiate(DestroyBallonPrefab, transform.position, Quaternion.identity);
            AudioSource.PlayClipAtPoint(DestroyBallonSE, transform.position);
            Destroy(col.gameObject);
            Destroy(gameObject);
        }
    }

}