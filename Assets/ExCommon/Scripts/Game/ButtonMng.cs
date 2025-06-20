using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonMng : Button {

	[SerializeField]
	public Text ButtonTxt;

	[SerializeField]
	public bool IsAlphaOnly = false;

	[System.NonSerialized]
	public int Index;

	[System.NonSerialized]
	public System.Action<int> Callback;

	new public void Awake(){
		base.Awake();
		if (ButtonTxt == null) {
			ButtonTxt = GetComponentInChildren<Text> ();
		}
	}

	new public void Start(){
		base.Start();
		setEnable (this.interactable);
	}

	public void setEnable( bool ena ){
		this.interactable = ena;
		if (ena) {
			setColor (this.image.color);
		} else {
			setColor (this.colors.disabledColor);
		}
	}

	public void setColor( Color color ){
		if (IsAlphaOnly) {
			Color c = ButtonTxt.color;
			if (c.a != color.a) {
				c.a = color.a;
				ButtonTxt.color = c;
			}
		} else {
			if (ButtonTxt.color != color) {
				ButtonTxt.color = color;
			}
		}
	}
		
	public void pushButton(){
		Callback (Index);
	}


}
