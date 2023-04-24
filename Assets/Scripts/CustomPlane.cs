// https://gist.github.com/knasa21/93cd947f78aeb0664bf317cd206b3701
//
// https://github.com/hiryma/UnitySamples/blob/master/ProceduralMesh/Assets/Kayac/Graphics/ObjFileWriter.cs
// (c) 2018 hirayama takashi. (c) 2018 KAYAC Inc.

using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.InteropServices;

[RequireComponent(typeof(MeshRenderer))]
[ExecuteInEditMode]
public class CustomPlane : MonoBehaviour
{
    [DllImport("__Internal")]
    public static extern void BrowserTextDownload(string filename, string textContent);

    [Range(1, 1000)]
    public   int cols = 1,  rows = 1;
    [Range(0.01f, 100.0f)]
    public float width = 1.0f, height = 1.0f;

    [SerializeReference]
    Mesh m_OriginalMesh;

    [SerializeField]
    Camera m_Camera;

    [SerializeField]
    UIInput m_ColInput;

    [SerializeField]
    UIInput m_RawInput;

    [SerializeField]
    UIInput m_WidthInput;

    [SerializeField]
    UIInput m_HeightInput;

    string m_Hash;

    void Start()
    {
        MeshFilter meshFilter;
        meshFilter = gameObject.AddComponent<MeshFilter>();
        if (meshFilter == null) return;
        Mesh mesh  = CreateCustomPlane(cols, rows, width, height);
        m_OriginalMesh = mesh;
        
        var wire = Instantiate(mesh);
        wire.SetIndices(MakeIndices(mesh.triangles), MeshTopology.Lines, 0);
        meshFilter.mesh = wire;
    }

    void Update()
    {
        cols = (int)m_ColInput.value;
        rows = (int)m_RawInput.value;
        width = m_WidthInput.value;
        height = m_HeightInput.value;

        m_Camera.orthographicSize = Mathf.Max(1, m_Camera.orthographicSize - Input.mouseScrollDelta.y);
        
        var hash = string.Format("{0}:{1}:{2}:{3}", cols, rows, width, height);
        if (hash == m_Hash)
        {
            return;
        }
        m_Hash = hash;

        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter == null) return;
        Mesh mesh  = CreateCustomPlane(cols, rows, width, height);
        m_OriginalMesh = mesh;

        var wire = Instantiate(mesh);
        wire.SetIndices(MakeIndices(mesh.triangles), MeshTopology.Lines, 0);
        meshFilter.mesh = wire;
        
        // var saspect = m_Camera.aspect;
        // var maspect = width / height;
        // if (maspect > saspect)
        // {
        //     m_Camera.orthographicSize *= maspect / saspect;
        // }
    }

    private Mesh CreateCustomPlane(int cols, int rows, float width, float height)
    {
        if (cols <= 0 || rows <= 0 || width <= 0 || height <= 0) return new Mesh(); 
        Mesh mesh = new Mesh();
        mesh.name = "CustomPlane";
        mesh.vertices = CreateCustomVertices(cols, rows, width, height);
        mesh.triangles =  CreateCustomTriangles(cols, rows);
        mesh.uv         = CreateCustomUv(cols, rows);

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        return mesh;
    }

    Vector3[] CreateCustomVertices(int cols, int rows, float width, float height)
    {
        var vertices = new Vector3[(cols + 1) * (rows + 1)];

        int length = vertices.Length;
        for (int i = 0; i < length; i++) {
            int x = i % (cols + 1);
            int z = i / (cols + 1);
            vertices[i] = new Vector3(
                width / 2 - (width / cols) * x,
                0,
                height / 2 - (height / rows) * z
            );
        }
        /*
        foreach(var v in vertices) {
            print("vertex : " + v.ToString("F3"));
        }
        */
        return vertices;
    }

    int[] CreateCustomTriangles(int cols, int rows)
    {
        var listTriangles   = new List<int>();
        int verticesLength  = (cols + 1) * (rows + 1);

        //頂点を順になぞる、最後の行はやらない
        for (int i = 0; i < verticesLength - (cols + 1); i++) {
            //左端を飛ばす
            if ((i % (cols + 1)) == cols) continue;

            //四角1ずつ(三角2つずつ)
            listTriangles.Add(i);
            listTriangles.Add(i + cols + 1);
            listTriangles.Add(i + cols + 2);
            listTriangles.Add(i);
            listTriangles.Add(i + cols + 2);
            listTriangles.Add(i + 1);
        }
        /*
        foreach(var t in listTriangles.ToArray()) {
            print("triangle : " + t);
        }
        */

        return listTriangles.ToArray();
    }

    Vector2[] CreateCustomUv(int cols, int rows)
    {
        var uv     = new Vector2[(cols + 1) * (rows + 1)];
        int length  = uv.Length;

        for (int i = 0; i < length; i++) {
            int u = i % (cols + 1);
            int v = i / (cols + 1);
            uv[i] = new Vector2(
                1.0f - (1.0f / cols) * u,
                1.0f - (1.0f / rows) * v
            );
        }
        /*
        foreach(var u in uv) {
            print("uv : " + u.ToString("F3"));
        }
        */

        return uv;
    }

    public int[] MakeIndices(int[] triangles)
    {
        int[] indices = new int[2 * triangles.Length];
        int i = 0;
        for( int t = 0; t < triangles.Length; t+=3 )
        {
            indices[i++] = triangles[t];        //start
            indices[i++] = triangles[t + 1];   //end
            indices[i++] = triangles[t + 1];   //start
            indices[i++] = triangles[t + 2];   //end
            indices[i++] = triangles[t + 2];   //start
            indices[i++] = triangles[t];        //end
        }
        return indices;
    }

    public void Download()
    {
        var fileName = string.Format("plane-{0}x{1} ({2}x{3})", width, height, cols, rows);
        BrowserTextDownload(fileName, ToText(m_OriginalMesh, 0));
    }

    public static string ToText(Mesh mesh, int subMeshIndex)
		{
			return ToText(mesh.vertices, mesh.uv, mesh.normals, mesh.GetIndices(subMeshIndex));
		}

		public static string ToText(
			IList<Vector3> positions,
			IList<Vector2> uvs,
			IList<Vector3> normals,
			IList<int> indices)
		{
			var sb = new System.Text.StringBuilder();
			Debug.Assert(positions != null);
			// sb.AppendFormat("Generated by Kayac.ObjFileWriter. {0} vertices, {1} faces.\n", positions.Count, indices.Count / 3);
			// sb.AppendLine("# positions");
			foreach (var item in positions)
			{
				sb.AppendFormat("v {0} {1} {2}\n",
					item.x.ToString("F8"), //精度指定しないとfloat精度の全体を吐かないので劣化してしまう。10進8桁必要
					item.y.ToString("F8"),
					item.z.ToString("F8"));
			}

			bool hasUv = (uvs != null) && (uvs.Count > 0);
			if (hasUv)
			{
				Debug.Assert(uvs.Count == positions.Count);
				// sb.AppendLine("\n# texcoords");
				foreach (var item in uvs)
				{
					sb.AppendFormat("vt {0} {1}\n",
						item.x.ToString("F8"),
						item.y.ToString("F8"));
				}
			}

			Debug.Assert(normals != null);
			// sb.AppendLine("\n# normals");
			foreach (var item in normals)
			{
				sb.AppendFormat("vn {0} {1} {2}\n",
					item.x.ToString("F8"),
					item.y.ToString("F8"),
					item.z.ToString("F8"));
			}

			Debug.Assert(indices != null);
			Debug.Assert((indices.Count % 3) == 0);
			// sb.AppendLine("\n# triangle faces");
			for (var i = 0; i < indices.Count; i += 3)
			{
				var i0 = indices[i + 0] + 1; // 1 based index.
				var i1 = indices[i + 1] + 1;
				var i2 = indices[i + 2] + 1;
				if (hasUv)
				{
					sb.AppendFormat("f {0}/{0}/{0} {1}/{1}/{1} {2}/{2}/{2}\n",
						i0,
						i1,
						i2);
				}
				else
				{
					sb.AppendFormat("f {0}//{0} {1}//{1} {2}//{2}\n",
						i0,
						i1,
						i2);
				}
			}
			return sb.ToString();
		}
}
