
using Fishing.components.FishingRod;
using Fishing.components.Items.FishClasses;
using Fishing.components.Items.TrashClasses;
using System.Xml.Serialization;

namespace Fishing.components.Items
{
    [Serializable]
    [XmlInclude(typeof(Fish))]
    [XmlInclude(typeof(Trash))]
    [XmlInclude(typeof(YaziFish))]
    [XmlInclude(typeof(ClownFish))]
    [XmlInclude(typeof(GoldFish))]
    [XmlInclude(typeof(AnglerFish))]
    [XmlInclude(typeof(BootTrash))]
    public class Item
    {
        [XmlAttribute("Name")]
        public string Name { get; set; }
        [XmlAttribute("Value")]
        public int Value { get; set; }

        [XmlIgnore]
        public Image Texture { get; set; }

        [XmlAttribute("TexturePath")]
        public string TexturePath { get; set; }

        [XmlAttribute("Speed")]
        public int Speed { get; set; }

        [XmlAttribute("BaseSpeed")]
        public int BaseSpeed { get; set; }

        public bool isCaught = false;

        public int x = 0;
        public int y = 0;

        public Item()
        {
            Name = "Unknown";
            Value = 0;
            Speed = 0;
            BaseSpeed = 0;
        }
        public Item(string name, int value, string texture, int speed)
        {
            Name = name;
            Value = value;
            TexturePath = texture;
            Texture = Image.FromFile(texture);
            Speed = speed;
            BaseSpeed = speed;
        }

        

        public void DrawItem(Graphics g) 
        {
            if (Texture != null)
            {
                g.DrawImage(this.Texture, x, y, 80, 80);
            }
        }

        public void ChangePosition(int liftSpeed)
        {
            if (!this.isCaught)
            {
                this.x += this.Speed;
            }
            else if (this.isCaught)
            {
                this.y -= liftSpeed;
            }
        }

        public virtual string GetCaught()
        {
            return $"Вы поймали {Name}";
        }
    }
}