var guidEmpty = "00000000-0000-0000-0000-000000000000";
var app;
axios.defaults.baseURL = 'http://localhost:8080';
axios.defaults.withCredentials = true;
// 拦截request,设置全局请求为ajax请求
axios.interceptors.request.use((config) => {
    console.log("请求拦截");
    return config
})

// 拦截响应response，并做一些错误处理
axios.interceptors.response.use((response) => {
    console.log("相应拦截");

    const data = response.data;
    if (response.status != 200) {
        const err = new Error();
        err.response = response
        throw err
    }
    console.log(response.status);
    if (response.status == 302) {
        console.log("302302302302");
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
            case 302:
                message = '要去登录页吗'
                console.log(message);
                break

            default:
        }
        app.$message({
            type: 'error',
            message: message
        });
    }

    //return Promise.reject(err)
    throw err;
})

//判断Ajax业务是否处理成功
function isSuccess(ajaxModel) {
    return ajaxModel.result == "Success"
}
// function ajaxCallback(response, successFun, failedFun) {
//     var ajaxModel = response.data
//     //判断Ajax业务是否处理成功    
//     if (isSuccess(ajaxModel)) {
//         //业务处理 成功 时执行的回调函数
//         if (successFun) {
//             successFun(response);
//         }
//     } else {
//         //业务处理 失败 时执行的回调函数
//         if (failedFun) {
//             failedFun(response);
//         }
//     }
// }


Vue.component('menus', {
    props: ['menusdata', 'pid'],
    template: `
                <div>
                <div v-for="menu in menusdata" >
                    <template v-if="menu.parentId==pid">
                        <el-submenu :index="menu.id" v-if="existChild(menu.id)" >
                    <template slot="title">
                            <i v-if="menu.icon" :class="menu.icon"></i>
                        <span slot="title">{{menu.name}} </span>
                    </template>
                    <menus :menusdata="menusdata" :pid="menu.id" ></menus>
                        </el-submenu >
                    <el-menu-item :index="menu.id" v-else ><i v-if="menu.icon" :class="menu.icon"></i>{{menu.name}}
                        </el-menu-item>
                    </template>
                </div>
              </div> 
            `,
    methods: {
        existChild: function (id) {
            var array = this.menusdata.filter(item => item.parentId == id)
            //判断是否有子节点
            return array.length > 0
        }
    }
});


Vue.component('my-aside', {
    template: `
                <el-aside width="200px" style="background-color:#545c64">
                <el-menu background-color="#545c64" text-color="#fff" active-text-color="#ffd04b"
                default-active="5fb42499-298c-4268-010c-08d6b33ff0d2" class="el-menu-vertical-demo">
                <menus :menusdata="menuData" pid="00000000-0000-0000-0000-000000000000"></menus>
                </el-menu>
                </el-aside>
                `,
    data: function () {
        return {
            menuData: [],//左侧菜单数据
        }
    },
    created: function () {
        var currObj = this;
        this.$axios.get('/api/Account/GetMenu').then(function (response) {
            var ajaxModel = response.data
            currObj.menuData = ajaxModel.data
        })
    }
});

Vue.prototype.$axios = axios;
