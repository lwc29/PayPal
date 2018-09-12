layui.use(['form', 'table', 'laydate'], function () {
    var form = layui.form,
		laydate = layui.laydate,
		table = layui.table,
    layer = parent.layer === undefined ? layui.layer : top.layer;

    //添加验证规则
    form.verify({
        oldPwd: function (value, item) {
            if (value.length < 3 || value.length > 15) {
                return "密码长度3-15位！";
            }
        },
        newPwd: function (value, item) {
            if (value.length < 3 || value.length > 15) {
                return "密码长度3-15位！";
            }
        },
        confirmPwd: function (value, item) {
            if (!new RegExp($("#oldPwd").val()).test(value)) {
                return "两次输入密码不一致，请重新输入！";
            }
        }
    })


    //监听提交
    form.on('submit(changePwd)', function (data) {
        var tanchu = top.layer.msg('数据提交中，请稍候', {
            icon: 16,
            time: false,
            shade: 0.8
        });
        data.field.Pwd = hex_md5(data.field.Pwd);
        data.field.NewPwd = hex_md5(data.field.NewPwd);
        var index = parent.layer.getFrameIndex(window.name);
        xh.Post('/api/Users/UpdatePwd', data.field, function (d) {
            top.layer.close(tanchu);
            if (d.suc) {
                setTimeout(function () {
                    parent.layer.close(index);
                    window.location.href = "/Login/Logout";
                    top.layer.msg(d.msg);
                }, 200);
            } else {
                layer.msg(d.msg, {
                    icon: 5
                });
            }
        });
        return false;
    });
});