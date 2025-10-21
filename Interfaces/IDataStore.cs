using CMCS_PROG6212_POE.Data;

namespace CMCS_PROG6212_POE.Interfaces
{
    public interface IDataStore
    {
        List<ClaimModel> Claims { get; }
        void AddClaim(ClaimModel claim);
    }
}