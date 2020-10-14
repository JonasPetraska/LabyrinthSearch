using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabyrinthSearch.Services
{
    public interface ILoggerService : IDisposable
    {
        void Write(string str);
        void WriteLine(string str);
        void Write(StringBuilder str);
        void WriteLine(StringBuilder str);
    }
}
