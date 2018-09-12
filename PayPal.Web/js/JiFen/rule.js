layui.config({
    base: "/js/"
}).use(['form', 'layer', 'formHelp'], function () {
    var form = layui.form,
		layer = parent.layer === undefined ? layui.layer : top.layer,
        formHelp = layer.formHelp,
		$ = layui.jquery;

    form.on("submit(rule)", function (data) {
        var index = top.layer.msg('数据提交中，请稍候', { icon: 16, time: false, shade: 0.8 });
        xh.Post("/api/Setting/Update", { Name: "PointRule", Value: $(".rule1").val() + "|" + $(".rule2").val() + "|" + $(".rule3").val() },
             function (d) {
                 if (d.suc) {
                     layer.close(index);
                     layer.msg("积分规则修改成功！");
                 }    
             }
         )
        return false;
    })

    form.verify({
        rule1: function (val) {
            if (parseInt(val) < 0) {
                return "规则1有误";
            }
        },
        rule2: function (val) {
            if (parseFloat(val) < 0) {
                return "规则2有误";
            }
        },
        rule3: function (val) {
            if (parseFloat(val) < 0) {
                return "规则3有误";
            }
        }
    })

    //加载默认数据
    xh.Post("/api/Setting/GetSetting",{ Name: "PointRule" },
         function (d) {
             if (d.suc) {
                 fillData(d.data);
             }
         }
    )
     

    //填充数据方法
    function fillData(data) {
        data = data.Value.split("|");
        $(".rule1").val(data[0]);  
        $(".rule2").val(data[1]);
        $(".rule3").val(data[2]);
    }

})
