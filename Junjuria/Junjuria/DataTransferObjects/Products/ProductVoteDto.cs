namespace Junjuria.DataTransferObjects.Products
{
using Junjuria.Common.Interfaces.AutoMapper;
using Junjuria.Infrastructure.Models;
using Junjuria.Infrastructure.Models.Enumerations;
    public class ProductVoteDto:IMapFrom<ProductVote>
    {
        public string  VoterUserName { get; set; }

        public Grade Grade { get; set; }
    }
}