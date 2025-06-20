using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

[RequireComponent(typeof(ScrollRect))]
public class ScrollTextMng : MonoBehaviour
{
    private float SCROLL_SPEED = 10;

    [SerializeField]
    public ScrollRect RectArea;

    [System.NonSerialized]
    public bool IsActive = true;

    //private Vector2 Size = new Vector2();

    private void Awake() {
        if (RectArea == null) {
            RectArea = this.GetComponent<ScrollRect>();
        }
        //Size = this.GetComponent<RectTransform>().sizeDelta;
    }

    private void Update() {
        if (IsActive) {

            Vector3 add = new Vector3();

            if (RectArea.horizontal) {
                add.x = CrossPlatformInputManager.GetAxis("Horizontal");
                //add.x = add.x == 0 ? CrossPlatformInputManager.GetAxis("HorizontalRaw") : add.x;
            } else {
                add.y = -CrossPlatformInputManager.GetAxis("Vertical");
                //add.y = add.y == 0 ? -CrossPlatformInputManager.GetAxis("VerticalRaw") : add.y;
            }
            if (add != Vector3.zero) {
                add *= SCROLL_SPEED;
                RectArea.content.localPosition += add;
            }
        }
    }
}
