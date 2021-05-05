using System;
using Xunit;

namespace Airframe.Tests.Timers
{
    internal class DivisibleTimeSpanData : TheoryData
    {
        public DivisibleTimeSpanData()
        {
            AddRow(3, TimeSpan.FromHours(2));
            AddRow(108, TimeSpan.FromMinutes(45));
            AddRow(108, TimeSpan.FromMinutes(55));
            AddRow(108, TimeSpan.FromMinutes(65));
            AddRow(1, TimeSpan.FromMinutes(3));
            AddRow(10, TimeSpan.FromMinutes(30));
            AddRow(100, TimeSpan.FromMinutes(3000));
            AddRow(3, TimeSpan.FromMinutes(700));
            AddRow(2, TimeSpan.FromMinutes(5));
            AddRow(108, TimeSpan.FromSeconds(45));
            AddRow(108, TimeSpan.FromSeconds(55));
            AddRow(108, TimeSpan.FromSeconds(65));
            AddRow(1, TimeSpan.FromSeconds(3));
            AddRow(10, TimeSpan.FromSeconds(30));
            AddRow(100, TimeSpan.FromSeconds(3000));
            AddRow(3, TimeSpan.FromSeconds(700));
            AddRow(2, TimeSpan.FromSeconds(5));
        }
    }
}