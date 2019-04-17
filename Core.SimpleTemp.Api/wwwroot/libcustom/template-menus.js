<div>
    <template v-for="menu in menusdata" >
        <template v-if="menu.parentId==pid">
            <el-submenu : index="menu.id" v-if="existChild(menu.id)" >
        <template slot="title">
                <i v-if="menu.icon" : class="menu.icon"></i>
            <span slot="title">{{ menu.name }}</span>
        </template>
        <menus : menusdata="menusdata" :pid="menu.id" ></menus>
    </el-submenu>
    <el-menu-item : index="menu.id" v-else ><i v-if="menu.icon" : class="menu.icon"></i>{ { menu.name } }</el - menu - item >
  </template >
 </template >
</div >