using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

[RequireComponent(typeof(RectTransform))]
public class BoxLayoutGroupMng : UIBehaviour, ILayoutGroup  {

//	public enum SORT_HORIZON{
//		LEFT,
//		CENTER_TO_RIGHT,
//		CENTER_TO_LEFT,
//		RIGHT,
//	}
	public enum SORT_VERTICAL{
		UPPER,
//		CENTER_TO_DOWN,
//		CENTER_TO_UP,
		UNDER,
	}

	[SerializeField]
	public Vector2 ItemSize;

	[SerializeField]
	public Vector2 Spacing;

	[SerializeField]
	public Vector2 AddSpacing;

	[SerializeField]
	public SORT_VERTICAL SortV;

//	[SerializeField]
//	public SORT_HORIZON SortH;

	[SerializeField]
	public bool IsVerticalCenter;

	//[SerializeField]
	//public bool FreeSize;

	[SerializeField]
	public bool IsAuto = true;

	[System.NonSerialized]
	public RectTransform[] Items;

	[System.NonSerialized]
	public RectTransform RectTra;

	#if UNITY_EDITOR
	protected override void OnValidate ()
	{
	base.OnValidate ();
	Arrange ();
	}
	#endif

	#region ILayoutController implementation
	public void SetLayoutHorizontal (){}
	public void SetLayoutVertical ()
	{
		if (IsAuto) {
			Arrange ();
		}
	}
    #endregion

    new public void OnEnable() {
        Arrange();
    }

    void Arrange()
	{
		if (!this.gameObject.activeSelf) {
			return;
		}

		if (RectTra == null) {
			RectTra = GetComponent<RectTransform> ();
		}

		List<RectTransform> itms = new List<RectTransform> ();
		for (int i = 0,count=0; i < this.transform.childCount; i++) {
			RectTransform re = this.transform.GetChild (i).GetComponent<RectTransform> ();
			if (re.gameObject.activeSelf) {
				itms.Add (re);
				count++;
			}
		}
		Items = itms.ToArray ();

		if (ItemSize.x <= 0f || ItemSize.y <= 0f) {
			if (Items.Length == 0) {
				return;
			}
			ItemSize.x = Items [0].sizeDelta.x;
			ItemSize.y = Items [0].sizeDelta.y;
		}

		float unit_x = ItemSize.x + Spacing.x;
		float unit_y = ItemSize.y + Spacing.y;

		int x_num = (int)(RectTra.rect.width / unit_x);

		if (x_num == 0) {
			x_num = 1;
		}

		int y_num = Mathf.CeilToInt ((float)Items.Length / (float)x_num);// (Items.Length + (x_num - (x_num-1))) / x_num;

		if (Items.Length > 2 && Items.Length % 2 == 0) {
			int half = Items.Length / 2;
			if (half < x_num) {
				x_num = half;
			}
		}

		float adj_x = unit_x * (x_num / 2);
		float adj_y = (unit_y/2) * (y_num / 2);

		if (x_num > 0) {
			if (x_num % 2 == 0 ) {
				adj_x -= unit_x / 2;
			}

			//if (y_num % 2 == 0 && IsVerticalCenter ) {
				//adj_y -= (unit_y * y_num) / 2;
			//}

			for (int i = 0; i < Items.Length; i++) {
				int x = i % x_num;
				int y = i / x_num;

				if (y == y_num-1 && x == 0 && Items.Length % x_num > 0 ) {
					int blank_num = x_num * y_num - Items.Length;// x_num - Items.Length % x_num;
					if (blank_num > 0) {
						adj_x -= unit_x * blank_num / 2;
					}
				}

				y = SortV == SORT_VERTICAL.UPPER ? -y : y;
				if (!IsVerticalCenter) {
					adj_y = 0;
				}

				Vector2 posi = new Vector2 (x * unit_x - adj_x, y * unit_y + adj_y);
				Items [i].localPosition = posi;

			}
		}

		if (SortV == SORT_VERTICAL.UNDER) {
			for (int i = 0; i < Items.Length; i++) {
				Items [i].SetSiblingIndex (Items.Length-(i+1));
			}
		}
	}
}


