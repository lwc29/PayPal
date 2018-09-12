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
        url: '/api/TakeList/GetTakeList',
        cols: [[
            { field: 'StoreName', title: '商家名称', width: 150 },
            { field: 'Balance', title: '余额', width: 150 },
            { field: 'TakeOut', title: '提现金额', width: 180 },
            { field: 'CreateTime', title: '提现时间', width: 180 },
            { field: 'TakeTypeName', title: '收款渠道', width: 130 },
            {
                field: 'StateName', title: '状态', width: 170, templet: function (d) {
                    if (d.State == 1)
                        return '<span style="color: green">成功</span>';
                    else if (d.State == 2)
                        return '<span style="color: red">失败</span>';
                    else if (d.State == 0)
                        return '<span style="color: pink">打款中...</span>'
                }

            },
            { field: 'Account', title: '收款人', width: 170 },
            { field: 'AccountId', title: '收款人账号', width: 170 },
            { field: 'Out_trade_no', title: '平台订单', width: 280 }
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