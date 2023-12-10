using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateGrounds : MonoBehaviour
{
    public Transform player;
    public GameObject DynamicGround;
    public int scale;
    public int viewscale;

    private int X;
    private int Y;

    private GameObject[,] grounds;
    private bool[,] mask;
    private List<GameObject> ground_to_move;

    public bool ActivatePlayerWhenInit;

    // Start is called before the first frame update
    void Start()
    {
        Initialize();

        if (ActivatePlayerWhenInit)
            player.GetComponent<CharacterController>().enabled = true;
    }

    private void Initialize()
    {
        grounds = new GameObject[viewscale + 4, viewscale + 4];
        ground_to_move = new List<GameObject>();
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
                    grounds[i, j] = GenerateGround(X + i - ((viewscale + 4) / 2), Y + j - ((viewscale + 4) / 2));
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
        }
    }

    private void Shift(int X_Shift, int Y_Shift)
    {
        GameObject[,] newgrounds = new GameObject[viewscale + 4, viewscale + 4];
        List<int> newxs = new List<int>();
        List<int> newys = new List<int>();

        //Shift over everything
        for (int i = 0; i < viewscale + 4; i++)
        {
            for (int j = 0; j < viewscale + 4; j++)
            {
                if (grounds[i, j] != null && (i - X_Shift < 0 || i - X_Shift > viewscale + 4 - 1 || j - Y_Shift < 0 || j - Y_Shift > viewscale + 4 - 1))
                {
                    ground_to_move.Add(grounds[i, j]);
                }
                if (i + X_Shift >= 0 && i + X_Shift < viewscale + 4 && j + Y_Shift >= 0 && j + Y_Shift < viewscale + 4)
                {
                    newgrounds[i, j] = grounds[i + X_Shift, j + Y_Shift];
                }
            }
        }

        // Destroy and allocate for creation
        for (int i = 0; i < viewscale + 4; i++)
        {
            for (int j = 0; j < viewscale + 4; j++)
            {
                if (newgrounds[i, j] != null && !mask[i, j])
                {
                    ground_to_move.Add(newgrounds[i, j]);
                    newgrounds[i, j].SetActive(false);
                    newgrounds[i, j] = null;
                }
                if (newgrounds[i, j] == null && mask[i, j])
                {
                    newxs.Add(i);
                    newys.Add(j);
                }
            }
        }

        // Place moved grounds, or add new ones if needed
        for (int i = 0; i < newxs.Count; i++)
        {
            int x = newxs[i] - ((viewscale + 4) / 2) + X;
            int y = newys[i] - ((viewscale + 4) / 2) + Y;
            Random.InitState(WorldSeed.seed);
            Random.InitState(Random.Range(int.MinValue, int.MaxValue) + x);
            Random.InitState(Random.Range(int.MinValue, int.MaxValue) + y);
            Seed seed = new Seed(Random.Range(int.MinValue, int.MaxValue), x, y);
            if (ground_to_move.Count == 0)
            {
                GameObject ground = Instantiate(DynamicGround, transform);
                ground.name = "Ground: " + x.ToString() + ", " + y.ToString();
                ground_to_move.Add(ground);
            }
            ground_to_move[0].transform.position = new Vector3(x * scale, 0, y * scale);
            ground_to_move[0].SetActive(true);
            if (x == X && y == Y)
            {
                ground_to_move[0].SendMessage("GenerateUrgent", seed, SendMessageOptions.DontRequireReceiver);
            }
            else
            {
                ground_to_move[0].SendMessage("Generate", seed, SendMessageOptions.DontRequireReceiver);
            }
            newgrounds[newxs[i], newys[i]] = ground_to_move[0];
            ground_to_move.RemoveAt(0);
        }

        grounds = newgrounds;
    }

    private GameObject GenerateGround(int x, int y)
    {
        Random.InitState(WorldSeed.seed);
        Random.InitState(Random.Range(int.MinValue, int.MaxValue) + x);
        Random.InitState(Random.Range(int.MinValue, int.MaxValue) + y);
        Seed seed = new Seed(Random.Range(int.MinValue, int.MaxValue), x, y);

        GameObject ground = Instantiate(DynamicGround, transform);
        ground.name = "Ground: " + x.ToString() + ", " + y.ToString();
        ground.transform.position = new Vector3(x * scale, 0, y * scale);

        if (x == X && y == Y)
        {
            ground.SendMessage("GenerateUrgent", seed, SendMessageOptions.DontRequireReceiver);
        }
        else
        {
            ground.SendMessage("Generate", seed, SendMessageOptions.DontRequireReceiver);
        }

        return ground;
    }
}
