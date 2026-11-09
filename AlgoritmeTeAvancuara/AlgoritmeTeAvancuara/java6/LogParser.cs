using System.Globalization;
using System.Text.Json;

namespace AlgoritmeTeAvancuara.java6
{
    /// <summary>
    /// Kthen nje rresht JSON si
    /// {"timestamp": "2025-10-23 14:00:00", "ip": "192.168.1.100", "server": "WEB-01", "status": "FAIL"}
    /// ne nje LoginAttempt.
    /// </summary>
    public static class LogParser
    {
        // Formati i kohes ne log-e, p.sh. "2025-10-23 14:00:00".
        public const string TimestampFormat = "yyyy-MM-dd HH:mm:ss";

        public static LoginAttempt Parse(string jsonLine)
        {
            // Lexojme tekstin JSON; "using" e liron memorien kur mbarojme.
            using var doc = JsonDocument.Parse(jsonLine);

            // Objekti kryesor { ... } i rreshtit.
            var root = doc.RootElement;

            // Lexojme fushen "timestamp" dhe e kthejme ne DateTime sipas formatit te mesiperm.
            var timestamp = DateTime.ParseExact(root.GetProperty("timestamp").GetString()!,
                                                TimestampFormat, CultureInfo.InvariantCulture);

            // Lexojme adresen IP.
            var ip = root.GetProperty("ip").GetString()!;

            // Lexojme serverin (p.sh. WEB-01).
            var server = root.GetProperty("server").GetString()!;

            // Lexojme statusin: "SUCCESS" ose "FAIL".
            var status = root.GetProperty("status").GetString()!;

            // Krijojme LoginAttempt; IsSuccess = true vetem kur statusi eshte "SUCCESS".
            return new LoginAttempt(timestamp, ip, server,
                                    status.Equals("SUCCESS", StringComparison.OrdinalIgnoreCase));
        }
    }
}
