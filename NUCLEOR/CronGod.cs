using _UTIL_;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace _ARK_
{
    public interface ICron : IDisposable
    {
        bool Disposed { get; }
        void Cron();
    }

    public class CronGod : Disposable
    {
        readonly List<(float time, ICron cron)> crons = new();

        //----------------------------------------------------------------------------------------------------------

        public void Add(in float time, in ICron cron)
        {
            lock (crons)
                if (crons.Contains((time, cron)))
                    throw new Exception($"{this}.{nameof(Add)}({nameof(cron)}) -> {cron} is already scheduled");
                else
                    for (int i = 0; i <= crons.Count; i++)
                        if (i == crons.Count)
                            crons.Add((time, cron));
                        else if (time <= crons[i].time)
                        {
                            crons.Insert(i, (time, cron));
                            return;
                        }
        }

        public void Tick()
        {
            lock (crons)
                while (crons.Count > 0 && crons[0].time <= Time.unscaledTime)
                {
                    if (!crons[0].cron.Disposed)
                        crons[0].cron.Cron();
                    crons.RemoveAt(0);
                }
        }

        //----------------------------------------------------------------------------------------------------------

        protected override void OnDispose()
        {
            base.OnDispose();

            lock (crons)
            {
                foreach ((_, ICron cron) in crons)
                    cron.Dispose();
                crons.Clear();
            }
        }
    }
}