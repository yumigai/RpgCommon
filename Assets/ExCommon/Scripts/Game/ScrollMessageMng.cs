using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScrollMessageMng : MonoBehaviour
{
    [SerializeField]
    Scrollbar Scroll;

    [SerializeField]
    public System.Action CloseCallback;

    // Start is called before the first frame update
    void Awake()
    {
        if(Scroll == null) {
            Scroll = GetComponentInChildren<Scrollbar>();
        }
    }

    private void OnEnable() {
        EventSystem.current.SetSelectedGameObject(Scroll.gameObject);
    }

    public void CloseWindow() {
        if (CloseCallback != null) {
            CloseCallback();
        }
    }

    private void OnDestroy() {
        CloseWindow();
    }

    private void OnDisable() {
        CloseWindow();
    }

    public void SetScrollValue(float val) {
        Scroll.value = val;
    }

    public float GetScrollValue() {
        return Scroll.value;
    }
}
