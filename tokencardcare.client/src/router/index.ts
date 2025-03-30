import { createRouter, createWebHistory } from 'vue-router'
import type {RouteRecordRaw} from 'vue-router'

import Home from '@/views/Home.vue'


// 2. 定义路由（使用 RouteRecordRaw 类型）
const routes: Array<RouteRecordRaw> = [
  {
    path: '/',
    name: 'Home',
    component: Home,
    meta: {
      title: '首页', // 自定义元信息
      requiresAuth: false, // 是否需要登录
    },
  }
]

// 3. 创建路由实例
const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL), // 使用 History 模式
  routes,
})

// 4. 全局路由守卫（可选）
router.beforeEach((to, from, next) => {
  // 修改页面标题
  if (to.meta.title) {
    document.title = `${to.meta.title} | My App`
  }
  next()
})

export default router