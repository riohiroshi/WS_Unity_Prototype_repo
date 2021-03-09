using System;
using System.Collections.Generic;
using UnityEngine;

using GenericUtility.Singleton;

namespace GenericUtility.Timer
{
    public class TimerManager : Singleton<TimerManager>
    {
        private void Update() { Timer.UpdateAllTimer(); }
    }

    [Serializable]
    public class Timer
    {
        public static TimerManager _timerManager = null;

        private static List<Timer> _timerList = new List<Timer>();

        private static bool _showLog = true;

        public static bool ShowLog { set => _showLog = value; }

        public float Duration => _duration;

        public bool IsPause
        {
            get => _isPause;
            set
            {
                if (value) { Pause(); }
                else { Resume(); }
            }
        }

        private float CurrentTime => _ignorTimescale ? Time.realtimeSinceStartup : Time.time;

        private Action<float> _updateEvent;
        private Action _endEvent;

        private float _duration = -1;
        private bool _loop;
        private bool _ignorTimescale;
        private string _flag;

        private float cachedTime;

        private float _timePassed;

        private bool _isFinish = false;
        private bool _isPause = false;

        private Timer(float duration, string flag, bool loop = false, bool ignorTimescale = true)
        {
            if (_timerManager is null) { _timerManager = TimerManager.Instance; }

            _duration = duration;
            _loop = loop;
            _ignorTimescale = ignorTimescale;
            cachedTime = CurrentTime;

            if (_timerList.Exists((timer) => { return timer._flag == flag; }))
            {
                if (_showLog) { Debug.LogWarningFormat("Same timer flag {0} is Existing.", flag); }
            }

            _flag = string.IsNullOrEmpty(flag) ? GetHashCode().ToString() : flag;
        }

        #region Public Method

        public static Timer AddTimer(float duration, string flag = "", bool loop = false, bool ignorTimescale = true)
        {
            Timer timer = new Timer(duration, flag, loop, ignorTimescale);
            _timerList.Add(timer);
            return timer;
        }

        public static void UpdateAllTimer()
        {
            for (int i = 0; i < _timerList.Count; i++)
            {
                if (_timerList[i] is null) { continue; }

                _timerList[i].Update();
            }
        }

        public static bool Exist(string flag) => _timerList.Exists((timer) => { return timer._flag == flag; });

        public static bool Exist(Timer timer) => _timerList.Contains(timer);

        public static Timer GetTimer(string flag) => _timerList.Find((timer) => { return timer._flag == flag; });

        public static void Pause(string flag)
        {
            Timer timer = GetTimer(flag);

            if (timer != null)
            {
                timer.Pause();
                return;
            }

            if (_showLog) { Debug.Log("Timer is null, flag (" + flag + ")."); }
        }

        public static void Pause(Timer timer)
        {
            if (Exist(timer))
            {
                timer.Pause();
                return;
            }

            if (_showLog) { Debug.Log("Timer is null."); }
        }

        public static void Resume(string flag)
        {
            Timer timer = GetTimer(flag);

            if (timer != null)
            {
                timer.Resume();
                return;
            }

            if (_showLog) { Debug.Log("Timer is null, flag (" + flag + ")."); }
        }

        public static void Resume(Timer timer)
        {
            if (Exist(timer))
            {
                timer.Resume();
                return;
            }

            if (_showLog) { Debug.Log("Timer is null."); }
        }

        public static void DelTimer(string flag)
        {
            Timer timer = GetTimer(flag);

            if (timer != null)
            {
                timer.Stop();
                return;
            }

            if (_showLog) { Debug.Log("Timer is null, flag (" + flag + ")."); }
        }

        public static void DelTimer(Timer timer)
        {
            if (Exist(timer))
            {
                timer.Stop();
                return;
            }

            if (_showLog) { Debug.Log("Timer is null."); }
        }

        public static void DelTimer(Action completedEvent)
        {
            Timer timer = _timerList.Find((tmr) => { return tmr._endEvent == completedEvent; });

            if (timer != null)
            {
                timer.Stop();
                return;
            }

            if (_showLog) { Debug.Log("Timer is null, method name (" + completedEvent.Method.Name + ")."); }
        }

        public static void DelTimer(Action<float> updateEvent)
        {
            Timer timer = _timerList.Find((tmr) => { return tmr._updateEvent == updateEvent; });

            if (timer != null)
            {
                timer.Stop();
                return;
            }

            if (_showLog) { Debug.Log("Timer is null, method name (" + updateEvent.Method.Name + ")."); }
        }

        public static void RemoveAll()
        {
            _timerList.ForEach((timer) => { timer.Stop(); });
            _timerList.Clear();
        }

        public void AddEvent(Action completedEvent)
        {
            if (_endEvent == null)
            {
                _endEvent = completedEvent;
                return;
            }

            Delegate[] delegates = _endEvent.GetInvocationList();

            if (!Array.Exists(delegates, (evt) => { return evt == (Delegate)completedEvent; })) { _endEvent += completedEvent; }
        }

        public void AddEvent(Action<float> updateEvent)
        {
            if (_updateEvent is null)
            {
                _updateEvent = updateEvent;
                return;
            }

            Delegate[] delegates = _updateEvent.GetInvocationList();

            if (!Array.Exists(delegates, (checkEvent) => { return checkEvent == (Delegate)updateEvent; })) { _updateEvent += updateEvent; }
        }

        public Timer SetDuration(float endTime)
        {
            if (_isFinish)
            {
                if (_showLog) { Debug.Log("Timer is Finished. Set Duration Failed."); }
                return this;
            }

            if (endTime == _duration)
            {
                if (_showLog) { Debug.Log("Duration is Set. Set Duration Failed."); }
                return this;
            }

            if (endTime < 0)
            {
                if (_showLog) { Debug.Log("Duration could not be negative, set the absolute value."); }
                endTime = Mathf.Abs(endTime);
            }

            if (endTime < _timePassed)
            {
                if (_showLog) { Debug.LogFormat("Set value is too short, timer is already passed this duration, timer is finished immediately. Passed : Set => {0} : {1}.", _timePassed, endTime); }
            }

            _duration = endTime;
            return this;
        }

        public Timer Setloop(bool loop)
        {
            if (_isFinish)
            {
                if (_showLog) { Debug.Log("Timer is Finished. Set Loop Failed."); }
                return this;
            }

            _loop = loop;
            return this;
        }

        public Timer SetIgnoreTimeScale(bool ignoreTimescale)
        {
            if (_isFinish)
            {
                if (_showLog) { Debug.Log("Timer is Finished. Set IgnoreTimescale Failed."); }
                return this;
            }

            _ignorTimescale = ignoreTimescale;
            return this;
        }

        #endregion

        private void Pause()
        {
            if (_isFinish)
            {
                if (_showLog) { Debug.LogWarning("Timer is Finished!"); }
                return;
            }

            _isPause = true;
        }

        private void Resume()
        {
            if (_isFinish)
            {
                if (_showLog) Debug.LogWarning("Timer is Finished!");
                return;
            }

            if (!_isPause)
            {
                if (_showLog) Debug.LogWarning("Timer is not Paused!");
                return;
            }

            cachedTime = CurrentTime - _timePassed;
            _isPause = false;
        }

        private void Update()
        {
            if (_isFinish || _isPause) { return; }

            _timePassed = CurrentTime - cachedTime;
            _updateEvent?.Invoke(Mathf.Clamp01(_timePassed / _duration));

            if (_timePassed < _duration) { return; }

            _endEvent?.Invoke();

            if (_loop)
            {
                cachedTime = CurrentTime;
                return;
            }

            Stop();
        }

        private void Stop()
        {
            if (_timerList.Contains(this)) { _timerList.Remove(this); }

            _duration = -1;
            _isFinish = true;
            _isPause = false;
            _updateEvent = null;
            _endEvent = null;
        }
    }

    public static class TimerExtend
    {
        public static Timer OnCompleted(this Timer timer, Action completedEvent)
        {
            if (timer is null) { return null; }

            timer.AddEvent(completedEvent);
            return timer;
        }

        public static Timer OnUpdated(this Timer timer, Action<float> updateEvent)
        {
            if (timer is null) { return null; }

            timer.AddEvent(updateEvent);
            return timer;
        }
    }
}