using TodoApi.Models;

namespace TodoApi.Dtos
{
    public class TodoItemDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public TodoItemState State { get; set; }
        public int Order { get; set; }
        public long BoardId { get; set; }
    }
}
