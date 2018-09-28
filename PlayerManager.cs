//学習内容:①Input.GetAxisRaw("取得するキーの種類")_②animator.SetBool("変数名", bool値)
//①GetAxisRawでキーボードのカーソルを取得して1、-1を返す。("Horizontal"):←左は-1、→右は1。("Vertical"):↑上は1、↓下は-1。
//②Animatorウィンドウからbool型変数を登録可能。アニメーションの変移時にtrue,falseを切り替えられる。スクリプト側はanimator.SetBool("変数名", bool値)。
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class PlayerManager : MonoBehaviour {

    public GameObject gameManager;  //ゲームマネージャー
    public LayerMask blockLayer;     //ブロックレイヤー

    private Rigidbody2D rbody;  //プレイヤー制御用Rigidbody2D

    private Animator animator; //アニメーター

    private const float MOVE_SPEED = 3; //移動速度固定値
    private float moveSpeed;            //移動速度

    private float jumpPower = 400;     //ジャンプの力
    private bool goJump = false;       //ジャンプしたかどうか
    private bool canJump = false;      // ブロックに設置しているかどうか
    private bool usingButtons = false; //ボタンを押しているか否か　PCでデバック時に必要

    private bool afterStart = false;

    public enum MOVE_DIR                //移動方向定義 ※enum 列挙型
    {
        STOP,
        LEFT,
        RIGHT,
    }

    private MOVE_DIR moveDirection = MOVE_DIR.STOP; //移動方向 最初は動かんからSTOPを入れとく

    // Use this for initialization
    void Start() {
        rbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        animator.SetBool("onGround", true); 
        /*スタート時、ブロックから離れた状態で始まるためジャンプアニメーションが再生される。
         なのでここでブロックに接地したことにしておく*/

        Invoke("AfterStart", 0.5f);  
       

    }

    void AfterStart()
    {
        afterStart = true;
    }

    //衝突処理
    void OnTriggerEnter2D(Collider2D col)
    {
        //プレイ中のみ接触判定を与える
        if (gameManager.GetComponent<GameManager>().gameMode != GameManager.GAME_MODE.PLAY)
        {
            return;  //プレイ中でなければリターンで処理を終了する
        }

        if (col.gameObject.tag == "Trap") //トラップのタグに触れたら…
        {
            gameManager.GetComponent<GameManager>().GameOver();　　//GameManager(スクリプト)からGameOverメソッドを取得
            DestroyPlayer();
        }

        if (col.gameObject.tag == "Goal")
        {
            gameManager.GetComponent<GameManager>().GameClear();
        }

        if(col.gameObject.tag == "Enemy")
        {
            if(transform.position.y > col.gameObject.transform.position.y + 0.4f)　　//敵の上の方を踏んだら
            {
                rbody.velocity = new Vector2(rbody.velocity.x, 0);　　　　　//落下しているので上方向の力は０にしとく
                rbody.AddForce (Vector2.up * jumpPower);                    //踏んだら上方向の力を４００足す
                col.gameObject.GetComponent<EnemyManager>().DestroyEnemy();
            }
            else
            {
                //衝突
                gameManager.GetComponent<GameManager>().GameOver();
                DestroyPlayer();
            }
        }

        if(col.gameObject.tag == "Orb")
        {
            col.gameObject.GetComponent<OrbManager>().GetOrb();
        }

    }
        void DestroyPlayer()
        {
        gameManager.GetComponent<GameManager>().gameMode =  GameManager.GAME_MODE.GAMEOVER;

        //コライダーを削除  取得→削除
        CircleCollider2D circleCollider = GetComponent<CircleCollider2D>();
        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
        Destroy(circleCollider);
        Destroy(boxCollider);

        //死亡アニメーション
        Sequence animSet = DOTween.Sequence();  //DOTweenの拡張メソッド Sequence各アニメーションを連結したり、同時に実行したり
        animSet.Append(transform.DOLocalMoveY(1.0f, 0.2f).SetRelative());
        animSet.Append(transform.DOLocalMoveY(-10.0f, 1.0f).SetRelative());

        Destroy(this.gameObject,1.2f);  //Destroy(Object,float f) で消す時間を指定できる
        }


    // Update is called once per frame
    void Update()
    {

        

        if (afterStart)
        /*Updateの中に"animator.SetBool("onGround", canJump);"があるため
        スタート時にtrueにしたが意味をなさない。なので数秒後にafterStartをtrueにしてUpdateの処理を遅らせる。
        たぶんもっといい方法がある？はず;*/
        {

            canJump = Physics2D.Linecast(transform.position - (transform.right * 0.3f),             //position：座標、right,up：ベクトル
                                         transform.position - (transform.up * 0.1f), blockLayer) || //Linecst：（始点,終点,レイヤー）
                                                                                                    //　↑bool型。始点と終点を設定し、そこに線を張る。
                                                                                                    //コライダーがヒットしたらtrueを返す
                      Physics2D.Linecast(transform.position + (transform.right * 0.3f),
                                         transform.position - (transform.up * 0.1f), blockLayer);

            animator.SetBool("onGround", canJump);

            if (!usingButtons)
            {
                float x = Input.GetAxisRaw("Horizontal");

                if (x == 0)
                {
                    moveDirection = MOVE_DIR.STOP;
                }
                else
                {
                    if (x < 0)
                    {
                        moveDirection = MOVE_DIR.LEFT;
                    }
                    else
                    {
                        moveDirection = MOVE_DIR.RIGHT;
                    }
                }
            }
            if (Input.GetKeyDown("space"))
            {
                PushJumpButton();
            }
        }
    }
    private void FixedUpdate() //固定フレームレートで呼び出される　defaultでは0.02秒(50fps)
    {
        //移動方向で処理を分岐
        switch (moveDirection)     //条件判断が何行も繰り返される場合、switch caseの方が向いてる
        {
            case MOVE_DIR.STOP: //停止
                moveSpeed = 0;
                break;

            case MOVE_DIR.LEFT: //左に移動
                moveSpeed = MOVE_SPEED * -1;
                transform.localScale = new Vector2(-1, 1);　//これで反転する
                break;

            case MOVE_DIR.RIGHT: //右に移動
                moveSpeed = MOVE_SPEED;
                transform.localScale = new Vector2(1, 1);
                break;
            
        }
        rbody.velocity = new Vector2(moveSpeed, rbody.velocity.y);
        // ※volocityは速度のこと　ｙ軸方向の速度はそのままなので変えない

        //ジャンプ処理
        if (goJump)
        {
            rbody.AddForce(Vector2.up * jumpPower);
            goJump = false;

           /*   if (bool型 == true){ } のように 条件 がtrueと等しいか評価する式は書かずにif(bool型){ }
            
            と書くことができる。*/
                
        }

    }

    // 左ボタンを押した
    public void PushLeftButton()
    {
        moveDirection = MOVE_DIR.LEFT;
        usingButtons = true;
    }

    //右ボタンを押した
    public void PushRightButton()
    {
        moveDirection = MOVE_DIR.RIGHT;
        usingButtons = true;
    }

    public void PushJumpButton()
    {
        if (canJump)　//canJumpがtrue ならgoJumpをtrueにする
        {
            goJump = true;
        }
    }

    //移動ボタンを離した
    public void ReleaseMoveButton()
    {
        moveDirection = MOVE_DIR.STOP;
        usingButtons = false;
    }
}


