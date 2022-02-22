<template>
    <div class="nav-opener" @click="openNav()">
        <img id="nav-image" src="../assets/arrow-19.svg" />
    </div>
    <div class="sidenav" id="navigation">
        <div class="close-navigation" @click="closeNav()">
            <img id="nav-close-image" src="../assets/arrow-19.svg" />
        </div>
        <div class="board-header sidenav-elem">
            <div>Your Boards</div>
            <div class="icon-container add-board-icon-container" @click="addBoardClicked()">
                <img id="add-board-icon" class="icons" src="../assets/plus.svg" />
            </div>
        </div>
        <div class="board-item sidenav-elem" v-for="board in todoBoards" :key="board.id" @click="selectionChanged(board)">
            {{board.name}}
        </div>
    </div>
    <div>
        <div class="board-name" ref="board-name" @click="boardNameClicked()" draggable="true" @dragstart="startDragBoard($event, this.selectedBoard)">
            <div ref="board-name-text">{{this.selectedBoard.name}}</div>
            <input id="board-name-editor" ref="board-name-editor" type="text" v-model="this.selectedBoard.name" 
            @keypress.enter="endBoardNameEdit()" @blur="endBoardNameEdit()" />
        </div>
        <DeleteItem :callbackForItem="GetItemsByBoardFromService" :callbackForBoard="DeleteCallback" />
        <div class="col-container">
            <div v-for="state in states" :key="state.key" class="col" 
            @dragover.prevent @dragenter.prevent @drop.prevent="onDropOnColumn($event, state.key)" >
                <h3 :class="'_' + state.key + ' headline'">{{state.value}}</h3>
                <div class="pos-relative ">
                    <div :class="'_'+state.key + ' placeholder'">
                    <div class="icon-container add-icon-container" @click="addClicked(state.key)">
                        <img src="../assets/plus.svg" class="icons add-icon" />
                    </div>
                    </div>
                    <div class="todoItem-container">
                        <div v-for="todoItem in todoItems.sort((a, b) => (a.order > b.order) ? 1 : -1).filter(x => x.state == state.key)" 
                            :index="todoItem.order" :key="todoItem.key" class="todoItem pos-relative"
                            @dragstart="startDrag($event, todoItem)" draggable="true" @click="editTodoItem(todoItem.id)" 
                            :ref="'todoItem'+todoItem.id">

                            <!-- drop zones (top for adding the element into the list before this element, bottom for after) -->
                            <div class="drop-top" 
                            @drop.prevent='onDroppo($event, todoItem, true)'  @dragover.prevent @dragenter.prevent></div>
                            <div class="drop-bot" 
                            @drop.prevent='onDroppo($event, todoItem, false)'  @dragover.prevent @dragenter.prevent></div>

                            <span class="readonly-text">
                                <span>{{todoItem.name}}</span>
                            </span>
                            
                            <div>
                                <input class="todo-item-editor" :ref="'edit'+todoItem.id" @keypress.enter.prevent="endEditItem(todoItem)"
                                @blur="endEditItemNoAction(todoItem)" type="text" v-model="todoItem.name" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
import TodoItemService from '@/services/TodoItemService'
import TodoBoardService from '@/services/TodoBoardService'
import DeleteItem from '@/components/DeleteItem.vue'
import { nextTick } from '@vue/runtime-core'

export default ({
    name: 'TodoBoard',
    data() {
        return {
            todoBoards: [],
            todoItems: [],
            states: [
                { key:0, value:'TODO' },
                { key:1, value:'WAIT' },
                { key:2, value:'IN PROGRESS' },
                { key:3, value:'DONE' },
                { key:4, value:'DONT' }
            ],
            droppedOnItem: false,
            selectedBoard: {},
            newItemIsBeingEdited: false
        }
    },
    components: {
        // CreateItem,
        DeleteItem
    },
    created() {
        this.GetBoardsFromService();
    },
    methods: {
        GetBoardsFromService() {
            this.GetBoardsFromServiceWithCallback(null);
        },
        GetBoardsFromServiceWithCallback(callback) {
            TodoBoardService.get().then(x => {
                this.todoBoards = x;
                if(callback != null) {
                    callback();
                }
            }).catch(error => console.log(error));
        },
        DeleteCallback() {
            this.GetBoardsFromService();
            this.selectedBoard = {};
        },
        GetItemsByBoardFromService() {
            this.GetItemsByBoardFromServiceWithCallback(null);
        },
        GetItemsByBoardFromServiceWithCallback(callback) {
            if (this.selectedBoard.id == 0)
            {
                return [];
            }
            TodoBoardService.getItems(this.selectedBoard.id).then(x => {
                this.todoItems = x;
                if(callback != null) {
                    callback();
                }
            }).catch(error => console.log(error));
        },
        selectionChanged(board) {
            this.selectedBoard = board;
            this.GetItemsByBoardFromService();
            this.closeNav();
        },
        startDrag: (evt, item) => {
            evt.dataTransfer.dropEffect = 'move';
            evt.dataTransfer.effectAllowed = 'move';
            evt.dataTransfer.setData("item", JSON.stringify(item));
            evt.dataTransfer.setData("type", "item");
        },
        startDragBoard: (evt, item) => {
            evt.dataTransfer.dropEffect = 'move';
            evt.dataTransfer.effectAllowed = 'move';
            evt.dataTransfer.setData("item", JSON.stringify(item));
        },
        onDroppo(evt, droppedOn, top) {
            this.droppedOnItem = true;
            let item = JSON.parse(evt.dataTransfer.getData('item'));
            if (item == droppedOn) {
                return;
            }

            if (top) {
                item.state = droppedOn.state;
                item.order = droppedOn.order;
            }
            else {
                item.state = droppedOn.state;
                item.order = droppedOn.order + 1;
            }
            TodoItemService.modify(item).then(() => {
                this.GetItemsByBoardFromService(); 
            });
        },
        onDropOnColumn(evt, state) {
            let item = JSON.parse(evt.dataTransfer.getData('item'));
            if (item.state == state || this.droppedOnItem) {
                this.droppedOnItem = false;
                return;
            }
            item.order = 0;
            item.state = state;
            TodoItemService.modify(item).then(() => {
                this.GetItemsByBoardFromService();
            });
        },
        addClicked(state) {
            TodoItemService.create('', this.selectedBoard.id, state).then(() => {
                this.GetItemsByBoardFromServiceWithCallback(() => {
                    // wait for the next tick so the DOM can refresh and add the new item
                    nextTick(() => {
                        this.newItemIsBeingEdited = true;
                        this.editTodoItem(this.todoItems[this.todoItems.length - 1].id);
                    });
                });
            });
        },
        openNav() {
            document.getElementById("navigation").style.width = "250px";
        },
        closeNav() {
            document.getElementById("navigation").style.width = "0";
        },
        boardNameClicked() {
            var nameElem = this.$refs['board-name'];
            var editor = this.$refs['board-name-editor'];
            var text = this.$refs['board-name-text'];
            editor.style.visibility = "visible";
            text.style.visibility = "collapse";
            nameElem.draggable = false;
            editor.focus();
        },
        endBoardNameEdit() {
            var nameElem = this.$refs['board-name'];
            var editor = this.$refs['board-name-editor'];
            var text = this.$refs['board-name-text'];
            editor.style.visibility = "collapse";
            text.style.visibility = "visible";
            nameElem.draggable = true; 

            TodoBoardService.modify(this.selectedBoard).then(() => {
                this.GetBoardsFromService();
                this.GetItemsByBoardFromService();
            });
        },
        editTodoItem(itemId) {
            var editor = this.$refs["edit"+itemId];
            var itemElem = this.$refs["todoItem"+itemId];
            
            if (editor.style.visibility != 'visible') {
                itemElem.draggable = false;
                editor.style.visibility='visible';
                editor.focus();
            }
        },
        endEditItem(item) {
            this.newItemIsBeingEdited = false;
            TodoItemService.modify(item).then(() => {
                document.activeElement.blur();
            });
        },
        endEditItemNoAction(item) {
            var editor = this.$refs["edit"+item.id];
            var itemElem = this.$refs["todoItem"+item.id];

            editor.style.visibility = "collapse";
            itemElem.draggable = true;

            var getExecuted = false;

            // check if item has to be delted
            if(this.newItemIsBeingEdited) {
                getExecuted = true;
                this.newItemIsBeingEdited = false;
                if(item.name == '') {
                    TodoItemService.delete(item).then(() => {
                        this.GetItemsByBoardFromService();
                    });
                } 
                else {
                    TodoItemService.modify(item).then(() => {
                        this.GetItemsByBoardFromService();
                    });
                }
            }
            if (!getExecuted) {
                this.GetItemsByBoardFromService();
            }
        },
        addBoardClicked() {
            TodoBoardService.create('name').then(() => {
                this.GetBoardsFromServiceWithCallback(() => {
                    this.selectedBoard = this.todoBoards[this.todoBoards.length - 1];
                    document.getElementById('board-name-editor').click();
                });
            });
            this.closeNav();
        }
    }
})
</script>

<style scoped>
    .board-name { 
        color: rgba(255,255,255, 0.9);
        font-weight: bold;
        font-size: 20px;
        background-color: rgba(255,255,255,0.2);
        margin-left: 40px;
        border-radius: 4px;
        float: left;
        padding: 5px 15px 5px 15px;
        position: relative;
    }

    .board-name:hover {
        background-color: rgba(255,255,255,0.4);
    }

    #board-name-editor {
        visibility: collapse;
        position: absolute;
        left:0;
        top:0;
        width:100%;
        height: 100%;
        background-color: white;
        font-weight: bold;
        font-size: 17px;
        border:0px;
        border-radius: 4px;
        text-align: center;
    }

    .todoItem {
        display: flex;
        justify-content: center;
        align-items: center;
        /* width: 100%-20px; */
        background-color: #d9e1f2;
        border-bottom: 2px solid #555;
        border-collapse: collapse;
        padding: 5px;
        text-align: center;
        font-size: 11pt;
        min-height: 50px;
        border-radius: 4px;
        margin:4px;
    }

    .todo-item-editor {
        position: absolute;
        top: 0;
        left: 0;
        height: 100%;
        width: 100%;
        border: 0px;
        border-radius: 4px;
        visibility: collapse;
        text-align: center;
    }

    .todoItem:hover {
        background-color: #c9d1e2;
    }

    .column {
        min-height: 50px;
    }

    h3 {
        text-align: center;
        padding: 5px;
        margin:0%;
    }

    .placeholder {
        width:20px;
        height: 100%;
        padding-top: 4px;
        min-height: 60px;
        position: absolute;
        border-bottom-left-radius: 4px;
        border-bottom-right-radius: 4px;
        margin-top: -4px;
    }
    
    ._0 { background-color: #f4b084; }
    ._1 { background-color: #b4c6e7; }
    ._2 { background-color: #ffe699; }
    ._3 { background-color: #c6e0b4; }
    ._4 { background-color: #b4c6e7; }
    
    .col-container {
        opacity: 0.85;
        margin-left: 32px;
        position: relative;
        max-height: 600px;
    }

    .col {
        float:left;
        width: 19%;
        position: relative;
        margin-left: 8px;
    }

    .todoItem-container {
        margin-left: 20px;
        max-height: calc(100vh - 85px);
        overflow-y:auto;
        scrollbar-width:thin;
    }

    .headline {
        border-top-left-radius: 4px;
        border-top-right-radius: 4px;
        border-bottom-right-radius: 4px;
    }

    .input-editor {
        visibility: collapse;
        display: none;
        background-color: transparent;
        resize: none;
        font-family: 'Calibri';
        width: 100%;
        height: 100%;
    }

    .icons {
        height: 15px;
        widows: 15px;
        opacity: 0.5;
    }

    .icon-container:hover .icons {
        opacity: 1;
    }

    .add-icon {
        margin-top: 2.5px;
    }

    .icon-container {
        margin: 2.5px;
    }

    .add-icon-container {  
        position: absolute;
        height: 37px;
        bottom: 0;
        padding-top: 20px;
    }

    .icon-container:hover {
        background-color: white;
        opacity: 0.6;
        border-radius: 3px;
    }

    textarea {
        height: 0px;
        width: 0px;
        resize: none;
        font-family: 'Calibri';
        opacity: 0;
    }
    
    textarea:focus {
        height: 60px;
        width: 200px;
        visibility: visible;
        opacity: 1;
    }

    .pos-relative {
        position: relative;
    }

    .drop-top {
        position: absolute;
        height: 50%;
        width: 100%;
        top:0;
    }
    .drop-bot {
        position: absolute;
        height: 50%;
        width: 100%;
        bottom:0;
    }

    .sidenav {
        height: 100%;
        width: 0;
        position: fixed;
        z-index: 1;
        top: 0;
        left: 0;
        background-color: #eee;
        overflow-x: hidden;
        transition: 0.5s;
        padding-top: 60px;
        opacity: 0.9;
    }
    .board-header {
        font-weight:bold;
    }
    .board-item {
        margin-left:10px;
        margin-right: 10px;
        cursor: pointer;
    }

    .sidenav-elem {
        padding: 8px 8px 8px 27px;
        text-decoration: none;
        color: #818181;
        display: block;
        position: relative;
        transition: 0.3s;
        font-size: 18px;
    }

    .add-board-icon-container {
        position: absolute;
        right: 0;
        top: 0;
        margin:2.5px;
        margin-top: 9px;
        margin-right: 20px;
        height: 20px;
        width: 20px;
        border-radius: 3px;
        cursor: pointer;
    }

    #add-board-icon {
        margin: 2.5px;
    }

    .close-navigation {
        font-size: 50px;
        position: absolute;
        top: 0;
        right: 25px;
        font-size: 36px;
        margin-left: 50px;
    }
    .board-item:hover {
        color: #555;
        background-color: #ddd;
        border-radius: 4px;
    }

    .nav-opener {
        width:40px;
        font-size:30px;
        float:left;
        height: 100%;
        cursor:pointer;
        position: fixed;
        left: 0;
        top:0;
        background-color: rgba(1, 1, 1, 0.1);
    }

    .nav-opener:hover {
        background-color: #eee;
        opacity: 0.9;
    }

    #nav-image {
        height: 20px;
        width: 20px;
        margin: 10px;
        opacity: 0.3;
        transform: scaleX(-1);
    }

    .nav-opener:hover #nav-image {
        filter: invert(100%);
    }

    #nav-close-image {
        height: 20px;
        width: 20px;
        filter: invert(100%);
        opacity: 0.3;
    }

    .close-navigation:hover #nav-close-image {
        opacity: 0.5;
    }
    
</style>
