namespace IntradayDashboard.WebApi.Dto
{
    public class OfferDto
    {
        public int Id {get;set;}
        public string Data { get; set; }
        public int Value  { get; set; }
        public int TenantId { get; set; }
    }
}