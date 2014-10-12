using System;

namespace Ext.Core
{
    public static class DateTimeHelper
    {
        public static TimeSpan MeasureOperationTime(Action action)
        {
            var startTime = DateTime.Now;

            action.Invoke();

            var endTime = DateTime.Now;

            return endTime - startTime;
        }
    }
}
