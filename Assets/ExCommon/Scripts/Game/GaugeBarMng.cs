using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GaugeBarMng : MonoBehaviour {


	[SerializeField]
	public Image MainGauge;

    [SerializeField]
    public Image DelayGauge;

    [SerializeField]
    public Text NumTxt;

	[SerializeField]
	public bool IsDelay;

	[SerializeField]
	public float MaxNum;

	[SerializeField]
	public float GaugeMoveSpeed;

	[SerializeField]
	private bool IsShowMax;

	[System.NonSerialized]
	public float NowNum;

	[System.NonSerialized]
	public float DisplayNum;

	[System.NonSerialized]
	public float NowGaugeAmount;

	[System.NonSerialized]
	public bool IsSynchroFinish = false;

	void FixedUpdate () {

		if (IsDelay && IsSynchroFinish && DelayGauge != null  ) {
            DelayGauge.fillAmount -= GaugeMoveSpeed;
			if (DelayGauge.fillAmount <= NowGaugeAmount || DelayGauge.fillAmount <= 0f) {
				IsSynchroFinish = false;
			}
		}
	}

    public void init(float max_val, float num) {
        MaxNum = max_val;
        setValue(num);
    }

	IEnumerator delayRedGauge(){
		yield return new WaitForSeconds (0.5f);
		IsSynchroFinish = true;
	}

	//public void setMaxGauge( float max_value, float now_num = -1 ){
	//	MaxNum = max_value;
	//	NowNum = now_num < 0 ? MaxNum : now_num;
	//	DisplayNum = NowNum;
	//	Vector2 size = GaugeBase.sizeDelta;
	//	size.x = max_value > GaugeValue ? InitGauge : InitGauge * ( max_value / GaugeValue );
	//	GaugeBase.sizeDelta = size;

	//	valueUpdate (NowNum);
	//}

	public void setValue( float val ){
		NowNum = val;
        if (NumTxt != null) {
            if (IsShowMax) {
				NumTxt.text = string.Format("{0} / {1}", NowNum, MaxNum);
            } else {
				NumTxt.text = NowNum.ToString();
			}
            
        }
		if (IsDelay && DelayGauge != null ) {
            DelayGauge.fillAmount = NowGaugeAmount;
			StartCoroutine (delayRedGauge());
		} else {
			valueUpdate (val);
		}
	}

	public void valueUpdate( float val) {
        if (MaxNum > 0) {
            MainGauge.fillAmount = val / MaxNum;
        } else {
            MainGauge.fillAmount = 0;
        }
        

		//for (int i = 0; i < MainGauges.Length; i++) {
		//	MainGauges [i].fillAmount = 0f;
		//}

		//for (int i = 0; i < MainGauges.Length; i++) {

		//	float gauge_max = GaugeValue * (i + 1);

		//	if (val >= gauge_max) {
		//		MainGauges [i].fillAmount = 1f;
		//	} else {
		//		if (i > 0) {
		//			MainGauges [i].fillAmount = (val % GaugeValue ) / GaugeValue;
		//		}else{
		//			MainGauges [i].fillAmount = val / MaxNum;
		//		}
		//		//BackLifeRed.transform.SetSiblingIndex (LifeGauges [i].transform.GetSiblingIndex ());
		//		NowGaugeAmount = MainGauges [i].fillAmount;
		//		break;
		//	}
		//}
	}

}
