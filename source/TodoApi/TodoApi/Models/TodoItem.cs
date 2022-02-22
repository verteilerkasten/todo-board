using Newtonsoft.Json;

namespace TodoApi.Models
{
    public class TodoItem
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public TodoItemState State { get; set; }
        public int Order { get; set; }
        public long BoardId { get; set; }

        /// <summary>
        /// Converts the TodoItem to a json string
        /// </summary>
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        /// <summary>
        /// Returns a new TodoItem from the json string
        /// </summary>
        public static TodoItem FromJson(string todoItemJson)
        {
            return JsonConvert.DeserializeObject<TodoItem>(todoItemJson);
        }
    }
}