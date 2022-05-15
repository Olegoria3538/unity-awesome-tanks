using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Logic
{
	internal enum CellSpace
	{
		Empty,
		Bedrock,
		Destructible
	}

	internal class Cell
	{
		public CellSpace Space { get; private set; }
		public Tank Occupant;
		public UnityEngine.GameObject Voxel;


		public Cell(CellSpace space)
		{
			Space = space;
		}

		public void Occupy(Tank occupant)
		{
			Occupant = occupant;
		}
		public void setVoxel(UnityEngine.GameObject voxel)
		{
			Voxel = voxel;
		}

		public void SetCell(CellSpace space) {
			Space = space;
		}
	}
}
