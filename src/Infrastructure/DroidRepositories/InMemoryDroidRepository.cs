using Cds.DroidManagement.Domain.DroidAggregate;
using Cds.DroidManagement.Domain.DroidAggregate.Abstractions;
using Cds.DroidManagement.Domain.DroidAggregate.RepositoryAction;
using Cds.DroidManagement.Domain.DroidAggregate.ValueObjects;
using Cds.DroidManagement.Infrastructure.DroidRepositories.Abstractions;
using Cds.DroidManagement.Infrastructure.DroidRepositories.Dtos;
using Cds.DroidManagement.Infrastructure.DroidRepositories.Dtos.Extensions;
using Cds.DroidManagement.Infrastructure.DroidRepositories.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cds.DroidManagement.Infrastructure.DroidRepositories
{
    public class InMemoryDroidRepository : IDroidRepository, IReadDroidRepository
    {
        private readonly List<IDroidDto> _droids = new List<IDroidDto>
        {
            new DroidDto
            {
                DroidId = Guid.Parse("341d9e5b-ae06-4c06-ba4f-7256317792cd"),
                CreationDate = new DateTimeOffset(2019, 2, 6, 9, 0, 0, 0, new TimeSpan(0, 0, 0)),
                Name = "Toto",
                Nickname = "To",
                Quote = "La peur est le chemin vers le côté obscur: la peur mène à la colère, la colère mène à la haine, la haine mène à la souffrance. - Yoda"
            },
            new DroidDto
            {
                DroidId = Guid.Parse("c50e2592-0a71-4ff6-90ce-052cca08598d"),
                CreationDate = new DateTimeOffset(2019, 2, 7, 9, 0, 0, 0, new TimeSpan(0, 0, 0)),
                Name = "Tata",
                Nickname = "Ta",
                Quote = "Il y en a toujours un pour manger l'autre. - Qui-Gon Jinn"
            },
            new DroidDto
            {
                DroidId = Guid.Parse("7e202cf3-d50f-4d70-bab9-c0f3a35c5bb2"),
                CreationDate = new DateTimeOffset(2019, 2, 8, 9, 0, 0, 0, new TimeSpan(0, 0, 0)),
                Name = "Titi",
                Nickname = "Ti",
                Quote ="Toujours par deux ils vont, ni plus, ni moins... Le Maître et son Apprenti... - Yoda"
            }
        };

        public Task<(IReadOnlyCollection<IDroidDto>, bool)> GetAllPagedAsync(int skip, int take)
        {
            var hasNextPage = false;

            var nbElementsFromFirstPage = skip + take;
            if (_droids.Count > nbElementsFromFirstPage)
            {
                hasNextPage = true;
            }

            IReadOnlyCollection<IDroidDto> droids = _droids.Skip(skip).Take(take).ToArray();
            return Task.FromResult((droids, hasNextPage));
        }

        public Task<IDroidDto> GetBySerialNumberAsync(DroidId serialNumber, Action<IDroidUnicityValidationInfo> assertDroidExists)
        {
            var droidDto = _droids.FirstOrDefault(droid => droid.DroidId == (Guid)serialNumber);
            assertDroidExists(new DroidUnicityValidationInfo { Droid = droidDto });
            return Task.FromResult(droidDto);
        }

        public Task<bool> DoesNameAlreadyExistsAsync(DroidName name)
        {
            var doesNameAlreadyExists = _droids.Any(droid => droid.Name == name.Value);
            return Task.FromResult(doesNameAlreadyExists);
        }

        public Task InsertAsync(Droid droid)
        {
            _droids.Add(droid.ToDto());
            return Task.CompletedTask;
        }

        public async Task UpdateAsync(Droid droid)
        {
            if (droid == null)
            {
                return; 
            }

            var droidDto = await GetBySerialNumberAsync(droid.SerialNumber, Droid.AssertExists).ConfigureAwait(false);
            droidDto.Name = (string)droid.Name;
            droidDto.Nickname = (string)droid.Nickname;
        }

        private Task<bool> DeleteInternalAsync(DroidId serialNumber)
        {
            int deletedLines = _droids.RemoveAll(d => d.DroidId == (Guid)serialNumber);
            bool deleted = deletedLines != 0;
            return Task.FromResult(deleted);
        }

        public async Task<bool> DeleteAsync(DroidId serialNumber, DeleteArmListAction previousActions)
        {
            if (!await previousActions.Action(previousActions.SerialNumbers, null).ConfigureAwait(false))
            {
                return false;
            }

            if (!await DeleteInternalAsync(serialNumber).ConfigureAwait(false))
            {
                return false;
            }

            return true;
        }
    }
}
