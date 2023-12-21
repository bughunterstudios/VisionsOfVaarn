using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DynamicGround : MonoBehaviour
{
    private float time;
    private bool generated;
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

    public int parts;

    public double cutposition;

    public bool setcolor;

    private Transform cam;

    private Seed seed;
    private Vector3[] vertices;
    private Color[] colors;
    private Mesh clonedMesh;
    private int i;
    private bool generating;

    private bool urgent;

    private bool already_generated_once;

    private int[] original_triangles;

    private void Start()
    {
        cam = Camera.main.transform;
    }

    public bool DoneGenerating()
    {
        return generated;
    }

    private void Update()
    {
        if (!generated && received_seed && !generating)
        {
            if (current_player_distance == -1)
            {
                time -= Time.deltaTime;
                if (time < 0)
                    ActuallyGenerate();
            }
            else
            {
                Vector3 campos = cam.transform.position;
                campos.y = 0;

                if (Vector3.Distance(campos, transform.position) < (current_player_distance * 50))
                    current_player_distance = -1;
            }
        }

        if (generating)
            Generate_Part();
    }

    public void GenerateUrgent(Seed seed)
    {
        urgent = true;
        Generate(seed);
        ActuallyGenerate();
    }

    public void Generate(Seed seed)
    {
        if (already_generated_once)
            GetComponent<MeshRenderer>().enabled = false;

        generated = false;
        generating = false;
        received_seed = true;
        current_player_distance = player_distance;
        this.seed = seed;
        time = Random.Range(0f, 0.5f);
    }

    public void ActuallyGenerate()
    {
        if (already_generated_once)
        {
            clonedMesh.triangles = original_triangles;
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
        }

        generating = true;
        i = 0;

        if (urgent)
            Generate_Part();
    }

    private void Generate_Part()
    {
        for (; i < vertices.Length; i++)
        {
            vertices[i].z = NoiseControl.NoiseMapHeight(transform.TransformPoint(vertices[i]).x, transform.TransformPoint(vertices[i]).z);
            if (setcolor)
                colors[i] = NoiseControl.NoiseColorMap(transform.TransformPoint(vertices[i]).x, transform.TransformPoint(vertices[i]).z);

            if (i % parts == 0)
            {
                i++;
                break;
            }
        }

        if (i >= vertices.Length)
            Generate_Final();
        else if (urgent)
            Generate_Part();
    }

    private void Generate_Final()
    {
        generating = false;

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

        generated = true;
        already_generated_once = true;
        GetComponent<MeshRenderer>().enabled = true;
    }
}
