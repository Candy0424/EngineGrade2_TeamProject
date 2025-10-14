using UnityEngine;
using Work.CUH.Code.Commands;

namespace Work.CUH.Code.Commandable
{
    public class Lever : ICommandable, ISwitch
    {
        public void Activate()
        {
            isActive = !isActive;
        }

        public void UnActivate()
        {
            isActive = !isActive;
        }

        public bool isActive { get; private set; }
    }
}