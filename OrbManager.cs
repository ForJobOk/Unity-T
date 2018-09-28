using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class OrbManager : MonoBehaviour {

    private const int ORB_POINT = 100; //オーブの得点

    private GameObject gameManager;   //ゲームマネージャー

	// Use this for initialization
	void Start () {
        gameManager = GameObject.Find("GameManager");  //Find　は　全オブジェクトから検索するため処理の負荷が大きい
                                                       //　↑Start時に検索しておくことで負荷を最小限にできる　

    }

    // Update is called once per frame
    void Update () {
		
	}

    //オーブ入手
    public void GetOrb() {

        gameManager.GetComponent<GameManager>().AddScore(ORB_POINT);

        //コライダーを削除
        CircleCollider2D circleCollider = GetComponent<CircleCollider2D>();
        Destroy(circleCollider);

        //消失アニメーション
        transform.DOScale(2.5f, 0.3f);  //大きさが変わる
        SpriteRenderer spriteRenderer = transform.GetComponent<SpriteRenderer>();　　//画像取得
        DOTween.ToAlpha(() => spriteRenderer.color, a => spriteRenderer.color = a, 0.0f, 0.3f);

        Destroy(this.gameObject,0.5f);
    }
}
