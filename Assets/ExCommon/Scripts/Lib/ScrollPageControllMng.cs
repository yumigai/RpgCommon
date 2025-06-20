
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;
using System.Collections.Generic;

public class ScrollPageControllMng : MonoBehaviour
{
    //public enum SCROLL_TO
    //{
    //    HORIZON,
    //    VERTICAL,
    //    GRID
    //}

    //[SerializeField]
    //public float Smooth = 10f;
    //[SerializeField]
    //public float StopSpeed = 2f;
    //[SerializeField]
    //public bool ShiftEndPage = true;
    [SerializeField]
    public AudioClip PlayScrollSe;
    [SerializeField]
    public ScrollRect Scroll;
    [SerializeField]
    private bool IsAutoScroll = true;
    //[SerializeField, Header("ページ数にカウントしないダミー数")]
    //public int IgnorePageNum;
    //[SerializeField]
    //private RectTransform ViewSize;

    //[System.NonSerialized]
    //public int IndexH;

    //[System.NonSerialized]
    //public int IndexV;

    //    [System.NonSerialized]
    //    public float PageNum = 10;

    [System.NonSerialized]
    private float ViewSize = 0f;

    //Vector2 TargetPosition;
    //float PerPage;

    //private bool IsStop;

    /// <summary>
    /// リストアイテムのサイズ
    /// </summary>
    //private float ItemSize = 0;

    /// <summary>
    /// 画面に表示できるリストアイテム数
    /// </summary>
    //private int ViewItemNum = 0;

    /// <summary>
    /// アイテムサイズに対するリストの表示余り
    /// </summary>
    //private float ContentSizeSurplus = 0;

    //private int BeforePage = 0;

    private GameObject SelectedObject;

    //[System.NonSerialized]
    //public bool forcePositionUpdate = false;

    [System.NonSerialized]
    public int OrderPage = -1;


    private float ContentPosi {
        get {
            return Scroll.vertical ? Scroll.content.localPosition.y : Scroll.content.localPosition.x;
        }
    }

    void Awake() {
        if (Scroll == null) {
            Scroll = GetComponent<ScrollRect>();
        }
    }

    void Start() {
        init();
    }

    public void init() {
        var rect = Scroll.GetComponent<RectTransform>();

        if (rect.localScale != Vector3.one) {
            Debug.LogWarning("not LocalScale is one");
        }

        if (Scroll.vertical) {
            ViewSize = rect.rect.height;
        } else {
            ViewSize = rect.rect.width;
        }

        setPage();

    }

    void Update() {
        autoUpdate();

    }

    void autoUpdate() {
        if (IsAutoScroll) {
            setPage();
        }
    }

    void UpdateScroll() {

        //Scroll.normalizedPosition = TargetPosition;
    }

    /// <summary>
    /// ページ設定
    /// 現在は縦のみ対応
    /// </summary>
    public void setPage() {
        GameObject selected = EventSystem.current.currentSelectedGameObject;
        if (selected == null) {
            return;
        }
        if (SelectedObject != selected && selected.transform.parent == Scroll.content) {

            if (selected.transform.localScale != Vector3.one) {
                Debug.LogWarning("not LocalScale is one");
            }

            var posi = Scroll.content.transform.localPosition;

            if (SelectedObject != null) {

                var rect = selected.GetComponent<RectTransform>();
                var item_size = Scroll.vertical ? rect.sizeDelta.y * rect.pivot.y : rect.sizeDelta.x * rect.pivot.x;
                var selected_posi = selected.transform.localPosition;
                var scroll_posi = Scroll.content.transform.localPosition;

                if (selected.transform.GetSiblingIndex() > SelectedObject.transform.GetSiblingIndex()) {
                    //下
                    if (Scroll.vertical) {
                        if (-(scroll_posi.y + ViewSize - item_size) > selected_posi.y) {
                            posi.y = -selected_posi.y - ViewSize + item_size;
                        }
                    } else {
                        if (-scroll_posi.x > selected_posi.x) {
                            posi.x = -selected_posi.x;
                        }
                    }
                } else {
                    //上
                    if (Scroll.vertical) {
                        if (-scroll_posi.y - item_size < selected_posi.y) {
                            posi.y = -selected_posi.y - item_size;
                        }
                    } else {
                        if (-scroll_posi.x < selected_posi.x) {
                            posi.x = -selected_posi.x;
                        }
                    }
                }

            }

            Scroll.content.transform.localPosition = posi;

            SelectedObject = selected;
        }

    }

    public void PageJump(int num) {
        var items = GetItemList();
        var count = items.Count();
        if (count == 0) {
            return;
        }
        var onePageSize = Scroll.content.sizeDelta.y;
        var itemSize = items.First().GetComponent<RectTransform>().sizeDelta.y;
        int pageItemNum = (int)(onePageSize / itemSize);
        int index = items.IndexOf(SelectedObject);
        if (index < 0) {
            return;
        }
        int nextIndex = Mathf.Clamp(index + (pageItemNum * num), 0, count);
        var nextItem = items.ElementAt(nextIndex);
        EventSystem.current.SetSelectedGameObject(nextItem);
    }

    public List<GameObject> GetItemList() {
        return GetComponentsInChildren<GameObject>().Where(it => it.activeSelf && it != this.gameObject).ToList();
    }
    public int ItemCouunt() {
        return GetItemList().Count();
    }

}
