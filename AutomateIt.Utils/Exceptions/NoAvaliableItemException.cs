using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Natu.Utils.Exceptions
{
    public class NoAvaliableItemException : Exception
    {
        public NoAvaliableItemException(string message) : base(message)
        {
        }
    }
}
