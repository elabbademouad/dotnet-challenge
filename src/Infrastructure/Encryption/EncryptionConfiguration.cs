using System;
using Microsoft.Extensions.Options;

namespace Cds.DroidManagement.Infrastructure.Encryption
{
    public class EncryptionConfiguration : IOptions<EncryptionConfiguration>, IConfigureOptions<EncryptionConfiguration>
    {
        public string Password { get; set; }

        public string Base64Salt { get; set; }

        public byte[] Salt { get; set; }

        public EncryptionConfiguration Value => this;

        public void Configure(EncryptionConfiguration options)
        {
            options.Salt = Convert.FromBase64String(options.Base64Salt);
        }
    }
}
