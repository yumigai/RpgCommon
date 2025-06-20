using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FilterMng : MonoBehaviour
{
    private Image FilterImage;

    private void Awake() {
        FilterImage = this.GetComponent<Image>();
    }

    private void OnEnable() {
        StartCoroutine(setSize());
    }

    //OnEnable時点だとsetParentされていないタイミングがあるので、フレーム終わりまで待つ
    IEnumerator setSize() {
        yield return new WaitForEndOfFrame();
        FilterImage.rectTransform.sizeDelta = new Vector2(Screen.width, Screen.height);
        FilterImage.rectTransform.position = this.GetComponentInParent<Canvas>().transform.position;
    }

}
