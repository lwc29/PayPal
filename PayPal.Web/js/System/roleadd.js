layui.config({
    base: "/js/"
}).use(['form', 'table', 'formHelp'], function () {
    var form = layui.form,
        layer = parent.layer === undefined ? layui.layer : top.layer,
        $ = layui.jquery,
        formHelp = layer.formHelp,
		table = layui.table;

    var index = parent.layer.getFrameIndex(window.name);
    var Id = sessionStorage.getItem('TabId');

    document.getElementById("addForm").reset();
    sessionStorage.setItem('UpdateTab', '');

    //加载默认数据
 
    if (Id != '') {
        var that = this;
        xh.Post('/api/Role/GetModel', { Id: Id }, function (d) {
            if (d.suc) {
                var dt = d.data;
                $('#addForm').formEdit(dt);
                form.render();
            } else
                layer.msg(d.msg, {
                    icon: 5
                });
        });
    }
 

    //监听提交
    form.on('submit(Add)', function (data) {
        var v = data.field;

        var url = "/api/Role/Add";
        if (Id != "") {
            url = "/api/Role/Update";
            v.Id = Id;
        }
        var tanchu = top.layer.msg('数据提交中，请稍候', {
            icon: 16,
            time: false,
            shade: 0.8
        });
   
        xh.Post(url, v,
            function (d) {
                top.layer.close(tanchu);
                if (d.suc) {
                    sessionStorage.setItem('UpdateTab', JSON.stringify(v));
                    top.layer.msg("操作成功!");
                    setTimeout(function () {
                        parent.layer.close(index);
                    }, 200);
                }
                else
                    layer.msg(d.msg, { icon: 5 });
            });
        return false;
    })
});
