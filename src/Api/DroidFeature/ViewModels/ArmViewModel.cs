using System;
using System.Runtime.Serialization;

namespace Cds.DroidManagement.Api.DroidFeature.ViewModels
{
    /// <summary>
    /// The Arm.
    /// </summary>
    [Serializable]
    [DataContract(Name = "Arm")]
    public class ArmViewModel
    {
        /// <summary>
        /// The Serial Number.
        /// </summary>
        [DataMember]
        public Guid SerialNumber { get; set; }
    }
}
