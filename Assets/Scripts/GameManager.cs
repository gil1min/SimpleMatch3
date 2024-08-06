using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    public static GameManager Instance { get => _instance; set => _instance = value; }

    public int rows;
    public int columns;

    public float fillTime = 1f;

    public GameObject gridPrefab;

    public enum SweetsType
    {
        EMPTY,
        NORMAL,
        BARRIER,
        ROW_CLEAR,
        COLUMN_CLEAR,
        RAINBOWCANDY,
        COUNT
    }

    private Dictionary<SweetsType, GameObject> sweetPrefabDictionary;

    [System.Serializable]
    public struct SweetPrefab
    {
        public SweetsType type;
        public GameObject prefab;
    }

    public SweetPrefab[] sweetPrefabs;

    private GameSweet[,] sweets;


    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        sweetPrefabDictionary = new Dictionary<SweetsType, GameObject>();
        for (int i = 0; i < sweetPrefabs.Length; ++i)
        {
            if (!sweetPrefabDictionary.ContainsKey(sweetPrefabs[i].type))
            {
                sweetPrefabDictionary.Add(sweetPrefabs[i].type, sweetPrefabs[i].prefab);
            }
        }

        sweets = new GameSweet[rows, columns];

        for (int i = 0; i < rows; ++i)
        {
            for (int j = 0; j < columns; ++j)
            {
                GameObject chocolate = Instantiate(gridPrefab, CorrectPosition(i , j), Quaternion.identity);
                chocolate.transform.SetParent(transform);
            }
        }

        for (int i = 0; i < rows; ++i)
        {
            for (int j = 0; j < columns; ++j)
            {
                CreateNewSweet(i, j, SweetsType.EMPTY);
            }
        }

        Destroy(sweets[4, 4].gameObject);
        CreateNewSweet(4, 4, SweetsType.BARRIER);


        StartCoroutine(AllFill());

    }

    public Vector3 CorrectPosition(int x, int y)
    {
        return new Vector3(transform.position.x - columns / 2f + x, transform.position.y + rows / 2f - y);
    }

    public GameSweet CreateNewSweet(int x, int y, SweetsType type)
    {
        GameObject newSweet = Instantiate(sweetPrefabDictionary[type], CorrectPosition(x, y), Quaternion.identity);
        newSweet.transform.parent = transform;

        sweets[x, y] = newSweet.GetComponent<GameSweet>();
        sweets[x, y].Init(x, y, this, type);
        return sweets[x, y];
    }

    public IEnumerator AllFill()
    {
        bool needFilled = true;
        while (needFilled)
        {
            yield return new WaitForSeconds(fillTime);
            while (Fill())
            {
                yield return new WaitForSeconds(fillTime);

            }
            needFilled = false;
        }
    }

    public bool Fill()
    {
        bool filledNotFinished = false;

        for (int y = rows - 2; y >= 0; --y)
        {
            for (int x = 0; x < columns; ++x)
            {
                GameSweet sweet = sweets[x, y];

                if (sweet.CanMove())
                {
                    GameSweet sweetBelow = sweets[x, y + 1];
                    if (sweetBelow.Type == SweetsType.EMPTY)
                    {
                        Destroy(sweetBelow.gameObject);
                        sweet.MovedComponent.Move(x, y + 1, fillTime);
                        sweets[x, y + 1] = sweet;
                        CreateNewSweet(x, y, SweetsType.EMPTY);
                        filledNotFinished = true;
                    }
                    else // 斜向填充
                    {
                        for (int down = -1; down <= 1; down += 2)
                        {
                            int downX = x + down;
                            if (downX >= 0 && downX < rows) {
                                GameSweet downSweet = sweets[downX, y + 1];
                                if (downSweet.Type == SweetsType.EMPTY)
                                {
                                    bool canFill = true;
                                    for (int aboveY = y; aboveY >= 0; --aboveY)
                                    {
                                        GameSweet sweetAbove = sweets[downX, aboveY];
                                        if (sweetAbove.CanMove())
                                        {
                                            break;
                                        }
                                        else if (!sweetAbove.CanMove() && sweetAbove.Type != SweetsType.EMPTY)
                                        {
                                            canFill = false;
                                            break;
                                        }
                                    }

                                    if (!canFill)
                                    {
                                        Destroy(downSweet.gameObject);
                                        sweet.MovedComponent.Move(downX, y + 1, fillTime);
                                        sweets[downX, y + 1] = sweet;
                                        CreateNewSweet(x, y, SweetsType.EMPTY);
                                        filledNotFinished = true;
                                        break;
                                    }
                                }
                            }

                        }
                    }
                }
            }
        }

        for (int x = 0; x < columns; ++x)
        {
            GameSweet sweet = sweets[x, 0];
            if (sweet.Type == SweetsType.EMPTY)
            {
                GameObject newSweet = Instantiate(sweetPrefabDictionary[SweetsType.NORMAL], CorrectPosition(x, -1), Quaternion.identity);
                newSweet.transform.parent = transform;

                sweets[x, 0] = newSweet.GetComponent<GameSweet>();
                sweets[x, 0].Init(x, -1, this, SweetsType.NORMAL);
                sweets[x, 0].MovedComponent.Move(x, 0, fillTime);
                sweets[x, 0].ColoredComponent.SetColor((ColorSweet.ColorType)Random.Range(0, sweets[x, 0].ColoredComponent.NumColors));
                filledNotFinished = true;
            }
        }

        return filledNotFinished;
    }
}
