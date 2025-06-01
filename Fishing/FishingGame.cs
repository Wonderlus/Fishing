using Fishing.components.Items;
using Fishing.components.FishingRod;
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Xml.Serialization;
using Fishing.components.GameStats;
using Fishing.components.Items.FishClasses;
using Fishing.components.Items.TrashClasses;


namespace Fishing
{
    public class FishingGame : Form
    {
        private Button buttonSave;
        private Button buttonLoad;
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

            buttonSave = new Button
            {
                Text = "Сохранить",
                Location = new Point(this.Width - 120, 50),
                Size = new Size(100, 30)
            };
            buttonSave.Click +=  (s, e) =>
            {
                SaveGame();
            };
            this.Controls.Add(buttonSave);

            buttonLoad = new Button
            {
                Text = "Загрузить",
                Location = new Point(this.Width - 120, 90),
                Size = new Size(100, 30)
            };

            buttonLoad.Click += (s, e) =>
            {
                LoadGame();
            };
            this.Controls.Add(buttonLoad);

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

        private void ButtonSave_Click(object? sender, EventArgs e)
        {
            throw new NotImplementedException();
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
            bool isTrash = random.Next(0, 10) == 0; // 10% вероятность выбора мусора
            int startY = random.Next(260, this.Height - 80);

            Item newItem;

            if (!isTrash)
            {
                switch (random.Next(0, 4)) // 4 вида рыб
                {
                    case 0: newItem = new YaziFish(); break;
                    case 1: newItem = new ClownFish(); break;
                    case 2: newItem = new GoldFish(); break;
                    case 3: newItem = new AnglerFish(); break;
                    default: newItem = new YaziFish(); break;
                }
            }

            else
            {
                newItem = new BootTrash(); // пока один вид мусора
                
            }

            newItem.y = startY;
            items.Add(newItem);
        }

        // Сохранение
        private void SaveGame() {
            soundManager.PlaySave();
            var saveFileDialog = new SaveFileDialog { 
                Filter = "Файлы сохранения (*.fish) | *.fish",
                Title = "Сохранить игру"
            };
            if (saveFileDialog.ShowDialog() == DialogResult.OK) {
                var state = new GameStats
                {
                    Score = this.Score,
                    FishCombo = this.fishCombo,
                    ComboMultiplier = this.comboMultiplier,
                    CurrentRodTexture = fishingRod.TexturePath,
                    RodSpeedMultiplier = fishingRod.SpeedMultiplier,
                    Items = this.items,
                
                };

                var serializer = new XmlSerializer(typeof(GameStats));
                using (var stream = File.Create(saveFileDialog.FileName))
                {
                    serializer.Serialize(stream, state);
                }

                MessageBox.Show("Игра успешно сохранена!");
            };
        }

        // Загрузка
        private void LoadGame()
        {
            soundManager.PlayLoad();
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Файлы загрузки (*.fish) | *.fish",
                Title = "Загрузить игру"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var serializer = new XmlSerializer(typeof(GameStats));
                    using (var stream = File.OpenRead(openFileDialog.FileName))
                    {
                        var state = (GameStats)serializer.Deserialize(stream);
                        this.Score = state.Score;
                        this.fishCombo = state.FishCombo;
                        this.comboMultiplier = state.ComboMultiplier;
                        this.fishingRod = new FishingRod(
                            state.CurrentRodTexture,
                            "Удочка",
                            0,
                            state.RodSpeedMultiplier
                        );
                        this.items = state.Items;
                        foreach (var item in items)
                        {
                            item.Texture = Image.FromFile(item.TexturePath);
                        }

                        scoreLabel.Text = $"Золото: {this.Score}";
                        this.Invalidate();

                        MessageBox.Show("Игра загружена успешно");
                    }
                }
                catch (Exception exception) {
                    MessageBox.Show($"Ошибка загрузки: {exception.Message}");
                }
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
