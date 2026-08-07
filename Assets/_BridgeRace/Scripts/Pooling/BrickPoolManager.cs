using System.Collections.Generic;
using UnityEngine;

public class BrickPoolManager : MonoBehaviour
{
    [Header("Brick Prefabs")]
    [SerializeField] private Brick blueBrickPrefab;
    [SerializeField] private Brick redBrickPrefab;
    [SerializeField] private Brick greenBrickPrefab;
    [SerializeField] private Brick yellowBrickPrefab;

    [Header("Pool Settings")]
    [SerializeField] private int poolSizePerColor = 60;

    [SerializeField] private Transform poolParent;


    private Dictionary<TeamColor, Queue<Brick>> brickPools;


    private void Awake()
    {
        brickPools =
            new Dictionary<TeamColor, Queue<Brick>>();

        CreatePool(
            TeamColor.Blue,
            blueBrickPrefab
        );

        CreatePool(
            TeamColor.Red,
            redBrickPrefab
        );

        CreatePool(
            TeamColor.Green,
            greenBrickPrefab
        );

        CreatePool(
            TeamColor.Yellow,
            yellowBrickPrefab
        );
    }


    private void CreatePool(
        TeamColor color,
        Brick brickPrefab)
    {
        if (brickPrefab == null)
        {
            Debug.LogError(
                color + " brick prefab atanmadı!"
            );

            return;
        }


        Queue<Brick> newPool =
            new Queue<Brick>();


        for (int i = 0; i < poolSizePerColor; i++)
        {
            Brick newBrick = Instantiate(
                brickPrefab,
                poolParent
            );

            newBrick.gameObject.SetActive(false);

            newPool.Enqueue(newBrick);
        }


        brickPools.Add(
            color,
            newPool
        );
    }


    public Brick GetBrickFromPool(
        TeamColor color)
    {
        if (!brickPools.ContainsKey(color))
        {
            Debug.LogError(
                color + " için Brick Pool bulunamadı!"
            );

            return null;
        }


        Queue<Brick> selectedPool =
            brickPools[color];


        if (selectedPool.Count <= 0)
        {
            Debug.LogWarning(
                color + " Brick Pool boş!"
            );

            return null;
        }


        Brick brick =
            selectedPool.Dequeue();

        brick.gameObject.SetActive(true);

        return brick;
    }


    public void ReturnBrickToPool(
        Brick brick)
    {
        if (brick == null)
        {
            return;
        }


        TeamColor brickColor =
            brick.CollectableColor;


        if (!brickPools.ContainsKey(brickColor))
        {
            Debug.LogError(
                brickColor +
                " için Brick Pool bulunamadı!"
            );

            return;
        }


        brick.transform.SetParent(poolParent);

        brick.gameObject.SetActive(false);


        brickPools[brickColor].Enqueue(brick);
    }
}