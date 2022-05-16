using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Logic
{
    internal class Player : Tank
    {
        /// <summary>
        /// The function is called when the player kills an enemy. It increments the number of kills by one
        /// </summary>
        public override void Killing(Tank occupant)
        {
            base.Killing(occupant);
            fieldController.incrementKilling();
        }
    }
}
