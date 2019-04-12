namespace IntradayDashboard.WebApi.Model.MQModels.Consumption
{
    public interface ConsumptionDone
    {
        int Id { get; set; }
        int Value {get;set;}
        string Data {get;set;}
        int TenantId { get; set; }

    }
}