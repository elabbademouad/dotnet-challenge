using Cds.DroidManagement.Domain.DroidAggregate;
using Cds.DroidManagement.Domain.DroidAggregate.Abstractions;
using Cds.DroidManagement.Domain.DroidAggregate.ValueObjects;
using Cds.DroidManagement.Infrastructure.DroidRepositories.Abstractions;
using Cds.DroidManagement.Infrastructure.DroidRepositories.Dtos;
using Cds.DroidManagement.Infrastructure.DroidRepositories.Dtos.Extensions;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Cds.DroidManagement.Infrastructure.DroidRepositories
{
    public class SqlServerArmRepository : IArmRepository, IReadArmRepository
    {
        public SqlServerArmRepository(Func<IDbConnection> connectionProvider)
        {
        }

        public Task<IReadOnlyCollection<IArmDto>> GetDroidArmsAsync(DroidId serialNumber)
        {
            return null;
        }

        public Task InsertDroidArmAsync(DroidId serialNumber, Arm arm, Action<IDroidValidationInfo> assertDroidIsValid)
        {
            return null;
        }

        public Task<bool> DeleteAsync(ArmId serialNumber, object connection)
        {
            return null;
        }

        public Task<bool> DeleteAsync(List<ArmId> serialNumbers, object connection)
        {
            return null;
        }

        private static Task<bool> DeleteAsync(List<ArmId> serialNumbers, IDbConnection connection)
        {
            return null;
        }

        #region Database Access

        private static async Task<int> ExecuteInsertDroidArmAsync(DroidId serialNumber, Arm arm, IDbConnection connection)
        {
            return await connection.ExecuteAsync(
                DroidStoredProcedures.InsertArm,
                arm.ToDto(serialNumber),
                commandType: CommandType.StoredProcedure);
        }

        private static async Task<IEnumerable<IArmDto>> ExecuteGetDroidArmsAsync(DroidId serialNumber, IDbConnection connection)
        {
            return await connection.QueryAsync<ArmDto>(
                DroidStoredProcedures.DroidArms,
                new { DroidId = (Guid)serialNumber },
                commandType: CommandType.StoredProcedure);
        }

        private static async Task<int> ExecuteDeleteArmAsync(ArmIdListDto serialNumbers, IDbConnection connection)
        {
            return await connection.ExecuteAsync(
                DroidStoredProcedures.DeleteArm,
                serialNumbers,
                commandType: CommandType.StoredProcedure).ConfigureAwait(false);
        }

        #endregion

        private static class DroidStoredProcedures
        {
            internal const string InsertArm = "ps_arm_i_add";
            internal const string DroidArms = "ps_arm_s_droidArms";
            internal const string DeleteArm = "ps_arm_d_deleteArm";
        }
    }
}
