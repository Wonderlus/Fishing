

namespace Fishing.components.Items
{
    public class Item
    {
        public string Name { get; set; }
        public int Value { get; set; }

        public Image Texture { get; set; }

        public Item(string name, int value, string texture)
        {
            Name = name;
            Value = value;
            Texture = Image.FromFile(texture);
        }

        public virtual string GetCaught()
        {
            return $"Вы поймали {Name}";
        }
    }
}