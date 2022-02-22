using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Collections.Generic;

namespace TodoApi.Models
{
    public class TodoContext : DbContext
    {
        private const string JSONFILE_ITEMS = "TodoItems.json";
        private const string JSONFILE_BOARDS = "TodoBoards.json";
        public TodoContext(DbContextOptions<TodoContext> options)
            : base(options)
        {
            try
            {
                bool anyChanged = false;
                if (!TodoItems.AnyAsync().Result)
                {
                    var json = File.ReadAllText(JSONFILE_ITEMS);
                    if (!string.IsNullOrEmpty(json))
                    {
                        var todoItems = JsonConvert.DeserializeObject<List<TodoItem>>(json);
                        foreach (var item in todoItems)
                        {
                            TodoItems.Add(item);
                            anyChanged = true;
                        }
                    }
                }

                if (!TodoBoards.AnyAsync().Result)
                {
                    var json = File.ReadAllText(JSONFILE_BOARDS);
                    if (!string.IsNullOrEmpty(json))
                    {
                        var todoBoards = JsonConvert.DeserializeObject<List<TodoBoard>>(json);
                        foreach (var board in todoBoards)
                        {
                            TodoBoards.Add(board);
                            anyChanged = true;
                        }
                    }
                }

                if (anyChanged)
                {
                    this.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public DbSet<TodoItem> TodoItems { get; set; }
        public DbSet<TodoBoard> TodoBoards { get; set; }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var res = base.SaveChangesAsync(cancellationToken);

            string json = JsonConvert.SerializeObject(TodoItems);
            File.WriteAllText(JSONFILE_ITEMS, json);

            json = JsonConvert.SerializeObject(TodoBoards);
            File.WriteAllText(JSONFILE_BOARDS, json);

            return res;
        }
    }
}