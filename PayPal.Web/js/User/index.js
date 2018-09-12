layui.use(['form', 'layer', 'table'], function () {
    var form = layui.form,
        layer = parent.layer === undefined ? layui.layer : top.layer,
        $ = layui.jquery,
        table = layui.table;
 

    //商家列表
    table.render({
        elem: '#getTable',
        url: '/api/Users/GetUserList',
        cols: [[

            { type: "checkbox", fixed: "left", width: 50 },
            { field: 'UserName', title: '用户名', width: 200, sort: true },
            { field: 'Id', width: 0, style: "display:none;" },
            { field: 'RoleId', width: 0, style: "display:none;" },
            { field: 'Tel', title: '手机号', width: 200, sort: true },
            { field: 'Email', title: '邮箱', width: 200 },
            {
                field: 'IsActiveText', title: '当前状态', width: 100, align: 'center', sort: true, unresize: true,toolbar: '#isActiveTpl'
            },
            { field: 'CreateDate', title: '创建日期', width: 200, sort: true }, 
            {
                title: '操作', width: 150, fixed: "right", align: "center", templet: function (d) {
                    if (d.UserName == "admin")
                        return "";
                    return '<a class="layui-btn layui-btn-xs" lay-event="edit">编辑</a><a class="layui-btn layui-btn-xs layui-btn-danger" lay-event="del">删除</a>';
                }
            }
        ]],
        page: true,
        height: "full-104",
        limit: 20,
        limits: [10, 15, 20, 25],
        id: "tabSearch",
        where: {
            Name: $(".searchVal").val(),
            Email: ""
        },
        done: function (d) {
            $(".layui-table-header th:eq(2)").hide();
            $(".layui-table-header th:eq(3)").hide();
        }
    });

    //角色
    xh.Get('/api/Role/GetRoleList','', function (d) {
        if (d.code == 0)
        {
            var str = "";
            for (i in d.data) {
               str += '<p><input type="radio" name="roles" lay-filter="roles" value=' + d.data[i].Id + ' title="' + d.data[i].RoleName + '" /></p>';
            }
            $(".category").html(str);
            form.render("radio"); 
        }
    });

    form.on('radio(roles)', function (data) {
        if (uid == 0)
            return;
        var index = layer.msg('修改中，请稍候', { icon: 16, time: false, shade: 0.8 });
        xh.Post('/api/Users/Update', { Id: uid, RoleId:data.value }, function () {
            table.reload("tabSearch", {});
            layer.close(index);
        });
    });
 

    $('.demoTable .layui-btn').on('click', function () {
        var type = $(this).data('type');
        active[type] ? active[type].call(this) : '';
    });
  
    var active = {
        reload: function () {
            table.reload("tabSearch", {
                page: {
                    curr: 1 //重新从第 1 页开始
                },
                where: {
                    Name: $(".searchVal").val(),  //搜索的关键字
                    Email:""
                }
            })
        }
    };
    var uid = 0;
    $(document).on("click", ".layui-table-main tr", function (data) {
        var rolename = $(this).context.cells[1].innerText.trim();
        uid = $(this).context.cells[2].innerText
       $(".bqrole").text(rolename);
       if (rolename == "admin")
           $(".category :radio").attr("disabled", "disabled");
       else
           $(".category :radio").removeAttr("disabled");
        var rid = $(this).context.cells[3].innerText;
        $(".category :radio").prop('checked', false);
        $(".category :radio").each(function (i, item) {
            if ($(item).val() == rid) {
                $(item).prop('checked', true);
            }
        });
        form.render("radio");
    });

    //是否禁用
    form.on('switch(newsTop)', function (data) {
 
        var index = layer.msg('修改中，请稍候', { icon: 16, time: false, shade: 0.8 });
        xh.Post('/api/Users/Update', { Id: data.elem.getAttribute("sid"), CreateDate: new Date() }, function () {
            layer.close(index);
            if (data.elem.checked) {
                layer.msg("激活成功！");
            } else {
                layer.msg("禁用成功！");
            }
        });
 
    })

    table.on('tool(getTable)', function (obj) {
        var v = {};
        v.Id = obj.data.Id;
    
        if (obj.event === 'del') {
            if (obj.data.UserName == "admin")
                return;
            layer.confirm('确认删除该数据?', function (index) {
                xh.Post('/api/Users/Del', v, function (d) {
                    if (d.suc) {
                        obj.del();
                        layer.msg("删除成功!");
                        table.reload("tabSearch", {});
                    } else {
                        layer.msg(d.msg);
                    }
                });
            });
        } else if (obj.event === 'edit') {
            //sessionStorage.setItem('TabId', obj.data.Id);
            //layer.open({
            //    type: 2,
            //    title: '修改',
            //    area: ['800px', '650px'], //长宽
            //    content: '/Store/Add',
            //    end: function () {
            //        var UpdateTab = sessionStorage.getItem('UpdateTab');

            //        if (UpdateTab != '' & UpdateTab != null) {
            //            var jsonTab = JSON.parse(UpdateTab);
            //            jsonTab.PName += jsonTab.CName + jsonTab.AName;
            //            jsonTab.IsActive = !!jsonTab.IsActive;
            //            obj.update(jsonTab);
            //           // form.render("checkbox");
            //        }
            //    }
            //});
        }  
    });
})