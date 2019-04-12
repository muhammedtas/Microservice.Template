namespace IntradayDashboard.WebApi.Model.MQModels.Offer
{
    public interface OfferDone
    {
        int Id { get; set; }
        int Value {get;set;}
        string Data {get;set;}
        int TenantId { get; set; }
    }
}