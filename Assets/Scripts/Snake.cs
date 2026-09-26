using CodeMonkey;
using CodeMonkey.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Snake : MonoBehaviour
{
    // Olay Dinleyicileri (UI ve Skor için)
    public event EventHandler OnDied;
    public event EventHandler OnAteFood;

    private enum State
    {
        Alive,
        Dead
    }

    private State state;
    private Vector2Int gridMoveDirection;
    private Vector2Int gridPosition;
    private float gridMoveTimer;
    private float gridMoveTimerMax;
    private LevelGrid levelGrid;
    private int snakeBodySize;
    private List<Vector2Int> snakeMovePositionList;
    private List<Transform> snakeBodyTransformList;
    private float targetAngle;
    [SerializeField] private float rotationSpeed = 25f;

    public void Setup(LevelGrid levelGrid)
    {
        this.levelGrid = levelGrid;
    }

    private void Awake()
    {
        state = State.Alive;
        gridPosition = new Vector2Int(10, 10);
        gridMoveTimerMax = 0.25f; // Yılanın hızı (küçüldükçe hızlanır)
        gridMoveTimer = gridMoveTimerMax;
        gridMoveDirection = new Vector2Int(1, 0); // Başlangıçta sağa gider

        snakeMovePositionList = new List<Vector2Int>();
        snakeBodySize = 0;
        snakeBodyTransformList = new List<Transform>();
    }

    private void Update()
    {
        switch (state)
        {
            case State.Alive:
                HandleInput();
                HandleGridMovement();
                HandleRotation();
                break;

            case State.Dead:
                // Öldüğünde hiçbir hareket veya giriş kabul edilmez
                break;
        }
    }

    private void HandleInput()
    {
        // Doğrudan yön ataması yapılarak dash / katlanarak artma bug'ı engellendi
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (gridMoveDirection.y != -1) gridMoveDirection = new Vector2Int(0, 1);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (gridMoveDirection.y != 1) gridMoveDirection = new Vector2Int(0, -1);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (gridMoveDirection.x != 1) gridMoveDirection = new Vector2Int(-1, 0);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (gridMoveDirection.x != -1) gridMoveDirection = new Vector2Int(1, 0);
        }
    }

    private void HandleGridMovement()
    {
        gridMoveTimer += Time.deltaTime;

        if (gridMoveTimer >= gridMoveTimerMax)
        {
            gridMoveTimer -= gridMoveTimerMax;

            
            Vector2Int nextGridPosition = gridPosition + gridMoveDirection;

            
            if (levelGrid != null)
            {
                nextGridPosition = levelGrid.ValidateGridPosition(nextGridPosition);
            }

           
            for (int i = 0; i < snakeMovePositionList.Count; i++)
            {
                if (nextGridPosition == snakeMovePositionList[i])
                {
                    
                    targetAngle = GetAngleFromVector(gridMoveDirection);
                    transform.eulerAngles = new Vector3(0, 0, targetAngle);

                    state = State.Dead;
                    Debug.Log("GAME OVER! Yılan kendine çarptı.");

                    if (OnDied != null) OnDied(this, EventArgs.Empty);

                    if (GameOverWindow.Instance != null)
                    {
                        GameOverWindow.Instance.Show();
                    }

                    return; 
                }
            }

            
            snakeMovePositionList.Insert(0, gridPosition);
            gridPosition = nextGridPosition;

            transform.position = new Vector3(gridPosition.x, gridPosition.y);
            targetAngle = GetAngleFromVector(gridMoveDirection);

            
            bool snakeAteFood = levelGrid.TrySnakeAteFood(gridPosition);
            if (snakeAteFood)
            {
                snakeBodySize++;
                CreateSnakeBody();

                if (OnAteFood != null) OnAteFood(this, EventArgs.Empty);
            }

           
            if (snakeMovePositionList.Count > snakeBodySize)
            {
                snakeMovePositionList.RemoveAt(snakeMovePositionList.Count - 1);
            }

            
            for (int i = 0; i < snakeBodyTransformList.Count; i++)
            {
                Vector2Int currentPos = snakeMovePositionList[i];
                snakeBodyTransformList[i].position = new Vector3(currentPos.x, currentPos.y);

                Vector2Int frontPos = (i == 0) ? gridPosition : snakeMovePositionList[i - 1];
                Vector2Int dirToFront = frontPos - currentPos;

                if (Mathf.Abs(dirToFront.x) > 1 || Mathf.Abs(dirToFront.y) > 1)
                {
                    continue;
                }

                float angle = GetAngleFromVector(dirToFront);

                if (i < snakeMovePositionList.Count - 1)
                {
                    Vector2Int backPos = snakeMovePositionList[i + 1];
                    Vector2Int dirFromBack = currentPos - backPos;

                    if (Mathf.Abs(dirFromBack.x) <= 1 && Mathf.Abs(dirFromBack.y) <= 1)
                    {
                        if (dirToFront != dirFromBack)
                        {
                            Vector2 avgDir = ((Vector2)dirToFront + (Vector2)dirFromBack).normalized;
                            angle = -Mathf.Atan2(avgDir.x, avgDir.y) * Mathf.Rad2Deg;
                            if (angle < 0) angle += 360;
                        }
                    }
                }

                snakeBodyTransformList[i].eulerAngles = new Vector3(0, 0, angle);
            }
        }
    }

    private void CreateSnakeBody()
    {
        GameObject snakeBodyGameObject = new GameObject("SnakeBody", typeof(SpriteRenderer));
        snakeBodyGameObject.GetComponent<SpriteRenderer>().sprite = GameAssets.i.snakeBodySprite;
        snakeBodyTransformList.Add(snakeBodyGameObject.transform);
    }

    private float GetAngleFromVector(Vector2Int dir)
    {
        float n = -Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg;
        if (n < 0) n += 360;
        return n;
    }

    private void HandleRotation()
    {
        float currentAngle = transform.eulerAngles.z;
        float smoothAngle = Mathf.LerpAngle(currentAngle, targetAngle, Time.deltaTime * rotationSpeed);
        transform.eulerAngles = new Vector3(0, 0, smoothAngle);
    }

    public Vector2Int getGridPosition()
    {
        return gridPosition;
    }

    public List<Vector2Int> getFullSnakeGridPositionList()
    {
        List<Vector2Int> gridPositionList = new List<Vector2Int>() { gridPosition };
        gridPositionList.AddRange(snakeMovePositionList);
        return gridPositionList;
    }
}