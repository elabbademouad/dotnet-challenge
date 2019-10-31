using Cds.DroidManagement.Domain.DroidAggregate;
using Cds.DroidManagement.Domain.DroidAggregate.Abstractions;
using Cds.DroidManagement.Domain.DroidAggregate.RepositoryAction;
using Cds.DroidManagement.Domain.DroidAggregate.ValueObjects;
using Cds.DroidManagement.Infrastructure.DroidRepositories.Abstractions;
using Cds.DroidManagement.Infrastructure.DroidRepositories.Dtos;
using Cds.DroidManagement.Infrastructure.DroidRepositories.Dtos.Extensions;
using Cds.DroidManagement.Infrastructure.DroidRepositories.Validation;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace Cds.DroidManagement.Infrastructure.DroidRepositories
{
    public class SqlServerDroidRepository : IDroidRepository, IReadDroidRepository
    {
        public SqlServerDroidRepository(Func<IDbConnection> connectionProvider)
        {
            DefaultTypeMap.MatchNamesWithUnderscores = true;
            _connectionProvider = connectionProvider ?? throw new ArgumentNullException(nameof(connectionProvider));
        }

        public async Task<(IReadOnlyCollection<IDroidDto>, bool)> GetAllPagedAsync(int skip, int take)
        {
            using (var connection = _connectionProvider())
            {
                return await ExecuteGetAllPagedAsync(skip, take, connection);
            }
        }

        public async Task<IDroidDto> GetBySerialNumberAsync(DroidId serialNumber, Action<IDroidUnicityValidationInfo> assertDroidExists)
        {
            using (var connection = _connectionProvider())
            {
                var droidDto = await ExecuteGetSerialNumberAsync(serialNumber, connection);
                assertDroidExists(new DroidUnicityValidationInfo { Droid = droidDto });
                return droidDto;
            }
        }
       
        public async Task InsertAsync(Droid droid)
        {
            using (var connection = _connectionProvider())
            {
                await ExecuteInsertAsync(droid, connection);
            }
        }

        public async Task UpdateAsync(Droid droid)
        {
            using (var connection = _connectionProvider())
            {
                await ExecuteUpdateAsync(droid, connection);
            }
        }

        public async Task<bool> DeleteAsync(DroidId serialNumber, DeleteArmListAction previousActions)
        {
            using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            using (var connection = _connectionProvider())
            {
                if (!await previousActions.Action(previousActions.SerialNumbers, connection))
                {
                    return false;
                }

                if ((await ExecuteDeleteAsync(serialNumber, connection)) > 0)
                {
                    transaction.Complete();
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> DoesNameAlreadyExistsAsync(DroidName name)
        {
            using (var connection = _connectionProvider())
            {
                return await ExecuteDoesNameAlreadyExistsAsync(name, connection);
            }
        }

        #region Database Access

        private static async Task<bool> ExecuteDoesNameAlreadyExistsAsync(DroidName name, IDbConnection connection)
        {
            return await connection.QueryFirstOrDefaultAsync<bool>(
                    DroidStoredProcedures.DisplayDroidByName,
                    new { name = (string)name },
                    commandType: CommandType.StoredProcedure);
        }

        private static async Task<IDroidDto> ExecuteGetSerialNumberAsync(DroidId serialNumber, IDbConnection connection)
        {
            return await connection.QueryFirstOrDefaultAsync<DroidDto>(
                DroidStoredProcedures.DisplayDroidBySerial,
                new { DroidId = (Guid)serialNumber },
                commandType: CommandType.StoredProcedure);
        }

        private static async Task<int> ExecuteInsertAsync(Droid droid, IDbConnection connection)
        {
            return await connection.ExecuteAsync(
                DroidStoredProcedures.InsertDroid,
                droid.ToDto(),
                commandType: CommandType.StoredProcedure);
        }

        private static async Task<int> ExecuteUpdateAsync(Droid droid, IDbConnection connection)
        {
            return await connection.ExecuteAsync(
                DroidStoredProcedures.UpdateDroid,
                droid.ToDto(),
                commandType: CommandType.StoredProcedure);
        }

        private static async Task<int> ExecuteDeleteAsync(DroidId serialNumber, IDbConnection connection)
        {
            return await connection.ExecuteAsync(
                DroidStoredProcedures.DeleteDroid,
                new { serial_number = (Guid)serialNumber },
                commandType: CommandType.StoredProcedure);
        }

        private static async Task<(IReadOnlyCollection<IDroidDto>, bool)> ExecuteGetAllPagedAsync(int skip, int take, IDbConnection connection)
        {
            var pagedDroidsWithNext = (await connection.QueryAsync<DroidDto>(
                    DroidStoredProcedures.SelectPaged,
                    new { skip, take },
                    commandType: CommandType.StoredProcedure)).AsList();

            var pagedDroids = pagedDroidsWithNext.Take(take).AsList();
            return (pagedDroids, pagedDroidsWithNext.Count > take);
        }

        #endregion

        private static class DroidStoredProcedures
        {            
            internal const string DeleteDroid = "ps_droid_d_deleteDroid";
            internal const string InsertDroid = "ps_droid_i_add";            
            internal const string DisplayDroidBySerial = "ps_droid_s_displayBySerialNumber";
            internal const string UpdateDroid = "ps_droid_u_updatedDroid";
            internal const string DisplayDroidByName = "ps_droid_s_nameExist";
            
            // INFO: ps_droid_s_selectAllPaged.sql is here to avoid to open Droids.mdf file
            internal const string SelectPaged = "ps_droid_s_selectAllPaged";
        }

        private readonly Func<IDbConnection> _connectionProvider;
    }
}
