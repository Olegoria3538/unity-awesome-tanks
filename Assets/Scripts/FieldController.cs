using Assets.Scripts.Logic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldController : MonoBehaviour
{
    public GroundChessBoard bedrockVoxel;
    public GameObject destructibleVoxel;
    public GameObject flagVoxel;
    public GameObject playerPrefab;
    public GameObject enemyPrefab;
    public GameObject mainCamera;

    public float playerSpeed = 3;
    public float enemySpeed = 2;

    private Cell[,] cells;
    private int width;
    private int height;
    private Player player;
    
    private Vector2Int enemyCellSpawn;

    private Vector2Int playerCellSpawn;

    private int countSpawnEnemy = 0;

    private string[] map = new[] {
        "P...#F#...",
        "....###...",
        "..........",
        "....##....",
        "...####...",
        "...####...",
        "...####...",
        "....##....",
        "..........",
        ".....E...."
    };

    private Dictionary<char, CellSpace> cellVoxel = new Dictionary<char, CellSpace>()
    {
        { '$', CellSpace.Bedrock },
        { '#', CellSpace.Destructible },
        { 'F', CellSpace.Flag },
        { 'E', CellSpace.Empty },
        { 'P', CellSpace.Empty },
        { '.', CellSpace.Empty }
    };

    void Start()
    {
        height = map.Length;
        width = map[0].Length;

        cells = new Cell[width + 2, height + 2];

        for (int i = 0; i < height; i++)
        {
            if (map[i].Length != width)
            {
                throw new System.Exception("Invalid map");
            }
            for (int j = 0; j < width; j++)
            {
                cells[j + 1, i + 1] = new Cell(cellVoxel[map[i][j]]);
                if (map[i][j] == 'P')
                {
                    playerCellSpawn = new Vector2Int(j + 1, i + 1);
                    SpawnPlayer();
                }
                if (map[i][j] == 'E')
                {
                    enemyCellSpawn = new Vector2Int(j + 1, i + 1);
                }
            }
        }

        for (int i = 0; i < width + 2; i++)
        {
            cells[i, 0] = new Cell(CellSpace.Bedrock);
            cells[i, height + 1] = new Cell(CellSpace.Bedrock);
        }

        for (int i = 0; i < height + 2; i++)
        {
            cells[0, i] = new Cell(CellSpace.Bedrock);
            cells[width + 1, i] = new Cell(CellSpace.Bedrock);
        }

        for (var x = 0; x < width + 2; x++)
        {
            for (var y = 0; y < height + 2; y++)
            {
                var c = Instantiate(bedrockVoxel, new Vector3(x, 0, y), Quaternion.identity, transform);
                c.SetColor((x + y) % 2 == 0);
                if (cells[x, y].Space == CellSpace.Bedrock)
				{
                    var bedrock = Instantiate(bedrockVoxel, new Vector3(x, 1, y), Quaternion.identity, transform);
                    cells[x, y].setVoxel(bedrock.GetComponent<GameObject>());
                }
                if(cells[x, y].Space == CellSpace.Destructible) {
                    var destructible = Instantiate(destructibleVoxel, new Vector3(x, 1, y), Quaternion.identity, transform);
                    cells[x, y].setVoxel(destructible);
                }
                if(cells[x, y].Space == CellSpace.Flag) {
                    var flag = Instantiate(flagVoxel, new Vector3(x, 1, y), Quaternion.identity, transform);
                    cells[x, y].setVoxel(flag);
                }
            }
        }

        mainCamera.transform.position = new Vector3((width + 2) / 2, 13,(height + 2) / 2);
        mainCamera.transform.eulerAngles = new Vector3(90, 0, 0);
        InvokeRepeating("SpawnEnemy", 0f, 1f);
    }

    // Update is called once per frame
    void Update()
    {
		if (Input.GetKeyDown(KeyCode.RightArrow))
		{
			StartCoroutine(player.TryMove(Vector2Int.right));
		}
		if (Input.GetKeyDown(KeyCode.LeftArrow))
		{
			StartCoroutine(player.TryMove(Vector2Int.left));
		}
		if (Input.GetKeyDown(KeyCode.UpArrow))
		{
			StartCoroutine(player.TryMove(Vector2Int.up));
		}
		if (Input.GetKeyDown(KeyCode.DownArrow))
		{
			StartCoroutine(player.TryMove(Vector2Int.down));
		}
        if (Input.GetKeyDown(KeyCode.Space))
        {
            player.Fire();
        }

        for (var x = 0; x < cells.GetLength(0); x++)
        {
            for (var y = 0; y < cells.GetLength(1); y++)
            {
                if (cells[x, y].Occupant is EnemyAI enemy)
                {
                    enemy.StartCoroutine(enemy.Think());
                }
            }
        }
    }

    void SpawnEnemy()
    {   
        var x = enemyCellSpawn[0];
        var y = enemyCellSpawn[1];
        if(cells[x, y].Occupant != null) {
            return ;
        }
        var enemyGO = Instantiate(enemyPrefab, new Vector3(x, 1, y), Quaternion.identity, transform);
        var e = enemyGO.GetComponent<EnemyAI>();
        e.Initialize(enemySpeed, cells);
        cells[x, y].Occupy(e);
        countSpawnEnemy++;
        if(countSpawnEnemy >= 0)
        {
            CancelInvoke("SpawnEnemy");
        }
    }

    void SpawnPlayer()
    {
        var x = playerCellSpawn[0];
        var y = playerCellSpawn[1];
        var playerGO = Instantiate(playerPrefab, new Vector3(x, 1, y), Quaternion.identity, transform);
        player = playerGO.GetComponent<Player>();
        player.Initialize(playerSpeed, cells);
        cells[x, y].Occupy(player);
    }
}
