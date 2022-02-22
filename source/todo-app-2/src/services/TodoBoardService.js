import axios from 'axios'

const endpoint = 'todoboards';
export default {
    get() {
        return axios.get(endpoint).then(response => {return response.data});
    },
    getItems(id) {
        return axios.get(`${endpoint}/${id}/todoitems`).then(response => {return response.data});
    },
    create(name) {
        return axios.post(endpoint, 
        {
            "name":name
        })
        .then(function (response) {
            console.log(response);
        })
    },
    modify(todoBoard) {
        return axios.put(`${endpoint}/${todoBoard.id}`, todoBoard)
        .then(function (response) {
            console.log(response);
        })
    },
    delete(todoItem) {
        return axios.delete(`${endpoint}/${todoItem.id}`)
        .then(function(response) {
            console.log(response);
        });
    }
}