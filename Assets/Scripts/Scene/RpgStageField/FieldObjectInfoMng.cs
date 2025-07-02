using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// マップ上に表示する情報
/// </summary>
public class FieldObjectInfoMng : MonoBehaviour
{
    [SerializeField]
    public Text Info;

    [SerializeField]
    public Image Gauge;

    [SerializeField]
    public GameObject ClientObject; //情報元のオブジェクト

    private RectTransform RecTra;

    protected void Awake() {
        RecTra = GetComponent<RectTransform>();
    }

    public void setInfo(string txt) {
        Info.text = txt;
    }

    public void updGauge( float val ) {
        Gauge.fillAmount = val;
    }

    protected void FixedUpdate() {

        if (ClientObject == null) {
            Destroy(this.gameObject);
            return;
        }
        setPosition();
        //Vector2 posi = Camera.main.WorldToScreenPoint(ClientObject.transform.position);
        //RecTra.position = posi;
    }

    public void setClient( GameObject obj) {
        ClientObject = obj;
        setPosition();
    }

    public void setPosition() {
        Vector2 posi = Camera.main.WorldToScreenPoint(ClientObject.transform.position);
        RecTra.position = posi;

    }

}
