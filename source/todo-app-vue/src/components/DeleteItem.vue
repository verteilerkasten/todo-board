<template>
    <div class="container-container">
        <div class="icon-container" id="icon-container" @drop="onDroppo($event)" 
        @dragover.prevent @dragenter.prevent @dragenter="dragenter()" @dragleave="dragleave()" @dragover="dragenter()">
            <img class="delete-icon" src='../assets/trash.svg' />
        </div>
    </div>
</template>

<script>
import TodoItemService from '@/services/TodoItemService'
import TodoBoardService from '@/services/TodoBoardService'

export default ({
    name: 'DeleteItem',
    props: {
        callbackForItem: Function,
        callbackForBoard: Function
    },
    methods: {
        onDroppo(evt) {
            if(evt.dataTransfer.getData('type') != 'item') {
                let item = JSON.parse(evt.dataTransfer.getData('item'));
                if(confirm('Are you sure you want delete the current board with all of its items?')) {
                    TodoBoardService.delete(item).then(() => {
                        this.callbackForBoard();
                    });
                }
            }
            else {
                let item = JSON.parse(evt.dataTransfer.getData('item'));
                TodoItemService.delete(item).then(() => {
                    this.callbackForItem();
                });
            }
            this.dragleave();
        },
        dragenter() {
            document.getElementById('icon-container').className = 'icon-container icon-container-hover';
        },
        dragleave() {
            document.getElementById('icon-container').className = 'icon-container';
        }
    }
})
</script>


<style scoped>
    .container-container {
        padding-top: 20px;
        height: 22px;
        width: 100%;
    }

    .icon-container {
        float: right;
    }

    .delete-icon {
        height: 15px;
        widows: 15px;
        opacity: 0.7;
        margin-top: 3px;
        margin-left: 3.5px;
        margin-right: 3.5px;
        margin-bottom: 0px;
    }

    .icon-container-hover {
        background-color: lightpink;
        border-radius: 3px;
    }

    .icon-container:hover {
        background-color: lightpink;
        border-radius: 3px;
    }

    .icon-container:hover .delete-icon {
        filter: invert(21%) sepia(100%) saturate(4414%) hue-rotate(359deg) brightness(94%) contrast(117%);
    }

    .icon-container-hover .delete-icon {
        filter: invert(21%) sepia(100%) saturate(4414%) hue-rotate(359deg) brightness(94%) contrast(117%);
    }

</style>