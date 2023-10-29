using Class_Scheduler.Common.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Class_Scheduler.Extensions
{
    public class StudentIdComparer : IEqualityComparer<Student>
    {
        public bool Equals(Student? x, Student? y)
        {
            if (x == null && y == null)
                return false;
            return x.Id == y.Id;
        }

        public int GetHashCode([DisallowNull] Student obj)
        {
            if (obj == null)
                return 0;
            return obj.Id.GetHashCode();
        }
    }
}
