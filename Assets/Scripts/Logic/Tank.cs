using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Logic
{
    internal class Tank : MonoBehaviour
    {
        public bool isMoving { get; private set; }
        public BulletController bulletPrefab;
        private float moveSpeed;
        public Cell[,] cells;
        public int countKilling = 0;
        public Vector2Int direction = Vector2Int.up;
        public FieldController fieldController;

        public void Initialize(float moveSpeed, Cell[,] cells, FieldController FieldController)
        {
            this.moveSpeed = moveSpeed;
            this.cells = cells;
            fieldController = FieldController;
        }

        public IEnumerator TryMove(Vector2Int delta)
        {
            if (isMoving)
            {
                yield break;
            }

            isMoving = true;

            var rotationY = Vector2.SignedAngle(Vector2.up, delta * new Vector2Int(-1, 1));
            var from = GetCoords();
            var tc = from + delta;

            var targetCell = cells[tc.x, tc.y];
            if (targetCell.Occupant == null && targetCell.Space == CellSpace.Empty)
            {
                var oldDirection = direction;
                var oldX = oldDirection[0];
                direction = delta;
                var newX = direction[0];
                cells[tc.x, tc.y].Occupy(this);
                cells[from.x, from.y].Occupy(null);
                var currentPosition = new Vector3(from.x, 1, from.y);
                var targetPosition = new Vector3(tc.x, 1, tc.y);

                var currentRotation = gameObject.transform.eulerAngles;
                var targetRotation = new Vector3(0, rotationY, 0);

                var moveTime = 1f / moveSpeed;
                float t = 0;
                while (t < moveTime)
                {
                    t += Time.deltaTime;
                    gameObject.transform.position = currentPosition + (t / moveTime) * (targetPosition - currentPosition);
                    var f = Mathf.Min(1, 2 * t / moveTime);
                    if (!(oldX == -1 && newX == -1))
                    {
                        gameObject.transform.eulerAngles = currentRotation + f * (targetRotation - currentRotation);
                    }
                    yield return null;
                }
                gameObject.transform.position = targetPosition;
                gameObject.transform.eulerAngles = targetRotation;
            }

            isMoving = false;
        }

        public Vector2Int GetCoords()
        {
            Vector2Int p = default;
            for (var x = 0; x < cells.GetLength(0); x++)
            {
                for (var y = 0; y < cells.GetLength(1); y++)
                {
                    if (cells[x, y].Occupant == this)
                    {
                        p = new Vector2Int(x, y);
                    }
                }
            }
            return p;
        }

        public void Fire()
        {

            var bullet = Instantiate(bulletPrefab, transform.position, transform.rotation);
            bullet.Initialize(cells, this);
            bullet.Fire(direction);

        }

        public void Die()
        {
            StopAllCoroutines();
            var p = GetCoords();
            cells[p.x, p.y].Occupy(null);
            Destroy(gameObject);
        }

        public virtual void incrementKilling() {
            countKilling++;
        }
    }
}
