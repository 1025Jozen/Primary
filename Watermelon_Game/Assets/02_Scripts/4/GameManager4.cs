using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using UnityEngine;


namespace SampleGame_Watermelon4
{
    //次の2パターンが考えられます。
    //・新たな野菜としてトレイ上に生成
    //・2つの同じ野菜が接触したときに、次の（進化した）野菜として生成
    public class GameManager : MonoBehaviour
    {
        public GameObject[] v_Prefab = new GameObject[4];   //野菜プレハブ
        public bool isReady;                    //生成を管理するフラグ
        int v_index;                            //生成する野菜の番号
        Vector3 createPos;                      //生成する座標
        GameObject tray, crane, vegetables;     //各オブジェクト

        public float[] v_size = new float[4];  //野菜の大きさを管理する配列

        [SerializeField] Sprite[] neatImg;      //3種類の肉の画像を管理する配列
        [SerializeField] GameObject meatPrefab; //肉のプレハブ

        public int[] v_score = new int[5];     //スコアを管理する配列
        int score;                              //Scoreを管理する変数
        TextMeshProUGUI scoreText;              //Scoreを表示するUI

        void Start() {
            isReady = true;                     //最初は生成できる状態
            tray = GameObject.Find("Tray");     //Tray を探して代入
            crane = GameObject.Find("Crane");   //Crane を探して代入
            vegetables = GameObject.Find("Vegetables"); //Vegetablesを探して代入

            v_size = new float[] { 0.25f, 0.5f, 0.9f, 1.2f }; //大きさの初期値を格納

            v_score = new int[] { 0, 1, 2, 4, 8 };
            scoreText = GameObject.Find("ScoreText").GetComponent<TextMeshProUGUI>();
            scoreText.text = "Score:0";
        }

        void Update() {
            //もしisReadyがtrueで、Trayの子要素が何もない状態なら
            if (isReady && tray.transform.childCount == 0) {
                v_index = Random.Range(0, 4);       //0番から3番のいずれかの番号を取得
                                                    //取得した番号番目の野菜のインスタンスを、Trayの座標に、生成モードで生成
                CreateVegetable(0, v_index, tray.transform.position);
                isReady = false;                    //isReady をfalseにし、連続生成を防ぐ
            }
        }

        //野菜生成
        public void CreateVegetable(int t, int v, Vector3 p) {

            if (v == 4)  //かぼちゃからの進化の場合
            {                
                GameObject meat = Instantiate(meatPrefab, p, Quaternion.identity);//meatPrefab を生成
                meat.GetComponent<SpriteRenderer>().sprite = neatImg[Random.Range(0, 3)];//meatPrefab の画像をランダムで指定

                AddScore(v);  //AddScore()関数を呼び出す

                return;       //以下の処理を強制終了
            }

            GameObject vege = Instantiate(v_Prefab[v], p, Quaternion.identity);//引数受け取った引数番目の野菜を、受け取った場所に生成
            VegetableManager vm = vege.GetComponent<VegetableManager>();       //生成したオブジェクトから　VegetableManagerクラスを取得
            vm.index = v;       //生成されたオブジェクトの index を受け取った引数 v に指定
            if (t == 0)         //新規生成モードなら
            {
                vm.type = VegetableManager.MOVETYPE.NEXT;               //type をMOVETYPE.NEXT に指定
                vege.GetComponent<CircleCollider2D>().enabled = false;  //当たり判定を無効に
                vege.transform.SetParent(tray.transform);               //vege をTrayの子要素に指定
            }
            else if (t == 1) //進化モードなら
            {
                vm.type = VegetableManager.MOVETYPE.STAY;                   //typeをSTAYに指定
                vege.transform.localScale = Vector3.one * v_size[v];        //大きさを変更
                vege.transform.SetParent(vegetables.transform);             //Vegetablesの子要素に指定
                vege.GetComponent<Rigidbody2D>().AddForce(Vector2.up * 80); //ジャンプ

                AddScore(v);
            }
        }

        public void GameStart() //初回のみ SetCrane()を発動
        {            
            StartCoroutine(tray.transform.GetChild(0).GetComponent<VegetableManager>().SetCrane());//トレイの子要素の野菜を探して、 SetCrane()を呼出す
            GameObject.Find("Canvas/StartButton").gameObject.SetActive(false);//StartButtonを探して隠す
        }

        void AddScore(int n)    //引数を受け取ってスコアを加算する関数
        {
            score += v_score[n];                //scoreにv_score[]のn番目の値を加算
            scoreText.text = "Score:" + score;  //UIを更新
        }

    }
}
