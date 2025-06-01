using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fishing.components.Items.FishClasses
{
    [Serializable]
    public class GoldFish : Fish
    {
        public GoldFish() : base("Золотая рыбка", 80, "../../../templates/fish/2.png", 30) { }
    }
}
