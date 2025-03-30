import axios from "axios";
import type { AxiosInstance, AxiosRequestConfig, AxiosResponse, InternalAxiosRequestConfig } from "axios";
import { errorMessages } from "vue/compiler-sfc";

//定义接口返回数据的格式
interface ApiResponse<T = any> {
    code: number,
    data: T,
    message: string,
    [key :string]: any
}

const baseURL = '/api'

const service: AxiosInstance = axios.create({
    baseURL,
    timeout: 10000,
    headers: {
        'Content-Type': 'application/json; charset=utf-8'
    }
})

//请求拦截器
service.interceptors.request.use(
    (config: InternalAxiosRequestConfig) => {
        //添加token
        const token = localStorage.getItem('token')
        if(token && config.headers) {
            config.headers.Authorization = `Beaer ${token}`
        }
        return config
    },
    (error: any) => {
        return Promise.reject(error)
    }
)

//响应拦截器
service.interceptors.response.use(
    (response: AxiosResponse<ApiResponse>) => {
      const res = response.data
      if (res.code !== 0) {
        // 处理业务错误
        console.error(res.message || 'Error')
        return Promise.reject(new Error(res.message || 'Error'))
      }
      return res as any
    },
    (error: any) => {
      // 处理 HTTP 错误
      if (error.response) {
        const status = error.response.status
        const message = error.response.data?.message || '请求错误'
        switch (status) {
          case 400:
            console.error('请求参数错误:', message)
            break
          case 401:
            console.error('未授权，请重新登录')
            // 跳转到登录页
            break
          case 403:
            console.error('拒绝访问')

            break
          case 404:
            console.error('请求的资源不存在')
            break
          case 500:
            console.error('服务器错误')
            break
          default:
            console.error('未知错误')
        }
      } else if (error.request) {
        console.error('请求未响应，可能是网络问题')
      } else {
        console.error('请求配置错误:', error.message)
      }
      return Promise.reject(error)
    }
  )
export default service
