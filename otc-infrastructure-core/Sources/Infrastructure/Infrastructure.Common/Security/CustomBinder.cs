using System;
using System.Reflection;
using System.Runtime.Serialization;
using Infrastructure.Common.Enums;
using Infrastructure.Security.Models;

namespace Infrastructure.Security
{
    public class CustomBinder : SerializationBinder
    {
        [Serializable]
        public class SUserInfo
        {
            /// <summary>
            /// Идентификатор пользователя.
            /// </summary>
            public int UserId { get; set; }

            /// <summary>
            /// Идентификатор сотрудника.
            /// </summary>
            public int EmployeeId { get; set; }

            /// <summary>
            /// Идентификатор организации.
            /// </summary>
            public int OrganizationId { get; set; }

            /// <summary>
            /// Номер приложения.
            /// </summary>
            public int PlatformId { get; set; }

            public UserInfo AsUserInfo()
            {
                return new UserInfo
                {
                    Platform = (Platforms)PlatformId,
                    EmployeeId = EmployeeId,
                    OrganizationId = OrganizationId,
                    UserId = UserId,
                };
            }
        }

        public override Type BindToType(string assemblyName, string typeName)
        {
            if (typeName == "BaseTrade.Common.SecurityTokenV2.UserInfo")
            {
                return typeof(SUserInfo);
            }

            Type ttd = null;
            try
            {
                string toassname = assemblyName.Split(',')[0];
                Assembly[] asmblies = AppDomain.CurrentDomain.GetAssemblies();
                foreach (Assembly ass in asmblies)
                {
                    if (ass.FullName.Split(',')[0] == toassname)
                    {
                        ttd = ass.GetType(typeName);
                        break;
                    }
                }
            }
            catch (Exception)
            {
                //Debug.WriteLine(e.Message);
            }

            return ttd;
        }
    }
}