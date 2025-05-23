﻿using Fishing.components.Items;
using Fishing.components.FishingRod;
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Xml.Serialization;


namespace Fishing
{
    public class FishingGame : Form
    {

        private Button buttonShop;
        private SoundManager soundManager;
        private FishingRod fishingRod;
        public int Score { get; set; }   // Очки
        private System.Windows.Forms.Timer gameTimer;     // Таймер для обновления игры
        private Random random = new Random(); // Рандомный спавн объектов
        private Label scoreLabel; // Вывод набранных очков
        private Label eventsLabel; // Вывод событий игры
        private Image waterTexture; // Текстура воды
        private int riverBottom = 780;
        private bool caughtAnything = false;

        private bool isCurrentActive = false;
        
        private List<float> speedArray = new List<float> { 
        0.5f, 0.6f, 0.7f, 0.8f, 0.9f, 1f, 1.1f, 1.2f, 1.3f, 1.4f, 1.5f
        };
        private float currentEffect = 1f; // 1 - обычная скорость

        private int currentDuration = 0;

        private int fishCombo = 0;
        private int comboMultiplier = 1;

        private List<Item> items;
        private int spawnTimer = 0;
        private readonly List<(string Name, int Value, string Texture, int Speed)> fishTypes = new List<(string, int , string , int)> 
        {
            ("Язь", 15, "../../../templates/fish/1.png", 10),
            ("Рыба клоун", 40, "../../../templates/fish/3.png", 20),
            ("Золотая рыбка", 80, "../../../templates/fish/2.png", 30),
            ("Рыба удильщик", 120, "../../../templates/fish/4.png", 40)
        };

        private readonly List<(string Name, int Value, string Texture, int Speed)> trashTypes = new List<(string, int, string, int)>
        {
            ("Старый ботинок", -45, "../../../templates/trash/boots.png", 45),
        };
        public FishingGame()
        {
            soundManager = new SoundManager();
            soundManager.PlayBackgroundMusic();
            // Массив предметов
            items = new List<Item>();
            // Высота картинки - 20 пикселей
            waterTexture = Image.FromFile("../../../templates/river.jpg");
            // Настройки окна
            this.Text = "Игра: Рыбалка";
            this.Size = new Size(1280, 800);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.DoubleBuffered = true; // Уменьшение мерцания графики
            fishingRod = new FishingRod("../../../templates/rods/fishingRod1.png");
            // Элемент для отображения очков
            scoreLabel = new Label
            {
                Text = "Золото: 0",
                Font = new Font("Roboto", 16),
                ForeColor = Color.Black,
                Location = new Point(10, 10),
                AutoSize = true
            };
            this.Controls.Add(scoreLabel);

            eventsLabel = new Label
            {
                Text = "",
                Font = new Font("Roboto", 16),
                ForeColor = Color.Black,
                Location = new Point(600, 10),
                AutoSize = true
            };
            this.Controls.Add(eventsLabel);

            buttonShop = new Button
            {
                Text = "Магазин",
                Location = new Point(this.Width - 120, 10),
                Size = new Size(100, 30)
            };
            buttonShop.Click += (s, e) =>
            {
                var shop = new ShopForm(this);
                shop.ShowDialog();
            };
            this.Controls.Add(buttonShop);

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

        public void EquipRod(FishingRod newRod)
        {
            fishingRod = newRod;
            fishingRod.UpdatePosition(new Point(fishingRod.Position.X, fishingRod.Position.Y));
            this.Invalidate();

        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            soundManager.Dispose();
            base.OnFormClosed(e);
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            // Управление выбором ряда
            
            if (e.KeyCode == Keys.Escape)
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

        private async void OnMouseClick(object sender, MouseEventArgs e)
        {
            if (!fishingRod.isLineCasting)
            {
                
                await soundManager.PlayRandomCastAsync();
                fishingRod.isLineCasting = true;
                fishingRod.isReturning = false;
                fishingRod.lineY = fishingRod.rodY;


            }

            this.Invalidate();
        }

        private void SpawnRandomItem()
        {
            bool isFish = random.Next(0, 1) == 0; // 50% вероятность выбора рыбы или мусора
            int startY = random.Next(260, this.Height - 80);

            if (isFish)
            {
                var fishType = fishTypes[random.Next(fishTypes.Count)];
                var fish = new Fish(fishType.Name, fishType.Value, fishType.Texture, fishType.Speed);
                fish.y = startY;
                items.Add(fish);
            }

            else
            {
                var trashType = trashTypes[random.Next(trashTypes.Count)];
                var trash = new Trash(trashType.Name, trashType.Value, trashType.Texture, trashType.Speed);
                trash.y = startY;
                items.Add(trash);
            }
        }

        // Логика обновления игры
        private void GameTick(object sender, EventArgs e)
        {
            scoreLabel.Text = $"Золото: {this.Score}";
            spawnTimer++;
            if (spawnTimer >= 10)
            {
                SpawnRandomItem(); 
                eventsLabel.Text = "";
                spawnTimer = 0;
            }

            if (!isCurrentActive && random.Next(0, 100) < 35)
            {
                isCurrentActive = true;
                currentDuration = random.Next(50, 100);
                currentEffect = speedArray[random.Next(speedArray.Count)];
            }

            if (isCurrentActive)
            {
                currentDuration--;
                if (currentDuration <= 0)
                {
                    isCurrentActive = false;
                    currentEffect = 1f;
                }
            }

            // Преобразование скорости
            foreach (var item in items)
            {
                item.Speed = (int)(item.BaseSpeed * currentEffect);
            }
            
            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                if (fishingRod.isLineCasting)
                {
                    // Поймали рыбу
                    if ((fishingRod.Position.X >= item.x) & (fishingRod.Position.X <= item.x + 80) & (fishingRod.lineY >= item.y) & (fishingRod.lineY <= item.y + 80)) {
                        item.isCaught = true;
                        fishingRod.isReturning = true;
                        caughtAnything = true;

                    }
                    
                }
                item.ChangePosition(fishingRod.GetHookSpeed());


                // Вернули удочку
                if(item.x > this.Width | item.y < 250)
                {
                    if (item.isCaught)
                    {

                        caughtAnything = true;
                        if (item is Fish)
                        {
                            fishCombo++;
                            comboMultiplier = Math.Min(5, fishCombo);
                        }
                        else
                        {
                            fishCombo = 0;
                            comboMultiplier = 1;
                        }

                        eventsLabel.Text = fishCombo > 1 ? $"{item.GetCaught()} Комбо x{comboMultiplier}" : item.GetCaught();
                        this.Score += item.Value * comboMultiplier;
                    } 
                    
                    
                    items.RemoveAt(i);

                }
            }
            
            // Логика хитбоксов - lineY лежит в диапазоне пикселей текстуры рыбы или мусора по Y,
            // и Position.X лежит в диапазоне текстуры, создать отдельную переменную, которая хранит заблокированное значение позиции по X при броске удочки
            if (fishingRod.isLineCasting)
            {

                for (int i = 0; i < items.Count; i++)
                {
                    var item = items[i];

                }
                if(!fishingRod.isReturning)
                {
                    fishingRod.lineY += fishingRod.GetHookSpeed();
                    if(fishingRod.lineY >= riverBottom)
                    {
                        
                        fishingRod.isReturning = true;

                    }
                }

                else
                {
                    fishingRod.lineY -= fishingRod.GetHookSpeed();
                    if(fishingRod.lineY <= fishingRod.rodY)
                    {
                        fishingRod.isLineCasting  = false;

                        if (caughtAnything == false)
                        {
                            fishCombo = 0;
                            comboMultiplier = 1;
                            eventsLabel.Text = "Ничего не поймано! Комбо сброшено!";
                        }

                        caughtAnything = false;

                    }
                }
            } 

            this.Invalidate();
        }   

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;

            if (waterTexture != null)
            {
                g.DrawImage(waterTexture, 0, 0, waterTexture.Width, waterTexture.Height);

            }

            foreach (var item in items)
            {
                item.DrawItem(g);
            }

            if (fishingRod.isLineCasting)
            {
                fishingRod.DrawLine(g, fishingRod.circleCenter);
            }

            fishingRod.Draw(g);
            
        }
    }
}
