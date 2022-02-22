<template>
    <div>
        <textarea type="textarea" wrap="hard"
        v-model="newItemName" placeholder="enter new task" @keypress.enter.prevent="create(newItemName)" @blur="clearText()" />
    </div>
</template>

<script>
import TodoItemService from '@/services/TodoItemService'

export default ({
    name: 'CreateItem',
    props: {
        state: Number,
        boardId: Number,
        callback: Function
    },
    data() {
        return {
            newItemName: ''
        }
    },
    methods: {
        create(name) {
            TodoItemService.create(name, this.boardId, this.state).then(() => {
                this.callback();
            });
            document.activeElement.blur();
        },
        clearText() {
            this.newItemName = '';
        }
    }
})
</script>


<style scoped>
    div {
        /* margin: 10px;
        margin-left: 20px; */
        text-align: center;
    }
    textarea:focus {
        height: 60px;
        width: 200px;
        visibility: visible;
        opacity: 1;
    }
    textarea {
        height: 0px;
        width: 0px;
        resize: none;
        font-family: 'Calibri';
        opacity: 0;
    }
</style>