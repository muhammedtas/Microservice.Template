namespace IntradayDashboard.WebApi.Model
{
    public class ConsumptionModel
    {
        public int Id { get; set; }
        public int Value;
        public string Data;
        public int TenantId { get; set; }
    }
}