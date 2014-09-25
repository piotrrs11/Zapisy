using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zapisy
{
    class Course
    {
        private string name;
        private string type;
        private List<Group> groups;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public string Type
        {
            get { return type; }
            set { type = value; }
        }
        public List<Group> Groups
        {
            get { return groups; }
            set { groups = value; }
        }

        public Course(string n, string t)
        {
            name = n;
            type = t;
            groups = new List<Group>();
        }

        public void addGroup(Group g)
        {
            groups.Add(g);
        }

        public override string ToString()
        {
            return name + " - " + type;
        }
    }
}
