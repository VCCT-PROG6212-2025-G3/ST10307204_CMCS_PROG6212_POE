using System.Collections.Generic;
using CMCS_PROG6212_POE.Models;
using CMCS_PROG6212_POE.Interfaces;

namespace CMCS_PROG6212_POE.Data
{


    public class DataStore : IDataStore
    {
        private readonly List<ClaimModel> _claims = new();

        public List<ClaimModel> Claims => _claims;

        public void AddClaim(ClaimModel claim)
        {
            claim.ClaimId = _claims.Count + 1;
            _claims.Add(claim);
        }
    }
}
