﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fishing.components.Items
{
    public class Trash : Item
    {

        public Trash(string name, int value, string texture, int speed) : base(name, value, texture, speed)
        {

        }

        public override string GetCaught()
        {
            return $"Вы поймали {Name}... {Value} очков";
        }

    }
}
