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