using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DebugMultiSceneMng : MonoBehaviour
{
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
        foreach (UnitStatusTran unit in SaveMng.Units) {
            unit.addExp(LevelMast.List[unit.Lv].Exp);
            //unit.lvUp();
        }
        SaveMng.UnitData.save();
    }

    public void addAllArchive() {
        SaveMng.Status.Archives.Clear();
        foreach (var story in StoryListMast.List) {
            SaveMng.Status.addArchive(story.Id);
        }
        SaveMng.Status.save();
    }

    public void clearAllStage() {
        SaveMng.Status.DiscoveryStage.Clear();
        SaveMng.Status.ClearStage.Clear();
        System.Array.ForEach(StageMast.List, it => SaveMng.Status.DiscoveryStage.Add(it.Id));
        System.Array.ForEach(StageMast.List, it => SaveMng.Status.ClearStage.Add(it.Id));
        SaveMng.Status.save();
    }

    public void pushResetData() {
        SaveMng.resetSave();
    }

    public void pushBack() {
        SceneManagerWrap.loadScene("TitleScene");
    }

}
