using Cds.DroidManagement.Domain.DroidAggregate.Abstractions;
using Cds.DroidManagement.Domain.DroidAggregate.Commands;
using Cds.DroidManagement.Domain.DroidAggregate.Exceptions;
using Cds.DroidManagement.Domain.DroidAggregate.RepositoryAction;
using Cds.DroidManagement.Domain.DroidAggregate.ValueObjects;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Cds.DroidManagement.Domain.DroidAggregate
{
    // INFO: manages dependencies + reload/save aggregate
    // WARNING: no try/catch here
    public class DroidHandler : IDroidHandler
    {
        private readonly IDroidRepository _droidRepository;
        private readonly IArmRepository _armRepository;
        private readonly IDroidQuotesService _quotesService;
        private readonly IEncryptionService _encryption;

        public DroidHandler(
            IDroidRepository droidRepository,
            IArmRepository armRepository,
            IDroidQuotesService quotesService,
            IEncryptionService encryption)
        {
            _droidRepository = droidRepository ?? throw new ArgumentNullException(nameof(droidRepository));
            _armRepository = armRepository ?? throw new ArgumentNullException(nameof(armRepository));
            _quotesService = quotesService ?? throw new ArgumentNullException(nameof(quotesService));
            _encryption = encryption ?? throw new ArgumentNullException(nameof(encryption));
        }

        public async Task<Droid> HandleAsync(CreateDroid createDroid)
        {
            // Functionnal
            var droid = await Droid.CreateNewAsync(
                _droidRepository.DoesNameAlreadyExistsAsync,
                _quotesService.GetRandomQuoteAsync,
                _encryption.Encrypt,
                createDroid);

            // Save
            await _droidRepository.InsertAsync(droid);
            return droid;
        }

        public async Task HandleAsync(UpdateDroid updateDroid)
        {
            // Reload
            var droid = await ReloadDroidAggregateAsync(updateDroid.SerialNumber);

            // Functionnal
            droid = await droid.UpdateAsync(_droidRepository.DoesNameAlreadyExistsAsync, updateDroid);

            // Save
            await _droidRepository.UpdateAsync(droid);
        }

        public async Task HandleAsync(DeleteDroid deleteDroid)
        {
            // Reload
            var droid = await ReloadDroidAggregateAsync(deleteDroid.SerialNumber);

            // Functionnal
            var armIds = droid.Arms.Select(a => a.SerialNumber).ToList();
            var deleteArmListActions = DeleteArmListAction.CreateNew(armIds, _armRepository.DeleteAsync);

            // Save
            var isDeletedDroid = await _droidRepository.DeleteAsync(deleteDroid.SerialNumber, deleteArmListActions);

            if (!isDeletedDroid)
            {
                throw new DroidNotFoundException();
            }
        }

        public async Task HandleAsync(CreateArm createArm)
        {
            // Reload
            var droid = await ReloadDroidAggregateAsync(createArm.SerialNumber);

            // Functionnal
            var arm = droid.AddArm();

            // Save
            await _armRepository.InsertDroidArmAsync(createArm.SerialNumber, arm, Droid.AssertIsValid);
        }

        private async Task<Droid> ReloadDroidAggregateAsync(DroidId serialNumber)
        {
            var droidDto = await _droidRepository.GetBySerialNumberAsync(serialNumber, Droid.AssertExists);
            var armListDto = await _armRepository.GetDroidArmsAsync(serialNumber);
            
            return Droid.FromDto(droidDto).WithArms(armListDto);
        }
    }
}
