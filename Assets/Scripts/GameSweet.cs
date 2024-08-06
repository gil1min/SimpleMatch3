using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSweet : MonoBehaviour
{
  
    private int x;
    public int X {
        get
        {
            return x;
        }
        set
        {
            if (CanMove())
            {
                x = value;
            }
        }
    }

    private int y;
    public int Y
    {
        get
        {
            return y;
        }
        set
        {
            if (CanMove())
            {
                y = value;
            }
        }
    }

    private GameManager.SweetsType type;

    public GameManager.SweetsType Type { get => type; }
    [HideInInspector]
    public GameManager gameManager;

    public MovedSweet MovedComponent { get => movedComponent; }
    private MovedSweet movedComponent;

    private ColorSweet coloredComponent;
    public ColorSweet ColoredComponent
    {
        get
        {
            return coloredComponent;
        }
    }

    public void Init(int _x, int _y, GameManager _gameManager, GameManager.SweetsType _type)
    {
        x = _x;
        y = _y;
        gameManager = _gameManager;
        type = _type;
    }

    public bool CanMove()
    {
        return movedComponent != null;
    }

    public bool CanColor()
    {
        return coloredComponent != null;
    }

    private void Awake()
    {
        movedComponent = GetComponent<MovedSweet>();
        coloredComponent = GetComponent<ColorSweet>();
    }

}
