using System;
using System.IO;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using Infrastructure.Common.Helpers;
using Infrastructure.Security.Models;

namespace Infrastructure.Security.Helpers
{
    public static class SecurityTokenUtility
    {
        static SecurityTokenUtility()
        {
            AppContext.SetSwitch("System.Runtime.Serialization.EnableUnsafeBinaryFormatterSerialization", true);
        }
        
        /// <summary>
        /// Десериализует токен безопастости из строки.
        /// </summary>
        /// <param name="securityTokenString">Строка, содержащая токен безопасности.</param>
        /// <returns></returns>
        public static UserInfo Deserialize(MachineKeyDto machineKeyDto, string securityTokenString)
        {
            if (string.IsNullOrEmpty(securityTokenString))
                throw new ApplicationException("Не задан токен безопасности.");

            var decryptTokenBytes = UnprotectToBytes(machineKeyDto, securityTokenString);
            
            if (decryptTokenBytes == null)
                throw new ApplicationException("Токен безопасности не может быть расшифрован.");

            using MemoryStream stream = new MemoryStream(decryptTokenBytes);
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.AssemblyFormat = FormatterAssemblyStyle.Simple;
            formatter.Binder = new CustomBinder();
#pragma warning disable 618
#pragma warning disable SYSLIB0011 // Type or member is obsolete
            var tmp = (CustomBinder.SUserInfo)formatter.Deserialize(stream);
#pragma warning restore SYSLIB0011 // Type or member is obsolete
#pragma warning restore 618
            return tmp?.AsUserInfo();
        }

        public static byte[] UnprotectToBytes(MachineKeyDto machineKeyDto, string encryptedValue)
        {
            try
            {
                var decodeToken = Convert.FromBase64String(encryptedValue);

                var decryptedBytes = MachineKey.Unprotect(decodeToken,
                                                          machineKeyDto.ValidationKey,
                                                          machineKeyDto.DecryptionKey,
                                                          machineKeyDto.DecryptionAlgorithm,
                                                          machineKeyDto.ValidationAlgorithm,
                                                          machineKeyDto.PrimaryPurpose);

                return decryptedBytes;
            }
            catch (Exception)
            {
                //_logger.Error(ex);
                return null;
            }
        }
	}
}
