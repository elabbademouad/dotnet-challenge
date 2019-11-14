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
using System.Linq;
using System.Threading.Tasks;

namespace Cds.DroidManagement.Infrastructure.DroidRepositories
{
    public class SqlServerArmRepository : IArmRepository, IReadArmRepository
    {

        public SqlServerArmRepository(Func<IDbConnection> connectionProvider)
        {
            DefaultTypeMap.MatchNamesWithUnderscores = true;
            _connectionProvider = connectionProvider ?? throw new ArgumentNullException(nameof(connectionProvider));
        }

        public async Task<IReadOnlyCollection<IArmDto>> GetDroidArmsAsync(DroidId serialNumber)
        {
            using (var connection = _connectionProvider())
            {
                IEnumerable<IArmDto> queryResult = await ExecuteGetDroidArmsAsync(serialNumber, connection);
                IReadOnlyCollection<IArmDto> armDtoList = queryResult.AsList();
                return armDtoList;
            }
        }

        public async Task InsertDroidArmAsync(DroidId serialNumber, Arm arm, Action<IDroidValidationInfo> assertDroidIsValid)
        {
            using (var connection = _connectionProvider())
            {
                await ExecuteInsertDroidArmAsync(serialNumber, arm, connection);
            }
        }

        public async Task<bool> DeleteAsync(ArmId serialNumber, object connection)
        {
            if (connection != null && connection is IDbConnection)
            {
                using (IDbConnection dbConnection = (IDbConnection)connection)
                {
                    var armIds = new List<ArmId>()
                    {
                        serialNumber
                    };
                    return await DeleteAsync(armIds, connection);
                }
            }
            else
            {
                throw new ArgumentNullException(nameof(connection));
            }

        }

        public async Task<bool> DeleteAsync(List<ArmId> serialNumbers, object connection)
        {
            if (connection != null && connection is IDbConnection)
            {
                using (IDbConnection dbConnection = (IDbConnection)connection)
                {
                    ArmIdListDto armIdListDto = new ArmIdListDto(serialNumbers);
                    return await ExecuteDeleteArmAsync(armIdListDto, dbConnection) == serialNumbers.Count;
                }
            }
            else
            {
                throw new ArgumentNullException(nameof(connection));
            }
        }

        private static Task<bool> DeleteAsync(List<ArmId> serialNumbers, IDbConnection connection)
        {
            return DeleteAsync(serialNumbers, connection);
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

        private readonly Func<IDbConnection> _connectionProvider;

    }
}
