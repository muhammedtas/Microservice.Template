namespace IntradayDashboard.WebApi.Model.MQModels.Offer
{
    public interface OfferMessage 
    {
        int Id { get; set; }
        int Value {get;set;}
        string Data {get;set;}
        int TenantId { get; set; }
    }
}