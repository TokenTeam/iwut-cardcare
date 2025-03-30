import service from "./axios";

interface ApiResponse<T = any> {
    code: number,
    data: T,
    message: string,
    [key :string]: any
}

/**
 * GET 请求封装
 * @param url 接口地址
 * @param params 请求参数
 */
export function get<T>(url: string, params?: object): Promise<ApiResponse<T>> {
    return service.get(url, { params })
}

/**
 * POST 请求封装
 * @param url 接口地址
 * @param data 请求体数据
 */
export function post<T>(url: string, data?: object): Promise<ApiResponse<T>> {
    return service.post(url, data)
}

//PUT\DELETE等方法