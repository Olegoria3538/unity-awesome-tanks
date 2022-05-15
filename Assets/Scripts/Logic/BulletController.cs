using Assets.Scripts.Logic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

internal class BulletController : MonoBehaviour
{
    private Vector2Int direction;
    public float speed = 10;
    public GameObject boomFx;
    private Cell[,] cells;

    public Tank selfTank;

    public void Initialize(Cell[,] cells, Tank selfTank)
    {
        this.cells = cells;
        this.selfTank = selfTank;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.position += new Vector3(direction.x, 0, direction.y) * speed * Time.deltaTime;

        var ourCell = new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z));
        if (cells[ourCell.x, ourCell.y].Space != CellSpace.Empty)
        {
            Destroy(gameObject);
            if (cells[ourCell.x, ourCell.y].Space == CellSpace.Destructible ||
                cells[ourCell.x, ourCell.y].Space == CellSpace.Flag)
            {
                Boooooom();
                Destroy(cells[ourCell.x, ourCell.y].Voxel);
                cells[ourCell.x, ourCell.y].SetCell(CellSpace.Empty);
            }
        }
        if (cells[ourCell.x, ourCell.y].Occupant != null)
        {
            var tank = cells[ourCell.x, ourCell.y].Occupant.GetComponent<Tank>();
            if (tank != null && tank != selfTank)
            {
                cells[ourCell.x, ourCell.y].SetCell(CellSpace.Empty);
                cells[ourCell.x, ourCell.y].Occupy(null);
                Boooooom();
                tank.Die();
                Destroy(gameObject);
                selfTank.incrementKilling();
            }
        }
    }

    /// <summary>
    /// Instantiates a particle system at the position of the object that has this script
    /// attached to it
    /// </summary>
    void Boooooom()
    {
        var fx = boomFx.GetComponentInChildren<ParticleSystem>();
        Instantiate(fx, transform.position, Quaternion.identity);
        fx.Play();
    }

    public void Fire(Vector2Int direction)
    {
        this.direction = direction;
    }
}
