var app;
axios.defaults.baseURL = 'http://localhost:8080';
// 拦截request,设置全局请求为ajax请求
axios.interceptors.request.use((config) => {
    console.log("123");
    return config
})

// 拦截响应response，并做一些错误处理
axios.interceptors.response.use((response) => {
    const data = response.data
    if (response.status != 200) {
        const err = new Error();
        err.response = response
        throw err
    }
    return response;
}, (err) => { // 这里是返回状态码不为200时候的错误处理
    if (err && err.response) {
        var message = "";
        switch (err.response.status) {
            case 400:
                message = '请求错误'
                break

            case 401:
                message = '未授权，请登录'
                break

            case 403:
                message = '拒绝访问'
                break

            case 404:
                message = `请求地址出错: ${err.response.config.url}`
                break

            case 408:
                message = '请求超时'
                break

            case 500:
                message = '服务器内部错误'
                break

            case 501:
                message = '服务未实现'
                break

            case 502:
                message = '网关错误'
                break

            case 503:
                message = '服务不可用'
                break

            case 504:
                message = '网关超时'
                break

            case 505:
                message = 'HTTP版本不受支持'
                break

            default:
        }
        console.log(this);
        console.log(app);
        app.$message({
            type: 'error',
            message: message
        });
    }

    //return Promise.reject(err)
    throw err;
})
Vue.prototype.$axios = axios