;
layui.define(["form", "jquery"], function (exports) {
    var form = layui.form,
   
    $ = layui.jquery,
    Address = function () {  };

    var data = [];

    Address.prototype.get = function (p,c,a) {
        var that = this;
        var province = JSON.parse(localStorage.getItem("Area_" + p));
        if (province == null)
        {
            var tanchu = top.layer.msg('获取数据，请稍候', {
                icon: 16,
                time: false,
                shade: 0.8
            });
            xh.Get2("/api/Module/GetArea?id=" + p, null,
                function (d) {
                    top.layer.close(tanchu);
                    localStorage.setItem("Area_" + p, JSON.stringify(d.Result));
                    province = d.Result;
                }
            );
        }
        that.citys(province);
        if (c > 0) {
            var city = province.findIndex(c, "Id");
            that.areas(city.Childrens);
        }
    }

    Address.prototype.provinces = function () {
        //加载省数据
        var proHtml = '', that = this;
        var data = JSON.parse(localStorage.getItem("Area"));
        if (data == null)
             parent.location.reload();
        for (var i = 0, val; val = data[i++];) {
            proHtml += '<option value="' + val.Id + '">' + val.Shortname + '</option>';
        }
        //初始化省数据
        $("select[name=Province]").append(proHtml);
     
        form.on('select(Province)', function (proData) {
            $("select[name=Area]").html('<option value="">请选择县/区</option>');
            var value = parseInt(proData.value);
            if (value > 0) {
                that.get(value);  
            } else {
                $("select[name=City]").attr("disabled", "disabled");
            }
        });
        form.render();
        return this;
    }
 
    //加载市数据
    Address.prototype.citys = function(citys) {
        var cityHtml = '<option value="">请选择市</option>',that = this;
        for (var i = 0; i < citys.length; i++) {
            cityHtml += '<option value="' + citys[i].Id + '">' + citys[i].Area_name + '</option>';
        }
        $("select[name=City]").html(cityHtml).removeAttr("disabled");
        data = citys;
        form.render();
        form.on('select(City)', function (cityData) {
            var value = parseInt(cityData.value);
            if (value > 0) {
                var arr = $.grep(data, function (n, i) {
                    return n.Id == value;
                });
                that.areas(arr[0].Childrens);
            } else {
                $("select[name=Area]").attr("disabled", "disabled");
            }
        });
    }
 
    //加载县/区数据
    Address.prototype.areas = function(areas) {
        var areaHtml = '<option value="">请选择县/区</option>';
        for (var i = 0; i < areas.length; i++) {
            areaHtml += '<option value="' + areas[i].Id + '">' + areas[i].Area_name + '</option>';
        }
        $("select[name=Area]").html(areaHtml).removeAttr("disabled");
        form.render();
    }
 
    var address = new Address();
    exports("address",function(){
       return address.provinces();
    });
    
})