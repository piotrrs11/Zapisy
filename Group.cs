using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zapisy
{
    class Group
    {
        private string day;
        private string time;
        private string week;
        private string teacher;
        private int free;
        private int places;
        private string code;
        private string name;
        public string Day
        {
            get { return day; }
            set { day = value; }
        }
        public string Time
        {
            get { return time; }
            set { time = value; }
        }
        public string Week
        {
            get { return week; }
            set { week = value; }
        }
        public string Teacher
        {
            get { return teacher; }
            set { teacher = value; }
        }
        public int Free
        {
            get { return free; }
            set { free = value; }
        }
        public int Places
        {
            get { return places; }
            set { places = value; }
        }
        public string Code
        {
            get { return code; }
            set { code = value; }
        }
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public Group(string d, string t, string w, string te, int f, int p, string c, string n)
        {
            day = d;
            time = t;
            week = w;
            teacher = te;
            free = f;
            places = p;
            code = c;
            name = n;
        }
    }
}
