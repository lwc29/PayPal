layui.config({
    base: "/js/"
}).use(['form', 'table', 'formHelp'], function () {
    var form = layui.form,
        layer = parent.layer === undefined ? layui.layer : top.layer,
        $ = layui.jquery,
        formHelp = layer.formHelp,
		table = layui.table;

    var index = parent.layer.getFrameIndex(window.name);
    var Id = sessionStorage.getItem('sxfedit');

    if (parseInt(Id) > 0) {
        var that = this;
        xh.Post('/api/Collect/GetModel', { Id: Id }, function (d) {
            if (d.suc) {
                $('#accountForm').formEdit(d.data);
                $('#wxForm').formEdit(d.data);
                $('#aliForm').formEdit(d.data);
                $('#bankForm').formEdit(d.data);
                form.render();
            } else
                layer.msg(d.msg, {
                    icon: 5
                });
        });
    }
    else
        parent.layer.close(index);

    form.verify({
        wx: function (val) {
            if (parseFloat(val) < 0) {
                return "微信手续费设置有误";
            }
        },
        ali: function (val) {
            if (parseFloat(val) < 0) {
                return "支付宝手续费设置有误";
            }
        },
        bank: function (val) {
            if (val == '') {
                return "银行手续费设置有误";
            }
        }
    })

    //监听提交
    form.on('submit(add)', function (data) {
        var v = data.field;
        v.Id = Id;
       
        v.Account = $("#Account").val();
        if (!v.Account) {
            layer.msg("姓名不能为空");
            return false;
        }
        if (data.field.ALIPer == "0")
            v.Type = 1;
        else if (data.field.WXPer == "0")
            v.Type = 2;
        else if (data.field.BANKPer == "0")
            v.Type = 3;

        var tanchu = layer.msg('修改中，请稍候', { icon: 16, time: false, shade: 0.8 });
        xh.Post("/api/Collect/Update", v,
            function (d) {
                top.layer.close(tanchu);
                if (d.suc) {
                    top.layer.msg("操作成功!");

                }
                else
                    layer.msg(d.msg, { icon: 5 });
            });
        return false;
    })

});
