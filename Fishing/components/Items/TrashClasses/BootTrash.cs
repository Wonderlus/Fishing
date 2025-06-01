using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fishing.components.Items.TrashClasses
{
    [Serializable]

    public class BootTrash : Trash
    {
        public BootTrash() : base("Старый ботинок", -45, "../../../templates/trash/boots.png", 45) { }
    }
}
