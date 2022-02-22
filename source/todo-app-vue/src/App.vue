<template>
    <div>
      <TodoBoard />
    </div>
</template>

<script>
import TodoBoard from './components/TodoBoard.vue'

export default {
  name: 'App',
  components: {
    TodoBoard
  }
}
</script>

<style>
  #app {
    font-family: 'Calibri'!important;
    margin-top: 0px;
  }
  body {
    background-image: url('assets/default_background.jpg');
  }
</style>
