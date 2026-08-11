using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum BrickSpawnMode
{
    InitialAllColors,
    ActivateByArrival
}


public class BrickSpawner : MonoBehaviour
{
    [Header("Spawn Mode")]
    [SerializeField]
    private BrickSpawnMode spawnMode =
        BrickSpawnMode.InitialAllColors;


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


    private List<Vector3> spawnSlots =
        new List<Vector3>();


    // Hangi rengin hangi slotlara ait olduğunu tutuyor.
    private Dictionary<TeamColor, List<int>> colorSlots =
        new Dictionary<TeamColor, List<int>>();


    // Middle alanda hangi renklerin daha önce
    // aktif edildiğini tutuyor.
    private HashSet<TeamColor> activatedColors =
        new HashSet<TeamColor>();


    private void Awake()
    {
        if (spawnArea == null)
        {
            spawnArea =
                GetComponent<BoxCollider>();
        }
    }


    private void Start()
    {
        CreateGrid();

        CreateColorSlotMap();


        // Start Island gibi alanlarda
        // bütün renkleri baştan oluştur.
        if (spawnMode ==
            BrickSpawnMode.InitialAllColors)
        {
            SpawnAllColors();
        }

        // ActivateByArrival ise burada hiçbir şey
        // spawn etmiyoruz.
        //
        // Middle başlangıçta tamamen boş kalacak.
    }


    private void CreateGrid()
    {
        spawnSlots.Clear();


        int totalBrickAmount =
            blueBrickAmount +
            redBrickAmount +
            greenBrickAmount +
            yellowBrickAmount;


        int rowCount =
            Mathf.CeilToInt(
                totalBrickAmount /
                (float)columns
            );


        Bounds bounds =
            spawnArea.bounds;


        float minX =
            bounds.min.x +
            edgeMargin;


        float maxX =
            bounds.max.x -
            edgeMargin;


        float minZ =
            bounds.min.z +
            edgeMargin;


        float maxZ =
            bounds.max.z -
            edgeMargin;


        float spawnY =
            transform.position.y +
            spawnHeightOffset;


        for (int row = 0;
             row < rowCount;
             row++)
        {
            for (int column = 0;
                 column < columns;
                 column++)
            {
                if (spawnSlots.Count >=
                    totalBrickAmount)
                {
                    return;
                }


                float xPercent =
                    (column + 0.5f) /
                    columns;


                float zPercent =
                    (row + 0.5f) /
                    rowCount;


                float spawnX =
                    Mathf.Lerp(
                        minX,
                        maxX,
                        xPercent
                    );


                float spawnZ =
                    Mathf.Lerp(
                        minZ,
                        maxZ,
                        zPercent
                    );


                spawnSlots.Add(
                    new Vector3(
                        spawnX,
                        spawnY,
                        spawnZ
                    )
                );
            }
        }
    }


    private void CreateColorSlotMap()
    {
        colorSlots.Clear();


        colorSlots.Add(
            TeamColor.Blue,
            new List<int>()
        );


        colorSlots.Add(
            TeamColor.Red,
            new List<int>()
        );


        colorSlots.Add(
            TeamColor.Green,
            new List<int>()
        );


        colorSlots.Add(
            TeamColor.Yellow,
            new List<int>()
        );


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


        // Random renk listesindeki her renk
        // kendi slot numarasını saklıyor.
        for (int i = 0;
             i < colors.Count;
             i++)
        {
            TeamColor color =
                colors[i];


            colorSlots[color].Add(i);
        }
    }


    private void SpawnAllColors()
    {
        SpawnColor(
            TeamColor.Blue
        );


        SpawnColor(
            TeamColor.Red
        );


        SpawnColor(
            TeamColor.Green
        );


        SpawnColor(
            TeamColor.Yellow
        );
    }


    private void SpawnColor(
        TeamColor color)
    {
        if (!colorSlots.ContainsKey(color))
        {
            return;
        }


        List<int> slots =
            colorSlots[color];


        foreach (int slotIndex in slots)
        {
            SpawnBrick(
                color,
                slotIndex
            );
        }
    }


    // MiddleRoomActivator burayı çağıracak.
    public void ActivateColor(
        TeamColor color)
    {
        // Bu spawner Arrival modunda değilse
        // ekstra spawn yapma.
        if (spawnMode !=
            BrickSpawnMode.ActivateByArrival)
        {
            return;
        }


        // Aynı renk daha önce aktif olduysa
        // tekrar bütün Brickleri spawn etme.
        if (activatedColors.Contains(color))
        {
            return;
        }


        activatedColors.Add(color);


        SpawnColor(color);


        Debug.Log(
            color +
            " Middle Brick spawn aktif oldu."
        );
    }


    private void AddColorsToList(
        List<TeamColor> colors,
        TeamColor color,
        int amount)
    {
        for (int i = 0;
             i < amount;
             i++)
        {
            colors.Add(color);
        }
    }


    private void ShuffleColors(
        List<TeamColor> colors)
    {
        for (int i = 0;
             i < colors.Count;
             i++)
        {
            int randomIndex =
                Random.Range(
                    i,
                    colors.Count
                );


            TeamColor temp =
                colors[i];


            colors[i] =
                colors[randomIndex];


            colors[randomIndex] =
                temp;
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
            brickPoolManager.GetBrickFromPool(
                color
            );


        if (brick == null)
        {
            return;
        }


        brick.transform.SetParent(
            transform
        );


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