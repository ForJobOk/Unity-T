//学習内容:①列挙型enum_②Linecast:bool型のメソッド＿③localScale＿④DOTween
//①列挙型:enum name{列挙子１,列挙子２...}、列挙子を取り出す時はname.列挙子。不変の値に対して使う(曜日、方角等)。swichと相性良し。
//②LineCast(始点,終点,レイヤー)、始点から終点まで線を引いて、その線に対象のレイヤーが衝突したらtrueを返す。プレイヤーのジャンプ動作に使える。
//③localScaleでスクリプトからオブジェクトの反転が可能。transform.localScale = new Vector2(ｘ, ｙ)のx,yの正・負で反転。
//④DOTween：アセットストアで購入可能。大変、便利らしい。


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EnemyManager : MonoBehaviour {

    private const int ENEMY_POINT = 50; //敵の得点

    private GameObject gameManager;  //ゲームマネージャー

    public LayerMask blockLayer;  //ブロックのレイヤー

    private Rigidbody2D rbody;    //敵制御用Rigidbody2D

    private float moveSpeed = 1;  //移動速度

    public enum MOVE_DIR         //移動方向定義    
    {
        LEFT,
        RIGHT
    }

    private MOVE_DIR moveDirection = MOVE_DIR.LEFT; //移動方向

    // Use this for initialization
    void Start()
    {
        rbody = GetComponent<Rigidbody2D>();    //Rigidbody2D　＝　物理的な色々が可能

        gameManager = GameObject.Find("GameManager");

    }

	// Update is called once per frame
	void Update () {
		
	}

    void FixedUpdate()
    {
        bool isBlock; //進行方向にブロックがあるかどうか


        switch (moveDirection)
        {
            case MOVE_DIR.LEFT:   //左に移動
                rbody.velocity = new Vector2(moveSpeed * -1, rbody.velocity.y);
                transform.localScale = new Vector2(1, 1);  // localPosition = 親の Transform オブジェクトから見た相対的なスケール
                //↑これで左右の反転を行っている

                //Linecast(bool型)を使ってキャラクター前方に衝突判定を与える
                isBlock = Physics2D.Linecast　　　　//(始点、終点、レイヤー)　レイヤーは衝突の対象となるもの
                    (new Vector2(transform.position.x, transform.position.y + 0.5f),
                    new Vector2(transform.position.x - 0.3f, transform.position.y + 0.5f), blockLayer);

                if (isBlock)   //ぶつかったら移動方向反転
                {
                    moveDirection = MOVE_DIR.RIGHT;
                }
                break;

            case MOVE_DIR.RIGHT:   //右に移動
                rbody.velocity = new Vector2(moveSpeed * 1, rbody.velocity.y);
                transform.localScale = new Vector2(-1, 1);  //反転

                isBlock = Physics2D.Linecast
                    (new Vector2(transform.position.x, transform.position.y + 0.5f), 
                    (new Vector2(transform.position.x + 0.3f, transform.position.y + 0.5f)), blockLayer);

                if (isBlock)　　//ぶつかったら移動方向反転
                {
                    moveDirection = MOVE_DIR.LEFT;
                }
                break;
        }
        
    }
    //敵オブジェクト削除処理
    public void DestroyEnemy()
    {
        gameManager.GetComponent<GameManager>().AddScore(ENEMY_POINT);

        rbody.velocity = new Vector2(0,0);
        //コライダーを削除
        CircleCollider2D circleCollider = GetComponent<CircleCollider2D>();　　//画面外に消すのでコライダーを消す
        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
        Destroy(circleCollider);　　　　　　　　　　　　　　　　　　　　　　　　　//コライダーを消すには一度変数に入れてから
        Destroy(boxCollider);

        //死亡アニメーション
        Sequence animSet = DOTween.Sequence();
        animSet.Append(transform.DOLocalMoveY(0.5f, 0.2f).SetRelative());　
        animSet.Append(transform.DOLocalMoveY(-10.0f, 1.0f).SetRelative());


        Destroy(this.gameObject,1.2f);
    }
}
