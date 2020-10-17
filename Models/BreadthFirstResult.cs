using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabyrinthSearch.Models
{
    public class BreadthFirstResult
    {
        public bool IsSuccess { get; set; }
        public IEnumerable<KeyValuePair<int, int>> Path { get; set; }
        public IEnumerable<string> Rules { get; set; }
        public string Trace { get; set; }
    }
}
