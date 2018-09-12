layui.use(['form', 'layer', 'laydate', 'table', 'upload'], function () {
    var form = layui.form,
        layer = parent.layer === undefined ? layui.layer : top.layer,
        $ = layui.jquery,
        laydate = layui.laydate,
        upload = layui.upload,
        table = layui.table;

    laydate.render({
        elem: '#start',
    });

    laydate.render({
        elem: '#end',
    });

    //订单流水
    var tableIns = table.render({
        elem: '#getTable',
        url: '/api/TakeCheck/GetTakeCheckList',
        cols: [[
            { field: 'StoreName', title: '商家名称', width: 150 },
            { field: 'TakeOut', title: '体现金额', width: 280 },
            { field: 'BankNo', title: '银行卡', width: 180 },
            { field: 'Bank', title: '开户行', width: 150 },
            { field: 'CreateTime', title: '申请时间', width: 170 },
            { field: 'ConfirmTime', title: '打款确认时间', width: 170 },
            {
                title: '操作', width: 150, fixed: "right", align: "center", templet: function (d) {
                    if (!d.ConfirmTime)
                        return '<a class="layui-btn layui-btn-xs" lay-event="confirm">确认打款</a>';
                    else
                        return "";
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
            Start: $("#start").val(),
            End: $("#end").val()

        }
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
                    Name: $(".searchVal").val(),
                    Start: $("#start").val(),
                    End: $("#end").val()
                }
            })
        }, excel: function () {
            xh.Post('/api/TakeCheck/GetTakeCheckExcel', {
                Name: $(".searchVal").val(),
                Start: $("#start").val(),
                End: $("#end").val()
            }, function (d) {
                if (d.suc) {
                    //layer.msg(d.data);
                    window.location.href = d.data;
                }
                else layer.msg(d.msg);
            })
        }, muban: function () {
            window.location.href = "/Excel/import.xls";
        }
    };

    upload.render({
        elem: ".import",
        xhr: xh.xhrOnProgress,
        size: '3145728',
        accept: 'file',
        exts:'xls|xlsx',
        url: '/api/TakeCheck/Import',
        progress: function (value) {//上传进度回调 value进度值
            console.log(value + '%');
        },
        done: function (res, index, upload) {
            if (res.suc) {
                layer.msg(res.msg);
                table.reload('tabSearch', {});
            } else {
                layer.msg(res.msg, {
                    icon: 5
                });
                return false;
            }
        }, error: function (index, upload) {
            layer.msg("导入失败!", {
                icon: 5
            });
            return false;
        }
    });

    table.on('tool(getTable)', function (obj) {
        var v = {};
        v.Id = obj.data.Id;
        if (obj.event === 'confirm') {
            //layer.confirm('确认删除该数据?', function (index) {
            xh.Post('/api/TakeCheck/Update', v, function (d) {
                if (d.suc) {
                    table.reload('tabSearch', {});
                    layer.msg("确认成功!");
                } else {
                    layer.msg(d.msg);
                }
            });
            // });
        }
    });
})