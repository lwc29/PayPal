layui.use(['form', 'layer', 'table'], function () {
    var form = layui.form,
        layer = parent.layer === undefined ? layui.layer : top.layer,
        $ = layui.jquery,
        table = layui.table;


    //商家列表
    table.render({
        elem: '#getTable',
        url: '/api/Role/GetRoleList',
        cols: [[
            { type: "checkbox", fixed: "left", width: 50 },
            { field: 'RoleName', title: '角色名', width: 160 },
            {
                field: 'Remark', title: '备注', width: 260
            },
            { field: 'CreateTime', title: '时间', width: 170 },
            {
                title: '菜单', width: 110, align: "center", templet: function () {
                    return '<a class="layui-btn layui-btn-xs" lay-event="menu">菜单</a>'
                }
            },
            {
                title: '操作', width: 150, fixed: "right", align: "center", templet: function (d) {
                    if (d.RoleName == "普通成员" || d.RoleName == "管理员")
                        return '';
                    return '<a class="layui-btn layui-btn-xs" lay-event="edit">编辑</a><a class="layui-btn layui-btn-xs layui-btn-danger" lay-event="del">删除</a>';
                }
            }
        ]],
        height: "full-104",
        limit: 20,
        limits: [10, 15, 20, 25],
        id: "tabSearch"
    });

    $(".Add_btn").click(function () {
        sessionStorage.setItem('TabId', '');
        layer.open({
            type: 2,
            title: '添加',
            area: ['560px', '320px'], //长宽
            content: '/System/RoleAdd',
            end: function () {
                table.reload('tabSearch', {});
            }
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
            })
        }
    };
 

    table.on('tool(getTable)', function (obj) {
        var v = {};
        v.Id = obj.data.Id;
        if (obj.event === 'del') {
            layer.confirm('确认删除该数据?', function (index) {
                xh.Post('/api/Role/Del', v, function (d) {
                    if (d.suc) {
                        obj.del();
                        layer.msg("删除成功!");
                        //table.reload("tabSearch", {});
                    } else {
                        layer.msg(d.msg);
                    }
                });
            });
        } else if (obj.event === 'edit') {
            sessionStorage.setItem('TabId', obj.data.Id);
            layer.open({
                type: 2,
                title: '修改',
                area: ['560px', '380px'], //长宽
                content: '/System/RoleAdd',
                end: function () {
                    var UpdateTab = sessionStorage.getItem('UpdateTab');

                    if (UpdateTab != '' & UpdateTab != null) {
                        var jsonTab = JSON.parse(UpdateTab);
                        obj.update(jsonTab);
                    }
                }
            });
        } else if (obj.event === "menu") {
            sessionStorage.setItem('roleId', obj.data.Id);
            layer.open({
                type: 2,
                title: '菜单设置',
                area: ['560px', '560px'], //长宽
                content: '/System/RoleMenu',
                end: function () {

                }
            })
        } else if (obj.event == "person") {
            sessionStorage.setItem('roleId', obj.data.Id);
            layer.open({
                type: 2,
                title: '人员设置',
                area: ['520px', '360px'], //长宽
                content: '/Role/Person',
                end: function () {

                }
            })
        }
    });
})