using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FrameImageMng : MonoBehaviour
{
    [SerializeField]
    public Image Img;

    private void Awake() {
        if (Img == null) {
            //基本的に対象の画像以外は子要素にしないこと
            Img = this.GetComponentInChildren<Image>();
        }
    }

    private void OnEnable() {
        updImageSize();
    }
    public void updImageSize() {
        if (Img != null) {
            var result = UtilToolLib.changeImageSizeFrameFit(Img);
            if (!result) {
                StartCoroutine(waitLayout());
            }
        }
    }

    private IEnumerator waitLayout() {
        yield return new WaitForEndOfFrame();
        UtilToolLib.changeImageSizeFrameFit(Img);
    }
}
