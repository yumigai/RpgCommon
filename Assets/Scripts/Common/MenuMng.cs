using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class MenuMng : MonoBehaviour
{
    public enum MODE
    {
        MENU,
        ITEM,
        SKILL,
        STATUS,
        SORT,
        MAP,
        QUIT,
    }

    [SerializeField]
    private GameObject MenuPanel;

    [SerializeField]
    private CharaImgGroupMng UnitGroup;

    [SerializeField]
    private SkillBoardMng SkillPanel;

    [SerializeField]
    private ItemBoardMng ItemPanel;

    [SerializeField]
    private UnitDetailMng StatusPanel;

    [System.NonSerialized]
    public System.Action CloseCallback;

    private GamePadListRecivMng MenuRecive;

    private ItemTran SelectedItem;
    private SkillMast SelectedSkill;
    private List<UnitStatusTran> SelectedUnits = new List<UnitStatusTran>();
    private GameObject CopyMap;
    public static MODE Mode;

    private void Awake() {
        MenuRecive = MenuPanel.GetComponentInChildren<GamePadListRecivMng>();
    }

    private void OnEnable() {
        ShowMenu();
    }

    public void ShowMenu() {
        Mode = MODE.MENU;
        SelectedUnits.Clear();
        showPanel(MenuPanel);
        MenuRecive.active();
    }
    private void showMap() {
        Destroy(CopyMap);
        if (FieldUIMng.Singleton != null) {
            CopyMap = Instantiate(FieldUIMng.Singleton.MapBase.gameObject);
        }
    }
    protected void showPanel(GameObject panel) {
        UnitGroup.CreateGroup();
        //MenuPanel.gameObject.SetActive(false);
        //UnitGroup.GroupBase.SetActive(false);
        SkillPanel.gameObject.SetActive(false);
        ItemPanel.gameObject.SetActive(false);
        StatusPanel.gameObject.SetActive(false);
        if (panel == MenuPanel) {
        } else if (panel == UnitGroup.gameObject) {
            showUnitGroup();
        } else {
            panel.SetActive(true);
        }
    }
    public void pushSkill() {
        Mode = MODE.SKILL;
        showUnitGroup();
    }
    public void pushItem() {
        Mode = MODE.ITEM;
        ItemBoardMng.OrderMode = ItemBoardMng.MODE.USE;
        ItemBoardMng.OrderCategory = ItemMast.CATEGORY.CONSUMABLE;
        showPanel(ItemPanel.gameObject);
    }
    public void pushStatus() {
        Mode = MODE.STATUS;
        showUnitGroup();
    }
    public void pushMap() {
        Mode = MODE.MAP;
        showMap();
    }
    public void pushConfig() {
        UnitGroup.GroupBase.SetActive(false);
    }
    public void pushQuit() {
        string txt = LanguageStaticTextMng.getLangText("", "");
        CommonProcess.showConfirm(txt, _ => { Retire(); });
    }
    private void showUnitGroup() {
        UnitGroup.GroupBase.SetActive(true);
        UnitGroup.CreateGroup(true);
    }

    public void selectDoUnit(CharaImgGaugeMng chara) {

        switch (Mode) {
            case MODE.SKILL:
            showPanel(SkillPanel.gameObject);
            SkillPanel.changeUnit(chara.getStatus());
            break;
            case MODE.STATUS:
            showPanel(StatusPanel.gameObject);
            StatusPanel.setParams(chara.getStatus());
            break;
            case MODE.SORT:
            sortUnit(chara.getStatus());
            break;
            default:
            break;
        }
    }

    private void sortUnit(UnitStatusTran unit) {
        if (SelectedUnits.Count == 0) {
            SelectedUnits.Add(unit);
        } else {
            var before = SelectedUnits.FirstOrDefault();
            if (SaveMng.Quest != null) {
                SaveMng.Quest.partyMemberSort(before, unit);
            }
            SelectedUnits.Clear();
        }
    }

    public bool menuClose() {

        switch (Mode) {
            case MODE.MENU:
            return true;
            case MODE.ITEM:
            if (ItemPanel.closeWindow()) {
                ShowMenu();
            }
            break;
            case MODE.SKILL:
            if (SkillPanel.closeWindow()) {
                ShowMenu();
            }
            break;
            case MODE.STATUS:
            if (switchPanel(new GameObject[] { StatusPanel.gameObject, UnitGroup.GroupBase })) {
                ShowMenu();
            }
            break;
            case MODE.SORT:
            ShowMenu();
            break;
            case MODE.MAP:
            Destroy(CopyMap);
            ShowMenu();
            break;
            default:
            ShowMenu();
            break;
        }
        return false;
    }

    private bool switchPanel(GameObject[] panels) {
        for (var i = 0; i < panels.Length; i++) {
            if (panels[i].activeSelf) {
                if (panels[i] == UnitGroup.GroupBase) {
                    //unitgroupは非表示にしない
                    MenuRecive.active();
                } else {
                    panels[i].SetActive(false);
                    if (i + 1 < panels.Length) {
                        if (panels[i + 1] == UnitGroup.GroupBase) {
                            UnitGroup.CreateGroup(true);
                        } else {
                            panels[i + 1].SetActive(true);
                        }
                    }
                }
                return false;
            }
        }
        return true;
    }

    private void Retire() {
        BaseResultSceneMng.IsSuccsess = false;
        SceneManagerWrap.LoadScene(CmnConst.SCENE.ResultScene);
    }

}
