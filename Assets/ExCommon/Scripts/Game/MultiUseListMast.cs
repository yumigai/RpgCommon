using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class MulitiUseListMast : MasterCmn
{
	public int Id;
    public string Tag;
    public int Priority;
	public string Name;
	public string Icon;
	public string Detail;
    public string ImagePath;

    public static T FindById<T>(T[] list, int id) where T : MulitiUseListMast {
        return System.Array.Find(list, it => it.Id == id);
    }

    public static T FindByTag<T>(T[] list, string tag) where T : MulitiUseListMast {
        return System.Array.Find(list, it => it.Tag == tag);
    }

    public static T FindById<T>(IReadOnlyList<T> list, int id) where T : MulitiUseListMast {
        return list.FirstOrDefault(it => it.Id == id);
    }

    public static T FindByTag<T>(IReadOnlyList<T> list, string tag) where T : MulitiUseListMast {
        return list.FirstOrDefault(it => it.Tag == tag);
    }

    public int GetPriority() {
        return Priority == 0 ? Id : Priority;
    }
}