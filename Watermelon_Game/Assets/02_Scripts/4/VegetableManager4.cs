using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SampleGame_Watermelon4
{
    //野菜の状態を管理するクラス
    public class VegetableManager : MonoBehaviour
    {
        Rigidbody2D rb;          //Rigidbody2D を管理するための変数
        GameObject crane, tray;  //CraneとTrayオブジェクト
        public int index;        //オブジェクト自身の野菜の管理番号
                                 //野菜オブジェクトの動きの状態を管理する列挙型変数
        public enum MOVETYPE {
            NEXT,
            READY,
            FALL,
            STAY
        };
        public MOVETYPE type = MOVETYPE.NEXT;   //初期状態はNEXT

        GameManager gm;     //GameManagerクラス型の変数

        void Start() {
            rb    = GetComponent<Rigidbody2D>();//Rigidbody2Dを取得し代入
            crane = GameObject.Find("Crane");   //Craneを探して代入
            tray  = GameObject.Find("Tray");    //Trayを探して代入

            gm = FindFirstObjectByType<GameManager>();   //GameManagerクラスを探して代入
        }
        void Update() {
            StateSet();    //StateSet()を呼び出す
        }

        void StateSet() {
            switch (type)               //type の値によって振分け
            {
                case MOVETYPE.NEXT:     //NEXT のケース                                       
                    rb.bodyType = RigidbodyType2D.Static;      //BodyType を Static（Rigidbody2Dの影響なし）に
                    transform.position = tray.transform.position;
                    break;
                case MOVETYPE.READY:    //READY のケース                                       
                    rb.bodyType = RigidbodyType2D.Kinematic;   //BodyType を Kinematic（スクリプト文の影響のみ）に         
                    transform.position = crane.transform.position - Vector3.up * 0.7f;//座標を常にクレーンの0.7下側に配置
                    break;
                case MOVETYPE.FALL:     //FALL のケース                                       
                    rb.bodyType = RigidbodyType2D.Dynamic;  //BodyType を Dynamic（Rigidbody2Dの影響を受ける）に
                    rb.gravityScale = 0.6f;     //重力を0.6に
                    break;
                case MOVETYPE.STAY:     //STAY のケース
                    rb.gravityScale = 0.3f;     //重力を0.3に                                               
                    if (transform.position.y > 3.5f || Mathf.Abs(transform.position.x) > 5) {//もし高さが3.5より大きい、または壁の外に出たら
                        Debug.Log("GameOver");  //GameOver と標示
                    }
                    break;
            }
        }

        public IEnumerator SetCrane()    //クレーンに移動する処理
        {
            if (type == MOVETYPE.NEXT)   //TypeがNEXTなら
            {
                yield return new WaitForSeconds(0.8f);  //0.8秒待機
                type = MOVETYPE.READY;                  //TypeをREADYに

                transform.localScale = Vector3.one * gm.v_size[index];//野菜の大きさを指定した大きさに変更

                transform.SetParent(crane.transform);   //クレーンの子要素に
                Invoke("SetIsReady", 0.2f);     //0.2秒後にisReadyをtrueに
            }
        }

        void SetIsReady() {
            gm.isReady = true;  //GameManagerクラスの isReady をtrueに
        }

        void Evolution(Collision2D col)  //Collider2D型の引数 col をもつ関数
        {
            //2つの野菜の名前が同じで、ID大きい方が発動
            if (col.gameObject.name == gameObject.name &&
                      col.transform.GetInstanceID() < GetInstanceID()) {
                Vector3 pos = (transform.position + col.transform.position) / 2;  //❶2つの中間点を取得
                Destroy(col.gameObject);                                          //❷当たった相手を削除
                Destroy(gameObject);                                              //❸自分自身を削除
                gm.CreateVegetable(1, index + 1, pos);                            //進化モードで進化した野菜生成
            }
        }

        void OnCollisionEnter2D(Collision2D col) {
            Evolution(col);          //❶関数:Evolution()の発動
            if (type == MOVETYPE.FALL)  //もしMOVETYPEがFALLなら
            {
                //相手がVegetableタグでMOVETYPEがSTAY、または相手がWallタグなら
                if ((col.gameObject.tag == "Vegetable" && col.gameObject.GetComponent<VegetableManager>().type == MOVETYPE.STAY)
                    || col.gameObject.tag == "Wall") {
                    type = MOVETYPE.STAY;    //❷MOVETYPEを STAY に変更          
                }
            }
        }
    }
}
