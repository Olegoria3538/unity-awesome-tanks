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
        private int kek = 0;

        public IEnumerator Think(Player player)
        {
            var whereToMove = new List<Vector2Int>()
            {
                Vector2Int.left, Vector2Int.right, Vector2Int.up, Vector2Int.down
            };
            IsPlayerFront(player);
            yield return TryMove(whereToMove[UnityEngine.Random.Range(0, whereToMove.Count)]);
        }

        private void IsPlayerFront(Player player)
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
					IsPlayerFire(i, y);
                }
            }
            if (dX == -1)
            {
                for (var i = x; i >= 0; i--)
                {
                    IsPlayerFire(i, y);
                }
            }
            if (dY == 1)
            {
                for (var i = y; i < cells.GetLength(1); i++)
                {
                    IsPlayerFire(x, i);
                }
            }
            if (dY == -1)
            {
                for (var i = y; i >= 0; i--)
                {
                    IsPlayerFire(x, i);
                }
            }
        }

        private void IsPlayerFire(int x, int y)
        {
            if (cells[x, y].Occupant != null)
            {
                if (cells[x, y].Occupant.GetComponent<Player>() != null)
                {
                    Fire();
                }
            }
        }
    }
}
