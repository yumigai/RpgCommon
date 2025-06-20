using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(InfiniteScroll))]
public class ItemControllerLimited : UIBehaviour, IInfiniteScrollSetup {

	[SerializeField, Range(1, 999)]
	private int max = 30;

    private bool Ready = false;

	[System.NonSerialized]
	public int OrderPosiY;


	public void OnPostSetupItems()
	{
        Ready = true;
        //var infiniteScroll = GetComponent<InfiniteScroll>();
		//infiniteScroll.onUpdateItem.AddListener(OnUpdateItem);
		//GetComponentInParent<ScrollRect>().movementType = ScrollRect.MovementType.Elastic;

		//var rectTransform = GetComponent<RectTransform>();
		//var delta = rectTransform.sizeDelta;
		//delta.y = infiniteScroll.itemScale * max;
		//rectTransform.sizeDelta = delta;
	}

    public void readySetup( int item_count ){
        max = item_count;
        StartCoroutine(waitReady());
    }

    private void Setup()
    {
        var infiniteScroll = GetComponent<InfiniteScroll>();
        infiniteScroll.onUpdateItem.AddListener(OnUpdateItem);
        GetComponentInParent<ScrollRect>().movementType = ScrollRect.MovementType.Elastic;

        var rectTransform = GetComponent<RectTransform>();
        var delta = rectTransform.sizeDelta;
        delta.y = infiniteScroll.itemScale * max;
        rectTransform.sizeDelta = delta;

		infiniteScroll.transform.localPosition = new Vector2( infiniteScroll.transform.localPosition.x, (float)OrderPosiY * infiniteScroll.itemScale );
    }

    private IEnumerator waitReady()
    {
        while (!Ready)
        {
            yield return null;
        }
        Setup();
    }

	public void OnUpdateItem(int itemCount, GameObject obj)
	{
		if(itemCount < 0 || itemCount >= max) {
			obj.SetActive (false);
		}
		else {
			obj.SetActive (true);
			
			var item = obj.GetComponentInChildren<Item>();
			item.UpdateItem(itemCount);
		}
	}
}
