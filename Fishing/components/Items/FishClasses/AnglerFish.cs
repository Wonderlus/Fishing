using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fishing.components.Items.FishClasses
{
    [Serializable]
    public class AnglerFish : Fish
    {
        public AnglerFish() : base("Рыба удильщик", 120, "../../../templates/fish/4.png", 40) { }
    }
}
