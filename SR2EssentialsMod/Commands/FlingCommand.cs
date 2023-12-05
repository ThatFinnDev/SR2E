using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SR2E.Commands
{
    internal class FlingCommand : SR2CCommand
    {
        public override string ID => "fling";

        public override string Usage => "fling <strength>";

        public override string Description => "Flings any object you are looking at.";

        public override bool Execute(string[] args)
        {
            try
            {
                if (Physics.Raycast(new Ray(Camera.main.transform.position, Camera.main.transform.forward), out var hit))
                {
                    var transform = hit.transform;
                    var rb = hit.rigidbody;
                    var moveSpeed = float.Parse(args[0]);


                    Vector3 cameraPosition = Camera.main.transform.position;

                    Vector3 moveDirection = transform.position - cameraPosition;

                    moveDirection.Normalize();

                    rb.velocity += (moveDirection * moveSpeed) + Vector3.up;
                    return true;
                }
            }
            catch {}
            return false;
        }
    }
}
