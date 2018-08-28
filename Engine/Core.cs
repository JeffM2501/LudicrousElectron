using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;

using LudicrousElectron.Engine.Audio;
using LudicrousElectron.Engine.Graphics.Textures;
using LudicrousElectron.Engine.Window;
using LudicrousElectron.Entities.Collisions;

namespace LudicrousElectron.Engine
{
    public static class Core 
    {
		private static bool Running = false;

		internal delegate void RerunCallback();

		private static RerunCallback RerunFunciton = null;
		private static bool IsRerun = false;

        public class EngineState : EventArgs
        {
            public int CurrentContextID = -1;

            public Stopwatch Clock = new Stopwatch();

            public double LastUpdate = double.MinValue;
            public double LastRender = double.MinValue;

            public double Now = double.MinValue;

            public class AnimationTimers
            {
                public int ID = 0;
                public bool Recuring = false;
                public bool Active = false;

                internal bool TriggerThisFrame = false;

                public double Interval = 0;
                public double LastTrigger = 0;

                public EventHandler<EngineState> OnTriggerEvent = null;

                internal void Call(EngineState state)
                {
                    OnTriggerEvent?.Invoke(state, state);
                }
            }

            public Dictionary<int, AnimationTimers> Timers = new Dictionary<int, AnimationTimers>();
            protected int LastTimerID = 0;

            public int AddTimer(double interval, bool recuring, EventHandler<EngineState> eventToCall = null)
            {
                AnimationTimers timer = new AnimationTimers();
                timer.ID = LastTimerID++;
                timer.Interval = interval;
                timer.Recuring = recuring;
                timer.OnTriggerEvent = eventToCall;
                timer.Active = true;
                timer.LastTrigger = LastUpdate;

                Timers.Add(timer.ID,timer);
                return timer.ID;
            }

            public void RemoveTimer(int id)
            {
                if (Timers.ContainsKey(id))
                    Timers.Remove(id);
            }

            public bool TimerTrigger (int id)
            {
                if (!Timers.ContainsKey(id))
                    return false;

                return Timers[id].TriggerThisFrame;
            }

            public void PauseTimer(int id )
            {
                if (Timers.ContainsKey(id))
                    Timers[id].Active = false;
            }

            public void ResumeTimer(int id)
            {
                if (Timers.ContainsKey(id))
                {
                    Timers[id].LastTrigger = LastUpdate;
                    Timers[id].Active = true;
                }
            }
        }

        public static readonly EngineState State = new EngineState();

        public static event EventHandler<EngineState> Update = null;
        public static event EventHandler<EngineState> RenderFrame = null;

        public static void Setup()
		{
			SoundManager.Setup();

			CollisionManager.Initalize();
            TextureManager.Startup();

            Running = true;
		}

        public static void Run()
		{
			Running = true;
            State.Clock.Start();

            bool reallyDone = false;
			while (!reallyDone)
			{
				WindowManager.MainWindow?.Run();

				if (IsRerun)
				{
					RerunFunciton?.Invoke();
					IsRerun = false;
					RerunFunciton = null;
				}
				else
					reallyDone = true;
			}
			
			Running = false;
            State.Clock.Stop();
            TextureManager.Cleanup();
            SoundManager.Cleanup();
        }

		internal static void ReRun(RerunCallback callback)
		{
			RerunFunciton = callback;
			IsRerun = true;
		}

		public static void Exit()
		{
			if (!Running)
				return;

			WindowManager.MainWindow.Exit();
		}

		public static bool IsRunning()
		{
			return Running;
		}

        internal static void UpdateMain()
        {
            State.Now = State.Clock.ElapsedMilliseconds * 0.001;
            State.LastUpdate = State.Now;
            UpdateChild(WindowManager.MainWindowID);

            SoundManager.Update();
        }

        internal static void UpdateChild(int childID)
        {
            State.CurrentContextID = childID;

            // flag any timer events as being fired.
            foreach (var a in State.Timers)
            {
                if (State.Now - a.Value.LastTrigger > a.Value.Interval)
                {
                    a.Value.TriggerThisFrame = true;
                    a.Value.LastTrigger = State.Now;
                }  
            }

            Update?.Invoke(null, State);

            foreach (var a in State.Timers.Values.ToArray())
            {
                if (a.TriggerThisFrame)
                {
                    a.Call(State);
                    if (!a.Recuring)
                        State.Timers.Remove(a.ID);
                }
                a.TriggerThisFrame = false;
            }
        }

        internal static void RenderMain()
        {
            State.Now = State.Clock.ElapsedMilliseconds * 0.001;
            State.LastRender = State.Now;

            RenderChild(WindowManager.MainWindowID);
		}

        internal static void RenderChild(int childID)
        {
            State.CurrentContextID = childID;
            RenderFrame?.Invoke(null, State);

            TextureManager.CheckForPurge();
        }

		public static string GetLocalIPString()
		{
			var host = Dns.GetHostEntry(Dns.GetHostName());
			foreach (var ip in host.AddressList)
			{
				if (ip.AddressFamily == AddressFamily.InterNetwork)
					return ip.ToString();
			}

			return "127.0.0.1";
		}
    }
}
