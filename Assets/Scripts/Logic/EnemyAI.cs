using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Logic
{
    internal class EnemyAI : Tank
    {
        private bool enableFire = true;


        public void Think()
        {
            var whereToMove = new List<Vector2Int>()
            {
                Vector2Int.left, Vector2Int.right, Vector2Int.up, Vector2Int.down
            };
            FireIsPlayerFront();
            var playerCoo = getTargetCoo();
            int[,] cMap = findWave(playerCoo);
            // yield return move(cMap, playerCoo);
            StartCoroutine(move(cMap, playerCoo));
            // yield return TryMove(whereToMove[UnityEngine.Random.Range(0, whereToMove.Count)]);
        }

        /// <summary>
        /// Checking if the player or flag is in front of the enemy and fire;
        /// </summary>
        private void FireIsPlayerFront()
        {
            var coo = GetCoords();
            var x = coo[0];
            var y = coo[1];
            var dX = direction[0];
            var dY = direction[1];
            if (dX == 1)
            {
                for (var i = x; i < cells.GetLength(0); i++)
                {
                    StartCoroutine(FireWait(i, y));
                }
            }
            if (dX == -1)
            {
                for (var i = x; i >= 0; i--)
                {
                    StartCoroutine(FireWait(i, y));
                }
            }
            if (dY == 1)
            {
                for (var i = y; i < cells.GetLength(1); i++)
                {
                    StartCoroutine(FireWait(x, i));
                }
            }
            if (dY == -1)
            {
                for (var i = y; i >= 0; i--)
                {
                    StartCoroutine(FireWait(x, i));
                }
            }
        }

        private IEnumerator FireWait(int x, int y)
        {
            var flag =
                enableFire &&
                (((cells[x, y].Occupant != null) && (cells[x, y].Occupant.GetComponent<Player>() != null))
                || cells[x, y].Space == CellSpace.Flag);

            if (flag)
            {
                enableFire = false;
                Fire();
                yield return new WaitForSeconds(15);
                enableFire = true;
            }
            yield break;
        }

        //Ищем путь к таргет координатам
        public int[,] findWave(Vector2Int targetCoo)
        {
            var targetX = targetCoo[0];
            var targetY = targetCoo[1];
            bool add = true;
            // условие выхода из цикла
            // делаем копию карты локации, для дальнейшей ее разметки
            var cMap = GetWallMap();
            var cMap2 = GetWallMap();

            var currentPosition = GetCoords();

            int x, y, step = 0; // значение шага равно 0
            for (x = 0; x < cells.GetLength(0); x++)
            {
                for (y = 0; y < cells.GetLength(1); y++)
                {
                    if (cMap2[x, y] == 1)
                        cMap[x, y] = -2; //если ячейка равна 1, то это стена (пишим -2)
                    else cMap[x, y] = -1; //иначе еще не ступали сюда
                }
            }

            //начинаем отсчет с финиша, так будет удобней востанавливать путь
            cMap[targetX, targetY] = 0;
            while (add == true)
            {
                add = false;
                for (x = 0; x < cells.GetLength(0); x++)
                {
                    for (y = 0; y < cells.GetLength(1); y++)
                    {
                        if (cMap[x, y] == step)
                        {
                            // если соседняя клетка не стена, и если она еще не помечена
                            // то помечаем ее значением шага + 1
                            if (y - 1 >= 0 && cMap[x, y - 1] != -2 && cMap[x, y - 1] == -1)
                                cMap[x, y - 1] = step + 1;
                            if (x - 1 >= 0 && cMap[x - 1, y] != -2 && cMap[x - 1, y] == -1)
                                cMap[x - 1, y] = step + 1;
                            if (y + 1 >= 0 && cMap[x, y + 1] != -2 && cMap[x, y + 1] == -1)
                                cMap[x, y + 1] = step + 1;
                            if (x + 1 >= 0 && cMap[x + 1, y] != -2 && cMap[x + 1, y] == -1)
                                cMap[x + 1, y] = step + 1;
                        }
                    }
                }
                step++;
                add = true;
                if (cMap[currentPosition[0], currentPosition[1]] > 0) //решение найдено
                    add = false;
                if (step > cells.GetLength(0) * cells.GetLength(1)) //решение не найдено, если шагов больше чем клеток
                    add = false;
            }
            return cMap; // возвращаем помеченную матрицу, для востановления пути в методе move()
        }

        /// <summary>
        /// РЕАЛИЗАЦИЯ ВОЛНОВОГО АЛГОРИТМА
        ///	</summary>
        private IEnumerator move(int[,] cMap, Vector2Int targetCoo)
        {
            var targetX = targetCoo[0];
            var targetY = targetCoo[1];
            int[] neighbors = new int[4];

            var currentPosition = GetCoords();
            var direction = new Vector2Int(0, 0);

            neighbors[0] = cMap[currentPosition.x, currentPosition.y + 1];
            neighbors[1] = cMap[currentPosition.x - 1, currentPosition.y];
            neighbors[2] = cMap[currentPosition.x, currentPosition.y - 1];
            neighbors[3] = cMap[currentPosition.x + 1, currentPosition.y];
            if (neighbors.Max() > 0)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (neighbors[i] < 0)
                        neighbors[i] = 99999;
                }

                int minIndex = Array.IndexOf(neighbors, neighbors.Min());
                if (minIndex == 0)
                    direction = new Vector2Int(0, 1);
                if (minIndex == 1)
                    direction = new Vector2Int(-1, 0);
                if (minIndex == 2)
                    direction = new Vector2Int(0, -1);
                if (minIndex == 3)
                    direction = new Vector2Int(1, 0);
            }
            yield return TryMove(direction);
        }

        private int[,] GetWallMap()
        {
            int[,] cMap = new int[cells.GetLength(0), cells.GetLength(1)];
            for (var x = 0; x < cells.GetLength(0); x++)
            {
                for (var y = 0; y < cells.GetLength(1); y++)
                {
                    if (cells[x, y].Space == CellSpace.Bedrock)
                        cMap[x, y] = 1;
                    else if (cells[x, y].Space == CellSpace.Destructible)
                        cMap[x, y] = 1;
                    else if (cells[x, y].Occupant != null)
                        cMap[x, y] = 1;
                    else
                        cMap[x, y] = 0;
                }
            }
            return cMap;
        }

        private Vector2Int getTargetCoo()
        {
            var currentPosition = GetCoords();
            Vector2Int flagCoo = default;
            Vector2Int playerCoo = default;
            for (var x = 0; x < cells.GetLength(0); x++)
            {
                for (var y = 0; y < cells.GetLength(1); y++)
                {
                    if (cells[x, y].Space == CellSpace.Flag)
                    {
                        flagCoo = new Vector2Int(x, y);
                    }
                    if (cells[x, y].Occupant != null && cells[x, y].Occupant.GetComponent<Player>() != null)
                    {
                        playerCoo = new Vector2Int(x, y);
                    }
                }
            }
            float[] kek = new float[] {
                (flagCoo - currentPosition).magnitude,
                (playerCoo - currentPosition).magnitude
            };
            
            int minIndex = Array.IndexOf(kek, kek.Min());
            if (minIndex == 0) return flagCoo;
            else return playerCoo;
        }
    }
}
