
//prefixes of implementation that we want to test
var cookieName = 'csc-order';

function createOrUpdateOrder_v2(cusId, orderId, Qty) {
    if ((cusId != null && cusId.length > 0) || (orderId != null && orderId.length > 0) || (Qty != null && Qty.length > 0)) {
        var order = { CusId: cusId, OrderId: orderId, Qty: Qty }
        Cookies.remove(cookieName);
        Cookies.set(cookieName, order);
        console.log('createOrUpdateOrder CusId: ' + cusId + ' | OrderId: ' + orderId + ' | Qty: ' + Qty);
        console.log(Cookies.getJSON(cookieName));
    }
}

/// edit
var cookieName_v2 = "csc-order-v2";
function createOrUpdateOrder(cusId, orderId, Qty) {
    //debugger;
    if ((cusId != null && cusId.length > 0 && orderId !== "") || (orderId != null && orderId.length > 0 && orderId !== "") || (Qty != null && Qty.length > 0 && orderId !== "")) {
        var order = { CusId: cusId, OrderId: orderId, Qty: Qty }
        var Orders = [];
        if (!Cookies(cookieName_v2))
        {
            Orders.push(order);
            Cookies.set(cookieName_v2, Orders);
        } else {
            Orders = Cookies.getJSON(cookieName_v2);
            if (Orders.map(function (o) { return o.CusId }).indexOf(cusId) == -1)
            {
                Orders.push(order);
            } else {
                $.each(Orders, function (index, item) {
                    if (item.OrderId === orderId /*&& item.CusId !== ""*/) {
                        item.Qty = Qty;
                        item.CusId = cusId;
                    } else if (item.OrderId !== "" && item.CusId === "")
                    {
                        // Update List Order 
                        Orders.splice(index, 1);
                    }
                });
            }
            Cookies.remove(cookieName_v2);
            Cookies.set(cookieName_v2, Orders);
        }
       // console.log("Test" + Cookies.getJSON(cookieName_v2));
    }
}
//how to use
// var _data = getOrder();
//if(_data != undefined){ var cusId = _data.CusId;}
function getOrder_v2(CusId) {
    
    var result = Cookies.getJSON(cookieName);
    console.log('getOrder ' + result);

    if (result != undefined && CusId !== "" && result.CusId === CusId) {
        
        $('#countItemCart').html(result.Qty);
    }
    return result;
}

//Edit 
function getOrder(CusId) {
    var result = Cookies.getJSON(cookieName_v2);
    console.log(result);
    //debugger;
    var order = null;
    if (result != undefined)
    {
        for (var j = 0; j < result.length; j++)
        {
            if (result[j].CusId === CusId && CusId !== "")
            {
                //alert('getOrder')
                var _Qty = result[j].Qty;
                for (var i = 0; i < result.length; i++) {
                    if (result[i].OrderId !== "" && result[i].CusId === "") {
                        _Qty = parseInt(_Qty) + parseInt(result[i].Qty);
                        OrderMerge(result[i].OrderId, result[j].OrderId);
                        result.splice(i, 1);// remove order item in cookie
                        // update service
                    }
                }
              //  alert("s" + result[j].Qty)
                result[j].Qty = _Qty;
                $('.countItemCart').html(result[j].Qty);
                order = result[j];
                break;
            } else if (result[j].OrderId !== "" && result[j].CusId === "" && CusId !== "" && CusId != undefined) {
                result[j].CusId = CusId;
                $('.countItemCart').html(result[j].Qty);
                order = result[j];
              //  alert("b"+result[j].Qty)
            } else if (result[j].OrderId !== "" && result[j].CusId === "")
            {
                $('.countItemCart').html(result[j].Qty);
                order = result[j];
              //  alert("a"+result[j].Qty)
            }
        }
        Cookies.remove(cookieName_v2);
        Cookies.set(cookieName_v2, result);
    }
    console.log("result  : "+result);
   // deleteOrderPay("29a80f79-bcf2-41c7-a887-be752f1551b5");
    return order;
}

function deleteOrder_v2() {
   // Cookies.remove(cookieName);
    console.log('deleteOrder');
    $('#countItemCart').html(0);
}

function deleteOrderPay_v2() {
     Cookies.remove(cookieName);
    console.log('deleteOrder');
    $('#countItemCart').html(0);
}

//Edit
function deleteOrder() {
   // Cookies.remove(cookieName);
    console.log('deleteOrder');
    $('#countItemCart').html(0);
}

function deleteOrderPay(OrderID) {
   // alert(OrderID);
    var result = Cookies.getJSON(cookieName_v2);
    var flgRemoveCookie = false;
    for (var i = 0; i < result.length ; i++)
    {
        if (result[i].OrderId === OrderID)
        {
            result.splice(i, 1);
            flgRemoveCookie = true;
        }
    }
    if (flgRemoveCookie)
    {
        Cookies.remove(cookieName_v2);
        Cookies.set(cookieName_v2, result);
    } else {
        Cookies.remove(cookieName_v2);
    }
     console.log('deleteOrder');
    $('#countItemCart').html(0);
}
