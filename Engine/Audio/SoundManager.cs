using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Threading.Tasks;

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

        public class MusicChannel
        {
            public Stream ThisSream = null;
            public Stream NextStream = null;
            public MusicFile Music = null;
            public FadeMode Mode = FadeMode.None;
            public float FadeDelay = 0;
        }

        private static Stopwatch Clock = new Stopwatch();
        public static MusicChannel StreamChannel = new MusicChannel();

        private static List<string> MusicSet = new List<string>();
        private static Dictionary<string, int> SoundMap = new Dictionary<string, int>();

       
    }
}
