
layui.define(["element", "jquery"], function (exports) {
    var element = layui.element,
		$ = layui.jquery,
		formHelp = function () { };

    $.prototype.formEdit = function (data) {
        return this.each(function () {
            var input, name;
            if (data == null) { this.reset(); return; }
            for (var i = 0; i < this.length; i++) {
                input = this.elements[i];
                //checkbox的name可能是name[]数组形式
                name = (input.type == "checkbox") ? input.name.replace(/(.+)\[\]$/, "$1") : input.name;
                if (data[name] == undefined) continue;
                switch (input.type) {
                    case "checkbox":
                        input.checked = data[name];
                        break;
                    case "radio":
                        //if (data[name] == "") {
                        //    input.checked = false;
                        //} else
                        if (input.value == data[name]) {
                            input.checked = true;
                        }
                        else {
                            input.checked = false;
                        }
                        break;
                    case "button": break;
                    default: input.value = data[name];
                }
            }
        });
    };

    var formHelp = new formHelp();
    exports("formHelp", function () {
        return formHelp.formEdit();
    });
})