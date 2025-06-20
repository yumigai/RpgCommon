using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// GamePadButtonMngのグループ
/// ボタンをグループ管理して、シーンごとにON・OFFの切替え
/// イメージ的にはコントローラーのパッドそのもの
/// </summary>
public class GamePadControllerMng : MonoBehaviour
{
    [SerializeField]
    protected IReadOnlyList<GamePadButtonMng> ButtonList;

    //[SerializeField, Tooltip("優先度/未使用")]
    //public int Priority = 0;

    [SerializeField]
    public bool IsDeActiveHide = true;

    // [System.NonSerialized]
    // public bool IsReserve = false;

    [SerializeField]
    public GameObject Base;

    public GamePadControllerMng Next;

    public GamePadControllerMng Before;

    public static GamePadControllerMng ActivePad = null;
    public static List<GamePadControllerMng> ReservePadList = new List<GamePadControllerMng>();

    public static GamePadControllerMng Singleton;

    private void Awake() {
        Singleton = this;

        if (Base == null) {
            Base = this.gameObject;
        }

        if (ButtonList == null || ButtonList.Count == 0) {
            ButtonList = Base.GetComponentsInChildren<GamePadButtonMng>();
        }

    }


    private void Regist() {

        if (Before != ActivePad) {

            var beforeActive = ActivePad;
            ActivePad = this;

            beforeActive.Next = this;
            if (Before == null && beforeActive != Next)
                Before = beforeActive;

            beforeActive.SetBttonsActive(false);

            ReservePadList.Add(beforeActive);
        }
    }
    private void OnEnable() {
        Regist();
    }

    private void OnDisable() {

        if (IsActivePad() && ReservePadList.Count() > 0) {

            var reserve = ReservePadList.Last();
            //reserve.SetBttonsActive(true);
            ReservePadList.Remove(reserve);

            Before?.SetBttonsActive(true);
        }
    }

    private void OnDestroy() {
        Before.Next = Next;
        Next.Before = Before;
    }

    public bool IsActivePad() {
        return this == ActivePad;
    }

    public void SetBttonsActive(bool val) {

        if (IsDeActiveHide) {
            Base.SetActive(val);
            // foreach( var btn in ButtonList){
            //     btn.gameObject.SetActive(val);
            // }
        } else {
            foreach (var btn in ButtonList) {
                btn.setInteractable(val);
            }
        }
    }

    // public void SetActivation(bool val){
    //     foreach( var btn in ButtonList){
    //         btn.interactable = val;
    //     }
    // }

}
