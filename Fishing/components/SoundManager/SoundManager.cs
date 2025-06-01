using NAudio.Wave;
using System.Collections.Generic;
using System.Threading.Tasks;

public class SoundManager
{
    // Для фоновой музыки
    private WaveOutEvent backgroundOut;
    private AudioFileReader backgroundReader;
    // Для звуков заброса
    private List<AudioFileReader> castReaders;
    private List<AudioFileReader> notEnoughMoneyReaders;
    private List<WaveOutEvent> castOuts;
    private Random random;

    // Для других звуков
    private WaveOutEvent effectsOut;
    private AudioFileReader catchReader;
    private AudioFileReader trashReader;

    public SoundManager()
    {
        random = new Random();
        castReaders = new List<AudioFileReader>();
        castOuts = new List<WaveOutEvent>();
        notEnoughMoneyReaders = new List<AudioFileReader>();
        // Инициализация фоновой музыки
        backgroundReader = new AudioFileReader("../../../sounds/Main Theme/castlejam.wav");
        backgroundOut = new WaveOutEvent();
        backgroundOut.Init(backgroundReader);
        // Инициализация звуков заброса
        var castFiles = new[] { "cast1.wav", "cast2.wav", "cast3.wav", "cast4.wav" };
        foreach (var file in castFiles)
        {
            var reader = new AudioFileReader($"../../../sounds/Cast/{file}");
            castReaders.Add(reader);
        }

        var notEnoughMoneyFiles = new[]
        {
            "Resource_Need4.wav", "Resource_Need16.wav", "Resource_Need17.wav"
        };

        foreach (var file in notEnoughMoneyFiles)
        {
            var reader = new AudioFileReader($"../../../sounds/Info/{file}");
            notEnoughMoneyReaders.Add(reader);
        }

        // Инициализация других звуков
    }

    public void PlayBackgroundMusic()
    {
        backgroundOut.Play();
    }

    public void StopBackgroundMusic()
    {
        backgroundOut.Stop();
    }

    public async Task PlayNotEnoughMoney()
    {
        if (notEnoughMoneyReaders.Count == 0) return;

        var index = random.Next(notEnoughMoneyReaders.Count);
        var reader = notEnoughMoneyReaders[index];
        var player = new WaveOutEvent();

        await Task.Run(() =>
        {
            reader.Position = 0;
            player.Init(reader);
            player.Play();

            player.PlaybackStopped += (s, e) =>
            {
                player.Dispose();
            };
        });
    }
    public async Task PlaySave()
    {

        await Task.Run(() =>
        {
            using (var reader = new AudioFileReader("../../../sounds/info/General_Saving.wav"))
            using (var player = new WaveOutEvent())
            {
                player.Init(reader);
                player.Play();
          
                while (player.PlaybackState == PlaybackState.Playing)
                {
                    Thread.Sleep(100);
                }
            }
            
            
        });
    }

    public async Task PlayLoad()
    {
        await Task.Run(() =>
        {
            using (var reader = new AudioFileReader("../../../sounds/info/General_Loading.wav"))
            using (var player = new WaveOutEvent())
            {
                player.Init(reader);
                player.Play();
                while (player.PlaybackState == PlaybackState.Playing)
                {
                    Thread.Sleep(100);
                }
            }


        });
    }

    public async Task PlayRandomCastAsync()
    {
        if (castReaders.Count == 0) return;

        var index = random.Next(castReaders.Count);
        var reader = castReaders[index];
        var player = new WaveOutEvent();

        await Task.Run(() =>
        {
            reader.Position = 0; // Перемотка в начало
            player.Init(reader);
            player.Play();

            // Очистка после завершения
            player.PlaybackStopped += (s, e) =>
            {
                player.Dispose();
            };
        });
    }

    private async Task PlayEffectAsync(AudioFileReader reader)
    {
        await Task.Run(() =>
        {
            effectsOut.Stop();
            reader.Position = 0;
            effectsOut.Init(reader);
            effectsOut.Play();
        });
    }

    public void Dispose()
    {
        backgroundOut?.Stop();
        backgroundOut?.Dispose();
        backgroundReader?.Dispose();

        foreach (var player in castOuts) player?.Dispose();
        foreach (var reader in castReaders) reader?.Dispose();

        effectsOut?.Dispose();
        catchReader?.Dispose();
        trashReader?.Dispose();
    }
}