namespace Junjuria.Infrastructure.Models.Enumerations
{
    public enum Status
    {
        Canceled = 0,
        AwaitingConfirmation =1,
        PreparedForSending=2,
        Traveling=3,
        Arrived=4,
        Finalised=5
    }
}