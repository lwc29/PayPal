layui.use(['form', 'layer', 'table'], function () {
    var form = layui.form,
        layer = parent.layer === undefined ? layui.layer : top.layer,
        $ = layui.jquery,
        
        table = layui.table;

    var json = [];
    var id = sessionStorage.getItem('roleId');
    //xh.Get('/api/RoleAndMoudule/GetAccess?rid='+ id, null, function (d) {
    //    if (d.code == 0) {
    //        json = d.data;
    //        alert(d.data);
    //    }
    //})

     
    var xtree3 = new layuiXtree({
        elem: 'xtree1'                  //必填三兄弟之老大
           , form: form                    //必填三兄弟之这才是真老大
           , data: '/api/RoleAndMoudule/GetAccess?rid='+ id
           , isopen: false  //加载完毕后的展开状态，默认值：true
           , ckall: true    //启用全选功能，默认值：false
           , ckallback: function () { } //全选框状态改变后执行的回调函数
           , icon: {        //三种图标样式，更改几个都可以，用的是layui的图标
               open: "&#xe638;"       //节点打开的图标
               , close: "&#xe622;"    //节点关闭的图标
               , end: "&#xe621;"      //末尾节点的图标
           }
           , color: {       //三种图标颜色，独立配色，更改几个都可以
               open: "#EE9A00"        //节点图标打开的颜色
               , close: "#EEC591"     //节点图标关闭的颜色
               , end: "#828282"       //末级节点图标的颜色
           }
           , click: function (data) {  //节点选中状态改变事件监听，全选框有自己的监听事件
               console.log(data.elem); //得到checkbox原始DOM对象
               console.log(data.elem.checked); //开关是否开启，true或者false
               console.log(data.value); //开关value值，也可以通过data.elem.value得到
               console.log(data.othis); //得到美化后的DOM对象
           }
    });

    $(".Add_btn").click(function () {
 
        var oCks = xtree3.GetChecked(); //这是方法
    
        var arr = $.map(oCks, function (d) {
            return d.value;
        });
        var tanchu = top.layer.msg('数据提交中，请稍候', {
            icon: 16,
            time: false,
            shade: 0.8
        });
        
        xh.Post('/api/RoleAndMoudule/UpdateAccess', { Ids: arr,RoleId: id  }, function (d) {
            top.layer.close(tanchu);
            if (d.suc) {
                top.layer.msg("操作成功!");
            } else
                layer.msg(d.msg, {
                    icon: 5
                });
        });
        for (var i = 0; i < oCks.length; i++) {
            console.log(oCks[i].value);
        }
    });
})