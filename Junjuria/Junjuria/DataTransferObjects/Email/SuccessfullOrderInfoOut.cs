namespace Junjuria.DataTransferObjects.Email
{
    using System;
    public class SuccessfullOrderInfoOut
    {
        public string OrderId {get;set;}
        public decimal Value {get;set;}
        public DateTime OrderDateTime {get;set;}
    }
}