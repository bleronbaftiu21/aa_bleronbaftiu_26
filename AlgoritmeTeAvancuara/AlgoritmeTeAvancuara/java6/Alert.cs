using System.Globalization;

namespace AlgoritmeTeAvancuara.java6
{
    /// <summary>
    /// Alarmi qe ngrihet kur nje IP arrin pragun. Count, Minutes dhe Servers
    /// llogariten vetem nga deshtimet brenda dritares.
    /// </summary>
    public record Alert(DateTime Timestamp, string Ip, int Count, int Minutes, IReadOnlyList<string> Servers)
    {
        // Kthen alarmin ne formatin qe kerkohet ne detyre.
        public override string ToString()
        {
            // Koha e alarmit si tekst, p.sh. "2025-10-23 14:04:00".
            string time = Timestamp.ToString(LogParser.TimestampFormat, CultureInfo.InvariantCulture);

            // "1 minute" ne njejes, "4 minutes" ne shumes.
            string unit = Minutes == 1 ? "minute" : "minutes";

            // Ndertojme 4 rreshtat e alarmit; serveret ndahen me presje.
            return $"ALERT at {time}:\n" +
                   $"IP: {Ip} - {Count} failed attempts in {Minutes} {unit}\n" +
                   $"Servers affected: {string.Join(", ", Servers)}\n" +
                   "Status: BLACKLISTED";
        }
    }
}
