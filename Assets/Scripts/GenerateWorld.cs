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

public class GenerateWorld : MonoBehaviour
{
    public int seedoffset;
    public Transform player;
    public List<SetChunk> SetChunks;
    public List<RandomBit> Bits;
    public int scale;
    public int viewscale;

    private int X;
    private int Y;

    private GameObject[,] chunks;

    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        chunks = new GameObject[viewscale, viewscale];
        for (int i = 0; i < viewscale; i++)
        {
            for (int j = 0; j < viewscale; j++)
            {
                if (Vector2.Distance(new Vector2(i - (viewscale / 2), j - (viewscale / 2)), Vector2.zero) <= viewscale / 2)
                {
                    chunks[i, j] = GenerateChunk(i - (viewscale / 2), j - (viewscale / 2));
                    int distance = Mathf.Max(Mathf.Abs(i - (viewscale / 2)), Mathf.Abs(j - (viewscale / 2)));
                    chunks[i, j].SendMessage("PlayerDistance", distance, SendMessageOptions.DontRequireReceiver);
                }
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
                if (chunks[i, j] != null && (Vector2.Distance(new Vector2(i - (viewscale / 2), j - (viewscale / 2)), Vector2.zero) > viewscale / 2))
                    Destroy(chunks[i, j], Random.Range(0f, 3f));
                if (i + X_Shift >= 0 && i + X_Shift < viewscale && j + Y_Shift >= 0 && j + Y_Shift < viewscale)
                {
                    newchunks[i, j] = chunks[i + X_Shift, j + Y_Shift];
                    /*if (newchunks[i, j] != null)
                    {
                        int distance = Mathf.Max(Mathf.Abs(i - (viewscale / 2)), Mathf.Abs(j - (viewscale / 2)));
                        newchunks[i, j].SendMessage("PlayerDistance", distance, SendMessageOptions.DontRequireReceiver);
                    }*/
                }
                if (newchunks[i, j] == null && (Vector2.Distance(new Vector2(i - (viewscale / 2), j - (viewscale / 2)), Vector2.zero) < viewscale / 2))
                {
                    newchunks[i, j] = GenerateChunk(i - (viewscale / 2) + X, j - (viewscale / 2) + Y);
                }
            }
        }

        chunks = newchunks;
    }

    private GameObject GenerateChunk(int x, int y)
    {
        Random.InitState(seedoffset);
        Random.InitState(Random.Range(int.MinValue, int.MaxValue) + x);
        Random.InitState(Random.Range(int.MinValue, int.MaxValue) + y);
        Seed seed = new Seed(Random.Range(int.MinValue, int.MaxValue), x, y);

        foreach (SetChunk C in SetChunks)
        {
            if (C.X == x && C.Y == y)
            {
                GameObject chunk = Instantiate(C.Prefab, transform);
                chunk.transform.position = new Vector3(x * scale, 0, y * scale);
                chunk.SendMessage("Generate", seed, SendMessageOptions.DontRequireReceiver);
                return chunk;
            }
        }

        GameObject chosenbit = ChooseObject(Bits);
        if (chosenbit)
        {
            //return FindLowestChunk(seed, chosenbit, x, y);
            GameObject chunk = Instantiate(chosenbit, transform);
            chunk.transform.position = new Vector3(x * scale, 0, y * scale);
            chunk.SendMessage("Generate", seed, SendMessageOptions.DontRequireReceiver);
            return chunk;
        }

        return null;
    }

    private GameObject FindLowestChunk(Seed seed, GameObject chosenchunk, int x, int y)
    {
        if (chosenchunk.transform.childCount == 0)
        {
            if (chosenchunk.GetComponents(typeof(Component)).Length == 2)
            {
                if (chosenchunk.GetComponent<GenerateRandomBit>())
                {
                    return FindLowestChunk(seed, ChooseObject(chosenchunk.GetComponent<GenerateRandomBit>().Bits), x, y);
                }
            }
        }

        GameObject chunk = Instantiate(chosenchunk, transform);
        chunk.transform.position = new Vector3(x * scale, 0, y * scale);
        chunk.SendMessage("Generate", seed, SendMessageOptions.DontRequireReceiver);
        return chunk;
    }

    private GameObject ChooseObject(List<RandomBit> randombits)
    {
        int totalweight = 0;
        foreach (RandomBit Bit in randombits)
        {
            totalweight += Bit.Weight;
        }
        int chosenvalue = Random.Range(1, totalweight + 1);
        totalweight = 0;
        foreach (RandomBit Bit in randombits)
        {
            totalweight += Bit.Weight;
            if (chosenvalue <= totalweight)
            {
                return Bit.Prefab;
            }
        }

        return null;
    }
}
