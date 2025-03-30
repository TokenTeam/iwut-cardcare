import { createApp } from 'vue'
import { createPinia } from 'pinia'
import TDesign from 'tdesign-mobile-vue';
import 'tdesign-mobile-vue/es/style/index.css';
import router from './router';
import axios from 'axios'
import ElementPlus from 'element-plus'
import 'element-plus/dist/index.css'

import App from './App.vue'

const app = createApp(App)

app.use(createPinia())
app.use(TDesign);
app.use(ElementPlus)
app.use(router)
app.mount('#app')
