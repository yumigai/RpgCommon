using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// グループに指定した一つのみを表示・有効化するためのクラス
/// </summary>
public class PanelGroupMng : MonoBehaviour
{
    public enum OFF_TYPE
    {
        HIDE, //非表示
        BLOCK, //入力不可（未対応）
    }

    [SerializeField,Tooltip("インデックスの大きいオブジェクトが優先される")]
    protected List<GameObject> GroupList;

    [SerializeField, Tooltip("OFF時反応個別指定")]
    protected List<OFF_TYPE> OffTypes;

    [SerializeField,Tooltip("OFF時反応一括指定")]
    protected OFF_TYPE OffType;

    [SerializeField]
    protected bool IsAuto = true;

    public void Update() {
        if (IsAuto) {
            ChangeStatus();
        }
    }

    public void ChangeStatus() {
        if (GroupList.Count(it => it.activeSelf) > 1) {
            var index = GroupList.FindLastIndex(it => it.activeSelf);
            for (int i = index - 1; i >= 0; i--) {
                GroupList[i].SetActive(false);
            }
        }
    }

}
