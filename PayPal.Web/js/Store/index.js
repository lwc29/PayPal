layui.use(['form', 'layer', 'laydate', 'table', 'upload'], function () {
    var form = layui.form,
        layer = parent.layer === undefined ? layui.layer : top.layer,
        $ = layui.jquery,
        laydate = layui.laydate,
        upload = layui.upload,
        table = layui.table;
 

    //商家列表
    var tableIns = table.render({
        elem: '#getTable',
        url: '/api/Store/GetStoreList',
        cols: [[
            { type: "checkbox", fixed: "left", width: 50 },
            { field: 'StoreName', title: '商家名称', width: 210 },
            {
                field: 'PName', title: '商家位置', width: 160, toolbar: '#addressTpl'
            },
            { field: 'StoreTypeName', title: '行业', width: 80, sort: true },
            { field: 'Mobile', title: '手机号', width: 130 },
            { field: 'Point', title: '积分', width: 100 },
            { field: 'DeviceId', title: '设备ID', width: 130 },
            {
                field: 'IsActiveText', title: '当前状态', width: 100, align: 'center', sort: true, unresize: true,toolbar: '#isActiveTpl'
            },
            
            { field: 'CreateTime', title: '时间', width: 170 },
            {
                title: '二维码', width: 110, align: "center", templet: function () {
                    return '<a class="layui-btn layui-btn-xs" lay-event="qrdown">下载</a>'
                }
            },
            {
                title: '收款账号', width: 110, align: "center", templet: function () {
                    return '<a class="layui-btn layui-btn-xs" lay-event="skedit">设置</a>'
                }
            },
            {
                title: '手续费', width: 110, align: "center", templet: function () {
                    return '<a class="layui-btn layui-btn-xs" lay-event="sxfedit">设置</a>'
                }
            },
            {
                title: '操作', width: 150, fixed: "right", align: "center", templet: function () {
                    return '<a class="layui-btn layui-btn-xs" lay-event="edit">编辑</a><a class="layui-btn layui-btn-xs layui-btn-danger" lay-event="del">删除</a>';
                }
            }
        ]],
        page: true,
        cellMinWidth: 105,
        height: "full-104",
        limit: 20,
        limits: [10, 15, 20, 25],
        id: "tabSearch",
        where: {
            Name: $(".searchVal").val(),
            StoreType: $("select[name=IsActive]").val()
        }
    });

    $(".Add_btn").click(function () {
        sessionStorage.setItem('TabId', '');
        layer.open({
            type: 2,
            title: '添加',
            area: ['800px', '650px'], //长宽
            content: '/Store/Add',
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
                where: {
                    Name: $(".searchVal").val(),  //搜索的关键字
                    StoreType: $("select[name=IsActive]").val()
                }
            })
        }
    };

    //是否禁用
    form.on('switch(newsTop)', function (data) {
 
        var index = layer.msg('修改中，请稍候', { icon: 16, time: false, shade: 0.8 });
        xh.Post('/api/Store/Update', { Id: data.elem.getAttribute("sid"), CreateTime: new Date() }, function () {
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
            layer.confirm('确认删除该数据?', function (index) {
                xh.Post('/api/Store/Del', v, function (d) {
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
            sessionStorage.setItem('TabId', obj.data.Id);
            layer.open({
                type: 2,
                title: '修改',
                area: ['800px', '650px'], //长宽
                content: '/Store/Add',
                end: function () {
                    var UpdateTab = sessionStorage.getItem('UpdateTab');

                    if (UpdateTab != '' & UpdateTab != null) {
                        var jsonTab = JSON.parse(UpdateTab);
                        jsonTab.PName += jsonTab.CName + jsonTab.AName;
                        jsonTab.IsActive = !!jsonTab.IsActive;
                        obj.update(jsonTab);
                       // form.render("checkbox");
                    }
                }
            });
        } else if (obj.event == "qrdown") {
            layer.open({
                title: "商家二维码",
                area: ['760px', '433px'],
                type: "1",
                content: "<div class='qr' style='padding:10px'><img class='p1' src='" + obj.data.QRDown + "'>&nbsp;&nbsp;&nbsp;&nbsp;<img class='p2' style='float:right;' src='" + obj.data.QRUp + "'></div> ",
                success: function (layero, index) {
                   
                    $(".p1", layero).on("click", function () {
                        window.location.href = "/api/File/Down?Id=" + obj.data.Id;
                    });

                    upload.render({
                        elem: $(".p2", layero),
                        xhr: xh.xhrOnProgress,
                        url: '/api/File/Upload',
                        progress: function (value) {//上传进度回调 value进度值
                            console.log(value + '%');
                        },
                        before: function (dt) {
                            this.data = { 'Id': obj.data.Id };
                        },
                        done: function (res, index, upload) {
                            if (res.suc) {
                                $(".p2", layero).attr("src",res.data);
                                layer.msg(res.msg);
                            } else {
                                layer.msg("文件上传失败!", {
                                    icon: 5
                                });
                                return false;
                            }
                            
                        }, error: function (index, upload) {
                            layer.msg("文件上传异常!", {
                                icon: 5
                            });
                            return false;
                        }
                    });
                }
            })
        }
        else if (obj.event == "skedit") {
            sessionStorage.setItem('skedit', obj.data.Id);
            layer.open({
                type: 2,
                title: '收款账号',
                area: ['520px', '360px'], //长宽
                content: '/Store/SKAccount',
                end: function () {
                    
                }
            })
        }
        else if (obj.event == "sxfedit") {
            sessionStorage.setItem('sxfedit', obj.data.Id);
            layer.open({
                type: 2,
                title: '手续费设置',
                area: ['520px', '360px'], //长宽
                content: '/Store/ShouXuFei',
                end: function () {
                    
                }
            })
        }
    });
})