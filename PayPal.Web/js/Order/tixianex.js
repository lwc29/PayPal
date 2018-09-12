layui.use(['form', 'layer', 'laydate', 'table'], function () {
    var form = layui.form,
        layer = parent.layer === undefined ? layui.layer : top.layer,
        $ = layui.jquery,
        laydate = layui.laydate,
        table = layui.table;
     

    //订单流水
    var tableIns = table.render({
        elem: '#getTable',
        url: '/api/Order/GetOrderListEx',
        cols: [[
            { field: 'out_biz_no', title: '平台订单', width: 250 },
            { field: 'order_id', title: '第三方支付订单ID', width: 280 },
            { field: 'msg', title: '状态', width: 130 },
            { field: 'pay_date', title: '付款时间', width: 170 },
            {
                title: '操作', width: 150, fixed: "right", align: "center", templet: function () {
                    return '<a class="layui-btn layui-btn-xs" lay-event="repair">修复</a>';
                }
            }
             
        ]],
        cellMinWidth: 105,
        height: "full-104",
        limit: 20,
        limits: [10, 15, 20, 25],
        id: "tabSearch"
    });


    $('.demoTable .layui-btn').on('click', function () {
        var type = $(this).data('type');
        active[type] ? active[type].call(this) : '';
    });

    var active = {
        reload: function () {
            table.reload("tabSearch", {});
        }
    };

    table.on('tool(getTable)', function (obj) {
        var v = {};
        v.Id = obj.data.Id;
        v.Out_trade_no = obj.data.out_biz_no;
        if (obj.event === 'repair') {
            xh.Post('/api/Order/UpdateEx', v, function (d) {
                if (d.suc) {
                    layer.msg("修复成功!");
                    table.reload("tabSearch", {});
                } else {
                    layer.msg(d.msg);
                }
            });
        }
    });

    
})