using System.Collections;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR_WIN
using UnityEditor;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    [MenuItem("Custom/make/DungeonMakerWindow")]
    public static void DebugController() {
        EditorWindow window = EditorWindow.GetWindow(typeof(dungeonMakerWindow),false,"Dungeon Maker");
        window.Show();
    }

    ////// Window///////////////////////////////////////
    public class dungeonMakerWindow : EditorWindow
    {
        //private const float FLOOR_THICKNESS = 0.5f; //フロアの厚さ
        //private const float WALL_HEIGHT = 6; //壁の高さ
        //private const float WALL_THICKNESS = 1; //壁の厚さ

        //private enum ROOM_TYPE
        //{
        //    x1,
        //    x3,
        //    Stairx3,
        //    Slopex3,
        //}

        //private GUIContent[] RoomType = new[]
        //{
        //    new GUIContent("1x1部屋"),
        //    new GUIContent("3x3部屋"),
        //    new GUIContent("階段（3部屋）"),
        //    new GUIContent("坂道（3部屋）"),
        //};

        //private int RoomTypeIndex;

        string RoomName = "";
        string RoomUnitSizeStr = "";
        string RoomSizeStr = "";

        string FloorThicnessStr = "1"; //床の厚さ
        string WallHeightStr = "6"; //壁の高さ
        string WallThicnessStr = "1";   //壁の厚さ

        float FloorThicness = 1f;
        float WallHeight = 6f;
        float WallThicness = 1f;


        void OnGUI() {
            GUILayout.Label("説明");
            GUILayout.Space(10f);
            GUILayout.Label("ルーム名");
            RoomName = GUILayout.TextField(RoomName);
            GUILayout.Label("部屋単位スケール");
            RoomUnitSizeStr = GUILayout.TextField(RoomUnitSizeStr);

            GUILayout.Label("部屋の大きさ（最小部屋何個分か）");
            RoomSizeStr = GUILayout.TextField(RoomSizeStr);

            GUILayout.Space(20f);

            if (GUILayout.Button("生成", GUILayout.Height(30f))) {

                var roomSizeInt = int.Parse(RoomSizeStr);
                var roomUnitSize = int.Parse(RoomUnitSizeStr);
                int totalRoomSize = roomUnitSize * roomSizeInt;

                FloorThicness = float.Parse(FloorThicnessStr);
                WallHeight = float.Parse(WallHeightStr);
                WallThicness = float.Parse(WallThicnessStr);

                var room = makeObject(RoomName, null);
                var mng = room.AddComponent<StageRoomMng>();
                mng.RoomSize = new Vector2Int(totalRoomSize, totalRoomSize);
                mng.Rate = 10;

                //Floor
                var block = makeObject("Block", room.transform);
                makeCube("Floor", block.transform, new Vector3(totalRoomSize, FloorThicness, totalRoomSize));

                //Connect
                var connect = makeObject("Connect", room.transform);

                var n = makeConnect("n", connect.transform, new Vector3(0, 0, (float)totalRoomSize / 2), 0, roomUnitSize, roomSizeInt);
                var s = makeConnect("s", connect.transform, new Vector3(0, 0, -(float)totalRoomSize / 2), 180f, roomUnitSize, roomSizeInt);
                var w = makeConnect("w", connect.transform, new Vector3(-(float)totalRoomSize / 2, 0, 0), 90f, roomUnitSize, roomSizeInt);
                var e = makeConnect("e", connect.transform, new Vector3((float)totalRoomSize / 2, 0, 0), 270f, roomUnitSize, roomSizeInt);
                mng.ConnectionPosis = new List<Transform>();
                mng.ConnectionPosis.Add(n.transform);
                mng.ConnectionPosis.Add(s.transform);
                mng.ConnectionPosis.Add(w.transform);
                mng.ConnectionPosis.Add(e.transform);

                //Pillar
                //makePillars(room.transform, roomUnitSize, roomSizeInt, n, s, w, e);
                var edge = (float)roomUnitSize * roomSizeInt / 2 - WallThicness / 2;
                var pillars = makeObject("Pillars", room.transform).transform;
                //mng.Pillars = new List<RelationObjectMng>();
                //mng.Pillars.Add(makePillar("nw", pillars, -edge, edge, n, w));
                //mng.Pillars.Add(makePillar("ne", pillars, edge, edge, n, e));
                //mng.Pillars.Add(makePillar("sw", pillars, -edge, -edge, s, w));
                //mng.Pillars.Add(makePillar("se", pillars, edge, -edge, s, e));
                makePillar("nw", pillars, -edge, edge, n, w);
                makePillar("ne", pillars, edge, edge, n, e);
                makePillar("sw", pillars, -edge, -edge, s, w);
                makePillar("se", pillars, edge, -edge, s, e);



                //Respawn
                #region Respawn
                var respawn = makeObject("Respawn", room.transform);
                for( int i = 0; i < roomSizeInt; i++) {
                    for (int j = 0; j < roomSizeInt; j++) {
                        var resp = makeObject(string.Format("{0}_{1}",i,j), respawn.transform);
                        resp.transform.localPosition = new Vector3( i * roomUnitSize - roomUnitSize * (roomSizeInt / 2) , 0, j * roomUnitSize - roomUnitSize * (roomSizeInt / 2));
                    }
                }
                #endregion

                mng.Respawns = respawn.GetComponentsInChildren<Transform>().Where(it => respawn != it.gameObject).ToArray();
            }
            GUILayout.Space(10f);
            GUILayout.Box("--", GUILayout.ExpandWidth(true), GUILayout.Height(10));
            GUILayout.Space(10f);

            GUILayout.Label("床の厚さ");
            FloorThicnessStr = GUILayout.TextField(FloorThicnessStr);
            GUILayout.Label("壁の高さ");
            WallHeightStr = GUILayout.TextField(WallHeightStr);
            GUILayout.Label("壁の厚さ");
            WallThicnessStr = GUILayout.TextField(WallThicnessStr);

        }

        private GameObject makeObject(string name, Transform parent) {
            GameObject obj = new GameObject();
            return settingObject(obj, name, parent);
        }

        private GameObject makeCube(string name, Transform parent, Vector3 scale) {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.localScale = scale;
            return settingObject( cube, name, parent );
        }
        private GameObject settingObject( GameObject obj, string name, Transform parent ) {
            obj.name = name;
            obj.transform.SetParent(parent);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.rotation = new Quaternion();
            return obj;
        }

        private GameObject makeConnect(string name, Transform parent, Vector3 posi, float rotate, int roomUnit, int roomSize) {
            var conn = makeObject(name, parent);
            conn.transform.localPosition = posi;
            conn.transform.Rotate(new Vector3(0,rotate,0));

            var gate = makeObject("Gate", conn.transform);
            var wall = makeObject("Wall", conn.transform);

            var depth = posi.x == 0 ? -WallThicness / 2 : WallThicness / 2;

            //Gate
            var gateSize = roomSize > 1 ? roomUnit : roomUnit - WallThicness * 2;
            var gateCube = makeCube("gate", gate.transform, new Vector3(gateSize, WallHeight, WallThicness));
            gateCube.transform.localPosition = new Vector3(0, WallHeight / 2 + FloorThicness / 2, depth);

            if (roomSize > 1) {
                //Wall
                float gatePosiX = roomUnit + roomUnit * (float)(roomSize - 3) * 0.25f - WallThicness / 2;
                var l = makeCube("wall_l", wall.transform, new Vector3(roomUnit * (roomSize / 2) - WallThicness, WallHeight, WallThicness));
                l.transform.localPosition = new Vector3(gatePosiX, WallHeight / 2 + FloorThicness / 2, depth);
                var r = makeCube("wall_r", wall.transform, new Vector3(roomUnit * (roomSize / 2) - WallThicness, WallHeight, WallThicness));
                r.transform.localPosition = new Vector3(-gatePosiX, WallHeight / 2 + FloorThicness / 2, depth);
            }

            return conn;
        }

        private void makePillars(Transform room, float roomUnit, int roomSize, GameObject n, GameObject s, GameObject w, GameObject e) {
            var edge = roomUnit * roomSize / 2 - WallThicness / 2;
            var parent = makeObject("Pillars", room).transform;
            var nw = makeCube("nw", parent, new Vector3(WallThicness, WallHeight, WallThicness));
            nw.transform.localPosition = new Vector3(-edge, WallHeight / 2 + FloorThicness / 2, edge);
            var relation = nw.AddComponent<RelationObjectMng>();
            var ne = makeCube("ne", parent, new Vector3(WallThicness, WallHeight, WallThicness));
            ne.transform.localPosition = new Vector3(edge, WallHeight / 2 + FloorThicness / 2, edge);
            var sw = makeCube("sw", parent, new Vector3(WallThicness, WallHeight, WallThicness));
            sw.transform.localPosition = new Vector3(-edge, WallHeight / 2 + FloorThicness / 2, -edge);
            var se = makeCube("se", parent, new Vector3(WallThicness, WallHeight, WallThicness));
            se.transform.localPosition = new Vector3(edge, WallHeight / 2 + FloorThicness / 2, -edge);
        }

        private RelationObjectMng makePillar(string name, Transform parent, float x, float z, GameObject wall_1, GameObject wall_2 ) {
            var pillar = makeCube(name, parent, new Vector3(WallThicness, WallHeight, WallThicness));
            pillar.transform.localPosition = new Vector3(x, WallHeight / 2 + FloorThicness / 2, z);
            var walls = pillar.AddComponent<RelationObjectMng>();
            walls.Relation = new List<GameObject>();
            walls.Relation.Add(wall_1);
            walls.Relation.Add(wall_2);
            return walls;

        }
    }
}
#endif