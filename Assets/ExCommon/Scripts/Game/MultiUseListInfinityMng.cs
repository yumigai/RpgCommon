using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// これもう使用せんかも
/// </summary>
public class MultiUseListInfinityMng : MultiUseListMng {
	
	public static int[] Indexs;
	public static int[] Ids;
	public static string[] ButtonStrs;
	public static string[] NameStrs;
	public static string[] DetailStrs;
	public static string[] IconStrs;
	public static Sprite[] IconSprites;
	public static BUTTON[] ButtonState;

    public new static System.Action<MultiUseListInfinityMng> Callback;
	
	
	public static void setListSize( int size ){
		Indexs = new int[size];
		Ids = new int[size];
		ButtonStrs = new string[size];
		NameStrs = new string[size];
		DetailStrs = new string[size];
		IconStrs = new string[size];
		IconSprites = new Sprite[size];
		ButtonState = new BUTTON[size];
		InfiniteScroll.ListItemNum = size;
	}
	
	new public void UpdateItem(int count) 
	{
		if (Indexs ==null || Indexs.Length == 0) {
			return;
		}
        int i = count % Indexs.Length;
        Index = Indexs[i];
		Id = Ids [i];
		
		setText( ButtonTxt, ButtonStrs[i] );
		setText( Name, NameStrs[i] );
		setText( Detail, DetailStrs[i] );
		
		if( IconSprites[i] == null ){
			IconSprites[i] = setIcon(IconStrs[i]);
		}

		setButton( ButtonState[i] );
		
	}
	
	private void setText( Text tx, string str ){
		if( tx != null ){
			tx.text = str;
		}
	}

    public new void pushButton(){
        if (Callback != null){
            Callback(this);
        }
    }
}