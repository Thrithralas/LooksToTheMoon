using System;
using System.Text;

namespace LooksToTheMoon {
    public static class ExtensionMethods {
        public static string FormatTimeSpan(this TimeSpan timeSpan, bool useMillis = true, bool useSeconds = true) {

            StringBuilder builder = new StringBuilder();
            if (timeSpan.Hours > 0) builder.Append($"**{timeSpan.Hours}** hrs ");
            if (timeSpan.Minutes > 0 || builder.Length > 0) builder.Append($"**{timeSpan.Minutes}** mins ");
            if (useSeconds && (timeSpan.Seconds > 0 || builder.Length > 0)) builder.Append($"**{timeSpan.Seconds}** s ");
            if (useMillis) builder.Append($"**{timeSpan.Milliseconds}** ms");

            return builder.ToString();
        }
    }
}