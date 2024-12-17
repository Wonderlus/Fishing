using Fishing.components.Items;
using Fishing.components.FishingRod;
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Xml.Serialization;


namespace Fishing
{
    public class FishingGame : Form
    {
        private FishingRod fishingRod;
        private int selectedRow = 2; // Выбранный ряд (по умолчанию 2)
        private int score = 0;       // Очки
        private System.Windows.Forms.Timer gameTimer;     // Таймер для обновления игры
        private Random random = new Random(); // Рандомный спавн объектов
        private Label scoreLabel; // Вывод набранных очков
        private Image waterTexture; // Текстура воды
        private Image trashTexture; // Текстура мусора
        private Image fishTexture; // Текстура рыбы
        private int riverBottom = 780; 

        public FishingGame()
        {
            // Высота картинки - 20 пикселей
            waterTexture = Image.FromFile("../../../templates/river.jpg");
            // Настройки окна
            this.Text = "Игра: Рыбалка";
            this.Size = new Size(1280, 800);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.DoubleBuffered = true; // Уменьшение мерцания графики
            fishingRod = new FishingRod("../../../templates/fishingRod.png");
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


            this.MouseMove += OnMouseMove;
            // Событие нажатия клавиш
            this.KeyDown += OnKeyDown;
            this.MouseClick += OnMouseClick;
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            // Управление выбором ряда
            
            if (e.KeyCode == Keys.Space)
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
        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (fishingRod.isLineCasting)
            {
                return;
            }

            fishingRod.UpdatePosition(new Point(e.X, e.Y));
            this.Invalidate();
        }

        private void OnMouseClick(object sender, MouseEventArgs e)
        {
            if (!fishingRod.isLineCasting)
            {
                
                fishingRod.isLineCasting = true;
                fishingRod.isReturning = false;
                fishingRod.lineY = fishingRod.rodY;
               
            }
            
            this.Invalidate();
        }
        private void GameTick(object sender, EventArgs e)
        {
            // Логика обновления игры
            // Логика хитбоксов - lineY лежит в диапазоне пикселей текстуры рыбы или мусора по Y,
            // и Position.X лежит в диапазоне текстуры, создать отдельную переменную, которая хранит заблокированное значение позиции по X при броске удочки
            if (fishingRod.isLineCasting)
            {
                if(!fishingRod.isReturning)
                {
                    fishingRod.lineY += 30;
                    if(fishingRod.lineY >= riverBottom)
                    {
                        
                        fishingRod.isReturning = true;

                    }
                }

                else
                {
                    fishingRod.lineY -= 30;
                    if(fishingRod.lineY <= fishingRod.rodY)
                    {
                        fishingRod.isLineCasting  = false;
                    }
                }
            } 

            this.Invalidate();
        }

        private void CastFishingRod()
        {
            Item caughtItem;
            // Генерация случайного объекта
            bool isFish = random.Next(0, 2) == 0; // 50% шанс поймать рыбу

            if (isFish)
            {
                caughtItem = new Fish("Карась", 15, "../../../templates/boots.png");
                score += caughtItem.Value;
                MessageBox.Show(caughtItem.GetCaught());
            }
            else
            {
                caughtItem = new Trash("Башмак", -10, "../../../templates/boots.png");
                score += caughtItem.Value;
                MessageBox.Show(caughtItem.GetCaught());
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

            if (fishingRod.isLineCasting)
            {
                fishingRod.DrawLine(g, fishingRod.circleCenter);
            }

            // Рисуем ряды
            //for (int i = 0; i < 5; i++)
            //{
            //    int rowY = 100 + i * 80;
            //    g.DrawLine(Pens.Black, 0, rowY, this.Width, rowY);
            //}

            fishingRod.Draw(g);
            
        }
    }
}
