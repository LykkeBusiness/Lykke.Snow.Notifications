using System;
using JetBrains.Annotations;
using Lykke.Snow.Contracts.Responses;
using Newtonsoft.Json;

namespace Lykke.Snow.Notifications.Client.Models
{
    /// <summary>
    /// Device configuration response
    /// </summary>
    [PublicAPI]
    public class DeviceConfigurationResponse : ErrorCodeResponse<DeviceConfigurationErrorCodeContract>
    {
        /// <summary>
        /// Device configuration body
        /// </summary>
        public DeviceConfigurationContract? DeviceConfiguration { get; set; }

        /// <summary>
        /// Creates instance of <see cref="DeviceConfigurationResponse"/> with no error code.
        /// </summary>
        /// <param name="deviceConfiguration"></param>
        public DeviceConfigurationResponse(DeviceConfigurationContract deviceConfiguration)
        {
            DeviceConfiguration = deviceConfiguration;
            ErrorCode = DeviceConfigurationErrorCodeContract.None;
        }

        /// <summary>
        /// Creates instance of <see cref="DeviceConfigurationResponse"/> with error code.
        /// </summary>
        /// <param name="errorCode">Error code</param>
        public DeviceConfigurationResponse(DeviceConfigurationErrorCodeContract errorCode)
        {
            if (errorCode == DeviceConfigurationErrorCodeContract.None)
                throw new ArgumentOutOfRangeException(nameof(errorCode), errorCode, "None is not allowed");
            
            DeviceConfiguration = null;
            ErrorCode = errorCode;
        }
        
        /// <summary>
        /// Constructor for deserialization
        /// </summary>
        [JsonConstructor]
        private DeviceConfigurationResponse()
        {
        }

        /// <summary>
        /// Implicit conversion from <see cref="DeviceConfigurationErrorCodeContract"/>
        /// to <see cref="DeviceConfigurationResponse"/>.
        /// </summary>
        /// <param name="errorCode">Error code</param>
        /// <returns></returns>
        public static implicit operator DeviceConfigurationResponse(DeviceConfigurationErrorCodeContract errorCode) =>
            new DeviceConfigurationResponse(errorCode);
    }
}
