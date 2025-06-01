using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fishing.components.Items.FishClasses
{
    [Serializable]
    public class YaziFish : Fish
    {

        public YaziFish(): base("Язь", 15, "../../../templates/fish/1.png", 10){}
    }
}
