using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cw4.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using cw4.DAL;
using cw4.Other;

namespace cw3.Controllers
{
    [ApiController]
    [Route("api/enrollments")]
    public class EnrollmentsController : ControllerBase
    {
        /*
         * HttpGet - pobierz
         * HttpPost - dodaj zasób
         * HttpPut - zaktualizuj zasób
         * HttpPatch - załataj zasób (zaktualizuj część)
         * HttpDelete - usuń zasób
         */

        private readonly IEnrollmentDbService enrollmentDbService;

        public EnrollmentsController(IEnrollmentDbService enrollmentDbService)
        {
            this.enrollmentDbService = enrollmentDbService;
        }

        [HttpPost]
        public IActionResult EnrollStudent([FromBody] EnrollStudentRequest student)
        {
            EnrollStudentResponse response = new EnrollStudentResponse
            {
                IndexNumber = student.IndexNumber,
                FirstName = student.FirstName,
                LastName = student.LastName,
                BirthDate = DateTime.Today, //TODO
                Studies = student.Studies
            };
            EnrollResult result = enrollmentDbService.AddAndEnrollStudent(response);
            return StatusCode(result.Code, result.Message);
        }


    }
}