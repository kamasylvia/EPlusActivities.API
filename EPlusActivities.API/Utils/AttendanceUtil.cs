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

            #region Update LastAttendanceDate and SequentialAttendanceDays
            user.SequentialAttendanceDays = 
                IsSequential(user.LastAttendanceDate, now)
                ? user.SequentialAttendanceDays + 1
                : 1;
            #endregion

            #region Update credit
            user.Credit += 
                user.SequentialAttendanceDays < 7
                ? user.SequentialAttendanceDays * 10
                : 70;
            #endregion

            user.LastAttendanceDate = now;
            return true;
        }
    }
}