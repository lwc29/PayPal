
var xh = xh || {};
!function ($) {
     
    xh.AddUrl = "http://localhost:22950/";
    xh.Domain = "..";
    xh.Post = function (url, data, sucCallback, errCallback) {
        ajax(url, data,true, "POST", sucCallback, errCallback);
    };

    xh.Post2 = function (url, data, sucCallback, errCallback) {
        ajax(url, data, false, "POST", sucCallback, errCallback);
    };

    xh.Get = function (url, data, sucCallback, errCallback) {
        ajax(url, data,true, "Get", sucCallback, errCallback);
    };

    xh.Get2 = function (url, data, sucCallback, errCallback) {
        ajax(url, data, false, "Get", sucCallback, errCallback);
    };

    xh.Href = function (url) {
        location.href = xh.Domain + url;
    };

    xh.Error = function () {

    };
    xh.Limits = [10, 15, 20, 25];

    Array.prototype.findIndex = function (val, name) {
        for (var i = 0; i < this.length; i++) {
            if (this[i][name] == val)
                return this[i];
        }
        return [];
    };

    function ajax(url, data,async, type, sucCallback, errCallback) {
        if (typeof data !== 'string') {
            data = JSON.stringify(data);
        }
       
        var req =
        {
            type: type,
            url: url,
            data: data,
            async: async,
           // timeout: 15000,
            contentType: "application/json",
            success: function (dt, status, xhr) {
                if (sucCallback) {
                    if (dt.statusCode == "110")
                        location.href = "/Home/Login";
                    sucCallback(dt);
                }
            },
            error: function (request, textStatus, errorThrown) {
                if (textStatus === "timeout")
                    request.msg = "请求超时!";
                if (errCallback) {
                    errCallback(request, textStatus, errorThrown);
                }
                else  
                    alert(request.msg || textStatus);
            }
        };

        $.ajax(req);
    }

    xh.FormatMoney = function () {
        if (/[^0-9\.]/.test(s)) return "invalid value";
        s = s.replace(/^(\d*)$/, "$1.");
        s = (s + "00").replace(/(\d*\.\d\d)\d*/, "$1");
        s = s.replace(".", ",");
        var re = /(\d)(\d{3},)/;
        while (re.test(s))
            s = s.replace(re, "$1,$2");
        s = s.replace(/,(\d\d)$/, ".$1");
        return "￥" + s.replace(/^\./, "0.");
    }

    xh.getRootPath = function () {
       var domain = sessionStorage.getItem("Dodmain");
       if (!domain) {
           var strFullPath = window.document.location.href;
           var strPath = window.document.location.pathname;
           var pos = strFullPath.indexOf(strPath);
           var prePath = strFullPath.substring(0, pos);
           var postPath = strPath.substring(0, strPath.substr(1).indexOf('/') + 1);
           return (prePath + postPath);
       }
       return domain; 
    }

    //创建监听函数
    xh.xhrOnProgress = function (fun) {
        xh.xhrOnProgress.onprogress = fun; //绑定监听
        //使用闭包实现监听绑
        return function () {
            //通过$.ajaxSettings.xhr();获得XMLHttpRequest对象
            var xhr = $.ajaxSettings.xhr();
            //判断监听函数是否为函数
            if (typeof xh.xhrOnProgress.onprogress !== 'function')
                return xhr;
            //如果有监听函数并且xhr对象支持绑定时就把监听函数绑定上去
            if (xh.xhrOnProgress.onprogress && xhr.upload) {
                xhr.upload.onprogress = xh.xhrOnProgress.onprogress;
            }
            return xhr;
        }
    }
}(jQuery);

