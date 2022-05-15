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

        public IEnumerator Think()
        {
            var whereToMove = new List<Vector2Int>()
            {
                Vector2Int.left, Vector2Int.right, Vector2Int.up, Vector2Int.down
            };
            IsPlayerFront();
            yield return TryMove(whereToMove[UnityEngine.Random.Range(0, whereToMove.Count)]);
        }

        /* Checking if the player or flag is in front of the enemy. */
        private void IsPlayerFront()
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
    }
}
