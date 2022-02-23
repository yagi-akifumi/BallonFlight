using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class VerticalFloatingObject : MonoBehaviour
{
    public float moveTime;
    public float moveRange;

    Tweener tweener;    //DOTweenの処理の代入用

    void Start()
    {
        // DOTween による命令を実行し、それを tweener 変数に代入
        tweener = transform.DOMoveY(transform.position.y - moveRange, moveTime).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
    }

    private void OnDestroy()
    {
        // DOTween の処理を破棄する(Loop 処理を解消する)
        tweener.Kill();
    }
}
