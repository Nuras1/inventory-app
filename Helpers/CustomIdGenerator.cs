namespace project_itransition.Helpers
{
    public static class CustomIdGenerator
    {
        public static string Generate(string? format, int sequence)
        {
            if (string.IsNullOrWhiteSpace(format))
            {
                format = "ITEM-{SEQ}";
            }
            return format

                .Replace("{SEQ}", sequence.ToString("D4"))
                .Replace("{YEAR}", DateTime.UtcNow.Year.ToString())
                .Replace("{MONTH}", DateTime.UtcNow.Month.ToString("D2"))
                .Replace("{DAY}", DateTime.UtcNow.Day.ToString("D2"))
                .Replace("{RAND6}", Random.Shared
                        .Next(100000, 999999)
                        .ToString())
                .Replace("{GUID}",
                    Guid.NewGuid()
                        .ToString()[..8]
                        .ToUpper());
        }
    }
}