using cw4.Models;
using cw4.Other;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cw4.DAL
{
    public interface IEnrollmentDbService
    {
        public EnrollResult AddAndEnrollStudent(EnrollStudentResponse enrollStudentResponse);
    }
}