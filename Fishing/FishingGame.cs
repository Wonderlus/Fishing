using System;
using System.Drawing;
using System.Windows.Forms;

namespace Fishing
{
    public class FishingGame : Form
    {
        private int selectedRow = 2; // Выбранный ряд (по умолчанию 2)
        private int score = 0;       // Очки
        private System.Windows.Forms.Timer gameTimer;     // Таймер для обновления игры
        private Random random = new Random(); // Рандомный спавн объектов
        private Label scoreLabel; // Вывод набранных очков
        private Image waterTexture; // Текстура воды
        private Image fishingRodTexture; // Текстура удочки
        private Image trashTexture; // Текстура мусора
        private Image fishTexture; // Текстура рыбы

        public FishingGame()
        {
            waterTexture = Image.FromFile("../../../templates/river.jpg");
            fishingRodTexture = Image.FromFile("../../../templates/fishingRod.png");
            // Настройки окна
            this.Text = "Игра: Рыбалка";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.DoubleBuffered = true; // Уменьшение мерцания графики

            // Элемент для отображения очков
            scoreLabel = new Label
            {
                Text = "Очки: 0",
                Font = new Font("Arial", 16),
                ForeColor = Color.Black,
                Location = new Point(10, 10),
                AutoSize = true
            };
            this.Controls.Add(scoreLabel);

            // Таймер игры
            gameTimer = new System.Windows.Forms.Timer();
            gameTimer.Interval = 100; // Обновление каждые 100 мс
            gameTimer.Tick += GameTick;
            gameTimer.Start();

            // Событие нажатия клавиш
            this.KeyDown += OnKeyDown;
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            // Управление выбором ряда
            if (e.KeyCode == Keys.Up && selectedRow > 0)
            {
                selectedRow--;
            }
            else if (e.KeyCode == Keys.Down && selectedRow < 4)
            {
                selectedRow++;
            }
            else if (e.KeyCode == Keys.Space)
            {
                CastFishingRod();
            }
            else if (e.KeyCode == Keys.Escape)
            {
                Application.Exit();
            }

            // Перерисовка экрана
            this.Invalidate();
        }

        private void GameTick(object sender, EventArgs e)
        {
            // Логика обновления игры
            // Здесь можно генерировать движение предметов или их исчезновение
            this.Invalidate();
        }

        private void CastFishingRod()
        {
            // Генерация случайного объекта
            bool isFish = random.Next(0, 2) == 0; // 50% шанс поймать рыбу

            if (isFish)
            {
                score += 10;
                MessageBox.Show("Вы поймали рыбу! +10 очков");
            }
            else
            {
                score -= 5;
                MessageBox.Show("Вы поймали мусор... -5 очков");
            }

            // Обновление очков
            scoreLabel.Text = "Очки: " + score;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;

            // Рисуем фон
            //g.Clear(Color.Aqua);
            if (waterTexture != null)
            {
                g.DrawImage(waterTexture, 0, 0, waterTexture.Width, waterTexture.Height);
                
            }

            // Рисуем ряды
            for (int i = 0; i < 5; i++)
            {
                int rowY = 100 + i * 80;
                g.DrawLine(Pens.Black, 0, rowY, this.Width, rowY);
            }

            // Рисуем удочку
            int rodY = 100 + selectedRow * 80;
            g.DrawLine(Pens.Black, this.Width / 2, 0, this.Width / 2, rodY - 40);
            // Рисуем круг на конце удочки
            int circleSize = 20; // Размер круга
            g.FillEllipse(Brushes.Black, this.Width / 2 - circleSize / 2, rodY - circleSize / 2 - 40, circleSize, circleSize);
        }
    }
}
