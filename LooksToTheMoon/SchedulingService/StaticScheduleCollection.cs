using System.Collections.Generic;

namespace LooksToTheMoon.SchedulingService {
    public static class StaticScheduleCollection {
        
        public static List<ISchedule> Schedules = new List<ISchedule>();

        public static void RemoveSchedule(ISchedule schedule) {
            Schedules.RemoveAll(s => s.Checksum == schedule.Checksum);
        }
    }
}