using DotNurse.Injector.Attributes;
using Microsoft.AspNetCore.Mvc;
using MultiLayeredService.Repositories.Abstraction;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiLayeredService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        [InjectService] private IBookRepository bookRepository;
        [InjectService] public IBookRepository BookRepository { get; set; }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(bookRepository.Get());
        }

        [HttpGet("{id}")]
        public IActionResult Get(string id)
        {
            return Ok(bookRepository.GetSingle(id));
        }
    }
}
