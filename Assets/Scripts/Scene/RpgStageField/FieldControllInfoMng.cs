using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 入力ガイドをフィールド上に表示する
/// A長押し、等
/// </summary>
public class FieldControllInfoMng : FieldObjectInfoMng
{

    [SerializeField]
    public ButtonGuidMng ButtonGuid;

    new protected void FixedUpdate() {
        base.FixedUpdate();
    }
}
