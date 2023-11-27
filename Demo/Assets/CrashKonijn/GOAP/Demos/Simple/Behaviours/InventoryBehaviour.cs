﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CrashKonijn.Goap.Demos.Simple.Behaviours
{
    public class InventoryBehaviour : MonoBehaviour
    {
        public List<AppleBehaviour> Apples = new ();

        public void Put(AppleBehaviour apple)
        {
            if (this.Apples.Contains(apple))
                return;
            
            apple.PickUp();
            this.Apples.Add(apple);
        }

        public AppleBehaviour Hold()
        {
            var apple = this.Apples.FirstOrDefault();

            if (apple == null)
                return null;
            
            return apple;
        }

        public void Drop(AppleBehaviour apple)
        {
            if (!this.Apples.Contains(apple))
                return;
            
            this.Apples.Remove(apple);
            apple.Drop();
        }
    }
}