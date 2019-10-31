using Cds.DroidManagement.Domain.DroidAggregate.ValueObjects;
using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Cds.DroidManagement.Infrastructure.DroidRepositories.Dtos
{
    /// <summary>
    /// Class representing an arm identifier list DTO.
    /// </summary>
    public class ArmIdListDto : Dapper.SqlMapper.IDynamicParameters
    {
        private readonly IEnumerable<Guid> _armIds;

        /// <summary>
        /// The arm identifier list DTO constructor
        /// </summary>
        /// <param name="armIds">The <see cref="ArmId"/> list.</param>
        public ArmIdListDto(IEnumerable<ArmId> armIds)
        {
            _armIds = armIds.Select(a => (Guid)a);
        }

        /// <summary>
        /// Add all the parameters needed to the command just before it executes.
        /// </summary>
        /// <param name="command">The raw command prior to execution.</param>
        /// <param name="identity">Information about the query.</param>
        public void AddParameters(IDbCommand command, Dapper.SqlMapper.Identity identity)
        {
            var sqlCommand = (SqlCommand)command;
            sqlCommand.CommandType = CommandType.StoredProcedure;

            var armIdList = new List<SqlDataRecord>();

            var tvp_definition = new SqlMetaData[]
            {
                new SqlMetaData("ArmId", SqlDbType.UniqueIdentifier)
            };

            foreach (Guid elem in _armIds)
            {
                var record = new SqlDataRecord(tvp_definition);
                record.SetGuid(0, elem);
                armIdList.Add(record);
            }

            var p = sqlCommand.Parameters.Add("@ArmIdList", SqlDbType.Structured);
            p.Direction = ParameterDirection.Input;
            p.TypeName = "ArmIdList";
            p.Value = armIdList.Any() ? armIdList : null;
        }
    }
}
