using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapCenterArrowMng : MonoBehaviour
{
    /// <summary>
    /// ミニマップのプレイヤーが部屋に到達したらマップを表示する
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision) {
        FieldUIMng.Singleton.onMapCenter(collision.gameObject);
    }
}
