using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class FixedScrollRect : ScrollRect
{
	private const float POWER = 10;

	private float FreezePosi = 0f;
	private InfiniteScroll _infinityScroll;
	private InfiniteScroll infinityScroll {
		get {
			if (_infinityScroll == null)
				_infinityScroll = GetComponentInChildren<InfiniteScroll> ();
			return _infinityScroll;
		}
	}

	public bool isDrag
	{
		get;
		private set;
	}

	private float Velocity {
		get {
			return  (infinityScroll.direction == InfiniteScroll.Direction.Vertical) ? 
				-velocity.y :
				velocity.x;
		}
	}


	private RectTransform _rectTransform;
	private float AnchoredPosition {
		get {
			if (_rectTransform == null) {
				_rectTransform = transform.GetChild (0).GetChild (0).GetComponent<RectTransform> ();
				FreezePosi = (infinityScroll.direction == InfiniteScroll.Direction.Vertical) ? _rectTransform.localPosition.x : _rectTransform.localPosition.y;
			}
			return  (infinityScroll.direction == InfiniteScroll.Direction.Vertical ) ? 
				-_rectTransform.localPosition.y:
				_rectTransform.localPosition.x;
		}
		set{
			if (infinityScroll.direction == InfiniteScroll.Direction.Vertical)
				_rectTransform.localPosition = new Vector2 (FreezePosi, -value);
			else
				_rectTransform.localPosition =  new Vector2 (value,FreezePosi);
		}
	}

	void Update()
	{

		if (isDrag || Mathf.Abs (Velocity) > 200)
			return;

		float diff = AnchoredPosition % infinityScroll.itemScale;

		if (Mathf.Abs (diff) > infinityScroll.itemScale / 2) {
			var adjust = infinityScroll.itemScale * ((AnchoredPosition > 0f) ? 1 : -1);
			AnchoredPosition += (adjust - diff) * Time.deltaTime * POWER;
		} else {
			AnchoredPosition -= diff * Time.deltaTime * POWER;
		}

	}

	public override void OnBeginDrag(PointerEventData eventData){
		base.OnBeginDrag (eventData);	// 削除した場合、挙動に影響有り
		isDrag = true;
	}

	public override void OnEndDrag(PointerEventData eventData){
		base.OnEndDrag (eventData);	// 削除した場合、挙動に影響有り
		isDrag = false;
	}

}