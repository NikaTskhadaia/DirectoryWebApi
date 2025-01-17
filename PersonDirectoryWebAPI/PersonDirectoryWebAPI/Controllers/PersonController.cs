﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using PersonDirectory.Domain.Interfaces;
using PersonDirectory.Domain.Models;
using PersonDirectory.WebAPI.ActionFilters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PersonDirectory.WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PersonController : ControllerBase
    {

        private readonly ILogger<PersonModel> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStringLocalizer<PersonController> _localizer;

        public PersonController(ILogger<PersonModel> logger, IUnitOfWork unitOfWork, IStringLocalizer<PersonController> localizer)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _localizer = localizer;
        }


        [HttpGet("GetPerson")]
        public ObjectResult GetPerson(int personId)
        {
            var p = _unitOfWork.People.Get(personId);
            return Ok(p);
        }


        [HttpPost("AddPerson")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public void AddPerson([FromBody] PersonModel person)
        {
            _unitOfWork.People.Add(person);
            _unitOfWork.Save();
        }

        [HttpPut("ChangePerson")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public void ChangePerson([FromBody] PersonModel person)
        {
            _unitOfWork.People.Update(person);
            _unitOfWork.Save();
        }

        [HttpDelete("RemovePerson")]
        public void RemovePerson(int personId)
        {
            _unitOfWork.People.Remove(personId);
            _unitOfWork.Save();
        }


        [HttpPost("AddRelatedPerson")]
        public void AddRelatedPerson(int personId, int relatedPersonId, RelationType relationType)
        {
            _unitOfWork.People.AddRelatedPerson(personId, relatedPersonId, relationType);
            _unitOfWork.Save();
        }

        [HttpDelete("RemoveRelatedPerson")]
        public void RemoveRelatedPerson(int personId, int relatedPersonId)
        {
            _unitOfWork.People.RemoveRelatedPerson(personId, relatedPersonId);
            _unitOfWork.Save();
        }


        [HttpGet("CountRelatedPeople")]
        public int CountRelatedPeople(int personId, RelationType relation)
        {
            var count = _unitOfWork.People.RelatedPersonCount(personId, relation);
            return count;
        }

        [HttpPost("UploadFile")]
        public IActionResult UploadFile(IFormFile file, string personalNumber)
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", file.FileName);
            FileStream stream = new (path, FileMode.Create);
            file.CopyToAsync(stream);
            string relativepath = Path.Combine(@"wwwroot\images", file.FileName);
            _unitOfWork.People.UploadPesonImage(relativepath, personalNumber);
            _unitOfWork.Save();
            return Ok(new { length = file.Length, name = file.FileName });
        }

        [HttpGet("GetPeopleByIdOrName")]
        public IEnumerable<PersonModel> GetPeopleByIdOrName(string searchCrieteria, int numberOfObjectsPerPage, int pageNumber)
        {            
            return _unitOfWork.People.GetAll(searchCrieteria, numberOfObjectsPerPage, pageNumber); ;
        }

        [HttpGet("GetPeopleByAny{numberOfObjectsPerPage},{pageNumber}")]
        public IEnumerable<PersonModel> GetPeopleByAny(Gender gender, DateTime dob, string firstname, string lastname, string personalNumber, int? cityId, int numberOfObjectsPerPage, int pageNumber)
        {
            return _unitOfWork.People.GetPeopleByAny(firstname, lastname, gender, personalNumber, dob, cityId, numberOfObjectsPerPage, pageNumber);
        }
    }
}