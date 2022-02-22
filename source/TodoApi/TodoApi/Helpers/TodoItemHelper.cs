using System.Collections.Generic;
using System.Linq;
using TodoApi.Models;

namespace TodoApi.Helpers
{
    public static class TodoItemHelper
    {
        public static void OrderOrders(TodoItem todoItem, List<TodoItem> list)
        {
            list.OrderBy(x => x.Order).ToList().ForEach(x=>
            {
                if (x != todoItem && x.Order >= todoItem.Order)
                    x.Order += 1;
            });
        }
    }
}
