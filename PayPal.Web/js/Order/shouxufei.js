layui.use(['form', 'layer', 'laydate', 'table'], function () {
    var form = layui.form,
        layer = parent.layer === undefined ? layui.layer : top.layer,
        $ = layui.jquery,
        laydate = layui.laydate,
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
        url: '/api/TipsFlow/GetTipsFlowList',
        cols: [[
            { field: 'StoreName', title: '商家名称', width: 150 },
            { field: 'TakeOut', title: '提现金额', width: 180 },
            { field: 'Account', title: '账户人', width: 100 },
            { field: 'Tips', title: '扣取手续费', width: 130 },
            { field: 'CreateTime', title: '扣取时间', width: 170 },
            {
                field: 'Per', title: '手续费比率', width: 100, templet: function (d) {
                    return d.Per * 100 + "%";
                }
            },
            { field: 'Remark', title: '备注', width: 170 },
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
        }
    };

     
})