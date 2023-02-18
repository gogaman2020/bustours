using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Common.Exceptions
{
    public class BusinessLogicException: ApplicationException
    {
        public BusinessLogicException()
            : base()
        {

        }

        public BusinessLogicException(string message)
            : base(message)
        {

        }

        public BusinessLogicException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}
