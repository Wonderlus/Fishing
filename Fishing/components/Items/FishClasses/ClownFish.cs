using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fishing.components.Items.FishClasses
{
    [Serializable]
    public class ClownFish : Fish
    {
        public ClownFish() : base("Рыба клоун", 40, "../../../templates/fish/3.png", 20) { }
    }
}
