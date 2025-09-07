namespace Morhu.Infrustructure.Services.AudioSystem
{
    public interface IAudioService
    {
        float CurrentVolume { get; }

        SoundBuilder BuildSound();
        void ChangeVolumeForAll(float volume);
        void StopAll();
    }
}