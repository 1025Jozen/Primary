using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SampleGame_Watermelon4
{
    public class CraneController : MonoBehaviour
    {
        float stepX;    //InputSystemから受け取る値を管理
        float speed;    //移動速度を管理する変数

        GameObject tray, vegetables;  //TrayとVegatablesのオブジェクトにアクセスするための変数

        [SerializeField] Sprite[] craneImg; //クレーンの画像配列
        SpriteRenderer sr;  //SpriteRenderer型の変数

        void Start() {
            transform.position = new Vector2(0, 4.5f);  //Craneの初期位置
            speed = 0.05f;                              //移動速度に0.05を代入

            tray       = GameObject.Find("Tray");       //Tray を探して代入
            vegetables = GameObject.Find("Vegetables"); //Vegetables を探して代入

            sr = GetComponent<SpriteRenderer>();   //SpriteRendererコンポーネントを取得し代入
            sr.sprite = craneImg[0];               //画像の初期値を閉じたクレーン画像に
        }
        void FixedUpdate() {
            transform.position += transform.right * stepX * speed;//水平方向の移動

            //クレーンの移動制限
            float posX = transform.position.x;              //Craneのx座標
            posX       = Mathf.Clamp(posX, -1.2f, 1.2f);        //posXを-1.2～1.2の範囲におさめる 
            transform.position = new Vector3(posX, 4.5f, 0);//座標を再調整
        }

        public void Move(InputAction.CallbackContext context) {
            stepX = context.ReadValue<float>();//InputSystemからfloat型の値を取得、変数：stepXに代入
        }

        public void Release(InputAction.CallbackContext context) {
            //（初回の信号のみ取得）クレーンが野菜をつかんでいれば
            if (context.performed && transform.childCount == 1) {

                //つかんでいる野菜から VegetableManagarのスクリプトファイルを取得し、vmという名前で変数化
                VegetableManager vm = transform.GetChild(0).GetComponent<VegetableManager>();
                //vmのtypeがREADYで、かつTrayの子要素として野菜が存在していれば

                if (vm.type == VegetableManager.MOVETYPE.READY && tray.transform.childCount == 1) {
                    vm.type = VegetableManager.MOVETYPE.FALL;         //mvのtypeをFALLに指定
                    vm.gameObject.GetComponent<CircleCollider2D>().enabled = true;   //当たり判定を有効に
                    vm.transform.SetParent(vegetables.transform);   //mvをVegetablesの子要素に
                                                                    //Trayの子要素の野菜からVegetableManagerクラスを取得、SetCrean()を発動
                    StartCoroutine(tray.transform.GetChild(0).GetComponent<VegetableManager>().SetCrane());
                    StartCoroutine(CraneOpen());  //コルーチンの呼び出し
                }
            }
        }

        IEnumerator CraneOpen() {
            sr.sprite = craneImg[1];                //開いた画像
            yield return new WaitForSeconds(0.5f);  //0.5秒待機
            sr.sprite = craneImg[0];                //閉じた画像
        }
    }
}