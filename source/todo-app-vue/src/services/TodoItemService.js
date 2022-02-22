import axios from 'axios'

export default {
    get() {
        return axios.get('todoitems').then(response => {return response.data})
    },
    create(name, boardId, state) {
        console.log({
            "name":name,
            "state":state,
            "boardId":boardId
        });
        return axios.post('todoitems', 
        {
            "name":name,
            "state":state,
            "boardId":boardId
        })
        .then(function (response) {
            console.log(response);
        })
    },
    modify(todoItem) {
        return axios.put('todoitems/' + todoItem.id, todoItem)
        .then(function (response) {
            console.log(response);
        })
    },
    delete(todoItem) {
        return axios.delete('todoitems/' + todoItem.id)
        .then(function(response) {
            console.log(response);
        });
    }
}