namespace IntradayDashboard.WebApi.Model.MQModels.Consumption
{
    public interface ConsumptionMessage
    {
        int Id { get; set; }
        int Value {get;set;}
        string Data {get;set;}
        int TenantId { get; set; }
    }
}