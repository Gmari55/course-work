using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mail
{
    class AttachmentList
    {
        public AttachmentList(string fullName, string name)
        {
            Name = name;
            FullName = fullName;
        }

        public string Name { get; set; }
        public string FullName { get; set; }
       
    }
}
