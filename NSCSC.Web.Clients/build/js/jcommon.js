var checkAll = false;
$(document).ready(function () {
    jQuery.validator.methods["date"] = function (value, element) { return true; }
});
/* Toogle check all checkboxes from table or div with given element selector, ex: "#divID", ".tableClass" */
function ToogleCheckAll(e, containElementSelector) {
    checkAll = $(e).prop("checked");
    $(containElementSelector).find("input[type='checkbox']").prop("checked", checkAll);
    if ($(e).prop('id') != 'select-all')
        ToggleBtnDelete();
}

/* Toogle check all checkboxes from table or div with given element selector, ex: "#divID", ".tableClass" */
function IndexCheckAll(containElementSelector, e) {
    if ($(e).hasClass("check-all")) {
        $(containElementSelector).find("input[type='checkbox']").prop("checked", true);
        $(e).removeClass("check-all").addClass("uncheck-all");
    }
    else if ($(e).hasClass("uncheck-all")) {
        $(containElementSelector).find("input[type='checkbox']").prop("checked", false);
        $(e).removeClass("uncheck-all").addClass("check-all");
    }
    ToggleBtnDelete();
}

function ToggleBtnDelete() {
    //if ($(".gridview tbody input[type='checkbox']:checked").length > 0) {
    //    $("#btn-delete").removeClass('disabled');
    //    $("#btn-actives").removeClass('disabled');
    //} else {
    //    $("#btn-delete").addClass('disabled');
    //    $("#btn-actives").addClass('disabled');
    //}

    var totalRow = $(".gridview tbody input[type='checkbox']").length;
    var totalChecked = $(".gridview tbody input[type='checkbox']:checked").length;

    if (totalRow == totalChecked) {
        $("#chb-checkall").prop('checked', true);
    } else {
        $("#chb-checkall").prop('checked', false);

    }
}

function checkItem() {
    var countCheck = $('.employee-items').find("input[type='checkbox']").length;
    var index = 0;
    for (var i = 0; i < countCheck; i++) {
        var item = $('.employee-items').find("input[id='ListEmployees_" + i + "__Checked']")
        if (item.prop('checked')) {
            index++;
        }
    }
    if (index == countCheck) {
        $("#checkAllEmp").prop('checked', true);
    }
    else {
        $("#checkAllEmp").prop('checked', false);
    }
}

function include(scriptUrl) {
    document.write('<script src="' + scriptUrl + '"></script>');
}


/*Call Search - edit*/
/** Global variables **/
var StoreID = "";
var ControllerName;
var SelectingCateID = "0";

var SelectingRoleID = "0";
var SelectingEmpID = "0";

var SelectingPrinterID = "0";
var ItemType = 0;
var listP = "";

// new function 23/07
function CreateAbsoluteUrl(actionName) {
    var getUrl = window.location;
    return BaseUrl + ControllerName + "/" + actionName;
}

//For Area in MVC
function CreateAbsoluteUrlArea(area, actionName) {
    var getUrl = window.location;
    return BaseUrl + area + "/" + ControllerName + "/" + actionName;
}

/** Functions **/
function PreviewImage(e, previewElementID) {
    var oFReader = new FileReader();
    oFReader.readAsDataURL(e.files[0]);

    oFReader.onload = function (oFREvent) {
        document.getElementById(previewElementID).src = oFREvent.target.result;
    };
};


/* Call View or Edit action with given url and show response (html) in .detail-view element */
function ShowViewOrEdit(action) {
    $.ajax({
        url: action,
        beforeSend: function () {
            $(".se-pre-con").show();
        },
        error: function (jqXHR, textStatus, errorThrown) {
            alert(errorThrown);
            $(".se-pre-con").hide();
        },
        success: function (html) {
            $(".se-pre-con").hide();
            ShowDetail(html);
        }
    });
}

function ShowDetail(content) {
    $(".detail-view").html(content);
    $(".detail-view").show();
    $(".gridview").css("display", "none");

}

function CloseDetail() {
    $(".detail-view").hide();
    $(".gridview").css("display", "block");
}

function SubmitForm(form) {
    $(form).submit();
}

function Search() {
    var form = $(".search-form");
    $.ajax({
        url: $(form).attr('action'),
        type: 'post',
        data: $(form).serialize(),
        dataType: "html",
        beforeSend: function () {
            $(".se-pre-con").show();
        },
        error: function (jqXHR, textStatus, errorThrown) {
            //$(".search-form .validateLevel").html('' + errorThrown);
            //$(".search-form .validateLevel").parents('.form-group').addClass('has-error');
        },
        success: function (data) {
            $(".gridview").html(data);
            //$(".search-form .validateLevel").html('');
            //$(".search-form .validateLevel").parents('.form-group').removeClass('has-error');
        },
        complete: function () {
            $(".se-pre-con").hide();
        }
    });
    return false;
}

function ToggleComponent(chb, component) {
    $(component).attr('readonly', !$(chb).prop('checked'));
}

function ToggleComponent2(chb, component) {
    $(component).attr('readonly', $(chb).prop('checked'));
}

function HandleKeyPress(e) {
    var key = e.keyCode || e.which;
    if (key == 13) {
        e.preventDefault();
    }
}

function ChangeCategory(ddl) {
    if ($(ddl).val() != '')
        SelectingCateID = $(ddl).val();
    else
        SelectingCateID = "0";
}

function LoadPrinter(elementDiv) {
    $(".se-pre-con").show();
    $.ajax({
        url: CreateAbsoluteUrl("LoadPrinter"),
        type: "post",
        traditional: true,
        data: { StoreID: StoreID },
        dataType: 'html',
        success: function (data) {
            $(".se-pre-con").hide();
            $(elementDiv).html(data);
            LoadTimeSlot();
        }
    });
}

function LoadCategory(cateComponent) {
    $(".se-pre-con").show();
    $.ajax({
        url: CreateAbsoluteUrl("LoadCategory"),
        type: "post",
        traditional: true,
        data: { StoreID: StoreID, itemType: ItemType, cateID: SelectingCateID },
        dataType: 'html',
        success: function (data) {
            $(".se-pre-con").hide();
            $(cateComponent).html(data);
            LoadSeason();
        }
    });
}

function LoadTimeSlot() {
    $(".se-pre-con").show();
    $(".timeslot").html('');
    $.ajax({
        url: CreateAbsoluteUrl("LoadTimeSlot"),
        type: "post",
        traditional: true,
        data: { StoreID: StoreID },
        dataType: 'html',
        success: function (data) {
            $(".se-pre-con").hide();
            $(".timeslot").html(data);
            //loadServiceCharge
            LoadServiceCharge()
        }
    });
}

function LoadServiceCharge() {
    $('#txtServiceCharge').val(0);
    $(".se-pre-con").show();
    $.ajax({
        url: CreateAbsoluteUrl("LoadServiceCharge"),
        type: "post",
        traditional: true,
        data: { StoreID: StoreID },
        dataType: 'html',
        success: function (data) {
            $(".se-pre-con").hide();
            var obj = $.parseJSON(data)[0];
            if (obj != undefined) {
                var value = obj.Value;
                var IsCurrency = eval(obj.IsCurrency); //False : %
                var IsIncludedOnBill = eval(obj.IsIncludedOnBill);
                $('#txtServiceCharge').val(value);
                if (!IsCurrency) {
                    $('#chbServiceCharge').attr('disabled', false);
                } else if (IsCurrency || IsIncludedOnBill) {
                    $('#chbServiceCharge').attr('disabled', true);
                }
            }

        }
    });
}

function LoadSeason() {
    $(".se-pre-con").show();
    $.ajax({
        url: CreateAbsoluteUrl("LoadSeason"),
        type: "post",
        traditional: true,
        data: { StoreID: StoreID },
        dataType: 'json',
        success: function (lstSeason) {
            $(".se-pre-con").hide();
            $(".ddl-prices").each(function (e) {
                AddOptionForDDLSeason(this, lstSeason);
                SetSelectedSeason(this);
                //======
            });
            LoadPrinter('.printer');
        }
    });
}

function LoadZone(zoneComponent) {
    $(".se-pre-con").show();
    $.ajax({
        url: CreateAbsoluteUrl("LoadZone"),
        type: "post",
        traditional: true,
        data: { StoreID: StoreID },
        dataType: 'html',
        success: function (data) {
            $(".se-pre-con").hide();
            $(zoneComponent).html(data);
        }
    });
}

function LoadParentCategory(elementDiv, ProductTypeID) {
    $('#imgLoading').show();
    $.ajax({
        url: CreateAbsoluteUrl("LoadParentCategory"),
        type: "post",
        traditional: true,
        data: { StoreID: StoreID, ProductTypeID: ProductTypeID },
        dataType: 'html',
        success: function (data) {
            $('#imgLoading').hide();
            $(elementDiv).html(data);
        }
    });
}

//function LoadRole(Component) {
//    $(".se-pre-con").show();
//    var listStoreId = [];
//    listStoreId.push(StoreID);
//    $.ajax({
//        url: CreateAbsoluteUrl("GetRoles"),
//        type: "post",
//        traditional: true,
//        data: { listStoreId: listStoreId },
//        dataType: 'html',
//        success: function (data) {
//            $(".se-pre-con").hide();
//            $(Component).html(data);
//        }
//    });
//}


function AddOptionForDDLSeason(ddl, lstSeason) {
    $(ddl).empty();
    $(ddl).append($('<option/>', {
        value: "",
        text: "-- Select --",
    }));

    for (var i = 0; i < lstSeason.length; i++) {
        $(ddl).append($('<option/>', {
            value: lstSeason[i].ID,
            text: lstSeason[i].Name + " [" + lstSeason[i].StoreName + "]"
        }));
    }
}

function SetSelectedSeason(ddl) {
    var currentSeasonID = $(ddl).parents('.form-group').find('input[name="SelectingSeason"]').val();
    if (currentSeasonID != "")
        $(ddl).find('option[value="' + currentSeasonID + '"]').attr("selected", "selected");
}

function ChangeSeason(ddl) {
    if ($(ddl).val() != "")
        $(ddl).parents('.form-group').find('input[name="SelectingSeason"]').val($(ddl).val());
}

function ResizeModal(element, h) {
    var heightElement = $(element).height() + 100;
    var heightMain = $(window).height();
    if (heightElement > heightMain) {
        $(element).css({ "overflow": "auto", "height": heightMain - h + "px" })
    }
}



function formatDate(date) {
    var hours = date.getHours();
    var minutes = date.getMinutes();
    //var ampm = hours >= 12 ? 'pm' : 'am';
    //hours = hours % 12;
    //hours = hours ? hours : 12; // the hour '0' should be '12'
    hours = hours < 10 ? '0' + hours : hours;
    minutes = minutes < 10 ? '0' + minutes : minutes;
    var strTime = hours + ':' + minutes;// + ' ' + ampm;

    var month = (date.getMonth() + 1) < 10 ? '0' + (date.getMonth() + 1) : (date.getMonth() + 1);
    var day = date.getDate() < 10 ? '0' + date.getDate() : date.getDate();
    var strDate = day + "/" + month + "/" + date.getFullYear();

    //return date.getMonth() + 1 + "/" + date.getDate() + "/" + date.getFullYear() + " " + strTime;
    return strDate + " " + strTime;
}

//*01-08-2017*/
function AddOrSubtract(_type, txtInput, _this) {
    var value = $(txtInput).val();
    if (value == '')
        value = 0;
    if (_type == 'add') { //add
        value++;
        $(_this).parent().parent('.input-group').find('.subtract').removeClass('buttonDisabled');
    } else if (_type == 'sub') {//subtract
        if (value == 0) {
            value = 0;
        } else {
            value--;
            if (value == 1) {
                $(_this).addClass('buttonDisabled');
            }
        }
    }
    $(txtInput).val(value);
}

function SelectOrDeselect(_status, classObjectInput) {
    $(classObjectInput).prop('checked', _status);
}

function CheckPeriodForProduct(listItem, _value, _typePrice) {
    var _IsFlag = false;
    $(listItem).each(function (index, value) {
        var OffSet = $(this).data('value');
        var period = $(this).find('#lblperiod' + _typePrice + '_' + OffSet).text();
        var nameperiodtype = $(this).find('#lblnameperiodtype' + _typePrice + '_' + OffSet).text();
        var itemValue = period + nameperiodtype;
        if (itemValue == _value) {
            _IsFlag = true;
            return true;
        }
    });
    return _IsFlag;
}

//=========
function CheckStatusActiveForProduct(listItem, _typePrice) {
    var _IsFlag = false;
    $(listItem).each(function (index, value) {
        var OffSet = $(this).data('value');
        var status = $(this).find('#lblstatus' + _typePrice + '_' + OffSet).text().replace(' ', '');
        if (status == 'Active') {
            _IsFlag = true;
        }
        return true;
    });
    return _IsFlag;
}
//==================
function validateEmail(email) {
    var re = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
    return re.test(email);
}
//==============

function ReplaceNumber(currency) {
    var number = Number(currency.replace(/[^0-9\.]+/g, ""));
    return number;
}

function SetLocalStorage(key, value) {
    if (typeof (Storage) !== "undefined") {
        // Code for localStorage/sessionStorage.
        localStorage.setItem(key, value); //set
    } else {
        // Sorry! No Web Storage support..
    }
}

function GetLocalStorage(key) {
    if (typeof (Storage) !== "undefined") {
        // Code for localStorage/sessionStorage.
        return localStorage.getItem(key); //get
    } else {
        return "";
        // Sorry! No Web Storage support..
    }
}

function GetNumberProductCart() {
    var countItemCart = GetLocalStorage('countItemCart');
    if (countItemCart == null) {
        countItemCart = 0;
    }
    $('#countItemCart').html(countItemCart);
}

//Number Format Currency
//Number.prototype.format = function (n, x) {
//    var re = '(\\d)(?=(\\d{' + (x || 3) + '})+' + (n > 0 ? '\\.' : '$') + ')';
//    return this.toFixed(Math.max(0, ~~n)).replace(new RegExp(re, 'g'), '$1,');
//};

function NumberFormatCurrency(value, fixed) {
    return parseFloat(value).toFixed(fixed).replace(/(\d)(?=(\d{3})+\.)/g, '$1,');
}

function BackToLoginWhenSessionEnd(data) {
    var isloginForm = false;
    var res = $(data);
    if (res != null && res.length > 11) {
        if (res[11].childNodes != null && res[11].childNodes.length > 0) {
            var text = res[11].childNodes[0].data;
            //alert(text);
            if (text != null && text == "CSC | Sign In") {
                //window.location = "/Login/Index";
                isloginForm = true;
            }
        }
    }
    return isloginForm;
}