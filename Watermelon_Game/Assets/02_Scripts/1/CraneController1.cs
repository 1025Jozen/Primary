using UnityEngine;
using UnityEngine.InputSystem;

namespace SampleGame_Watermelon1
{
    public class CraneController : MonoBehaviour
    {
        float stepX;    //InputSystemから受け取る値を管理
        float speed;    //移動速度を管理する変数

        void Start() {
            transform.position = new Vector2(0, 4.5f);  //Craneの初期位置
            speed = 0.05f;                              //移動速度に0.05を代入
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
    }
}