namespace MetricsManager.Models
{
    public class RamMetric
    {
        public int Id { get; set; }
        public int AgentID { get; set; }
        public int Value { get; set; }
        public long Time { get; set; }
    }   
}
