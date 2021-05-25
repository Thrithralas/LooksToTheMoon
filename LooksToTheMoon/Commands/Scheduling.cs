using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using LooksToTheMoon.SchedulingService;

namespace LooksToTheMoon.Commands {
    public class Scheduling : ModuleBase {

        public enum EditorSetting {
            Nothing,
            TimeStamp,
            TimeSpan,
            RepeatInterval,
            Name,
            Description,
            Targeting
        }
        
        [Command("make a schedule")]
        [Alias("schedule an event", "new schedule")]
        public async Task ScheduleEvent([Remainder] string args) {

            string[] rawSplit = args.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            StringBuilder parseQueue = new StringBuilder();
            List<ScheduleSettings> settingCollection = new List<ScheduleSettings>();
            EditorSetting currentScope = EditorSetting.Nothing;
            
            for (int i = 0; i < rawSplit.Length; i++) {
                string currentWord = rawSplit[i].ToLower();
                switch (currentWord) {
                    case "at":
                        settingCollection.Add(new ScheduleSettings(currentScope, parseQueue.ToString()));
                        currentScope = EditorSetting.TimeStamp;
                        parseQueue.Clear();
                        break;
                    case "after":
                    case "in":    
                        settingCollection.Add(new ScheduleSettings(currentScope, parseQueue.ToString()));
                        currentScope = EditorSetting.TimeSpan;
                        parseQueue.Clear();
                        break;
                    case "every":
                        settingCollection.Add(new ScheduleSettings(currentScope, parseQueue.ToString()));
                        currentScope = EditorSetting.RepeatInterval;
                        parseQueue.Clear();
                        break;
                    case "titled":
                    case "dubbed":
                        settingCollection.Add(new ScheduleSettings(currentScope, parseQueue.ToString()));
                        currentScope = EditorSetting.Name;
                        parseQueue.Clear();
                        break;
                    case "described":
                        try {
                            if (rawSplit[i + 1].ToLower() == "as") {
                                i++;
                                settingCollection.Add(new ScheduleSettings(currentScope, parseQueue.ToString()));
                                currentScope = EditorSetting.Description;
                                parseQueue.Clear();
                            }
                            
                        }
                        catch (Exception) {
                            // ignored
                        }
                        break;
                    case "with":
                        try {
                            if (rawSplit[i + 1].ToLower() == "description") {
                                i++;
                                settingCollection.Add(new ScheduleSettings(currentScope, parseQueue.ToString()));
                                currentScope = EditorSetting.Description;
                                parseQueue.Clear();
                            }
                        }
                        catch (Exception) {
                            // ignored
                        }
                        break;
                    case "for":
                        try {
                            string next = rawSplit[i + 1];
                            var strComp = StringComparison.OrdinalIgnoreCase;
                            if (next.Contains("Monkland", strComp) || next.Contains("Jolly", strComp) || next.Contains("Event", strComp)) {
                                settingCollection.Add(new ScheduleSettings(currentScope, parseQueue.ToString()));
                                currentScope = EditorSetting.Targeting;
                                parseQueue.Clear();
                            }
                        }
                        catch (Exception) {
                            // ignored
                        }
                        break;
                    default:
                        parseQueue.Append(" " + rawSplit[i]);
                        break;
                }
            }
            settingCollection.Add(new ScheduleSettings(currentScope, parseQueue.ToString()));
            ScheduleSettings settings = new ScheduleSettings {Channel = Context.Channel.Id, Owner = Context.User.Id, Guild = Context.Guild.Id};
            settingCollection.ForEach(setting => settings.Merge(setting));
            
            ISchedule schedule = settings.ToSchedule();
            StaticScheduleCollection.Schedules.Add(schedule);
            schedule.Start();
            
        }
        
    }
}