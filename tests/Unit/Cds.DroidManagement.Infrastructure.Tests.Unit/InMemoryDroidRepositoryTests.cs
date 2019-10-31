using Cds.DroidManagement.Domain.DroidAggregate;
using Cds.DroidManagement.Domain.DroidAggregate.Abstractions;
using Cds.DroidManagement.Domain.DroidAggregate.RepositoryAction;
using Cds.DroidManagement.Domain.DroidAggregate.ValueObjects;
using Cds.DroidManagement.Infrastructure.DroidRepositories;
using Cds.DroidManagement.Infrastructure.DroidRepositories.Dtos;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Cds.DroidManagement.Infrastructure.Tests.Unit
{
    public class InMemoryDroidRepositoryTests
    {
        private readonly InMemoryDroidRepository _repo;
        private readonly InMemoryArmRepository _repoArm;

        private readonly Guid _existingDroidId = Guid.Parse("341d9e5b-ae06-4c06-ba4f-7256317792cd");
        private readonly Guid _existingArmId = Guid.Parse("8ef70fa1-5822-433d-9cb6-e39595eec42a");
        private readonly int _nbOfDroids = 3;
        
        public InMemoryDroidRepositoryTests()
        {
            _repo = new InMemoryDroidRepository();
            _repoArm = new InMemoryArmRepository();
        }

        [Fact]
        public async Task GetAllPaged_WithNotAllDroidsInPage_ReturnsDroidsWithNextPage()
        {
            // Act
            var (droidList, hasNext) = await _repo.GetAllPagedAsync(0, 1).ConfigureAwait(false);

            // Assert
            droidList.Should().HaveCount(1);
            hasNext.Should().BeTrue();
        }

        [Fact]
        public async Task GetAllPaged_WithAllDroidsInPage_ReturnsDroidsWithNoNextPage()
        {
            // Act
            var (droidList, hasNext) = await _repo.GetAllPagedAsync(0, _nbOfDroids);

            // Assert
            droidList.Should().HaveCount(_nbOfDroids);
            hasNext.Should().BeFalse();
        }

        [Fact]
        public async Task GetBySerialNumber_WithValidSerialNumber_ReturnsDroidSelectedAsync()
        {
            // Act
            var droidSelected = await _repo.GetBySerialNumberAsync(_existingDroidId, Droid.AssertExists);

            // Assert
            droidSelected.DroidId.Should().Be(_existingDroidId);
            droidSelected.Name.Should().Be("Toto");
            droidSelected.Nickname.Should().Be("To");
            droidSelected.Quote.Should().Be("La peur est le chemin vers le côté obscur: la peur mène à la colère, la colère mène à la haine, la haine mène à la souffrance. - Yoda");
        }

        [Fact]
        public async Task DoesNameAlreadyExists_WithName_ReturnsTrue()
        {
            // Arrange
            var nameChecked = "Titi";

            // Act
            var nameExists = await _repo.DoesNameAlreadyExistsAsync(nameChecked).ConfigureAwait(false);

            // Assert
            nameExists.Should().BeTrue();
        }

        [Fact]
        public async Task DeleteDroid_WithValidSerialNumber_ReturnsTrue()
        {
            // Arrange
            var (droidListBefore, _) = await _repo.GetAllPagedAsync(0, _nbOfDroids).ConfigureAwait(false);
            var armIdList = new List<ArmId>
            {
               _existingArmId
            };
            var actions = DeleteArmListAction.CreateNew(armIdList, _repoArm.DeleteAsync);

            // Act
            await _repo.DeleteAsync(_existingDroidId, actions);

            //Assert
            var (droidListAfter, _) = await _repo.GetAllPagedAsync(0, _nbOfDroids);
            droidListBefore.Should().HaveCount(_nbOfDroids);
            droidListAfter.Should().HaveCount(_nbOfDroids - 1);
        }

        [Fact]
        public async Task CreateDroid_WithDroidSet_DroidCreatedSuccessfully()
        {
            // Arrange
            var newDroidId = new Guid("c29a188b-a562-47b4-a74c-9e82702e0986");
            var droidDto = new DroidDto
            {
                DroidId = newDroidId,
                CreationDate = DateTimeOffset.Now,
                Name = "Dada",
                Nickname = "Da",
                Quote = "Votre manque de foi me consterne. - Dark Vador"
            };
            var armDtoList = new List<IArmDto>
            {
                new ArmDto { DroidId = newDroidId, ArmId = Guid.NewGuid() }
            };
            

            var droid = Droid.FromDto(droidDto).WithArms(armDtoList);


            // Act
            await _repo.InsertAsync(droid);
            var droidSelected = await _repo.GetBySerialNumberAsync(newDroidId, Droid.AssertExists);


            // Assert
            droidSelected.DroidId.Should().Be(Guid.Parse("c29a188b-a562-47b4-a74c-9e82702e0986"));
            droidSelected.Name.Should().Be("Dada");
            droidSelected.Nickname.Should().Be("Da");
            droidSelected.Quote.Should().Be("Votre manque de foi me consterne. - Dark Vador");
        }

        [Fact]
        public async Task UpdateDroid_WithValidNameAndNickname_DroidUpdatedSuccessfully()
        {
            // Arrange
            var droidDto = new DroidDto
            {
                DroidId = _existingDroidId,
                CreationDate = DateTimeOffset.Now,
                Name = "Didi",
                Nickname = "Di",
            };
            var armDtoList = new List<IArmDto>
            {
                new ArmDto { DroidId = _existingDroidId, ArmId = Guid.NewGuid() }
            };            

            var droid = Droid.FromDto(droidDto).WithArms(armDtoList);

            // Act
            await _repo.UpdateAsync(droid);
            var droidSelected = await _repo.GetBySerialNumberAsync(_existingDroidId, Droid.AssertExists);

            // Assert
            droidSelected.Name.Should().Be("Didi");
            droidSelected.Nickname.Should().Be("Di");
        }

        [Fact]
        public async Task UpdateDroid_WithNullDroid_NothingIsChanged()
        {
            // Arrange
            var (droidListBeforeUpdate, _) = await _repo.GetAllPagedAsync(0, _nbOfDroids);

            // Act
            await _repo.UpdateAsync(null);

            // Assert
            var (droidListAfterUpdate, _) = await _repo.GetAllPagedAsync(0, _nbOfDroids);
            droidListBeforeUpdate.Should().BeEquivalentTo(droidListAfterUpdate);
        }

        [Fact]
        public async Task DeleteDroidTransaction_WithDroidWithoutArms_ReturnsTrue()
        {
            // Arrange
            var droidIdWithoutArms = Guid.Parse("c50e2592-0a71-4ff6-90ce-052cca08598d");
            var (droidListBefore, _) = await _repo.GetAllPagedAsync(0, _nbOfDroids);
            var actions = DeleteArmListAction.CreateNew(new List<ArmId>(), _repoArm.DeleteAsync);

            // Act
            await _repo.DeleteAsync(droidIdWithoutArms, actions);

            // Assert
            var (droidListAfter, _) = await _repo.GetAllPagedAsync(0, _nbOfDroids);
            droidListBefore.Should().HaveCount(_nbOfDroids);
            droidListAfter.Should().HaveCount(_nbOfDroids - 1);
        }

        [Fact]
        public async Task DeleteDroidTransaction_WithValidSerialNumber_ReturnsTrue()
        {
            // Arrange
            var (droidListBefore, _) = await _repo.GetAllPagedAsync(0, _nbOfDroids);
            var armIdList = new List<ArmId>
            {
                _existingArmId
            };
            var actions = DeleteArmListAction.CreateNew(armIdList, _repoArm.DeleteAsync);

            // Act
            await _repo.DeleteAsync(_existingDroidId, actions);

            //Assert
            var (droidListAfter, _) = await _repo.GetAllPagedAsync(0, _nbOfDroids);
            droidListBefore.Should().HaveCount(_nbOfDroids);
            droidListAfter.Should().HaveCount(_nbOfDroids - 1);
        }

        [Fact]
        public async Task DeleteDroidTransaction_WithArmsWithoutDroid_ReturnsFalse()
        {
            // Arrange
            var droidIdWithoutDroid = Guid.Parse("9e3cf3eb-e866-47cf-8cb6-951ac9b27d8a");
            var (droidListBefore, _) = await _repo.GetAllPagedAsync(0, _nbOfDroids);
            var actions = DeleteArmListAction.CreateNew(new List<ArmId>(), _repoArm.DeleteAsync);

            // Act
            await _repo.DeleteAsync(droidIdWithoutDroid, actions);

            // Assert
            var (droidListAfter, _) = await _repo.GetAllPagedAsync(0, _nbOfDroids);
            droidListBefore.Should().HaveCount(_nbOfDroids);
            droidListAfter.Should().HaveCount(_nbOfDroids);
        }
    }
}
