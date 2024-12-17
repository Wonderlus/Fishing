﻿using System.Drawing;

namespace Fishing.components.FishingRod
{
    public class FishingRod
    {
        public Point Position { get; private set; }
        public Image Texture { get; private set; }

        public bool drawLine;
        public bool mouseLocked;
        public Point circleCenter;
        public bool isLineCasting = false; // Удочка идет вниз
        public bool isReturning = false; // Удочка поднимается
        public int lineY; // Текущая позиция линии по Y
        public int rodX = 640; // Позиция удочки по X
        public int rodY = 250; // Ограничение удочки в блоке на 250 пикселей сверху
        public FishingRod(string texturePath) {
            Texture = Image.FromFile(texturePath);
            Position = new Point(400, 200);
            
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
