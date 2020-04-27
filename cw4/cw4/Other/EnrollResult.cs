using cw4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cw4.Other
{
    public class EnrollResult
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public Enrollment Enrollment { get; set; }
    }
}
