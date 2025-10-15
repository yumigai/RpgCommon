using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class DebugMultiSceneMng : MonoBehaviour
{
    public Text Param1;

    public Text Param2;

    public Text Param3;

    public void pushAddMoney() {
        SaveMng.Status.addMoney(10000);
        SaveMng.Status.save();
    }

    public void pushAddAllItem() {
        SaveMng.Items.Clear();

        foreach (var item in ItemMast.List) {
            SaveMng.ItemData.addItem(item.Id);
        }
        SaveMng.ItemData.save();
    }

    public void pushAddAllConsumable() {
        SaveMng.Items.Clear();

        foreach (var item in ItemMast.List.Where(it => it.Category == ItemMast.CATEGORY.CONSUMABLE)) {
            SaveMng.ItemData.addItem(item.Id);
        }
        SaveMng.ItemData.save();
    }

    public void pushLevelUp() {
        if (Param1.text == string.Empty) {
            Param1.text = "1";
        }
        int add = int.Parse(Param1.text);
        foreach (UnitStatusTran unit in SaveMng.Units) {
            unit.addExp(LevelMast.List[unit.Lv+add].Exp);
            Debug.Log(unit.Name + ": Lv"+unit.LvNum);
        }
        SaveMng.UnitData.save();
    }

    public void addAllArchive() {
        SaveMng.Collection.Archives.Clear();
        foreach (var story in StoryListMast.List) {
            SaveMng.Collection.addArchive(story.Id);
        }
        SaveMng.Collection.save();
    }

    public void clearAllStage() {
        SaveMng.Status.DiscoveryStage.Clear();
        SaveMng.Status.ClearStage.Clear();
        //System.Array.ForEach(StageMast.List, it => SaveMng.Status.DiscoveryStage.Add(it.Id));
        //System.Array.ForEach(StageMast.List, it => SaveMng.Status.ClearStage.Add(it.Id));
        for( int i = 0; i < StageMast.List.Count(); i++)
        {
            SaveMng.Status.DiscoveryStage.Add(StageMast.List[i].Id);
            SaveMng.Status.ClearStage.Add(StageMast.List[i].Id);

        }
        SaveMng.Status.save();
    }

    public void setFriendShip()
    {
        var unit = SaveMng.Units.Find(it => it.MasterId == int.Parse(Param1.text));
        unit.FriendShip = int.Parse(Param2.text);
        SaveMng.saveUnit();
    }

    public void pushResetData() {
        SaveMng.resetSave();
    }

    public void pushBack() {
        SceneManagerWrap.loadScene("TitleScene");
    }

}
