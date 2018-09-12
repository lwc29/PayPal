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
        url: '/api/Account/GetAccountList',
        cols: [[
            { field: 'StoreName', title: '商家名称', width: 150 },
            { field: 'TotalIn', title: '总收入', width: 180 },
            { field: 'TodayIn', title: '当天收入', width: 130 },
            { field: 'TakeOut', title: '已体现金额', width: 170 },
            { field: 'Balance', title: '余额', width: 120, align:'center' },
            //{
            //    title: '操作', width: 150, fixed: "right", align: "center", templet: function () {
            //        return '<a class="layui-btn layui-btn-xs" lay-event="dongjie">冻结</a>';
            //    }
            //}
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

    table.on('tool(getTable)', function (obj) {
        var v = {};
        v.Id = obj.data.Id;
        if (obj.event === 'dongjie') {
            layer.confirm('确认冻结该数据?', function (index) {
                alert("冻结成功");
                //xh.Post('api/Store/DelStore', v, function (d) {
                //    if (d.suc) {
                //        obj.del();
                //        layer.msg("删除成功!");
                //    } else {
                //        layer.msg(d.msg);
                //    }
                //});
            });
        }
    });
    
})