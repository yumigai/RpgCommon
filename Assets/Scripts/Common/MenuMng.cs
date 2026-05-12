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
    //private SkillBoardMng skillPanelPrefab;
    private SkillBoardMng SkillPanel;

    [SerializeField]
    //private ItemBoardMng ItemPanelPrefab;
    private ItemBoardMng ItemPanel;

    [SerializeField]
    private UnitDetailMng StatusPanel;

    [System.NonSerialized]
    public System.Action CloseCallback;

    private GamePadListRecivMng MenuRecive;

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
        showUnitGroup(false);
    }
    private void showMap() {
        Destroy(CopyMap);
        if (FieldUIMng.Singleton != null) {
            CopyMap = Instantiate(FieldUIMng.Singleton.MapBase.gameObject);
        }
    }
    protected void showPanel(GameObject panel) {

        if (SkillPanel != null) {
            SkillPanel.gameObject.SetActive(false);
        }
        if (ItemPanel != null) {
            ItemPanel.gameObject.SetActive(false);
        }
        if (StatusPanel != null) {
            StatusPanel.gameObject.SetActive(false);
        }

        if (panel == MenuPanel) {
        } else {
            panel.SetActive(true);
        }
    }
    public void pushSkill() {
        Mode = MODE.SKILL;

        //UnitGroup.setInputReciv(true);

        //skillPanelPrefab.init(this.transform);
        showPanel(SkillPanel.gameObject);
        SkillPanel.setUnitSelectRecv();
    }
    public void pushItem() {
        Mode = MODE.ITEM;
        ItemBoardMng.OrderMode = ItemBoardMng.MODE.USE;
        ItemBoardMng.OrderCategory = ItemMast.CATEGORY.CONSUMABLE;
        showPanel(ItemPanel.gameObject);
        //ItemPanelPrefab.init(this.transform);
    }
    public void pushStatus() {
        Mode = MODE.STATUS;
        showUnitGroup(true);
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
    private void showUnitGroup(bool isInput) {
        UnitGroup.GroupBase.SetActive(true);
        UnitGroup.CreateGroup(isInput);
    }

    public void selectDoUnit(CharaImgGaugeMng chara) {

        switch (Mode) {
            case MODE.SKILL:
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
            //if (ItemPanel.closeWindow()) {
                ShowMenu();
            //}
            break;
            case MODE.SKILL:
                if (SkillPanel.closeWindow()) {
                    ShowMenu();
                }
                break;
            case MODE.STATUS:
                switch (StatusPanel.closePanels()) {
                    case CmnConst.BOARD_STATUS.CLOSING:
                        UnitGroup.InputReciv.initSetupWithFrameEnd();
                    break;
                    case CmnConst.BOARD_STATUS.CLOSED:
                        //if (switchPanel(new GameObject[] { StatusPanel.gameObject, UnitGroup.GroupBase })) {
                        ShowMenu();
                    //}
                    break;
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
                            UnitGroup.InputReciv.initSetupWithFrameEnd();
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
