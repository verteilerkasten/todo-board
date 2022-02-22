import { createApp } from 'vue'
import App from './App.vue'
import axios from 'axios'

axios.defaults.baseURL = 'https://localhost:5001/api/'

createApp(App).mount('#app')
