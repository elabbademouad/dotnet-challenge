using Cds.DroidManagement.Domain.DroidAggregate;
using Cds.DroidManagement.Domain.DroidAggregate.Abstractions;
using Cds.DroidManagement.Domain.DroidAggregate.Commands;
using Cds.DroidManagement.Domain.DroidAggregate.Exceptions;
using Cds.DroidManagement.Domain.DroidAggregate.ValueObjects;
using Cds.DroidManagement.Infrastructure.DroidRepositories.Validation;
using Cds.DroidManagement.Infrastructure.Encryption;
using FluentAssertions;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Cds.DroidManagement.Domain.Tests.Unit
{
    public class DroidTests
    {
        private EncryptionService _encrypt => GetEncryptionService();

        private readonly Func<DroidName, Task<bool>> _nameNotAlreadyExists = n => Task.FromResult(false);        

        private readonly Func<DroidName, Task<bool>> _nameAlreadyExists = n => Task.FromResult(true);

        private readonly Func<Task<string>> _starWarsQuote = () => Task.FromResult("The Force will be with you. Always. — Obi-Wan Kenobi");

        private readonly CreateDroid _createDroid = new CreateDroid("Toto", "To");

        private readonly UpdateDroid _updateDroid = new UpdateDroid("NewTiti", "NewTi", Guid.NewGuid());

        [Fact]
        public async Task CreateDroid_WithNoArmAndNamesSet_ReturnsDroidSetAsync()
        {
            // Act
            var droid = await Droid.CreateNewAsync(_nameNotAlreadyExists, _starWarsQuote, _encrypt.Encrypt,_createDroid).ConfigureAwait(false);

            // Assert
            droid.Arms.Should().HaveCount(0);

            droid.Name.Should().Be(_createDroid.Name);
            droid.Nickname.Should().Be(_createDroid.Nickname);
        }

        [Fact]
        public async Task UpdateDroid_WithNamesSet_ReturnsDroidUpdatedSetAsync()
        {
            // Act
            var droid = await Droid.CreateNewAsync(_nameNotAlreadyExists, _starWarsQuote, _encrypt.Encrypt, _createDroid).ConfigureAwait(false);
            droid = await droid.UpdateAsync(_nameNotAlreadyExists, _updateDroid).ConfigureAwait(false);

            // Assert
            droid.Name.Should().Be(_updateDroid.Name);
            droid.Nickname.Should().Be(_updateDroid.Nickname);
        }

        [Fact]
        public async Task AddArmDroid_ArmCreatedSuccessfullyAsync()
        {
            // Act
            var droid = await Droid.CreateNewAsync(_nameNotAlreadyExists, _starWarsQuote, _encrypt.Encrypt, _createDroid).ConfigureAwait(false);
            droid.AddArm();

            // Assert
            droid.Arms.Should().HaveCount(1);
        }

        [Fact]
        public async Task UpdateDroid_WithArmLimitReached_ReturnsDroidTooManyArmsExceptionAsync()
        {
            // Act
            var droid = await Droid.CreateNewAsync(_nameNotAlreadyExists, _starWarsQuote, _encrypt.Encrypt, _createDroid).ConfigureAwait(false);
            droid.AddArm();
            droid.AddArm();

            // Assert
            var exception = Record.Exception(() => droid.AddArm());
            exception.Should().BeOfType<DroidTooManyArmsException>();
        }

        [Fact]
        public void LoadDroid_WithFromNullDto_ReturnsDroidNotFoundExceptionAsync()
        {
            // Arrange
            IDroidDto droid = null;

            // Assert
            var exception = Record.Exception(() => Droid.FromDto(droid));
            exception.Should().BeOfType<DroidNotFoundException>();

        }

        [Fact]
        public async Task CreateDroid_WithNameAlreadyExists_ReturnsDroidConflictNameExceptionAsync()
        {
            // Arrange
            var exception = await Record.ExceptionAsync(() => Droid.CreateNewAsync(_nameAlreadyExists, _starWarsQuote, _encrypt.Encrypt, _createDroid)).ConfigureAwait(false);

            // Assert
            exception.Should().BeOfType<DroidConflictNameException>();
        }

        [Fact]
        public async Task UpdateDroid_WithNameAlreadyExists_ReturnsDroidConflictNameExceptionAsync()
        {
            // Arrange
            var droid = await Droid.CreateNewAsync(_nameNotAlreadyExists, _starWarsQuote, _encrypt.Encrypt, _createDroid).ConfigureAwait(false);
            var exception = await Record.ExceptionAsync(() => droid.UpdateAsync(_nameAlreadyExists, _updateDroid)).ConfigureAwait(false);

            // Assert
            exception.Should().BeOfType<DroidConflictNameException>();
        }
        
        [Fact]
        public void AssertIsValid_WithLimitofArmsNotReached_ShouldRaiseNoException()
        {
            // Arrange
            var validation = new DroidValidationInfo { NbArm = 1 };

            // Act
            var exception = Record.Exception(() => Droid.AssertIsValid(validation));

            // Assert
            exception.Should().BeNull();
        }

        [Fact]
        public void AssertIsValid_WithLimitofArmsReached_ReturnsDroidTooManyArmsException()
        {
            // Arrange
            var validation = new DroidValidationInfo { NbArm = 2 };

            // Act
            var exception = Record.Exception(() => Droid.AssertIsValid(validation));

            // Assert
            exception.Should().BeOfType<DroidTooManyArmsException>();
        }

        private EncryptionService GetEncryptionService()
        {
            var configuration = new EncryptionConfiguration();
            configuration.Password = "testKey";
            configuration.Salt = Convert.FromBase64String("Jtz/AK3teu7F/gevTQgiPA==");
            return new EncryptionService(configuration);
        }
    }
}
