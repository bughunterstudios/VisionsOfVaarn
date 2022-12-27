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
public class SetHunk
{
    public int X, Y;
    public GameObject Prefab;
}

[System.Serializable]
public class RandomHunk
{
    public int Weight;
    public GameObject Prefab;
}

public class GenerateWorld : MonoBehaviour
{
    public int seedoffset;
    public Transform player;
    public List<SetHunk> SetHunks;
    public List<RandomHunk> Hunks;
    public int scale;
    public int viewscale;

    private int X;
    private int Y;

    private GameObject[,] chunks;

    private bool[,] mask;

    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        chunks = new GameObject[viewscale, viewscale];
        mask = new bool[viewscale, viewscale];
        for (int i = 0; i < viewscale; i++)
        {
            for (int j = 0; j < viewscale; j++)
            {
                if (Vector2.Distance(new Vector2(i - (viewscale / 2), j - (viewscale / 2)), Vector2.zero) <= viewscale / 2)
                {
                    mask[i, j] = true;
                    chunks[i, j] = GenerateChunk(i - (viewscale / 2), j - (viewscale / 2));
                }
                else
                    mask[i, j] = false;
            }
        }
    }

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

            //Debug.Log(X.ToString() + ", " + Y.ToString());
        }
    }

    private void Shift(int X_Shift, int Y_Shift)
    {
        GameObject[,] newchunks = new GameObject[viewscale, viewscale];

        for (int i = 0; i < viewscale; i++)
        {
            for (int j = 0; j < viewscale; j++)
            {
                if (chunks[i, j] != null && (i - X_Shift < 0 || i - X_Shift > viewscale - 1 || j - Y_Shift < 0 || j - Y_Shift > viewscale - 1))
                    Destroy(chunks[i, j], Random.Range(0f, 3f));
                if (chunks[i, j] != null && !mask[i, j])
                    Destroy(chunks[i, j], Random.Range(0f, 3f));
                if (i + X_Shift >= 0 && i + X_Shift < viewscale && j + Y_Shift >= 0 && j + Y_Shift < viewscale)
                {
                    newchunks[i, j] = chunks[i + X_Shift, j + Y_Shift];
                }
                if (newchunks[i, j] == null && mask[i, j])
                {
                    newchunks[i, j] = GenerateChunk(i - (viewscale / 2) + X, j - (viewscale / 2) + Y);
                }
            }
        }

        chunks = newchunks;
    }

    private GameObject GenerateChunk(int x, int y)
    {
        int hunk_X = Mathf.RoundToInt(x / 5f);
        int hunk_Y = Mathf.RoundToInt(y / 5f);

        Random.InitState(seedoffset);
        Random.InitState(Random.Range(int.MinValue, int.MaxValue) + hunk_X);
        Random.InitState(Random.Range(int.MinValue, int.MaxValue) + hunk_Y);

        GameObject chosenhunk = ChooseObject(Hunks);

        Random.InitState(seedoffset);
        Random.InitState(Random.Range(int.MinValue, int.MaxValue) + x);
        Random.InitState(Random.Range(int.MinValue, int.MaxValue) + y);
        Seed seed = new Seed(Random.Range(int.MinValue, int.MaxValue), x, y);

        foreach (SetHunk C in SetHunks)
        {
            if (C.X == hunk_X && C.Y == hunk_Y)
                chosenhunk = C.Prefab;
        }

        if (chosenhunk)
        {
            int inner_x = (x - (hunk_X * 5)) * scale;
            int inner_y = (y - (hunk_Y * 5)) * scale;

            for (int i = 0; i < chosenhunk.transform.childCount; i++)
            {
                if (chosenhunk.transform.GetChild(i).transform.localPosition.x == inner_x &&
                    chosenhunk.transform.GetChild(i).transform.localPosition.z == inner_y)
                {
                    GameObject chunk = Instantiate(chosenhunk.transform.GetChild(i).gameObject, transform);
                    chunk.transform.position = new Vector3(x * scale, chunk.transform.position.y, y * scale);
                    chunk.SendMessage("Generate", seed, SendMessageOptions.DontRequireReceiver);
                    return chunk;
                }
            }
        }

        return null;
    }

    private GameObject ChooseObject(List<RandomHunk> randomhunks)
    {
        int totalweight = 0;
        foreach (RandomHunk Hunk in randomhunks)
        {
            totalweight += Hunk.Weight;
        }
        int chosenvalue = Random.Range(1, totalweight + 1);
        totalweight = 0;
        foreach (RandomHunk Hunk in randomhunks)
        {
            totalweight += Hunk.Weight;
            if (chosenvalue <= totalweight)
            {
                return Hunk.Prefab;
            }
        }

        return null;
    }
}
