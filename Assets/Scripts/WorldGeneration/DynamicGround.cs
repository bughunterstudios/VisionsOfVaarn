using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public enum generating_stage
{
    Waiting,
    Generating,
    VerticesDone,
    Generated
}

public class DynamicGround : MonoBehaviour
{
    public static int threads;

    private bool received_seed;
    private static int[] triangles_to_delete1;
    private static int[] triangles_to_delete2;
    private static int[] triangles_to_delete3;
    private static int[] triangles_to_delete4;
    private static int[] triangles_to_delete5;

    public int triangle_index;
    public bool delete_edges;

    public int player_distance = -1;
    private int current_player_distance;

    public double cutposition;

    public bool setcolor;

    private Transform cam;

    private Seed seed;
    private Vector3[] vertices;
    private float[] xs;
    private float[] zs;
    private Color[] colors;
    private Mesh clonedMesh;

    private generating_stage stage;

    private bool already_generated_once;

    private int[] original_triangles;

    private void Start()
    {
        cam = Camera.main.transform;
    }

    public bool DoneGenerating()
    {
        return stage == generating_stage.Generated;
    }

    private void Update()
    {
        if (stage == generating_stage.Waiting && received_seed)
        {
            if (current_player_distance == -1 && threads >= 1)
                Debug.Log(threads);

            if (current_player_distance == -1 && threads < 1)
            {
                ActuallyGenerate(false);
            }
            else
            {
                Vector3 campos = cam.transform.position;
                campos.y = 0;

                if (Vector3.Distance(campos, transform.position) < (current_player_distance * 50))
                    current_player_distance = -1;
            }
        }

        if (stage == generating_stage.VerticesDone)
            Generate_Final();
    }

    public void GenerateUrgent(Seed seed)
    {
        Generate(seed);
        ActuallyGenerate(true);
    }

    public void Generate(Seed seed)
    {
        if (already_generated_once)
            GetComponent<MeshRenderer>().enabled = false;

        stage = generating_stage.Waiting;
        received_seed = true;
        current_player_distance = player_distance;
        this.seed = seed;
    }

    private void ActuallyGenerate(bool urgent)
    {
        //if (gameObject.transform.parent.name != "Ground: 0, 0")
        //    return;

        if (already_generated_once)
        {
            clonedMesh.triangles = original_triangles;

            for (int i = 0; i < vertices.Length; i++)
            {
                xs[i] = transform.TransformPoint(vertices[i]).x;
                zs[i] = transform.TransformPoint(vertices[i]).z;
            }
        }
        else
        {
            MeshFilter meshFilter = GetComponent<MeshFilter>();
            Mesh originalMesh = meshFilter.mesh;
            clonedMesh = new Mesh();

            clonedMesh.name = "clone";
            clonedMesh.vertices = originalMesh.vertices;
            clonedMesh.triangles = originalMesh.triangles;
            original_triangles = clonedMesh.triangles;
            clonedMesh.normals = originalMesh.normals;
            clonedMesh.uv = originalMesh.uv;
            meshFilter.mesh = clonedMesh;

            vertices = clonedMesh.vertices;
            colors = new Color[vertices.Length];
            xs = new float[vertices.Length];
            zs = new float[vertices.Length];

            for (int i = 0; i < vertices.Length; i++)
            {
                xs[i] = transform.TransformPoint(vertices[i]).x;
                zs[i] = transform.TransformPoint(vertices[i]).z;
            }
        }

        stage = generating_stage.Generating;

        threads++;
        if (urgent)
            Generate_Vertices();
        else
        {
            Thread thread = new Thread(Generate_Vertices);
            thread.Start();
        }
    }

    private void Generate_Vertices()
    {
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i].z = NoiseControl.NoiseMapHeight(xs[i], zs[i]);
            if (setcolor)
                colors[i] = NoiseControl.NoiseColorMap(xs[i], zs[i]);
        }

        stage = generating_stage.VerticesDone;
        threads--;
    }

    private void Generate_Final()
    {
        clonedMesh.vertices = vertices;
        clonedMesh.colors = colors;

        clonedMesh.RecalculateNormals();
        clonedMesh.RecalculateTangents();

        if (delete_edges && !already_generated_once)
        {
            if ((triangles_to_delete1 == null && triangle_index == 1) || (triangles_to_delete2 == null && triangle_index == 2) || (triangles_to_delete3 == null && triangle_index == 3) || (triangles_to_delete4 == null && triangle_index == 4) || (triangles_to_delete5 == null && triangle_index == 5))
            {
                List<int> border_indices = new List<int>();
                for (int i = 0; i < vertices.Length; i++)
                {
                    if (vertices[i].x > cutposition || vertices[i].x < -cutposition || vertices[i].y > cutposition || vertices[i].y < -cutposition)
                    {
                        border_indices.Add(i);
                    }
                }

                int[] triangles = clonedMesh.triangles;
                List<int> new_triangles = new List<int>();
                for (int i = 0; i < triangles.Length / 3; i++)
                {
                    if (!(border_indices.Contains(triangles[i * 3]) || border_indices.Contains(triangles[(i * 3) + 1]) || border_indices.Contains(triangles[(i * 3) + 2])))
                    {
                        new_triangles.Add(triangles[i * 3]);
                        new_triangles.Add(triangles[(i * 3) + 1]);
                        new_triangles.Add(triangles[(i * 3) + 2]);
                    }
                }

                if (triangle_index == 1)
                    triangles_to_delete1 = new_triangles.ToArray();
                if (triangle_index == 2)
                    triangles_to_delete2 = new_triangles.ToArray();
                if (triangle_index == 3)
                    triangles_to_delete3 = new_triangles.ToArray();
                if (triangle_index == 4)
                    triangles_to_delete4 = new_triangles.ToArray();
                if (triangle_index == 5)
                    triangles_to_delete5 = new_triangles.ToArray();
            }

            if (triangle_index == 1)
                clonedMesh.triangles = triangles_to_delete1;
            if (triangle_index == 2)
                clonedMesh.triangles = triangles_to_delete2;
            if (triangle_index == 3)
                clonedMesh.triangles = triangles_to_delete3;
            if (triangle_index == 4)
                clonedMesh.triangles = triangles_to_delete4;
            if (triangle_index == 5)
                clonedMesh.triangles = triangles_to_delete5;
        }

        clonedMesh.RecalculateBounds();

        if (GetComponent<MeshCollider>())
            GetComponent<MeshCollider>().sharedMesh = clonedMesh;

        stage = generating_stage.Generated;
        already_generated_once = true;
        GetComponent<MeshRenderer>().enabled = true;
    }
}
