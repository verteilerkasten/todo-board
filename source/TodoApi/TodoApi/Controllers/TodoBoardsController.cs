using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Dtos;
using TodoApi.Models;

namespace TodoApi.Controllers
{
    [Route("api/TodoBoards")]
    [ApiController]
    public class TodoBoardsController : ControllerBase
    {
        private readonly TodoContext _context;

        public TodoBoardsController(TodoContext context)
        {
            _context = context;
        }

        // GET: api/TodoBoards
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoBoard>>> GetTodoBoards()
        {
            return await _context.TodoBoards.ToListAsync();
        }

        // GET: api/TodoBoards/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TodoBoard>> GetTodoBoard(long id)
        {
            var todoBoard = await _context.TodoBoards.FindAsync(id);

            if (todoBoard == null)
            {
                return NotFound();
            }

            return todoBoard;
        }

        // GET: api/TodoBoards/5/TodoItems
        [HttpGet("{id}/TodoItems")]
        public async Task<ActionResult<IEnumerable<TodoItem>>> GetTodoItems(long id)
        {
            var todoBoard = await _context.TodoBoards.FindAsync(id);

            if (todoBoard == null)
            {
                return NotFound();
            }

            return await _context.TodoItems.Where(x=> x.BoardId == todoBoard.Id).ToListAsync();
        }

        // PUT: api/TodoBoards/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTodoBoard(long id, TodoBoard todoBoard)
        {
            if (id != todoBoard.Id)
            {
                return BadRequest();
            }

            _context.Entry(todoBoard).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TodoBoardExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/TodoBoards
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TodoBoard>> PostTodoBoard(TodoBoard todoBoard)
        {
            _context.TodoBoards.Add(todoBoard);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTodoBoard", new { id = todoBoard.Id }, todoBoard);
        }

        // DELETE: api/TodoBoards/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodoBoard(long id)
        {
            var todoBoard = await _context.TodoBoards.FindAsync(id);
            if (todoBoard == null)
            {
                return NotFound();
            }

            _context.TodoBoards.Remove(todoBoard);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TodoBoardExists(long id)
        {
            return _context.TodoBoards.Any(e => e.Id == id);
        }
    }
}
