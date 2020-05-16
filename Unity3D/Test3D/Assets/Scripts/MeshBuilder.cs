using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;

public class MeshBuilder : MonoBehaviour
{
    public TileType TileType;

    private List<Vector3> vertices;
    private List<Vector3> normals;
    private List<Vector2> uvs;
    private List<int> tris;

    private MeshFilter meshFilter;

    private MeshCollider meshCollider;

    private MeshRenderer meshRenderer;

    public Material Material;

    private const float WallHeight = 2f;

    private bool initDone = false;

    void Awake ()
    {
        Debug.Log("MeshBuilder: " + this.TileType);

        this.vertices = new List<Vector3>();
        this.normals = new List<Vector3>();
        this.uvs = new List<Vector2>();
        this.tris = new List<int>();
    }

    public void Init()
    {
        if (this.initDone)
        {
            return;
        }

        this.meshFilter = this.gameObject.AddComponent<MeshFilter>();
        this.meshCollider = this.gameObject.AddComponent<MeshCollider>();
        this.meshRenderer = this.gameObject.AddComponent<MeshRenderer>();
        this.initDone = true;
    }

    public void BuildMesh()
    {
        Mesh m = new Mesh();
        m.vertices = this.vertices.ToArray();
        m.normals = this.normals.ToArray();
        m.uv = this.uvs.ToArray();
        m.triangles = this.tris.ToArray();
        //m.RecalculateNormals();
        //m.RecalculateBounds();
        //m.Optimize();
        this.meshRenderer.material = this.Material;
        this.meshRenderer.shadowCastingMode= ShadowCastingMode.Off;
        this.meshFilter.mesh = m;
        this.meshCollider.sharedMesh = m;
    }

    public void AddFloorOrCeilingToMesh(BaseArea r, bool ceiling)
    {
        this.Init();

        float y = ceiling ? 2f : 0f;
        Vector3 vert1 = new Vector3(r.X1, y, r.Y1);    // bottom left
        Vector3 vert2 = new Vector3(r.X1, y, r.Y2 + 1);    // top left
        Vector3 vert3 = new Vector3(r.X2 + 1, y, r.Y1);    // bottom right
        Vector3 vert4 = new Vector3(r.X2 + 1, y, r.Y2 + 1);    // top right

        Vector3 norm1 = ceiling ? Vector3.down : Vector3.up;
        Vector3 norm2 = ceiling ? Vector3.down : Vector3.up;
        Vector3 norm3 = ceiling ? Vector3.down : Vector3.up;
        Vector3 norm4 = ceiling ? Vector3.down : Vector3.up;

        Vector2 uv1 = new Vector2(0f, 0f);
        Vector2 uv2 = new Vector2(0f, r.Height);
        Vector2 uv3 = new Vector2(r.Width, 0f);
        Vector2 uv4 = new Vector2(r.Width, r.Height);

        int v = this.vertices.Count;

        int[] tri1 = ceiling ? new[] { v, v + 3, v + 1 } : new[] { v, v + 1, v + 3 };
        int[] tri2 = ceiling ?  new[] { v, v + 2, v + 3 } : new[] { v, v + 3, v + 2 };

        this.tris.AddRange(tri1);
        this.tris.AddRange(tri2);

        this.vertices.Add(vert1);
        this.vertices.Add(vert2);
        this.vertices.Add(vert3);
        this.vertices.Add(vert4);

        this.normals.Add(norm1);
        this.normals.Add(norm2);
        this.normals.Add(norm3);
        this.normals.Add(norm4);

        this.uvs.Add(uv1);
        this.uvs.Add(uv2);
        this.uvs.Add(uv3);
        this.uvs.Add(uv4);
    }

    public void AddNorthFacingWallToMesh(float y, float x1, float x2)
    {
        this.Init();

        Debug.Log(string.Format("North Wall: Y: {0} X: {1} - {2} ", y, x1,x2  ));

        float width = x2 - x1;
        Vector3 vert1 = new Vector3(x1, 0, y);    // bottom left
        Vector3 vert2 = new Vector3(x1, WallHeight, y);    // top left
        Vector3 vert3 = new Vector3(x2, WallHeight, y);    // bottom right
        Vector3 vert4 = new Vector3(x2, 0, y);    // top right

        Vector3 norm1 = Vector3.forward;    //back
        Vector3 norm2 = Vector3.forward;
        Vector3 norm3 = Vector3.forward;
        Vector3 norm4 = Vector3.forward;

        Vector2 uv1 = new Vector2(0f, 0f);
        Vector2 uv2 = new Vector2(0f, WallHeight);
        Vector2 uv3 = new Vector2(width, WallHeight);
        Vector2 uv4 = new Vector2(width, 0f);

        int v = this.vertices.Count;

        int[] tri1 = new[] { v, v + 2, v + 1 };
        int[] tri2 = new[] { v, v + 3, v + 2 };

        this.tris.AddRange(tri1);
        this.tris.AddRange(tri2);

        this.vertices.Add(vert1);
        this.vertices.Add(vert2);
        this.vertices.Add(vert3);
        this.vertices.Add(vert4);

        this.normals.Add(norm1);
        this.normals.Add(norm2);
        this.normals.Add(norm3);
        this.normals.Add(norm4);

        this.uvs.Add(uv1);
        this.uvs.Add(uv2);
        this.uvs.Add(uv3);
        this.uvs.Add(uv4);
    }

    public void AddSouthFacingWallToMesh(float y, float x1, float x2)
    {
        this.Init();

        Debug.Log(string.Format("South Wall: Y: {0} X: {1} - {2} ", y, x1, x2));

        float width = x2 - x1;
        Vector3 vert1 = new Vector3(x1, 0, y);    // bottom left
        Vector3 vert2 = new Vector3(x1, WallHeight, y);    // top left
        Vector3 vert3 = new Vector3(x2, WallHeight, y);    // bottom right
        Vector3 vert4 = new Vector3(x2, 0, y);    // top right

        Vector3 norm1 = Vector3.back; // forward
        Vector3 norm2 = Vector3.back;
        Vector3 norm3 = Vector3.back;
        Vector3 norm4 = Vector3.back;

        Vector2 uv1 = new Vector2(0f, 0f);
        Vector2 uv2 = new Vector2(0f, WallHeight);
        Vector2 uv3 = new Vector2(width, WallHeight);
        Vector2 uv4 = new Vector2(width, 0f);

        int v = this.vertices.Count;

        int[] tri1 = new[] { v, v + 1, v + 2 };
        int[] tri2 = new[] { v, v + 2, v + 3 };

        this.tris.AddRange(tri1);
        this.tris.AddRange(tri2);

        this.vertices.Add(vert1);
        this.vertices.Add(vert2);
        this.vertices.Add(vert3);
        this.vertices.Add(vert4);

        this.normals.Add(norm1);
        this.normals.Add(norm2);
        this.normals.Add(norm3);
        this.normals.Add(norm4);

        this.uvs.Add(uv1);
        this.uvs.Add(uv2);
        this.uvs.Add(uv3);
        this.uvs.Add(uv4);
    }

    public void AddWestFacingWallToMesh(float x, float y1, float y2)
    {
        this.Init();

        Debug.Log(string.Format("West Wall: X: {0} Y: {1} - {2} ", x, y1, y2));

        float width = y2 - y1;
        Vector3 vert1 = new Vector3(x, 0, y1);    // bottom front
        Vector3 vert2 = new Vector3(x, WallHeight, y1);    // top front
        Vector3 vert3 = new Vector3(x, WallHeight, y2);    // top back
        Vector3 vert4 = new Vector3(x, 0, y2);    // bottom back

        Vector3 norm1 = Vector3.right;   //right
        Vector3 norm2 = Vector3.right;
        Vector3 norm3 = Vector3.right;
        Vector3 norm4 = Vector3.right;

        Vector2 uv1 = new Vector2(0f, 0f);
        Vector2 uv2 = new Vector2(0f, WallHeight);
        Vector2 uv3 = new Vector2(width, WallHeight);
        Vector2 uv4 = new Vector2(width, 0f);

        int v = this.vertices.Count;

        int[] tri1 = new[] { v, v + 1, v + 2 };
        int[] tri2 = new[] { v, v + 2, v + 3 };

        this.tris.AddRange(tri1);
        this.tris.AddRange(tri2);

        this.vertices.Add(vert1);
        this.vertices.Add(vert2);
        this.vertices.Add(vert3);
        this.vertices.Add(vert4);

        this.normals.Add(norm1);
        this.normals.Add(norm2);
        this.normals.Add(norm3);
        this.normals.Add(norm4);

        this.uvs.Add(uv1);
        this.uvs.Add(uv2);
        this.uvs.Add(uv3);
        this.uvs.Add(uv4);
    }

    public void AddEastFacingWallToMesh(float x, float y1, float y2)
    {
        this.Init();

        Debug.Log(string.Format("East Wall: X: {0} Y: {1} - {2} ", x, y1, y2));

        float width = y2 - y1;
        Vector3 vert1 = new Vector3(x, 0, y1);    // bottom front
        Vector3 vert2 = new Vector3(x, WallHeight, y1);    // top front
        Vector3 vert3 = new Vector3(x, WallHeight, y2);    // top back
        Vector3 vert4 = new Vector3(x, 0, y2);    // bottom back

        Vector3 norm1 = Vector3.left; //lef
        Vector3 norm2 = Vector3.left;
        Vector3 norm3 = Vector3.left;
        Vector3 norm4 = Vector3.left;

        Vector2 uv1 = new Vector2(0f, 0f);
        Vector2 uv2 = new Vector2(0f, WallHeight);
        Vector2 uv3 = new Vector2(width, WallHeight);
        Vector2 uv4 = new Vector2(width, 0f);

        int v = this.vertices.Count;

        int[] tri1 = new[] { v + 3, v + 2, v };
        int[] tri2 = new[] { v + 2, v + 1, v  };

        this.tris.AddRange(tri1);
        this.tris.AddRange(tri2);

        this.vertices.Add(vert1);
        this.vertices.Add(vert2);
        this.vertices.Add(vert3);
        this.vertices.Add(vert4);

        this.normals.Add(norm1);
        this.normals.Add(norm2);
        this.normals.Add(norm3);
        this.normals.Add(norm4);

        this.uvs.Add(uv1);
        this.uvs.Add(uv2);
        this.uvs.Add(uv3);
        this.uvs.Add(uv4);
    }
}
