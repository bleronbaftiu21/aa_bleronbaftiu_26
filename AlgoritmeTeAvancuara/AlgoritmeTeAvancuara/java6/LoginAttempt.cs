namespace AlgoritmeTeAvancuara.java6
{
    /// <summary>Nje rresht log-u: kur, nga cila IP, ne cilin server dhe a pati sukses.</summary>
    // Timestamp = koha e tentimit, Ip = adresa, Server = ku u tentua,
    // IsSuccess = true per SUCCESS, false per FAIL.
    public record LoginAttempt(DateTime Timestamp, string Ip, string Server, bool IsSuccess);
}
