using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BrickPoolManager brickPoolManager;
    [SerializeField] private BoxCollider spawnArea;

    [Header("Brick Amounts")]
    [SerializeField] private int blueBrickAmount = 12;
    [SerializeField] private int redBrickAmount = 12;
    [SerializeField] private int greenBrickAmount = 12;
    [SerializeField] private int yellowBrickAmount = 12;

    [Header("Grid Settings")]
    [SerializeField] private int columns = 8;
    [SerializeField] private float edgeMargin = 1f;
    [SerializeField] private bool randomizeColors = true;

    [Header("Spawn Settings")]
    [SerializeField] private float respawnDelay = 2.5f;
    [SerializeField] private float spawnHeightOffset = 0.05f;

    private List<Vector3> spawnSlots = new List<Vector3>();


    private void Awake()
    {
        if (spawnArea == null)
        {
            spawnArea = GetComponent<BoxCollider>();
        }
    }


    private void Start()
    {
        CreateGrid();
        SpawnInitialBricks();
    }


    private void CreateGrid()
    {
        spawnSlots.Clear();

        int totalBrickAmount =
            blueBrickAmount +
            redBrickAmount +
            greenBrickAmount +
            yellowBrickAmount;


        int rowCount = Mathf.CeilToInt(
            totalBrickAmount / (float)columns
        );


        Bounds bounds = spawnArea.bounds;


        float minX = bounds.min.x + edgeMargin;
        float maxX = bounds.max.x - edgeMargin;

        float minZ = bounds.min.z + edgeMargin;
        float maxZ = bounds.max.z - edgeMargin;


        float spawnY =
            transform.position.y +
            spawnHeightOffset;


        for (int row = 0; row < rowCount; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                if (spawnSlots.Count >= totalBrickAmount)
                {
                    return;
                }


                float xPercent =
                    (column + 0.5f) / columns;

                float zPercent =
                    (row + 0.5f) / rowCount;


                float spawnX = Mathf.Lerp(
                    minX,
                    maxX,
                    xPercent
                );

                float spawnZ = Mathf.Lerp(
                    minZ,
                    maxZ,
                    zPercent
                );


                Vector3 spawnPosition = new Vector3(
                    spawnX,
                    spawnY,
                    spawnZ
                );


                spawnSlots.Add(spawnPosition);
            }
        }
    }


    private void SpawnInitialBricks()
    {
        List<TeamColor> colors =
            new List<TeamColor>();


        AddColorsToList(
            colors,
            TeamColor.Blue,
            blueBrickAmount
        );

        AddColorsToList(
            colors,
            TeamColor.Red,
            redBrickAmount
        );

        AddColorsToList(
            colors,
            TeamColor.Green,
            greenBrickAmount
        );

        AddColorsToList(
            colors,
            TeamColor.Yellow,
            yellowBrickAmount
        );


        if (randomizeColors)
        {
            ShuffleColors(colors);
        }


        for (int i = 0; i < colors.Count; i++)
        {
            SpawnBrick(
                colors[i],
                i
            );
        }
    }


    private void AddColorsToList(
        List<TeamColor> colors,
        TeamColor color,
        int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            colors.Add(color);
        }
    }


    private void ShuffleColors(
        List<TeamColor> colors)
    {
        for (int i = 0; i < colors.Count; i++)
        {
            int randomIndex = Random.Range(
                i,
                colors.Count
            );


            TeamColor temp = colors[i];

            colors[i] = colors[randomIndex];

            colors[randomIndex] = temp;
        }
    }


    private void SpawnBrick(
        TeamColor color,
        int slotIndex)
    {
        if (brickPoolManager == null)
        {
            Debug.LogError(
                gameObject.name +
                " üzerinde BrickPoolManager atanmadı!"
            );

            return;
        }


        if (slotIndex < 0 ||
            slotIndex >= spawnSlots.Count)
        {
            Debug.LogError(
                "Geçersiz Brick Slot Index: " +
                slotIndex
            );

            return;
        }


        Brick brick =
            brickPoolManager.GetBrickFromPool(color);


        if (brick == null)
        {
            return;
        }


        brick.transform.SetParent(transform);

        brick.transform.position =
            spawnSlots[slotIndex];

        brick.transform.rotation =
            Quaternion.identity;


        brick.SetSpawner(
            this,
            slotIndex
        );
    }


    public void BrickCollected(
        TeamColor color,
        int slotIndex)
    {
        StartCoroutine(
            RespawnBrick(
                color,
                slotIndex
            )
        );
    }


    private IEnumerator RespawnBrick(
        TeamColor color,
        int slotIndex)
    {
        yield return new WaitForSeconds(
            respawnDelay
        );


        SpawnBrick(
            color,
            slotIndex
        );
    }
}