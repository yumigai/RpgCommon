using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FreeTransformMng : MonoBehaviour
{
    [SerializeField]
    public Transform GuidBase;

    [SerializeField]
    private Vector3 MinSize = Vector3.one;

    [SerializeField]
    private Vector3 MaxSize = Vector3.one;

    private Vector3 MinPosi;

    private Vector3 MaxPosi;


    void Awake() {

        if (GuidBase == null) {
            GuidBase = this.transform;
        }

        Vector3[] vertices = null;

        var fil = GuidBase.GetComponent<MeshFilter>();
        if (fil != null) {
            vertices = fil.mesh.vertices;
        } else {
            var rect = GuidBase.GetComponent<RectTransform>();
            if (rect != null) {
                vertices = new Vector3[4];
                rect.GetWorldCorners(vertices);
            }
        }

        if (vertices != null) {
            MinPosi = new Vector3(
                vertices.Min(it => it.x),
                vertices.Min(it => it.y),
                vertices.Min(it => it.z)
            );
            MaxPosi = new Vector3(
                vertices.Max(it => it.x),
                vertices.Max(it => it.y),
                vertices.Max(it => it.z)
            );
        }
    }

    public void shape(Transform tra) {

        tra.position = randomPosition();

        if (MinSize != Vector3.one || MaxSize != Vector3.one) {
            tra.localScale = randomSize();
        }
    }

    public Vector3 randomPosition() {
        Vector3 posi = new Vector3(
            Random.Range(MinPosi.x, MaxPosi.x),
            Random.Range(MinPosi.y, MaxPosi.y),
            Random.Range(MinPosi.z, MaxPosi.z)
        );
        return posi;
    }

    public Vector3 randomSize() {
        Vector3 size = new Vector3(
            Random.Range(MinSize.x, MaxSize.x),
            Random.Range(MinSize.y, MaxSize.y),
            Random.Range(MinSize.z, MaxSize.z)
        );
        return size;
    }
}
