using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SampleGame_Watermelon4
{
    public class UIManager : MonoBehaviour
    {
        GameObject water;       //鍋の水のImage

        void Start() {
            //WaterImageを探して代入
            water = GameObject.Find("BackCanvas/WaterImage").gameObject;
        }

        void FixedUpdate() {
            //waterのローカル座標を三角関数を使って左右に移動
            water.transform.position += new Vector3(Mathf.Cos(Time.time) / 100, 0, 0);
        }

    }
}
