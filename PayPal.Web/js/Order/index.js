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
        url: '/api/Order/GetOrderList',
        cols: [[
            { field: 'Out_trade_no', title: '平台订单', width: 253 },
            { field: 'StoreName', title: '商家名称', width: 150 },
            { field: 'Buyer_id', title: '用户ID', width: 160 },
            { field: 'IsSuccess', title: '支付状态', width: 80, templet: function (d) {
                if(d.IsSuccess == 1)
                    return '<span style="color: green">成功</span>';
                else
                    return '<span style="color: red">失败</span>';
                }
             },
            { field: 'Total_amount', title: '付款金额(CNY)', width: 130 },
            { field: 'Tips', title: '扣费', width: 70 },
            { field: 'Gmt_payment', title: '支付时间', width: 170 },
            { field: 'PayTypeName', title: '支付渠道', width: 120, align:'center' },
            { field: 'Trade_no', title: '第三方支付订单ID', width: 280 },
            { field: 'Remark', title: '额外信息', width: 320 }
        ]],
        page: true,
        cellMinWidth: 105,
        height: "full-104",
        limit: 20,
        limits: [10, 15, 20, 25],
        id: "tabSearch",
        where: {
            Name: $(".searchVal").val(),
            Third: $("#thirdId").val(),
            Start: $("#start").val(),
            End: $("#end").val(),
            Type: $("select[name=PayType]").val()

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
                    Third: $("#thirdId").val(),
                    Start: $("#start").val(),
                    End: $("#end").val(),
                    Type: $("select[name=PayType]").val()
                }
            })
        },
        add: function () {
            var data = { Total_amount: 1.02, PayType: 5, Id: 26, StoreId: '42c32dae' };
            xh.Post("/api/Order/Add", data, function (d) {
                debugger;
                if (d.suc) {
                    alert(d.msg);
                }
                else {
                    alert(d.msg);
                }
            })
        }
    };
 
})