using System;
using System.Threading.Tasks;
using Infrastructure.Common.DI;
using Infrastructure.Db.Audit.RevisionQueries;
using Infrastructure.Db.Common;
using Infrastructure.Db.Common.Crud;
using Infrastructure.Security.Models;

namespace Infrastructure.Db.Audit
{
    [InjectAsScoped]
    public class RevisionManager : IRevisionManager
    {
        private readonly IDb _db;
        private readonly IUserInfo _userInfo;
        private readonly Lazy<Task<int>> _revision;

        public RevisionManager(
            IDb db,
            IUserInfo userInfo)
        {
            _db = db;
            _userInfo = userInfo;
            _revision = new Lazy<Task<int>>(GetRevisionAsync);
        }

        public int? CurrentUserId => _userInfo?.UserInfo?.UserId;

        public async Task<int> GetCurrentRevisionNumber()
        {
            return await _revision.Value;
        }

        private async Task<int> GetRevisionAsync()
        {
            var revision = new Revision
            {
                CommonUserId = CurrentUserId,
                CreatedOn = DateTime.UtcNow
            };

            int insertedId = await _db.QueryFirstOrDefaultAsync<int>(
                    new CrudQueryObject<Revision, RevisionQuery>(revision, CrudOperation.Insert));

            return insertedId;
        }
    }
}