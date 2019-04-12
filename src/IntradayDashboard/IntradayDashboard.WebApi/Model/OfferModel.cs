using System;

namespace IntradayDashboard.WebApi.Model
{
    public class OfferModel
    {   
        public int Id { get; set; }
        public int Value;
        public string Data;
        public int TenantId { get; set; }
    }
}