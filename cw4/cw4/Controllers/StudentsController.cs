using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cw3.Models;
using Microsoft.AspNetCore.Mvc;

namespace cw3.Controllers
{
    [ApiController]
    [Route("api/students")]
    public class StudentsController : ControllerBase
    {
        /*
         * HttpGet - pobierz
         * HttpPost - dodaj zasób
         * HttpPut - zaktualizuj zasób
         * HttpPatch - załataj zasób (zaktualizuj część)
         * HttpDelete - usuń zasób
         */

        [HttpGet]
        public string GetStudents([FromQuery] string orderBy)
        {
            return $"Kowalski,Malewski,Andrzejewski - sortowanie {orderBy}";
        }

        [HttpPost]
        public IActionResult AddStudent([FromBody] Student student)
        {
            // add to db, generating index numberfgfg
            student.IndexNumber = $"s{new Random().Next(1, 20000)}";
            return Ok(student);
        }

        [HttpPut("{id}")]
        public IActionResult ActualizeStudent([FromRoute] int id)
        {
            return Ok("Aktualizacja dokończona");
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteStudent([FromRoute] int id)
        {
            return Ok("Usuwanie ukończone");
        }

        [HttpGet("{id}")]
        public IActionResult GetStudentById([FromRoute] int id)
        {
            if (id == 1)
                return Ok("Kowalski");
            else if (id == 2)
                return Ok("Malewski");
            else
                return NotFound("Nie znaleziono studenta");
        }
    }
}