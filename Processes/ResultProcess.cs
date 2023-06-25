namespace Processes
{
    public class ResultProcess
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public string? Output { get; set; }

        public string? Error { get; set; }

        public DateTimeOffset StartTime { get; set; }

        public DateTimeOffset EndTime { get; set; }

        public TimeSpan ExitTime { get; set; }
    }
}
