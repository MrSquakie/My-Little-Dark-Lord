using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Gaia
{
    //Simple static wrapper to access one static Stopwatch instance across all Gaia
    //Can be used to measure performance across multiple parts of the application
    //without having to deal with passing a stopwatch instance around 
    public static class GaiaStopwatch
    {
        public static Stopwatch m_stopwatch = new Stopwatch();
        static long m_lastLogElapsed = 0;
        public static long m_accumulatedYieldTime = 0;

        public static void LogWithTime(string logText)
        {
            UnityEngine.Debug.Log(m_stopwatch.ElapsedMilliseconds.ToString() + " | Diff: " + (m_stopwatch.ElapsedMilliseconds - m_lastLogElapsed).ToString() + " | " + logText);
            m_lastLogElapsed = m_stopwatch.ElapsedMilliseconds;
        }

        public static void CollectYieldTime()
        {
            m_accumulatedYieldTime += m_stopwatch.ElapsedMilliseconds - m_lastLogElapsed;
        }
    }
}