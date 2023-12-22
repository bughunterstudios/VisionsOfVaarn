using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Seed
{
    public int seed;
    public int X;
    public int Y;

    public Seed(int seed, int X, int Y)
    {
        this.seed = seed;
        this.X = X;
        this.Y = Y;
    }
}

[System.Serializable]
public class SetChunk
{
    public int X, Y;
    public GameObject Prefab;
}

[System.Serializable]
public class RandomChunk
{
    public int Weight;
    public bool Tiles;
    public GameObject Prefab;
}

public class GenerateWorld : MonoBehaviour
{
    public Transform player;
    public List<SetChunk> SetHunks;
    public List<RandomChunk> RandomChunks;
    public List<RandomBit_NoiseWeighted> Tiles;
    public int scale;
    public int viewscale;

    private int X;
    private int Y;

    private GameObject[,] chunks;
    private bool[,] mask;

    private List<Transform> tile_queue_parent;
    private List<Seed> tile_queue_seed;
    private float totalweight;
    private float chosenvalue;
    private Region region;
    private float smooth;
    private int sub_x;
    private int sub_y;

    // Start is called before the first frame update
    void Start()
    {
        //Application.targetFrameRate = 140;

        tile_queue_parent = new List<Transform>();
        tile_queue_seed = new List<Seed>();

        Initialize();
    }

    private void Initialize()
    {
        chunks = new GameObject[viewscale + 4, viewscale + 4];
        mask = new bool[viewscale + 4, viewscale + 4];
        X = Mathf.RoundToInt(player.position.x / scale);
        Y = Mathf.RoundToInt(player.position.z / scale);
        for (int i = 0; i < viewscale + 4; i++)
        {
            for (int j = 0; j < viewscale + 4; j++)
            {
                if (Vector2.Distance(new Vector2(i - ((viewscale + 4) / 2), j - ((viewscale + 4) / 2)), Vector2.zero) <= viewscale / 2)
                {
                    mask[i, j] = true;
                    chunks[i, j] = GenerateChunk(X + i - ((viewscale + 4) / 2), Y + j - ((viewscale + 4) / 2));
                }
                else
                    mask[i, j] = false;
            }
        }
    }

    /*void OnGUI()
    {
        GUI.Label(new Rect(0, 15, 100, 100), tile_queue_parent.Count.ToString());
    }*/

    // Update is called once per frame
    void Update()
    {
        int new_X = Mathf.RoundToInt(player.position.x / scale);
        int new_Y = Mathf.RoundToInt(player.position.z / scale);

        if (new_X != X || new_Y != Y)
        {
            int change_X = new_X - X;
            int change_Y = new_Y - Y;
            X = new_X;
            Y = new_Y;
            Shift(change_X, change_Y);
        }

        /*if (tile_queue_parent.Count > 0)
        {
            if (tile_queue_parent[0] == null)
            {
                tile_queue_parent.RemoveAt(0);
                tile_queue_seed.RemoveAt(0);
            }
            else
            {
                for (sub_x = -20; sub_x <= 20; sub_x += 10)
                {
                    for (sub_y = -20; sub_y <= 20; sub_y += 10)
                    {
                        region = NoiseControl.Tile_Regions_value(tile_queue_parent[0].position.x + sub_x, tile_queue_parent[0].position.z + sub_y);
                        smooth = NoiseControl.Regions_smooth(tile_queue_parent[0].position.x + sub_x, tile_queue_parent[0].position.z + sub_y);

                        totalweight = 0;
                        foreach (RandomBit_NoiseWeighted tile in Tiles)
                        {
                            totalweight += tile.GetWeight(region, smooth);
                        }
                        Random.InitState(tile_queue_seed[0].seed);
                        Seed newseed = new Seed(Random.Range(int.MinValue, int.MaxValue), tile_queue_seed[0].X, tile_queue_seed[0].Y);
                        tile_queue_seed[0] = newseed;
                        chosenvalue = Random.Range(0f, totalweight);
                        totalweight = 0;
                        foreach (RandomBit_NoiseWeighted tile in Tiles)
                        {
                            totalweight += tile.GetWeight(region, smooth);
                            if (chosenvalue <= totalweight)
                            {
                                if (tile.Prefab != null)
                                {
                                    GameObject newtile = Instantiate(tile.Prefab, tile_queue_parent[0]);
                                    newtile.transform.position = new Vector3(tile_queue_parent[0].position.x + sub_x, 0, tile_queue_parent[0].position.z + sub_y);
                                    newtile.SendMessage("Generate", newseed, SendMessageOptions.DontRequireReceiver);
                                }
                                break;
                            }
                        }
                    }
                }
                tile_queue_parent.RemoveAt(0);
                tile_queue_seed.RemoveAt(0);
            }
        }*/
    }

    private void Shift(int X_Shift, int Y_Shift)
    {
        GameObject[,] newchunks = new GameObject[viewscale + 4, viewscale + 4];

        //Shift over everything
        for (int i = 0; i < viewscale + 4; i++)
        {
            for (int j = 0; j < viewscale + 4; j++)
            {
                if (chunks[i, j] != null && (i - X_Shift < 0 || i - X_Shift > viewscale + 4 - 1 || j - Y_Shift < 0 || j - Y_Shift > viewscale + 4 - 1))
                {
                    Destroy(chunks[i, j], Random.Range(0f, 3f));
                }
                if (i + X_Shift >= 0 && i + X_Shift < viewscale + 4 && j + Y_Shift >= 0 && j + Y_Shift < viewscale + 4)
                {
                    newchunks[i, j] = chunks[i + X_Shift, j + Y_Shift];
                }
            }
        }

        // Destroy and allocate for creation
        for (int i = 0; i < viewscale + 4; i++)
        {
            for (int j = 0; j < viewscale + 4; j++)
            {
                if (newchunks[i, j] != null && !mask[i, j])
                {
                    Destroy(newchunks[i, j], Random.Range(0f, 3f));
                    newchunks[i, j] = null;
                }
                if (newchunks[i, j] == null && mask[i, j])
                {
                    newchunks[i, j] = GenerateChunk(i - ((viewscale + 4) / 2) + X, j - ((viewscale + 4) / 2) + Y);
                }
            }
        }

        chunks = newchunks;
    }

    private GameObject GenerateChunk(int x, int y)
    {
        RandomChunk chosen = ChooseObject(RandomChunks);

        if (chosen == null)
            return null;

        Random.InitState(WorldSeed.seed);
        Random.InitState(Random.Range(int.MinValue, int.MaxValue) + x);
        Random.InitState(Random.Range(int.MinValue, int.MaxValue) + y);
        Seed seed = new Seed(Random.Range(int.MinValue, int.MaxValue), x, y);

        GameObject chunk = new GameObject();
        chunk.name = "Chunk " + x.ToString() + ", " + y.ToString();
        chunk.transform.SetParent(transform);
        chunk.transform.position = new Vector3(x * scale, 0, y * scale);

        if (chosen.Tiles)
        {
            tile_queue_parent.Add(chunk.transform);
            tile_queue_seed.Add(seed);
        }
        else
        {
            GameObject chunk_child = Instantiate(chosen.Prefab, chunk.transform);
            chunk_child.transform.position = new Vector3(x * scale, 0, y * scale);
            chunk_child.SendMessage("Generate", seed, SendMessageOptions.DontRequireReceiver);
        }

        return chunk;
    }

    private RandomChunk ChooseObject(List<RandomChunk> randomchunks)
    {

        int totalweight = 0;
        foreach (RandomChunk Chunk in RandomChunks)
        {
            totalweight += Chunk.Weight;
        }
        int chosenvalue = Random.Range(1, totalweight + 1);
        totalweight = 0;
        foreach (RandomChunk Chunk in randomchunks)
        {
            totalweight += Chunk.Weight;
            if (chosenvalue <= totalweight)
            {
                return Chunk;
            }
        }

        return null;
    }
}
