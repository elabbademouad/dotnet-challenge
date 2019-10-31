using System;
using System.Runtime.Serialization;

namespace Cds.DroidManagement.Api.DroidFeature.ViewModels
{
    /// <summary>
    /// The Droid.
    /// </summary>
    [Serializable]
    [DataContract(Name = "Droid")]
    public class DroidViewModel
    {
        /// <summary>
        /// The serial Number.
        /// </summary>
        [DataMember]
        public Guid SerialNumber { get; set; }

        /// <summary>
        /// The Creation Date.
        /// </summary>
        [DataMember]
        public DateTimeOffset CreatedOn { get; set; }

        /// <summary>
        /// The Name.
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// The Nickname.
        /// </summary>
        [DataMember]
        public string Nickname { get; set; }

        /// <summary>
        /// The quote.
        /// </summary>
        [DataMember]
        public string Quote { get; set; }
    }
}
