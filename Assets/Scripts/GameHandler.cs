using UnityEngine;
using CodeMonkey;
using CodeMonkey.Utils;
using System.Globalization;

public class GameHandler : MonoBehaviour
{

    [SerializeField] private Snake snake;
   private  LevelGrid levelGrid;
    void Start()
    {
        Debug.Log("Game Handler started");

        levelGrid = new LevelGrid(20, 20);

        snake.Setup(levelGrid);

        levelGrid.Setup(snake);
    }


}
