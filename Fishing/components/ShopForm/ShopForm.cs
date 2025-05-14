using Fishing.components.FishingRod;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Fishing
{
    public class ShopForm : Form
    {

        private SoundManager soundManager;

        private FishingGame mainGame;
        private Button buttonClose;

        private FlowLayoutPanel rodsPanel;

        public ShopForm(FishingGame game)
        {
            soundManager = new SoundManager();
            mainGame = game;
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            this.Text = "Магазин удочек";
            this.Size = new Size(600, 400);
            this.StartPosition = FormStartPosition.CenterParent;

            buttonClose = new Button
            {
                Text = "Закрыть",
                Location = new Point(10, 320),
                Size = new Size(100, 30)
            };

            buttonClose.Click += (s, e) => this.Close();

            this.Controls.Add(buttonClose);

            rodsPanel = new FlowLayoutPanel
            {
                Location = new Point(10, 10),
                Size = new Size(600, 300),
                AutoScroll = true
            };

            AddRodToShop("Обычная удочка", 0, 1, "../../../templates/rods/fishingRod1.png");
            AddRodToShop("Обычная удочка", 500, 2, "../../../templates/rods/fishingRod2.png");
            this.Controls.Add(rodsPanel);
        }


        private void AddRodToShop(string name, int price, int speedMultiplier, string texturePath)
        {
            var panel = new Panel { Size = new Size(550, 80), BorderStyle = BorderStyle.FixedSingle };

            var picture = new PictureBox
            {
                Image = Image.FromFile(texturePath),
                SizeMode = PictureBoxSizeMode.Zoom,
                Size = new Size(70, 70),
                Location = new Point(5, 5)

            };

            var labelName = new Label
            {
                Text = name,
                Location = new Point(80, 10),
                AutoSize = true,
                Font = new Font("Arial", 10, FontStyle.Bold)
            };

            var labelStats = new Label
            {
                Text = $"Скорость: x{speedMultiplier}",
                Location = new Point(80, 30),
                AutoSize = true,
            };

            var labelPrice = new Label
            {
                Text = $"Цена: {price} золота",
                Location = new Point(80, 50),
                AutoSize = true,
            };

            var buttonBuy = new Button
            {
                Text = "Купить",
                Location = new Point(450, 25),
                Size = new Size(80, 30),
                Tag = new { Name = name, Price = price, Multiplier = speedMultiplier, Texture = texturePath }
            };

            buttonBuy.Click += async (s, e) =>
            {
                var rodInfo = (dynamic)buttonBuy.Tag;
                if (mainGame.Score >= rodInfo.Price)
                {
                    mainGame.Score -= rodInfo.Price;
                    mainGame.EquipRod(new FishingRod(rodInfo.Texture, rodInfo.Name, rodInfo.Price, rodInfo.Multiplier));
                    MessageBox.Show($"Вы купили {rodInfo.Name}!");
                    mainGame.Invalidate();

                } else
                {
                    await soundManager.PlayNotEnoughMoney();
                }
            };

            panel.Controls.AddRange(new Control[] { picture, labelName, labelStats, labelPrice, buttonBuy });
            rodsPanel.Controls.Add(panel);
       }

    }
}
