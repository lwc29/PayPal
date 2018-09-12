layui.config({
    base: "/js/"
}).use(['form', 'element', 'layer', 'jquery'], function () {
    var form = layui.form,
		layer = parent.layer === undefined ? layui.layer : parent.layer,
		element = layui.element,
		$ = layui.jquery;

    $(".panel a").on("click", function () {
        window.parent.addTab($(this));
    })

    var area = localStorage.getItem("Area");
    if (area == null) {
        $.getJSON("/api/Module/GetArea",
    	    function (d) {
    	        localStorage.setItem("Area", JSON.stringify(d.Result));
    	    }
        )
    }



    //用户数
    $.getJSON("/api/Users/onLinePeople",
    	function(data){
    		$(".userAll span").text(data);
    	}
    )

    ////机构
    //$.getJSON("/UserSession/GetOrgs",
    //    function (data) {
    //        $(".orgs span").text(data.length);
    //    }
    //)

    ////机构
    //$.getJSON("/RoleManager/Load?limit=1&page=1",
    //    function (data) {
    //        $(".roles span").text(data.count);
    //    }
    //)

    ////我的流程
    //$.getJSON("/Flowinstances/Load?limit=1&page=1",
    //    function (data) {
    //        $(".flows span").text(data.count);
    //    }
    //)

    ////流程模板
    //$.getJSON("/flowschemes/Load?limit=1&page=1",
    //    function (data) {
    //        $(".flowschemes span").text(data.count);
    //    }
    //)

    ////表单
    //$.getJSON("/Forms/Load?limit=1&page=1",
    //    function (data) {
    //        $(".forms span").text(data.count);
    //    }
    //)

    //数字格式化
    $(".panel span").each(function () {
        $(this).html($(this).text() > 9999 ? ($(this).text() / 10000).toFixed(2) + "<em>万</em>" : $(this).text());
    })

    //系统基本参数
    $(".version").text("V1.0");      //当前版本
    $(".author").text("Lwc");        //开发作者
    $(".homePage").text("/Home/Index");    //网站首页
    $(".server").text("Windows Server 2012");        //服务器环境
    $(".dataBase").text("Sql Server 2012");    //数据库版本
    $(".maxUpload").text("2G");    //最大上传限制
    $(".userRights").text("管理员");//当前用户权限

})