using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PageScrollRect2 : MonoBehaviour {

		[SerializeField]
		public RectTransform ListItem;

		[SerializeField]
		public float Space = 0f;

		// 1ページの幅.
		private float pageWidth;
		// 前回のページIndex. 最も左を0とする.
		private int prevPageIndex = 0;


}
