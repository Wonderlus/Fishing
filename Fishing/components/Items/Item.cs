
using Fishing.components.FishingRod;

namespace Fishing.components.Items
{
    public class Item
    {
        public string Name { get; set; }
        public int Value { get; set; }


        public Image Texture { get; set; }

        public int Speed { get; set; }

        public int BaseSpeed { get; private set; }

        public bool isCaught = false;

        public int x = 0;
        public int y = 0;
        public Item(string name, int value, string texture, int speed)
        {
            Name = name;
            Value = value;
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