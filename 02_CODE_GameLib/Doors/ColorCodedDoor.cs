﻿using CODE_GameLib.Doors.Common;
using CODE_GameLib.Interfaces;
using CODE_GameLib.Items;
using System.Drawing;
using System.Linq;

namespace CODE_GameLib.Doors
{
    public class ColorCodedDoor : Door, IColorBinder
    {
        public Color ColorCode { get; private set; }

        public ColorCodedDoor(Color color)
        {
            ColorCode = color;
        }

        public override bool CanUseDoor(Player player)
        {
            var matchingKey = player.Items.FirstOrDefault(
                i => i.GetType() == typeof(Key) && ((Key)i).ColorCode == ColorCode
                );
            return matchingKey != null;
        }
    }
}