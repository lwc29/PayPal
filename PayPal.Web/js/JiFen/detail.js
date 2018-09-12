layui.use(['form', 'layer', 'laydate', 'table'], function () {
    var form = layui.form,
        layer = parent.layer === undefined ? layui.layer : top.layer,
        $ = layui.jquery,
        laydate = layui.laydate,
        table = layui.table;

    var Id = sessionStorage.getItem('TabId');

    laydate.render({
        elem: '#start',
    });

    laydate.render({
        elem: '#end',
    });

    //积分明细
    var tableIns = table.render({
        elem: '#getTable',
        url: '/api/PointDetail/GetPointDetail',
        cols: [[
            { field: 'FromTypeName', title: '来源', width: 180 },
            { field: 'BuyerId', title: '买家账户', width: 180 },
            { field: 'Money', title: '金额', width: 150 ,align:'center'},
            { field: 'Point', title: '积分', width: 150,align:'center' },
            { field: 'CreateTime', title: '时间', width: 180 }
        ]],
        page: true,
        cellMinWidth: 105,
      
        limit: 20,
        limits: [10, 15, 20, 25],
        id: "tabSearch",
        where: {
            Id: Id,
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
                    Id: Id,
                    Start: $("#start").val(),
                    End: $("#end").val(),
                    Type: $("select[name=PayType]").val()
                }
            })
        }
    };

})