
var glbSchoolName = "";
var glbImgStr = "";

//处理Ajax返回错误
function handleAjaxErr(result) {
    if (typeof(result) === 'string' && result.indexOf("redirectUrl") >= 0) {
        window.location.href = result.split(',')[1];
    }
    else if (typeof(result) === 'string' && result.indexOf("错误") >= 0) {
        swal({ title: "", text: "非常抱歉，系统突然抽风了，请及时联系相关人员", type: "error", timer: 2000, showConfirmButton: false });
    }
    else {
        swal({ title: "", text: result.errMsg, type:'warning' });
    }
}


//验证字符串是否是数字
function checkNumber(theObj) {
    var reg = /^[0-9]+.?[0-9]*$/;
    if (reg.test(theObj)) {
        return true;
    }
    return false;
}