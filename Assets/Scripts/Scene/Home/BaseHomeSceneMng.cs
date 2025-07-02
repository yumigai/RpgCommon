using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using UnityStandardAssets.CrossPlatformInput;

public class BaseHomeSceneMng : Base2DSceneMng
{


    public const float MESSAGE_RELOAD_TIME = 7f;

    [SerializeField]
    public Image BackImage;

    [SerializeField]
    public AudioClip Bgm;

    [SerializeField]
    private Transform StandBase;

    private HomeMessageMast NowMessage;

    private int MessageIndex;

    public static BaseHomeSceneMng Singleton;

    protected void Awake() {
        Singleton = this;
    }

    // Use this for initialization
    new protected void Start() {
        base.Start();

        homeMessage();

        SoundMng.Instance.playBGM(Bgm);

    }

    public void homeMessage(string chara = "" ) {
        HomeMessageMast[] masts = Array.FindAll(HomeMessageMast.List, it =>
            it.ChapterIds.Length == 0
            || it.ChapterIds.Any(it2 => SaveMng.Status.ClearStage.IndexOf(it2) >= 0));

        HomeMessageMast.MESSAGE_SITUATION situation = HomeMessageMast.MESSAGE_SITUATION.NORMAL;

        if (showSituationMessage(masts, situation)) {
            return;
        }
    }

    public bool showSituationMessage(HomeMessageMast[] masts, HomeMessageMast.MESSAGE_SITUATION situation) {
        masts = System.Array.FindAll(masts, it =>
                    it.Situation == situation
                    && (it.MainCharaId == 0 ||
                        SaveMng.Units.Exists(u => u.MasterId == it.MainCharaId && u.FriendShip >= it.NeedPoint
                        )
                    ));

        if (masts.Length > 0) {
            int rand = UnityEngine.Random.Range(0, masts.Length);
            HomeMessageMast mst = masts[rand];
            showMessage(mst);
            return true;
        }
        return false;
    }

    public void showMessage(HomeMessageMast mst) {

        NowMessage = mst;
        MessageIndex = 0;

        if (mst != null) {

            for (int i = 0; i < mst.Characters.Length; i++) {

                string name = mst.Characters[i];
                if (name.Length == 0) {
                    StandBase.GetChild(i).gameObject.SetActive(false);
                    continue;
                }

                string img_path = BaseStorySceneMng.PutImageDirectory + name;

                GameObject stand = Resources.Load<GameObject>(img_path);

                BaseStorySceneMng.putStandChara(stand, mst.Faces[i], new Vector2(), StandBase.GetChild(i));
            }

            Sprite sp = Resources.Load<Sprite>(CmnConst.Path.ADV_IMG_BACK + mst.Back);
            if (sp != null) {
                BackImage.sprite = sp;
            }

            messageUpdate();

        } else {
            FaceMessageMng.Singleton.init("");
        }
    }

    public void messageUpdate() {

        FaceMessageMng.Singleton.changeIconImage(NowMessage.Icons[MessageIndex]);
        FaceMessageMng.Singleton.changeNamePlate(NowMessage.Names[MessageIndex], NowMessage.Kanas[MessageIndex]);

        string txt = NowMessage.Texts[MessageIndex].Replace("@player@", SaveMng.Conf.PlayerName);

        FaceMessageMng.Singleton.init(txt);

        MessageIndex++;

        if (MessageIndex < NowMessage.Texts.Length && NowMessage.Texts[MessageIndex].Length > 0) {
            StartCoroutine(nextMessage());
        }
    }

    private IEnumerator nextMessage() {
        while (!FaceMessageMng.Singleton.IsReadyAutoNext) {
            yield return null;
        }
        messageUpdate();
    }

    public void pushQuest() {
        pushMenuButton(CmnConst.SCENE.AreaSelectScene);
    }

    public void pushShop() {
        pushMenuButton(CmnConst.SCENE.ShopMenuScene);
    }
    public void pushStory() {
        pushMenuButton(CmnConst.SCENE.StoryListScene);
    }
    public void pushArchive() {
        pushMenuButton(CmnConst.SCENE.ArchiveScene);
    }
    public void pushCollection() {
        pushMenuButton(CmnConst.SCENE.CollectionScene);
    }
    public void pushConfig() {
        pushMenuButton(CmnConst.SCENE.ConfigScene);
    }
    public void pushSystem() {
        pushMenuButton(CmnConst.SCENE.SystemScene);
    }
    public void pushSave() {
        pushMenuButton(CmnConst.SCENE.SaveScene);
    }
    public void pushTeamEdit() {
        pushMenuButton(CmnConst.SCENE.CharaMenuScene);
    }

    private void pushMenuButton(CmnConst.SCENE scene) {
        CommonProcess.playClickSe();
        SceneManagerWrap.LoadScene(scene);
    }

}
