using System;
using EPlusActivities.API.Entities;

namespace EPlusActivities.API.Utils
{
    public class AttendanceUtil
    {
        public static bool IsSequential(DateTime? dateTime1, DateTime dateTime2) =>
            dateTime1.HasValue ? dateTime1.Value.AddDays(1).Date == dateTime2.Date : false;

        public static bool AttendHelper(ApplicationUser user)
        {
            var now = DateTime.Now.Date;
            if (user.LastAttendanceDate == now)
            {
                return false;
            }
            else if (IsSequential(user.LastAttendanceDate, now))
            {
                user.SequentialAttendanceDays++;
            }
            else
            {
                user.SequentialAttendanceDays = 1;
            }
            user.LastAttendanceDate = now;
            return true;
        }
    }
}