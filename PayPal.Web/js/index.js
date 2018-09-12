var $, tab, skyconsWeather;
layui.config({
    base: "/js/"
}).use(['bodyTab', 'form', 'element', 'layer', 'jquery'], function () {
    var form = layui.form,
		layer = layui.layer,
		element = layui.element;
    $ = layui.jquery;
    tab = layui.bodyTab({
        openTabNum: "50",  //最大可打开窗口数量
        url: "/api/Module/GetModule" //获取菜单json地址
    });
   
    //var user = JSON.parse(sessionStorage.getItem("user"))
    //if(user != null)
    //    $(".userName").text(user.username);
        //获取系统时间
        var newDate = '';

        //值小于10时，在前面补0
        function dateFilter(date) {
            if (date < 10) { return "0" + date; }
            return date;
        }
  
        setInterval(function getLangDate() {
            var dateObj = new Date(); //表示当前系统时间的Date对象
            var year = dateObj.getFullYear(); //当前系统时间的完整年份值
            var month = dateObj.getMonth() + 1; //当前系统时间的月份值
            var date = dateObj.getDate(); //当前系统时间的月份中的日
            var day = dateObj.getDay(); //当前系统时间中的星期值
            var weeks = ["星期日", "星期一", "星期二", "星期三", "星期四", "星期五", "星期六"];
            var week = weeks[day]; //根据星期值，从数组中获取对应的星期字符串
            var hour = dateObj.getHours(); //当前系统时间的小时值
            var minute = dateObj.getMinutes(); //当前系统时间的分钟值
            var second = dateObj.getSeconds(); //当前系统时间的秒钟值
            var timeValue = "" + ((hour >= 12) ? (hour >= 18) ? "晚上" : "下午" : "上午"); //当前时间属于上午、晚上还是下午
            newDate = dateFilter(year) + "年" + dateFilter(month) + "月" + dateFilter(date) + "日 " + " " + dateFilter(hour) + ":" + dateFilter(minute) + ":" + dateFilter(second);
            document.getElementById("nowTime").innerHTML = year + " " + timeValue + "好, 当前时间为： " + newDate + "　" + week;
        }, 1000)
 
    //更换皮肤
    function skins() {
        var skin = window.sessionStorage.getItem("skin");
        if (skin) { //如果更换过皮肤
            if (window.sessionStorage.getItem("skinValue") != "自定义") {
                $("body").addClass(window.sessionStorage.getItem("skin"));
            } else {
                $(".layui-layout-admin .layui-header").css("background-color", skin.split(',')[0]);
                $(".layui-bg-black").css("background-color", skin.split(',')[1]);
                $(".hideMenu").css("background-color", skin.split(',')[2]);
            }
        } else {
            //$("body").addClass("blue");
        }
    }
    skins();
    $(".changeSkin").click(function () {
        layer.open({
            title: "更换皮肤",
            area: ["310px", "280px"],
            type: "1",
            content: '<div class="skins_box">' +
                            '<form class="layui-form">' +
                                '<div class="layui-form-item">' +
                                    '<input type="radio" name="skin" value="默认" title="默认" lay-filter="default" checked="">' +
                                    '<input type="radio" name="skin" value="橙色" title="橙色" lay-filter="orange">' +
                                    '<input type="radio" name="skin" value="蓝色" title="蓝色" lay-filter="blue">' +
                                    '<input type="radio" name="skin" value="藏青" title="藏青色" lay-filter="cyan">' +
                                    '<input type="radio" name="skin" value="自定义" title="自定义" lay-filter="custom">' +
                                    '<div class="skinCustom">' +
                                        '<input type="text" class="layui-input topColor" name="topSkin" placeholder="顶部颜色" />' +
                                        '<input type="text" class="layui-input leftColor" name="leftSkin" placeholder="左侧颜色" />' +
                                        '<input type="text" class="layui-input menuColor" name="btnSkin" placeholder="顶部菜单按钮" />' +
                                    '</div>' +
                                '</div>' +
                                '<div class="layui-form-item skinBtn">' +
                                    '<a href="javascript:;" class="layui-btn layui-btn-sm layui-btn-normal" lay-submit="" lay-filter="changeSkin">确定更换</a>' +
                                    '<a href="javascript:;" class="layui-btn layui-btn-sm layui-btn-primary" lay-submit="" lay-filter="noChangeSkin">朕再想想</a>' +
                                '</div>' +
                            '</form>' +
                        '</div>',
            success: function (index, layero) {
                var skin = window.sessionStorage.getItem("skin");
                if (window.sessionStorage.getItem("skinValue")) {
                    $(".skins_box input[value=" + window.sessionStorage.getItem("skinValue") + "]").attr("checked", "checked");
                };
                if ($(".skins_box input[value=自定义]").attr("checked")) {
                    $(".skinCustom").css("visibility", "inherit");
                    $(".topColor").val(skin.split(',')[0]);
                    $(".leftColor").val(skin.split(',')[1]);
                    $(".menuColor").val(skin.split(',')[2]);
                };
                form.render();
                $(".skins_box").removeClass("layui-hide");
                $(".skins_box .layui-form-radio").on("click", function () {
                    var skinColor;
                    if ($(this).find("div").text() == "橙色") {
                        skinColor = "orange";
                    } else if ($(this).find("div").text() == "蓝色") {
                        skinColor = "blue";
                    } else if ($(this).find("div").text() == "藏青") {
                        skinColor = "cyan";
                    } else if ($(this).find("div").text() == "默认") {
                        skinColor = "";
                    }
                    if ($(this).find("div").text() != "自定义") {
                        $(".topColor,.leftColor,.menuColor").val('');
                        $("body").removeAttr("class").addClass("main_body " + skinColor + "");
                        $(".skinCustom").removeAttr("style");
                        $(".layui-bg-black,.hideMenu,.layui-layout-admin .layui-header").removeAttr("style");
                    } else {
                        $(".skinCustom").css("visibility", "inherit");
                    }
                })
                var skinStr, skinColor;
                $(".topColor").blur(function () {
                    $(".layui-layout-admin .layui-header").css("background-color", $(this).val() + " !important");
                })
                $(".leftColor").blur(function () {
                    $(".layui-bg-black").css("background-color", $(this).val() + " !important");
                })
                $(".menuColor").blur(function () {
                    $(".hideMenu").css("background-color", $(this).val() + " !important");
                })

                form.on("submit(changeSkin)", function (data) {
                    if (data.field.skin != "自定义") {
                        if (data.field.skin == "橙色") {
                            skinColor = "orange";
                        } else if (data.field.skin == "蓝色") {
                            skinColor = "blue";
                        } else if (data.field.skin == "藏青") {
                            skinColor = "cyan";
                        } else if (data.field.skin == "默认") {
                            skinColor = "";
                        } 
                        window.sessionStorage.setItem("skin", skinColor);
                    } else {
                        skinStr = $(".topColor").val() + ',' + $(".leftColor").val() + ',' + $(".menuColor").val();
                        window.sessionStorage.setItem("skin", skinStr);
                        $("body").removeAttr("class").addClass("main_body");
                    }
                    window.sessionStorage.setItem("skinValue", data.field.skin);
                    layer.closeAll("page");
                });
                form.on("submit(noChangeSkin)", function () {
                    $("body").removeAttr("class").addClass("main_body " + window.sessionStorage.getItem("skin") + "");
                    $(".layui-bg-black,.hideMenu,.layui-layout-admin .layui-header").removeAttr("style");
                    skins();
                    layer.closeAll("page");
                });
            },
            cancel: function () {
                $("body").removeAttr("class").addClass("main_body " + window.sessionStorage.getItem("skin") + "");
                $(".layui-bg-black,.hideMenu,.layui-layout-admin .layui-header").removeAttr("style");
                skins();
            }
        })
    })
    
    $('.doUpdatePwd').click(function () {
        layer.open({
            type: 2,
            title: '修改密码',
            area: ['500px', '320px'], //长宽
            content: '/User/UpdatePwd',
            success: function (layero, index) { }
        });
    });

    //退出
    $(".signOut").click(function () {
        window.sessionStorage.removeItem("menu");
        menu = [];
        window.sessionStorage.removeItem("curmenu");
    })

    //隐藏左侧导航
    $(".hideMenu").click(function () {
        $(".layui-layout-admin").toggleClass("showMenu");
        //渲染顶部窗口
        tab.tabMove();
    })

    //渲染左侧菜单
    tab.render();
 
     

    //手机设备的简单适配
    var treeMobile = $('.site-tree-mobile'),
		shadeMobile = $('.site-mobile-shade')

    treeMobile.on('click', function () {
        $('body').addClass('site-mobile');
    });

    shadeMobile.on('click', function () {
        $('body').removeClass('site-mobile');
    });

    $("body").on("click", ".layui-nav-tree li", function () {
        window.sessionStorage.setItem("menuclick", $(".layui-nav-tree li").index($(this)));
    })

    // 添加新窗口
    $("body").on("click", ".layui-nav .layui-nav-item a", function () {
        //如果不存在子级
        if ($(this).siblings().length == 0) {
            addTab($(this));
            $('body').removeClass('site-mobile');  //移动端点击菜单关闭菜单层
        }
        $(this).parent("li").siblings().removeClass("layui-nav-itemed");
    })

    //公告层
    function showNotice() {
        window.localStorage.setItem("showNotice", "true");
        if ($(window).width() > 432) {  //如果页面宽度不足以显示顶部“系统公告”按钮，则不提示
            layer.tips('系统公告躲在了这里', '#showNotice', {
                tips: 3,
                time: 1000
            });
        }
    }
    //判断
    if (window.localStorage.getItem("showNotice") != "true") {
        showNotice();
    }
 
    //刷新后还原打开的窗口
    if (window.sessionStorage.getItem("menu") != null) {
        menu = JSON.parse(window.sessionStorage.getItem("menu"));
        curmenu = window.sessionStorage.getItem("curmenu");
        var openTitle = '';
        for (var i = 0; i < menu.length; i++) {
            openTitle = '';
            if (menu[i].icon) {
                if (menu[i].icon.split("-")[0] == 'icon') {
                    openTitle += '<i class="iconfont ' + menu[i].icon + '"></i>';
                } else {
                    openTitle += '<i class="layui-icon">' + menu[i].icon + '</i>';
                }
            }
            openTitle += '<cite>' + menu[i].title + '</cite>';
            openTitle += '<i class="layui-icon layui-unselect layui-tab-close" data-id="' + menu[i].layId + '">&#x1006;</i>';
            element.tabAdd("bodyTab", {
                title: openTitle,
                content: "<iframe src='" + menu[i].href + "' data-id='" + menu[i].layId + "'></frame>",
                id: menu[i].layId
            })
            //定位到刷新前的窗口
            if (curmenu != "undefined") {
                if (curmenu == '' || curmenu == "null") {  //定位到后台首页
                    element.tabChange("bodyTab", '');
                } else if (JSON.parse(curmenu).title == menu[i].title) {  //定位到刷新前的页面
                    element.tabChange("bodyTab", menu[i].layId);
                }
            } else {
                element.tabChange("bodyTab", menu[menu.length - 1].layId);
            }
        }
        //渲染顶部窗口
        tab.tabMove();
    }

    //刷新当前
    $(".refresh").on("click", function () {
        if ($(this).hasClass("refreshThis")) {
            $(this).removeClass("refreshThis");
            $(".clildFrame .layui-tab-item.layui-show").find("iframe")[0].contentWindow.location.reload(true);
        } else {
            layer.msg("您点击的速度超过了服务器的响应速度，还是等两秒再刷新吧！");
            setTimeout(function () {
                $(".refresh").addClass("refreshThis");
            }, 2000)
        }
    })

    //关闭其他
    $(".closePageOther").on("click", function () {
        if ($("#top_tabs li").length > 2 && $("#top_tabs li.layui-this cite").text() != "后台首页") {
            var menu = JSON.parse(window.sessionStorage.getItem("menu"));
            $("#top_tabs li").each(function () {
                if ($(this).attr("lay-id") != '' && !$(this).hasClass("layui-this")) {
                    element.tabDelete("bodyTab", $(this).attr("lay-id")).init();
                    //此处将当前窗口重新获取放入session，避免一个个删除来回循环造成的不必要工作量
                    for (var i = 0; i < menu.length; i++) {
                        if ($("#top_tabs li.layui-this cite").text() == menu[i].title) {
                            menu.splice(0, menu.length, menu[i]);
                            window.sessionStorage.setItem("menu", JSON.stringify(menu));
                        }
                    }
                }
            })
        } else if ($("#top_tabs li.layui-this cite").text() == "后台首页" && $("#top_tabs li").length > 1) {
            $("#top_tabs li").each(function () {
                if ($(this).attr("lay-id") != '' && !$(this).hasClass("layui-this")) {
                    element.tabDelete("bodyTab", $(this).attr("lay-id")).init();
                    window.sessionStorage.removeItem("menu");
                    menu = [];
                    window.sessionStorage.removeItem("curmenu");
                }
            })
        } else {
            layer.msg("没有可以关闭的窗口了@_@");
        }
        //渲染顶部窗口
        tab.tabMove();
    })
    //关闭全部
    $(".closePageAll").on("click", function () {
        if ($("#top_tabs li").length > 1) {
            $("#top_tabs li").each(function () {
                if ($(this).attr("lay-id") != '') {
                    element.tabDelete("bodyTab", $(this).attr("lay-id")).init();
                    window.sessionStorage.removeItem("menu");
                    menu = [];
                    window.sessionStorage.removeItem("curmenu");
                }
            })
        } else {
            layer.msg("没有可以关闭的窗口了@_@");
        }
        //渲染顶部窗口
        tab.tabMove();
    })
})

//打开新窗口
function addTab(_this) {
    tab.tabAdd(_this);
}


