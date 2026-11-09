using AlgoritmeTeAvancuara.java6;
using Xunit;

namespace AlgoritmeTeAvancuara.Tests
{
    public class IntrusionDetectorTests
    {
        static readonly DateTime T0 = new(2025, 10, 23, 14, 0, 0);

        static LoginAttempt Fail(int minute, string ip = "1.1.1.1", string server = "WEB-01") =>
            new(T0.AddMinutes(minute), ip, server, IsSuccess: false);

        static LoginAttempt Success(int minute, string ip = "1.1.1.1", string server = "WEB-01") =>
            new(T0.AddMinutes(minute), ip, server, IsSuccess: true);

        [Fact]
        public void Threshold_AlertOnlyWhenReached()
        {
            var d = new IntrusionDetector(3, 10);

            Assert.Null(d.Process(Fail(0)));
            Assert.Null(d.Process(Fail(1)));
            var alert = d.Process(Fail(2));

            Assert.NotNull(alert);
            Assert.Equal(3, alert!.Count);
            Assert.Equal(2, alert.Minutes);
        }

        [Fact]
        public void Window_IsInclusiveAtBoundary()
        {
            // PDF: me dritare 10 min, 14:00, 14:05 dhe 14:10 numerohen bashke.
            var d = new IntrusionDetector(3, 10);

            d.Process(Fail(0));
            d.Process(Fail(5));
            var alert = d.Process(Fail(10));

            Assert.NotNull(alert);
            Assert.Equal(3, alert!.Count);
            Assert.Equal(10, alert.Minutes);
        }

        [Fact]
        public void ExpiredAttempts_AreRemovedFromWindow()
        {
            var d = new IntrusionDetector(3, 10);

            d.Process(Fail(0));
            d.Process(Fail(5));
            // 14:11 -> 14:00 eshte jashte dritares [14:01, 14:11]
            Assert.Null(d.Process(Fail(11)));
            Assert.Equal(2, d.FailuresInWindow("1.1.1.1"));
            Assert.False(d.IsBlacklisted("1.1.1.1"));
        }

        [Fact]
        public void MultipleServers_AreCombinedForSameIp()
        {
            var d = new IntrusionDetector(3, 10);

            d.Process(Fail(0, server: "WEB-02"));
            d.Process(Fail(1, server: "WEB-01"));
            var alert = d.Process(Fail(2, server: "WEB-03"));

            Assert.NotNull(alert);
            Assert.Equal(new[] { "WEB-01", "WEB-02", "WEB-03" }, alert!.Servers);
        }

        [Fact]
        public void Success_IsIgnored()
        {
            var d = new IntrusionDetector(3, 10);

            d.Process(Fail(0));
            Assert.Null(d.Process(Success(1)));
            Assert.Null(d.Process(Success(2)));
            Assert.Null(d.Process(Fail(3)));

            Assert.Equal(2, d.FailuresInWindow("1.1.1.1"));
            Assert.False(d.IsBlacklisted("1.1.1.1"));

            Assert.NotNull(d.Process(Fail(4)));
        }

        [Fact]
        public void SuccessOnly_NeverBlacklists()
        {
            var d = new IntrusionDetector(1, 10);
            Assert.Null(d.Process(Success(0)));
            Assert.False(d.IsBlacklisted("1.1.1.1"));
            Assert.Equal(0, d.FailuresInWindow("1.1.1.1"));
        }

        [Fact]
        public void Blacklist_IsSetWhenThresholdReached()
        {
            var d = new IntrusionDetector(2, 10);

            Assert.False(d.IsBlacklisted("1.1.1.1"));
            d.Process(Fail(0));
            Assert.False(d.IsBlacklisted("1.1.1.1"));
            d.Process(Fail(1));
            Assert.True(d.IsBlacklisted("1.1.1.1"));
            Assert.False(d.IsBlacklisted("9.9.9.9"));
        }

        [Fact]
        public void NoDuplicateAlerts_ButAttemptsStillRecorded()
        {
            var d = new IntrusionDetector(3, 10);

            d.Process(Fail(0));
            d.Process(Fail(1));
            Assert.NotNull(d.Process(Fail(2)));

            Assert.Null(d.Process(Fail(3)));
            Assert.Null(d.Process(Fail(4)));
            Assert.Equal(5, d.FailuresInWindow("1.1.1.1"));

            // edhe pasi dritarja rinis plotesisht, nuk ka alarm te ri
            Assert.Null(d.Process(Fail(30)));
            Assert.Null(d.Process(Fail(31)));
            Assert.Null(d.Process(Fail(32)));
            Assert.True(d.IsBlacklisted("1.1.1.1"));
        }

        [Fact]
        public void MultipleIps_AreTrackedIndependently()
        {
            var d = new IntrusionDetector(2, 10);

            Assert.Null(d.Process(Fail(0, ip: "A")));
            Assert.Null(d.Process(Fail(1, ip: "B")));
            var alertA = d.Process(Fail(2, ip: "A"));
            Assert.Null(d.Process(Fail(3, ip: "C")));
            var alertB = d.Process(Fail(4, ip: "B"));

            Assert.Equal("A", alertA!.Ip);
            Assert.Equal("B", alertB!.Ip);
            Assert.True(d.IsBlacklisted("A"));
            Assert.True(d.IsBlacklisted("B"));
            Assert.False(d.IsBlacklisted("C"));
        }

        [Fact]
        public void PdfExample_ProducesExpectedOutput()
        {
            string[] logs =
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

            var d = new IntrusionDetector(3, 10);
            var alerts = logs.Select(l => d.Process(LogParser.Parse(l)))
                             .Where(a => a != null)
                             .Select(a => a!.ToString())
                             .ToList();

            Assert.Equal(new[]
            {
                "ALERT at 2025-10-23 14:04:00:\n" +
                "IP: 192.168.1.100 - 3 failed attempts in 4 minutes\n" +
                "Servers affected: WEB-01, WEB-02, WEB-03\n" +
                "Status: BLACKLISTED",

                "ALERT at 2025-10-23 14:08:00:\n" +
                "IP: 10.0.0.75 - 3 failed attempts in 2 minutes\n" +
                "Servers affected: WEB-02, WEB-03\n" +
                "Status: BLACKLISTED",
            }, alerts);

            Assert.True(d.IsBlacklisted("192.168.1.100"));
            Assert.True(d.IsBlacklisted("10.0.0.75"));
            Assert.False(d.IsBlacklisted("10.0.0.50"));
        }

        [Fact]
        public void LogParser_ReadsAllFields()
        {
            var a = LogParser.Parse(
                "{\"timestamp\": \"2025-10-23 14:03:00\", \"ip\": \"10.0.0.50\", \"server\": \"WEB-01\", \"status\": \"SUCCESS\"}");

            Assert.Equal(new DateTime(2025, 10, 23, 14, 3, 0), a.Timestamp);
            Assert.Equal("10.0.0.50", a.Ip);
            Assert.Equal("WEB-01", a.Server);
            Assert.True(a.IsSuccess);
        }
    }
}
