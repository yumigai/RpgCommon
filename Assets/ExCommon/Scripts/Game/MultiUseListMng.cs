using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MultiUseListMng : ListItemMng
{

	public enum BUTTON
	{
		SHOW,
		HIDE,
		LOCK,
	}

	[SerializeField]
	public Text ButtonTxt;

	[SerializeField]
	public Button Btn;

	[SerializeField]
	public Text Name;

	[SerializeField]
	public Text Detail;

	[SerializeField]
	public Text Value;

	[SerializeField]
	public Image Icon;

	[SerializeField]
	public string ImagePath; //   ŕێ     摜 p X

	[SerializeField]
	public Text TagLabel;

	[SerializeField]
	public Text[] ExtraTxts;

	[SerializeField]
	public Image[] ExtraImgs;

	//[SerializeField]
	//   K V [
	public System.Action<MultiUseListMng> Callback;

	public System.Action<MultiUseListMng> SelectedCallback;

	//   K V [
	public static MultiUseListMng SelectedItem;

	/// <summary>
	///  T u e L X g P
	/// </summary>
	public string ExtraText1 {
		set {
			SetExtraText1(value);
		}
	}
	/// <summary>
	///  T u e L X g Q
	/// </summary>
	public string ExtraText2 {
		set {
			SetExtraText2(value);
		}
	}

	public Sprite setIcon(string path) {
		if (path.Length == 0) {
			return null;
		}
		Sprite sp = Resources.Load<Sprite>(path);
		if (Icon != null) {
			Icon.sprite = sp;
		}
		return sp;
	}

	public void setButton(BUTTON state) {
		if (Btn == null) {
			return;
		}
		switch (state) {
			case BUTTON.SHOW:
			Btn.gameObject.SetActive(true);
			Btn.interactable = true;
			break;
			case BUTTON.HIDE:
			Btn.gameObject.SetActive(false);
			break;
			case BUTTON.LOCK:
			Btn.gameObject.SetActive(false); //  U  \   ɂ  Ȃ   interactable ̐ؑւ   ʂŃ`        
			Btn.interactable = false;
			Btn.gameObject.SetActive(true);
			break;
		}
	}

	/// <summary>
	///    K V [    
	/// </summary>
	public void pushButton() {
		SelectedItem = this;
		Callback?.Invoke(this);
	}

	/// <summary>
	///  I    ԕύX iEventTrigger.Select Ŏw  j
	/// </summary>
	public void changeSelected() {
		SelectedCallback?.Invoke(this);
	}

	public void SetExtraText1(string str) {
		if (ExtraTxts.Length > 0) {
			ExtraTxts[0].text = str;
		}
	}

	public void SetExtraText2(string str) {
		if (ExtraTxts.Length > 1) {
			ExtraTxts[1].text = str;
		}
	}

	#region
	/// <summary>
	///  { ^   C x   g ݒ 
	/// </summary>
	/// <param name="call"></param>
	public void SetButtonInvoke(UnityAction call) {
		Btn.onClick.RemoveAllListeners();
		Btn.onClick.AddListener(call);
	}

	public void SetButtonInvoke(UnityAction<MultiUseListMng> call) {
		Btn.onClick.RemoveAllListeners();
		Btn.onClick.AddListener(() => call(this));
	}

	public void SetButtonInvoke(UnityAction<int> call) {
		Btn.onClick.RemoveAllListeners();
		Btn.onClick.AddListener(() => call(this.Id));
	}

	public void SetButtonInvoke(UnityAction<int> call, int value) {
		Btn.onClick.RemoveAllListeners();
		Btn.onClick.AddListener(() => call(value));
	}
	#endregion

}