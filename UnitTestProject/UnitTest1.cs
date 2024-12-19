using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using static TaskScheduler.MainWindow;

namespace UnitTestProject
{
    [TestClass]
    public class CalendarDayTests
    {
        [TestMethod]
        public void Constructor_InitializesProperties()
        {
            // Arrange
            int day = 10;

            // Act
            CalendarDay calendarDay = new CalendarDay(day);

            // Assert
            Assert.AreEqual(day, calendarDay.Day);
            Assert.AreEqual(false, calendarDay.IsSelected);
        }
    }
}
