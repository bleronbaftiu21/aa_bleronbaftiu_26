namespace AlgoritmeTeAvancuara.java6
{
    /// <summary>
    /// Gjen IP-te me N ose me shume deshtime brenda nje dritareje kohore rreshqitese.
    ///
    ///  - failures : Dictionary (hash table) IP -> Queue me deshtimet brenda dritares.
    ///               Kerkimi i IP-se eshte O(1) mesatarisht.
    ///  - Queue    : deshtimet vijne te renditura sipas kohes, keshtu qe te vjetrat
    ///               jane gjithmone ne fillim (Dequeue) e te rejat ne fund (Enqueue).
    ///  - blacklist: HashSet me IP-te e bllokuara; IsBlacklisted eshte O(1).
    ///
    /// Supozim: log-et vijne te renditura sipas timestamp-it (si ne nje stream real-time).
    /// </summary>
    public class IntrusionDetector
    {
        // Pragu N: sa deshtime duhen brenda dritares qe IP te bllokohet.
        private readonly int _threshold;

        // Gjatesia e dritares kohore (p.sh. 10 minuta).
        private readonly TimeSpan _window;

        // Hash table: per cdo IP mbajme nje radhe (Queue) me deshtimet e saj brenda dritares.
        // Deshtimet nga te gjithe serveret e se njejtes IP shkojne ne te njejten radhe.
        private readonly Dictionary<string, Queue<LoginAttempt>> _failures = new();

        // Bashkesia e IP-ve te bllokuara (blacklist). HashSet nuk lejon dyfishime.
        private readonly HashSet<string> _blacklist = new();

        // Konstruktori kryesor: merr pragun dhe dritaren si TimeSpan.
        public IntrusionDetector(int threshold, TimeSpan window)
        {
            // Pragu duhet te jete te pakten 1, perndryshe s'ka kuptim.
            if (threshold < 1) throw new ArgumentOutOfRangeException(nameof(threshold));

            // Dritarja nuk mund te jete negative.
            if (window < TimeSpan.Zero) throw new ArgumentOutOfRangeException(nameof(window));

            // Ruajme parametrat qe te perdoren ne Process.
            _threshold = threshold;
            _window = window;
        }

        // Konstruktor ndihmes: dritarja jepet ne minuta (p.sh. 10) dhe kalohet te konstruktori kryesor.
        public IntrusionDetector(int threshold, int windowMinutes)
            : this(threshold, TimeSpan.FromMinutes(windowMinutes)) { }

        /// <summary>
        /// Perpunon nje tentim. Kthen Alert vetem heren e pare qe IP arrin pragun,
        /// perndryshe null.
        /// </summary>
        public Alert? Process(LoginAttempt attempt)
        {
            // Hapi 1: tentimet SUCCESS nuk numerohen, i injorojme menjehere.
            if (attempt.IsSuccess) return null;

            // Hapi 2: kerkojme radhen e kesaj IP-je ne hash table (O(1)).
            if (!_failures.TryGetValue(attempt.Ip, out var queue))
            {
                // IP shihet per here te pare -> krijojme nje radhe te re bosh...
                queue = new Queue<LoginAttempt>();
                // ...dhe e vendosim ne hash table nen celesin e IP-se.
                _failures[attempt.Ip] = queue;
            }

            // Hapi 3: shtojme deshtimin ne fund te radhes (me i riu).
            // Edhe IP-te e bllokuara vazhdojne te regjistrohen.
            queue.Enqueue(attempt);

            // Hapi 4: rreshqasim dritaren.
            // Fillimi i dritares = koha e tentimit aktual minus gjatesia e dritares.
            DateTime windowStart = attempt.Timestamp - _window;

            // Heqim nga fillimi i radhes deshtimet qe kane dale jashte dritares.
            // Krahasimi eshte STRIKT (<), prandaj dritarja eshte inkluzive:
            // me dritare 10 min, deshtimi ne 14:00 ende numerohet ne 14:10.
            // Radha s'mbetet kurre bosh ketu, sepse deshtimi aktual eshte gjithmone brenda.
            while (queue.Peek().Timestamp < windowStart)
                queue.Dequeue();

            // Hapi 5: nese ende s'kemi arritur pragun, s'ka alarm.
            if (queue.Count < _threshold) return null;

            // Hapi 6: shtojme IP-ne ne blacklist.
            // HashSet.Add kthen false nese IP ishte tashme aty -> nuk japim alarm te dyfishte.
            if (!_blacklist.Add(attempt.Ip)) return null;

            // Hapi 7: hera e pare qe IP arrin pragun -> ndertojme dhe kthejme alarmin.
            return BuildAlert(attempt, queue);
        }

        // Kontrollon nese nje IP eshte e bllokuar: kerkim O(1) ne HashSet.
        public bool IsBlacklisted(string ip) => _blacklist.Contains(ip);

        /// <summary>Numri i deshtimeve aktualisht ne dritaren e IP-se (0 nese s'ka).</summary>
        public int FailuresInWindow(string ip) =>
            // Nese IP ekziston ne hash table kthejme madhesine e radhes, perndryshe 0.
            _failures.TryGetValue(ip, out var queue) ? queue.Count : 0;

        // Nderton alarmin nga deshtimet qe jane brenda dritares.
        private static Alert BuildAlert(LoginAttempt current, Queue<LoginAttempt> queue)
        {
            // Deshtimi me i vjeter brenda dritares eshte ne fillim te radhes.
            DateTime first = queue.Peek().Timestamp;

            // Sa minuta kane kaluar nga deshtimi i pare deri te ai aktual (p.sh. 14:00 -> 14:04 = 4).
            int minutes = (int)(current.Timestamp - first).TotalMinutes;

            // SortedSet mban serveret pa dyfishime dhe te renditur (WEB-01, WEB-02, ...).
            var servers = new SortedSet<string>(StringComparer.Ordinal);

            // Kalojme neper te gjitha deshtimet ne dritare dhe mbledhim serverin e secilit.
            foreach (var a in queue) servers.Add(a.Server);

            // Krijojme alarmin: koha, IP, numri i deshtimeve, minutat dhe lista e serverave.
            return new Alert(current.Timestamp, current.Ip, queue.Count, minutes, servers.ToList());
        }
    }
}
