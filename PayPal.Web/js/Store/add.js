layui.config({
    base: "/js/"
}).use(['form', 'table', 'laydate', 'formHelp', 'address'], function () {
    var form = layui.form,
        layer = parent.layer === undefined ? layui.layer : top.layer,
        $ = layui.jquery,
		laydate = layui.laydate,
        formHelp = layer.formHelp,
        address = layui.address(),
		table = layui.table;

    var index = parent.layer.getFrameIndex(window.name);
    var Id = sessionStorage.getItem('TabId');
    //$(function () {
        document.getElementById("addForm").reset();
        sessionStorage.setItem('UpdateTab', '');

        //加载默认数据
        xh.Get("/api/StoreType/GetStoreType","",
             function (d) {
                 if (d.suc) {
                     var storetype = '';
                     for (var i = 0,val; val = d.data[i++];) {
                         storetype += '<option value="' + val.Id + '">' + val.StoreName + '</option>';
                     }
                     $("select[name=StoreType]").get(0).selectedIndex = 1;
                     $("select[name=StoreType]").html(storetype).removeAttr("disabled");
                 }
             }
        )

        if (Id != '') {
           
            var that = this;
            xh.Post('/api/Store/GetModel', { Id: Id }, function (d) {
                if (d.suc) {
                    var dt = d.data;
                    $(".layui-input-block button").eq(1).hide();
                    $("select[name=Province]").val(dt.Province);
                    var province = address.get(dt.Province, dt.City, dt.Area);
                    $("input[name=Mobile]").addClass("layui-disabled").attr("disabled", true);
                    $("input[name=DeviceId]").addClass("layui-disabled").attr("disabled", true);
                   // $("input[name=StoreName]").addClass("layui-disabled").attr("disabled", true);
                    $('#addForm').formEdit(dt);
                    form.render();
                } else
                    layer.msg(d.msg, {
                        icon: 5
                    });
            });
        }
   // });

    form.verify({
        storeName: function (val) {
            if (val == '') {
                return "商家名称不能为空";
            }
        },
        mobile: function (val) {
            if (val == '') {
                return "手机号码不能为空";
            }
            val = val.trim();
            var reg = myreg = /^1\d{10}$/;
            if (!reg.test(val)) {
                return "手机号码错误";
            }
            var v = { Mobile: val,Id :Id };
            var flag;
            xh.Post2("/api/Store/CheckMobile", v, function (d) {
                if (d.suc) {
                    if (!d.data) {
                        flag = d.data;
                    }
                }
            });
            
            if (flag == false) {
                return "电话号码已存在!";
            }
            return false;
        }
    })

 
    //监听提交
    form.on('submit(Add)', function (data) {
        var v = data.field;
         
        var url = "/api/Store/Add";
        if (Id != "") {
            url = "/api/Store/Update";
            v.Id = Id;
        }
        v.IsActive = 1;
        var tanchu = top.layer.msg('数据提交中，请稍候', {
            icon: 16,
            time: false,
            shade: 0.8
        });
        v.PName = $("select[name=Province]").find("option:selected").text(); 
        v.CName = $("select[name=City]").find("option:selected").text();
        v.AName = $("select[name=Area]").find("option:selected").text();
        v.StoreTypeName = $("select[name=StoreType]").find("option:selected").text();
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
