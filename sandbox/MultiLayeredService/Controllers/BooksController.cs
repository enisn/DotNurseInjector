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
        private readonly IBookRepository bookRepository;

        public BooksController(IBookRepository bookRepository)
        {
            this.bookRepository = bookRepository;
        }

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
