using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ZigzagLayoutGroup : UIBehaviour, ILayoutGroup {

    public enum POSI_Y {
        UNDER,
        UPPER
    }
    public enum POSI_X {
        LEFT,
        CENTER,
        RIGHT,
    }

    //public enum SORT {
    //    UNDER_LEFT,
    //    UPPER_LEFT,
    //    UNDER_CENTER,
    //    UPPER_CENTER,
    //    UNDER_RIGHT,
    //    UPPER_RIGHT,
    //}

    [SerializeField]
    public POSI_Y PosiY;

    [SerializeField]
    public POSI_X PosiX;

    [SerializeField]
    public int Row = 2;

    [SerializeField]
    public Vector2 Spacing = new Vector2(100f, 100f);

    [SerializeField]
    public bool IsAuto = true;

    [SerializeField, HeaderAttribute("範囲内に収まるならジグザグしない")]
    public bool IsInnerToNoZigzag;

    [SerializeField, HeaderAttribute("ジグザグしない場合のスペース")]
    public Vector2 NoZigzagSpacing = new Vector2(0f, 0f);


#if UNITY_EDITOR
    protected override void OnValidate() {
        base.OnValidate();
        if (IsAuto) {
            Arrange();
        }
    }
	#endif

    #region ILayoutController implementation
    public void SetLayoutHorizontal() { }
    public void SetLayoutVertical() {
        if (IsAuto) {
            Arrange();
        }
    }
    #endregion

    new void OnEnable() {
        Arrange();
    }

    public void Arrange() {

        List<RectTransform> itms = new List<RectTransform>();
        float total_width = 0f;

        for (int i = 0; i < this.transform.childCount; i++) {
            RectTransform re = this.transform.GetChild(i).GetComponent<RectTransform>();
            if (re.gameObject.activeSelf) {
                itms.Add(re);
                total_width += re.sizeDelta.x + Spacing.x;
            }
        }

        bool no_zigzag = IsInnerToNoZigzag && ( total_width <= this.GetComponent<RectTransform>().sizeDelta.x );

        RectTransform[] childs = itms.ToArray();

        float x_posi = 0f;

        for (int i = 0; i < childs.Length; i++) {

            float x_space = no_zigzag ? NoZigzagSpacing.x : Spacing.x;
            float y_shift = no_zigzag ? NoZigzagSpacing.y : Spacing.y;

            bool even = ( i % 2 == 0);

            if (PosiY == POSI_Y.UPPER) {
                y_shift = even ? y_shift : -y_shift;
            } else {
                y_shift = even ? -y_shift : y_shift;
            }

            if (i > 0) {
                if (PosiX == POSI_X.RIGHT) {
                    x_posi -= childs[i].sizeDelta.x + x_space;
                } else {
                    x_posi += childs[i].sizeDelta.x + x_space;
                }
            }

            Vector2 posi = new Vector2(x_posi, y_shift);

            childs[i].localPosition = posi;

        }

        if (PosiX == POSI_X.CENTER) {
            float adj_x = x_posi / 2f;
            for (int i = 0; i < childs.Length; i++) {
                Vector2 p = childs[i].localPosition;
                p.x -= adj_x;
                childs[i].localPosition = p;
            }
        }

        //座標が下にあるものほど、「手前にある」とさせるために並べ替える
        RectTransform[] sort = childs.OrderByDescending(it => it.localPosition.y).ToArray();

        for( int i = 0; i < sort.Length; i++) {
            sort[i].SetSiblingIndex(i);
        }
    }
}
