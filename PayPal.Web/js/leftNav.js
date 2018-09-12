function navBar(strData){
	var data;
	if(typeof(strData) == "string"){
		var data = JSON.parse(strData); //部分用户解析出来的是字符串，转换一下
	} else {
	    if (strData.Code != 200)
	        return;
	    data = strData.Result;
	    $(".userName").text(strData.Message);
	}
	
	var ulHtml = '<ul class="layui-nav layui-nav-tree"><li class="layui-nav-item layui-this"><a><i class="layui-icon" data-icon=""></i><cite>后台首页</cite></a></li>';
	for (var i = 0; i < data.length; i++) {
	    if (data[i].Spread || data[i].Spread == undefined) {
	        ulHtml += '<li class="layui-nav-item layui-nav-itemed">';
	    } else {
	        ulHtml += '<li class="layui-nav-item">';
	    }
	    if (data[i].Children != undefined && data[i].Children.length > 0) {
	        ulHtml += '<a>';
	        if (data[i].Icon != undefined && data[i].Icon != '') {
	            if (data[i].Icon.indexOf("Icon-") != -1) {
	                ulHtml += '<i class="seraph ' + data[i].Icon + '" data-icon="' + data[i].Icon + '"></i>';
	            } else {
	                ulHtml += '<i class="layui-icon" data-icon="' + data[i].Icon + '">' + data[i].Icon + '</i>';
	            }
	        }
	        ulHtml += '<cite>' + data[i].Title + '</cite>';
	        ulHtml += '<span class="layui-nav-more"></span>';
	        ulHtml += '</a>';
	        ulHtml += '<dl class="layui-nav-child">';
	        for (var j = 0; j < data[i].Children.length; j++) {
	            if (data[i].Children[j].target == "_blank") {
	                ulHtml += '<dd><a data-url="' + data[i].Children[j].Href + '" target="' + data[i].Children[j].target + '">';
	            } else {
	                ulHtml += '<dd><a data-url="' + data[i].Children[j].Href + '">';
	            }
	            if (data[i].Children[j].Icon != undefined && data[i].Children[j].Icon != '') {
	                if (data[i].Children[j].Icon.indexOf("Icon-") != -1) {
	                    ulHtml += '<i class="seraph ' + data[i].Children[j].Icon + '" data-icon="' + data[i].Children[j].Icon + '"></i>';
	                } else {
	                    ulHtml += '<i class="layui-icon" data-icon="' + data[i].Children[j].Icon + '">' + data[i].Children[j].Icon + '</i>';
	                }
	            }
	            ulHtml += '<cite>' + data[i].Children[j].Title + '</cite></a></dd>';
	        }
	        ulHtml += "</dl>";
	    } else {
	        if (data[i].target == "_blank") {
	            ulHtml += '<a data-url="' + data[i].Href + '" target="' + data[i].target + '">';
	        } else {
	            ulHtml += '<a data-url="' + data[i].Href + '">';
	        }
	        if (data[i].Icon != undefined && data[i].Icon != '') {
	            if (data[i].Icon.indexOf("Icon-") != -1) {
	                ulHtml += '<i class="seraph ' + data[i].Icon + '" data-icon="' + data[i].Icon + '"></i>';
	            } else {
	                ulHtml += '<i class="layui-icon" data-icon="' + data[i].Icon + '">' + data[i].Icon + '</i>';
	            }
	        }
	        ulHtml += '<cite>' + data[i].Title + '</cite></a>';
	    }
	    ulHtml += '</li>';
	}
	return ulHtml;
}
