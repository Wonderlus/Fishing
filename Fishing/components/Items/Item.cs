

namespace Fishing.components.Items
{
    public class Item
    {
        public string Name { get; set; }
        public int Value { get; set; }

        public Image Texture { get; set; }

        public int Speed { get; set; }

        public bool isCaught = false;

        public int x = 0;
        public int y = 0;
        public Item(string name, int value, string texture, int speed)
        {
            Name = name;
            Value = value;
            Texture = Image.FromFile(texture);
            Speed = speed;
        }

        

        public virtual void DrawItem(Graphics g) 
        {
            if (Texture != null)
            {
                g.DrawImage(this.Texture, x, y, 80, 80);
            }
        }

        public virtual void ChangePosition()
        {
            if (!this.isCaught)
            {
                this.x += this.Speed;
            }
            else if (this.isCaught)
            {
                this.y -= 30;
            }
        }

        public virtual string GetCaught()
        {
            return $"Вы поймали {Name}";
        }
    }
}