using System.Drawing;
using System.Xml.Serialization;

namespace Fishing.components.FishingRod
{
    public class FishingRod
    {

        [XmlIgnore]
        public Point Position { get; private set; }
        [XmlIgnore]
        public Image Texture { get; private set; }

        [XmlAttribute("TexturePath")]
        public string TexturePath { get; private set; }

        [XmlAttribute("SpeedMultiplier")]
        public int SpeedMultiplier { get; private set; } = 1;

        [XmlIgnore]
        public string RodName { get; private set; }
        public int Price { get; private set; }



        public bool drawLine;
        public bool mouseLocked;
        public Point circleCenter;
        public bool isLineCasting = false; // Удочка в принципе закинута
        public bool isReturning = false; // Удочка поднимается
        public int lineY; // Текущая позиция линии по Y
        public int rodX = 640; // Позиция удочки по X
        public int rodY = 250; // Ограничение удочки в блоке на 250 пикселей сверху

        public FishingRod(string texturePath, string name = "Обычная удочка", int price = 0, int speedMultiplier = 1) {
            TexturePath = texturePath;
            Texture = Image.FromFile(texturePath);
            Position = new Point(400, 200);
            RodName = name;
            Price = price;
            SpeedMultiplier = speedMultiplier;
        }

        public int GetHookSpeed()
        {
            return 30 * SpeedMultiplier;
        }

        public void UpdatePosition(Point newPosition)
        {
            Position = newPosition;
        }

        public void Draw(Graphics g)
        {
            if (Texture != null)
            {
                g.DrawImage(Texture, Position.X + 10, Position.Y - 90, 80, 120);
            }

            g.DrawEllipse(Pens.Black, Position.X - 10, Position.Y - 10, 20, 20);
        }

        public void DrawLine(Graphics g, Point circleCenter)
        {
            using (Pen thickPen = new Pen(Color.Black, 5))
            {
                g.DrawLine(thickPen, Position.X, rodY, Position.X, lineY);
                
            }


        }
    }
}
