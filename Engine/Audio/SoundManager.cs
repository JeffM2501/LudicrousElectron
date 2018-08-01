using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Threading.Tasks;

using Microsoft.Xna.Framework.Media;

namespace LudicrousElectron.Engine.Audio
{
    public static class SoundManager
    {
        private const float FadeMusicTime = 1.0f;
        private const float FadeSoundTime = 0.3f;

        public enum FadeMode
        {
            None,
            FadeIn,
            FadeOut
        }

        internal class MusicChannel
        {
            public Song ThisSong = null;
            public int ThisSongIndex = -1;

            public FadeMode Mode = FadeMode.None;
            public float FadeDelay = 0;
        }

        private static Stopwatch Clock = new Stopwatch();
        internal static MusicChannel StreamChannel = new MusicChannel();

        private static List<string> MusicSet = new List<string>();
        private static Dictionary<string, int> SoundMap = new Dictionary<string, int>();

        static SoundManager()
        {
            MediaPlayer.MediaStateChanged += MediaPlayer_MediaStateChanged;
        }

        private static void MediaPlayer_MediaStateChanged(object sender, EventArgs e)
        {
            if (MusicSet.Count > 0)
            {
                if (MediaPlayer.State == MediaState.Stopped)
                {
                    string filename = MusicSet[0];
                    MusicSet.RemoveAt(0);
                    PlayMusic(filename);
                }
            }
        }

        public static void PlayMusic(string filename)
        {
            MediaPlayer.Play(Song.FromUri(Path.GetFileNameWithoutExtension(filename), new Uri("file://" + filename)));
        }

        public static void PlayMusicSet(List<string> filenames)
        {
            MusicSet = new List<string>(filenames.ToArray());
            MediaPlayer_MediaStateChanged(null, EventArgs.Empty);
        }

        public static void StopMusic() { }
        public static void SetMusicVolume(float volume) { }
        public static float GetMusicVolume() { return 0.0f;  }

    }
}
