using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListItemMng : Item
{

    [SerializeField]
    public int Id;

    [SerializeField]
    public int Index;

    [SerializeField]
    public GameObject CheckMark;

    public void check(bool val) {
        if (CheckMark != null) {
            CheckMark?.SetActive(val);
        }
    }
}