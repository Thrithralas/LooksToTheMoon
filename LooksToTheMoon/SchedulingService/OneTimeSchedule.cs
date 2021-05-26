using System;
using System.Linq;
using System.Timers;
using Discord;
using Discord.WebSocket;

namespace LooksToTheMoon.SchedulingService {
    public class OneTimeSchedule : ISchedule {
        
        public DateTime? ScheduleAt { get; init; }
        public TimeSpan? ScheduleAfter { get; init; }
        public string Message { get; set; }
        public string Description { get; init; }
        public ulong GuildID { get; init; }
        public ulong ChannelID { get; init; }
        public string Name { get; init; }
        public ulong OwnerID { get; init; }
        private bool _disposed;
        
        public Guid Checksum { get; } = ISchedule.GenerateChecksum();

        private Timer _timer;
        private DateTime _started;

        public void Start() {
            _started = DateTime.Now;
            _timer = new Timer {Interval = (ScheduleAt - _started)?.TotalMilliseconds ?? ScheduleAfter?.TotalMilliseconds ?? 0};
            _timer.Elapsed += OnTimerExpire;
            _timer.Start();
        }

        public void Stop() {
            _timer.Stop();
            _timer.Elapsed -= OnTimerExpire;
            Dispose();
        }

        public TimeSpan TimeLeft() {
            return ScheduleAfter is null ? ScheduleAt!.Value - DateTime.Now : _started + ScheduleAfter.Value - DateTime.Now;
        }


        private void OnTimerExpire(object sender, ElapsedEventArgs args) {
            SocketGuild socketGuild = Core.Client.GetGuild(GuildID);
            SocketGuildChannel socketGuildChannel = socketGuild.Channels.First(c => c.Id == ChannelID);
            ((SocketTextChannel) socketGuildChannel).SendMessageAsync(Message);
            Stop();
        }

        public void Dispose() {
            _timer?.Dispose();
            StaticScheduleCollection.RemoveSchedule(this);
            _disposed = true;
        }

        ~OneTimeSchedule() {
            if (!_disposed) Dispose();
        }
    }
}