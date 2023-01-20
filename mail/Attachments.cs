using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace mail
{
    class AttachmentList
    {
        public AttachmentList(string fullName, string name)
        {
            Name = name;
            FullName = fullName;
        }
        public AttachmentList(string fullName)
        {
            Name =Path.GetFileNameWithoutExtension(fullName);
            FullName = fullName;
        }

        public string Name { get; set; }
        public string FullName { get; set; }
       
    }
}
