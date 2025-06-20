using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class HexLayoutGroup : UIBehaviour, ILayoutGroup
{
    [SerializeField]
    public int Row = 2;

    [SerializeField]
    public Vector2 CellSize = new Vector2(100f,100f);

    [SerializeField]
    public bool IsAuto;

	#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
        if (IsAuto) {
            Arrange();
        }
    }
	#endif

    #region ILayoutController implementation
    public void SetLayoutHorizontal() { }
    public void SetLayoutVertical()
    {
        if (IsAuto) {
            Arrange();
        }
    }
    #endregion
    
    new void OnEnable(){
    	Arrange();
    }

    void Arrange()
    {
        RectTransform[] childs = new RectTransform[ transform.childCount ]; //GetComponentsInChildren<RectTransform>(false).Where( it => it != this.transform ).ToArray();

        int row = 0;
        int col = 0;

        Vector2 cell_size = CellSize;

        for( int i = 0; i < childs.Length; i++){

            childs[i] = transform.GetChild(i).GetComponent<RectTransform>();

            int x_index = col;
            
            float y_normal = (cell_size.y * row);
            float y_shift = -(cell_size.y / 2) + (cell_size.y * row);

            bool even = (col % 2 == 0);

            float y_posi = even ? y_shift : y_normal;

            Vector2 posi = new Vector2(x_index * cell_size.x, y_posi);

            childs[i].localPosition = posi;

            row++;
            if( row >= Row || ( !even && row == Row - 1 ) ){
                row = 0;
                col++;
            }
            
        }

        //RectTransform[] sort = childs.OrderByDescending(it => it.position.y).ToArray();

        //for( int i = 0; i < sort.Length; i++) {
        //    //sort[i].SetSiblingIndex(i);
        //}

    }

}
