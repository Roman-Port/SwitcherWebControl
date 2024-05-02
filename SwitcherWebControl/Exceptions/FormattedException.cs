using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwitcherWebControl.Exceptions
{
    /// <summary>
    /// Exception that will have it's message displayed in the console
    /// </summary>
    public class FormattedException : Exception
    {
        public FormattedException(string message) : base(message)
        {

        }
    }
}
