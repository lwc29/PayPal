layui.use(['form', 'layer', 'table'], function () {
    var form = layui.form,
        layer = parent.layer === undefined ? layui.layer : top.layer,
        $ = layui.jquery,
        table = layui.table;
 

    //订单流水
    var tableIns = table.render({
        elem: '#getTable',
        url: '/api/Store/GetStoreJifen',
        cols: [[
            { field: 'StoreName', title: '商家名称', width: 150 },
            { field: 'Point', title: '积分', width: 280 },
            {
                title: '积分明细', width: 150, fixed: "right", align: "center", templet: function () {
                    return '<a class="layui-btn layui-btn-xs" lay-event="view">查看</a>';
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
            Name: $(".searchVal").val()
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
                    Name: $(".searchVal").val()
                }
            })
        }
    };

    table.on('tool(getTable)', function (obj) {
        if (obj.event === 'view') {
            sessionStorage.setItem('TabId', obj.data.Id);
            layer.open({
                type: 2,
                title: '积分明细',
                area: ['880px', '680px'], //长宽
                content: '/JiFen/Detail',
                end: function () {
                    
                }
            });
        } 
    });
    
})