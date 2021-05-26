using System;

namespace LooksToTheMoon.SchedulingService {
    public interface ISchedule : IDisposable {
        public void Start();
        public void Stop();
        public TimeSpan TimeLeft();
        public Guid Checksum { get; }
        public DateTime? ScheduleAt { get; init; }
        public TimeSpan? ScheduleAfter { get; init; }
        public string Message { get; set; }
        public string Description { get; init; }
        public ulong GuildID { get; init; }
        public ulong ChannelID { get; init; }
        public string Name { get; init; }
        public ulong OwnerID { get; init; }
        
        public static Guid GenerateChecksum() => Guid.NewGuid();
    }
}