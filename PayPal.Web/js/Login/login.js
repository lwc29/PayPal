layui.config({
    base: "/js/"
}).use(['form', 'layer'], function () {

    if (self != top) {
        //如果在iframe中，则跳转
        top.location.replace("/Login/Index");
    }

    var form = layui.form,
		layer = parent.layer === undefined ? layui.layer : parent.layer,
		$ = layui.jquery;

    // Cloud Float...
    var $main = $cloud = mainwidth = null;
    var offset1 = 450;
    var offset2 = 0;
    var offsetbg = 0;

    $(document).ready(
        function () {
            $main = $("#mainBody");
            $body = $("body");
            $cloud1 = $("#cloud1");
            $cloud2 = $("#cloud2");

            mainwidth = $main.outerWidth();

        }
    );

    var _hmt = _hmt || [];
    (function() {
        var hm = document.createElement("script");
        hm.src = "//hm.baidu.com/hm.js?0558502420ce5fee054b31425e77ffa6";
        var s = document.getElementsByTagName("script")[0];
        s.parentNode.insertBefore(hm, s);
    })();

    setInterval(function flutter() {
        if (offset1 >= mainwidth) {
            offset1 = -580;
        }

        if (offset2 >= mainwidth) {
            offset2 = -580;
        }

        offset1 += 1.1;
        offset2 += 1;
        $cloud1.css({"background-position": offset1 + "px 100px"})

        $cloud2.css({"background-position": offset2 + "px 460px"})
    }, 70);
    setInterval(function bg() {
       
        if (offsetbg >= mainwidth) {
            offsetbg = -580;
        }

        offsetbg += 10.9;
        $body.css({"background-position": -offsetbg + "px 0"})
    }, 90);
    $(function () {
        //得到焦点
        $("#password").focus(function () {
            $("#left_hand").animate({
                left: "150",
                top: " -38"
            }, {
                step: function () {
                    if (parseInt($("#left_hand").css("left")) > 140) {
                        $("#left_hand").attr("class", "left_hand");
                    }
                }
            }, 2000);
            $("#right_hand").animate({
                right: "-64",
                top: "-38px"
            }, {
                step: function () {
                    if (parseInt($("#right_hand").css("right")) > -70) {
                        $("#right_hand").attr("class", "right_hand");
                    }
                }
            }, 2000);
        });
        //失去焦点
        $("#password").blur(function () {
            $("#left_hand").attr("class", "initial_left_hand");
            $("#left_hand").attr("style", "left:100px;top:-12px;");
            $("#right_hand").attr("class", "initial_right_hand");
            $("#right_hand").attr("style", "right:-112px;top:-12px");
        });

        $('.loginbox').css({ 'position': 'absolute', 'left': ($(window).width() - 692) / 2 });
        $(window).resize(function () {
            $('.loginbox').css({ 'position': 'absolute', 'left': ($(window).width() - 692) / 2 });
        })
    });

    //登录按钮事件
    form.on("submit(login)", function (data) {
        var tanchu = top.layer.msg('登录中，请稍候！', {
            icon: 16,
            time: false,
            shade: 0.8
        });
 
        data.field.Pwd = hex_md5(data.field.Pwd);
        xh.Post("/Login/Login"
            , data.field
            , function (d) {
                top.layer.close(tanchu);
                
                if (d.suc) {
                    layer.msg("登录成功,正在跳转...", {
                        icon: 6
                    });
                    sessionStorage.setItem("user", JSON.stringify({ username: d.data.UserName }));
                    window.location.href = d.data.ReturnUrl;
                }
                else {
                    layer.msg(d.msg, {
                        icon: 5
                    });
                }
            }, function (data) {
                top.layer.close(tanchu);
                layer.msg(data.statusText == "timeout" ? "服务器返回超时！" : '请求失败，请重试或联系管理员！', {
                    icon: 5
                });
            });
        return false;
    })
})
