using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterFace : MonoBehaviour
{
    // アニメーション対応

    Animator animator;
    public string walkAnim = "Walk";
    public string deathAnim = "Death";
    string nowAnime = "";
    string oldAnime = "";
    public float knockbackpower;                // 敵と接触した際に吹き飛ばされる力

    public int EnemysHP;
    public GameObject DestroyEaglePrefab;       //イーグル爆発エフェクトのPrefab

    [SerializeField]
    private AudioClip DestroyEagleSE;                  // イーグルと弾が接触した際に鳴らすSE用のオーディオファイルをアサインする


    // Start is called before the first frame update
    void Start()
    {
        // Animatorを取ってくる
        animator = GetComponent<Animator>();
        nowAnime = walkAnim;
        oldAnime = deathAnim;
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
            Vector3 direction = (transform.position - col.transform.position).normalized;
            transform.position += direction * knockbackpower;
            animator.Play(deathAnim);
            EnemysHP -= 1;

            Destroy(col.gameObject);
            AudioSource.PlayClipAtPoint(DestroyEagleSE, transform.position);

            if (EnemysHP == 0)
            {
                Instantiate(DestroyEaglePrefab, transform.position, Quaternion.identity);
                Destroy(gameObject);
            }

        }
    }
}
