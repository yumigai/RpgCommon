using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CircleLayoutGroupMng : UIBehaviour, ILayoutGroup {

    [SerializeField]
    public float CircleSize = 200f;

    [SerializeField]
    public bool IsAuto = true;

    public void SetLayoutHorizontal() {
        
    }

    public void SetLayoutVertical() {
        if (IsAuto) {
            Arrange();
        }
    }

    new void OnEnable() {
        Arrange();
    }

    public void Arrange() {

        if (!this.gameObject.activeSelf) {
            return;
        }

        List<RectTransform> itms = new List<RectTransform>();

        var angle_unit = 360 / this.transform.childCount;

        var r = CircleSize / 2;

        for ( int i = 0; i < this.transform.childCount; i++) {

            var angle = angle_unit * i;

            var x = Mathf.Cos(angle * Mathf.Deg2Rad) * r;
            var y = Mathf.Sin(angle * Mathf.Deg2Rad) * r;

            this.transform.GetChild(i).GetComponent<RectTransform>().localPosition = new Vector2(x, y);

        }
    }
}
