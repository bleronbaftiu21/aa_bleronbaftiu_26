/*
  Detyra: Network Intrusion Detection - IP Address Tracking
  Prag: 3 deshtime brenda 10 minutave (dritare inkluzive).
*/

namespace AlgoritmeTeAvancuara.java6
{
    public class Detyra
    {
        static readonly string[] Logs =
        {
            "{\"timestamp\": \"2025-10-23 14:00:00\", \"ip\": \"192.168.1.100\", \"server\": \"WEB-01\", \"status\": \"FAIL\"}",
            "{\"timestamp\": \"2025-10-23 14:02:00\", \"ip\": \"192.168.1.100\", \"server\": \"WEB-02\", \"status\": \"FAIL\"}",
            "{\"timestamp\": \"2025-10-23 14:03:00\", \"ip\": \"10.0.0.50\", \"server\": \"WEB-01\", \"status\": \"SUCCESS\"}",
            "{\"timestamp\": \"2025-10-23 14:04:00\", \"ip\": \"192.168.1.100\", \"server\": \"WEB-03\", \"status\": \"FAIL\"}",
            "{\"timestamp\": \"2025-10-23 14:05:00\", \"ip\": \"192.168.1.100\", \"server\": \"WEB-01\", \"status\": \"FAIL\"}",
            "{\"timestamp\": \"2025-10-23 14:06:00\", \"ip\": \"10.0.0.75\", \"server\": \"WEB-02\", \"status\": \"FAIL\"}",
            "{\"timestamp\": \"2025-10-23 14:07:00\", \"ip\": \"10.0.0.75\", \"server\": \"WEB-02\", \"status\": \"FAIL\"}",
            "{\"timestamp\": \"2025-10-23 14:08:00\", \"ip\": \"10.0.0.75\", \"server\": \"WEB-03\", \"status\": \"FAIL\"}",
        };

        public static void Run()
        {
            // Titulli dhe parametrat e detyres.
            Console.WriteLine("\n--- java6: Network Intrusion Detection ---");
            Console.WriteLine("Threshold: 3 failed attempts within 10 minutes\n");

            // Krijojme detektorin: 3 deshtime brenda 10 minutave.
            var detector = new IntrusionDetector(threshold: 3, windowMinutes: 10);

            // Perpunojme log-et nje nga nje, ne rendin kohor (si stream real-time).
            foreach (var line in Logs)
            {
                // JSON -> LoginAttempt -> detektori; kthen Alert ose null.
                var alert = detector.Process(LogParser.Parse(line));

                // Shtypim vetem kur ka alarm (heren e pare qe IP arrin pragun).
                if (alert != null)
                {
                    Console.WriteLine(alert);
                    Console.WriteLine();
                }
            }

            // Ne fund kontrollojme blacklist-en per disa IP.
            foreach (var ip in new[] { "192.168.1.100", "10.0.0.75", "10.0.0.50" })
                Console.WriteLine($"IsBlacklisted({ip}) = {detector.IsBlacklisted(ip)}");
        }
    }
}
