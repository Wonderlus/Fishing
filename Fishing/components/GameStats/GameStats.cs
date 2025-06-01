using Fishing.components.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;



namespace Fishing.components.GameStats
{
    [Serializable]
    
    public class GameStats
    {
        public int Score { get; set; }
        public int FishCombo { get; set; }
        public int ComboMultiplier { get; set; }
        public string CurrentRodTexture { get; set; }

        public int RodSpeedMultiplier { get; set; }

        public List<Item> Items { get; set; }

    }
}
