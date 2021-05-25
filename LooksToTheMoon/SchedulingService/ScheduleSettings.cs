using System;
using System.Text;
using Discord.WebSocket;
using LooksToTheMoon.Commands;

namespace LooksToTheMoon.SchedulingService {
    public class ScheduleSettings {

        public bool Monkland;
        public bool JollyCoop;
        public bool Event;

        public DateTime? TimeStamp;
        public TimeSpan? After;
        public TimeSpan? RepeatInterval;

        public ulong Channel;
        public ulong Guild;
        public ulong Owner;

        public string Description;
        public string Name;

        public static ScheduleSettings Empty = new ScheduleSettings();

        public ISchedule ToSchedule() {
            
            ISchedule schedule = null;
            if (RepeatInterval is null) {
                
                schedule = new OneTimeSchedule {GuildID = Guild, ChannelID = Channel, ScheduleAfter = After, ScheduleAt = TimeStamp, Description = Description, OwnerID = Owner};
                SocketGuild socketGuild = Core.Client.GetGuild(Guild);
                
                SocketRole monkLand = socketGuild.GetRole(AppConfiguration.MonklandRoleID);
                SocketRole jollyCoop = socketGuild.GetRole(AppConfiguration.JollyCoopRoleID);
                SocketRole events = socketGuild.GetRole(AppConfiguration.EventRoleID);

                StringBuilder message = new StringBuilder();
                if (Monkland) message.Append(monkLand.Mention + " ");
                if (JollyCoop) message.Append(jollyCoop.Mention + " ");
                if (Event) message.Append(events.Mention + " ");
                
                message.AppendLine($"\nAn event is starting titled **{Name}**, hosted by {Core.Client.GetUser(Owner).Mention}! Please get in the appropriate voice chats!");
                message.Append($"**More Information:** {Description}");
                schedule.Message = message.ToString();

            }

            return schedule;
        }

        public void Merge(ScheduleSettings settings, bool prioritizeThis = false) {
            
            Monkland |= settings.Monkland;
            JollyCoop |= settings.JollyCoop;
            Event |= settings.Event;

            if (TimeStamp is null || !prioritizeThis && settings.TimeStamp is not null) TimeStamp = settings.TimeStamp;
            if (After is null || !prioritizeThis && settings.After is not null) After = settings.After;
            if (RepeatInterval is null || !prioritizeThis && settings.RepeatInterval is not null) RepeatInterval = settings.RepeatInterval;

            Description = prioritizeThis ? Description : settings.Description ?? Description;
            Name = prioritizeThis ? Name : settings.Name ?? Name;

        }

        public ScheduleSettings(Scheduling.EditorSetting editorSetting, string content) {
            content = content.Trim();
            switch (editorSetting) {
                case Scheduling.EditorSetting.Nothing:
                    return;
                case Scheduling.EditorSetting.TimeStamp:
                    TimeStamp = DateTime.Parse(content);
                    break;
                case Scheduling.EditorSetting.TimeSpan:
                    After = TimeSpan.Parse(content);
                    break;
                case Scheduling.EditorSetting.RepeatInterval:
                    RepeatInterval = TimeSpan.Parse(content);
                    break;
                case Scheduling.EditorSetting.Name:
                    Name = content;
                    break;
                case Scheduling.EditorSetting.Description:
                    Description = content;
                    break;
                case Scheduling.EditorSetting.Targeting:
                    Monkland = content.Contains("Monkland", StringComparison.OrdinalIgnoreCase);
                    JollyCoop = content.Contains("Jolly", StringComparison.OrdinalIgnoreCase) || content.Contains("JollyCoop", StringComparison.OrdinalIgnoreCase);
                    Event = content.Contains("Event", StringComparison.OrdinalIgnoreCase);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(editorSetting), editorSetting, null);
            }
        }

        public ScheduleSettings() {}
    }
}