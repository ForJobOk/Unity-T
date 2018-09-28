using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager: MonoBehaviour {

    //定数定義
    private const int MAX_SCORE = 999999;

    public GameObject textGameOver; //ゲームオーバーテキスト
    public GameObject textClear; 　//クリアテキスト
    public GameObject buttons;　　　//操作ボタン
    public GameObject textNumber;   //スコアのテキスト

    public enum GAME_MODE   //ゲーム状態定義  クリア状態とプレイ状態を分ける　＝　ゴール時のみ接触判定を与える　
    {　
        PLAY,　　　　　　　//プレイ中
        CLEAR,             //クリア
        GAMEOVER           //ゲームオーバー
    }

    public GAME_MODE gameMode = GAME_MODE.PLAY;  //ゲーム状態

    private int score = 0;          //スコア
    private int displayScore = 0;   //表示用スコア


    //スコア計算
    public void AddScore(int val)
    {
        score += val;
        if(score > MAX_SCORE)
        {
            score = MAX_SCORE; 　//上限を決める
        }
    }

    //スコア表示を更新
    void RefreshScore()
    {
        textNumber.GetComponent<Text>().text = displayScore.ToString();
    }



	// Use this for initialization
	void Start () {
        RefreshScore();
	}
	
	// Update is called once per frame
	void Update () {
		if(score > displayScore)
        {
            displayScore += 10;

            if(displayScore > score)
            {
                displayScore = score;
            }

            RefreshScore();
        }
	}

    //ゲームオーバーの処理
    public void GameOver()
    {
        textGameOver.SetActive(true);

        buttons.SetActive(false);
    }

    //ゲームクリア処理
    public void GameClear()
    {
        gameMode = GAME_MODE.CLEAR;
        textClear.SetActive(true);
        buttons.SetActive(false);
    }
}
