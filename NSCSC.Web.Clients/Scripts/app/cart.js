var app = angular.module("app");

app.directive('icheck', ['$timeout', '$parse', function ($timeout, $parse) {
    return {
        restrict: 'A',
        require: '?ngModel',
        link: function (scope, element, attr, ngModel) {
            $timeout(function () {
                var value = attr.value;

                function update(checked) {
                    if (attr.type === 'radio') {
                        ngModel.$setViewValue(value);
                    } else {
                        ngModel.$setViewValue(checked);
                    }
                }

                $(element).iCheck({
                    checkboxClass: attr.checkboxClass || 'icheckbox_flat-green',
                    radioClass: attr.radioClass || 'iradio_flat-green'
                }).on('ifChanged', function (e) {
                    scope.$apply(function () {
                        update(e.target.checked);
                    });
                });

                scope.$watch(attr.ngChecked, function (checked) {
                    if (typeof checked === 'undefined') checked = !!ngModel.$viewValue;
                    update(checked)
                }, true);

                scope.$watch(attr.ngModel, function (model) {
                    $(element).iCheck('update');
                }, true);

            })
        }
    }
}])

app.directive("select2", function ($timeout, $parse) {
    return {
        restrict: 'AC',
        require: 'ngModel',
        link: function (scope, element, attrs) {
            //console.log(attrs);
            $timeout(function () {
                element.select2();
                element.select2Initialized = true;
            });

            var refreshSelect = function () {
                if (!element.select2Initialized) return;
                $timeout(function () {
                    element.trigger('change');
                });
            };

            var recreateSelect = function () {
                if (!element.select2Initialized) return;
                $timeout(function () {
                    element.select2('destroy');
                    element.select2();
                });
            };

            scope.$watch(attrs.ngModel, refreshSelect);

            if (attrs.ngOptions) {
                var list = attrs.ngOptions.match(/ in ([^ ]*)/)[1];
                // watch for option list change
                scope.$watch(list, recreateSelect);
            }

            if (attrs.ngDisabled) {
                scope.$watch(attrs.ngDisabled, refreshSelect);
            }
        }
    };
});


app.controller("CartCtrl", ["$scope", "$http", "$cookies", function ($scope, $http, $cookies) {
    $scope.OrderID = null;
    /* init list function */
    $scope.CurrencySymbol = "";
    $scope.ListFunction = [];
    $scope.fnInfoAdvanced = function (obj) {
        $('[id=infoModal]').modal({ backdrop: 'static', keyboard: false });
        $scope.ListFunction = obj.ListFunction;
        // console.log($scope.ListFunction);
    }
    $scope.ListProductInterested = _Products;

    /* parse json */
    $scope.ParseJson = function () {
        $('.se-pre-con').show();
        $http({
            method: "POST",
            url: "/YourCart/ParseJson",
            data: { item: _Orders },
        }).then(function successCallback(response) {
            var data = response.data;
            //console.log(data);
            if (data !== null && data !== undefined) {
                /* List Orders */
                if (data.ListItems !== null && data.ListItems !== undefined && data.ListItems.length > 0) {
                    $scope.ListOrders = data.ListItems;
                    /* Order info */
                    $scope.OrderInfo = {
                        SubTotal: data.SubTotal,
                        Tax: data.Tax,
                        Total: data.Total,
                        TotalDiscount: data.TotalDiscount,
                        DiscountType: data.DiscountType,
                        TotalPromotion: data.TotalPromotion,
                        sTaxName: data.sTaxName,
                        ID: data.ID,
                        CustomerID: data.CustomerID,
                        HasMerchant: data.HasMerchant
                    }
                    $scope.CurrencySymbol = data.CurrencySymbol;
                    createOrUpdateOrder(data.CustomerID, data.ID, data.TotalQuantity);
                    getOrder(data.CustomerID);
                } else {
                    $scope.OrderInfo = {
                        SubTotal: data.SubTotal,
                        Tax: data.Tax,
                        Total: data.Total,
                        TotalDiscount: data.TotalDiscount,
                        DiscountType: data.DiscountType,
                        TotalPromotion: data.TotalPromotion,
                        sTaxName: data.sTaxName,
                        ID: data.ID,
                        CustomerID: data.CustomerID,
                        HasMerchant: data.HasMerchant
                    }
                    deleteOrderPay(data.ID);
                }
            }
        },
            function errorCallback(response) {

            }).finally(function () {
                $('.se-pre-con').hide();
            });
    }
    $scope.ParseJson();
    /* event button add to cart */
    $scope.AddToCart = function (obj, ID) {
        $('.se-pre-con').show();
        $http({
            method: "POST",
            url: "/YourCart/AddToCart",
            data: { item: obj, OrderId: ID },
        }).then(function successCallback(response) {
            var data = response.data;
            if (data !== null && data !== undefined) {
                /* List Orders */
                if (data.ListItems !== null && data.ListItems !== undefined && data.ListItems.length > 0) {
                    $scope.ListOrders = data.ListItems;
                    /* Order info */
                    $scope.OrderInfo = {
                        SubTotal: data.SubTotal,
                        Tax: data.Tax,
                        Total: data.Total,
                        TotalDiscount: data.TotalDiscount,
                        DiscountType: data.DiscountType,
                        TotalPromotion: data.TotalPromotion,
                        sTaxName: data.sTaxName,
                        ID: data.ID,
                        CustomerID: data.CustomerID
                    }
                    createOrUpdateOrder(data.CustomerID, data.ID, data.TotalQuantity);
                    getOrder(data.CustomerID);
                }
            }
        },
            function errorCallback(response) {

            }).finally(function () {
                $('.se-pre-con').hide();
            });
    }

    /* event button remove item */
    $scope.RemoveItemOrder = function (obj, ID) {
        $('.se-pre-con').show();
        $http({
            method: "POST",
            url: "/YourCart/RemoveItemOrder",
            data: { item: obj, OrderId: ID }
        }).then(function successCallback(response) {
            var data = response.data;
            if (data !== null && data !== undefined) {
                /* List Orders */
                if (data.ListItems !== null && data.ListItems !== undefined) {
                    $scope.ListOrders = data.ListItems;
                    /* Order info */
                    $scope.OrderInfo = {
                        SubTotal: data.SubTotal,
                        Tax: data.Tax,
                        Total: data.Total,
                        TotalDiscount: data.TotalDiscount,
                        DiscountType: data.DiscountType,
                        TotalPromotion: data.TotalPromotion,
                        sTaxName: data.sTaxName,
                        ID: data.ID,
                        CustomerID: data.CustomerID
                    }
                    createOrUpdateOrder(data.CustomerID, data.ID, data.TotalQuantity);
                    getOrder(data.CustomerID);
                }
            }
        },
            function errorCallback(response) {

            }).finally(function () {
                $('.se-pre-con').hide();
            });
    }

    /* event button add Or Edit Addition */
    $scope.ListAdditionType = [];
    $scope.ListAdditionTypeCommon = [];
    $scope.AddOrEditAddition = function (CusID, ProductID, OrderID) {
        $('.se-pre-con').show();
        $http({
            method: "POST",
            url: "/YourCart/GetListBuyAddition",
            data: { CusID: CusID, ProductID: ProductID, OrderID: OrderID }
        }).then(function successCallback(response) {
            if (response.data !== null && response.data !== undefined) {
                $scope.ListAdditionType = response.data.result;
                $scope.ListAdditionTypeCommon = response.data.result;
                $scope.Additions = response.data.Additions;
                /* show modal */
                $('[id=BuyAdditionModal]').modal({ backdrop: 'static', keyboard: false });
            }
        }, function errorCallback(response) {

        }).finally(function () {
            $('.se-pre-con').hide();
        });
    }

    /* event button add to cart addition */
    $scope.AddToCartAddition = function () {
        $('.se-pre-con').show();
        var obj = $scope.ListAdditionType;
        $http({
            method: "POST",
            url: "/YourCart/AddToCartOfAddition",
            data: { items: obj },
        }).then(function successCallback(response) {
            var data = response.data;
            if (data !== null && data !== undefined) {
                //console.log(data);
                /* List Orders */
                if (data.ListItems !== null && data.ListItems !== undefined && data.ListItems.length > 0) {
                    $scope.ListOrders = data.ListItems;
                    /* Order info */
                    $scope.OrderInfo = {
                        SubTotal: data.SubTotal,
                        Tax: data.Tax,
                        Total: data.Total,
                        TotalDiscount: data.TotalDiscount,
                        DiscountType: data.DiscountType,
                        TotalPromotion: data.TotalPromotion,
                        sTaxName: data.sTaxName,
                        ID: data.ID,
                        CustomerID: data.CustomerID,
                        HasMerchant: data.HasMerchant
                    }
                    createOrUpdateOrder(data.CustomerID, data.ID, data.TotalQuantity);
                    getOrder(data.CustomerID);
                }
                $("[id=BuyAdditionModal]").modal('toggle');
            }
        },
            function errorCallback(response) {

            }).finally(function () {
                $('.se-pre-con').hide();
            });
    }

    /* event button checkout  for reseller*/
    $scope.Done = function (AmountPay, OrderID, CusID) {
        $('.se-pre-con').show();
        $http({
            method: "POST",
            url: "/YourCart/OrderPayCart",
            data: { OrderID: OrderID },
        }).then(function successCallback(response) {
            var data = response.data;
            if (data !== null && data !== undefined) {
                var OrderId = data.ID;
                if (OrderId != "") {
                    deleteOrderPay(OrderId);
                }
                var Orders = data.result;
                /* List Orders */
                if (Orders.ListItems !== null && Orders.ListItems !== undefined) {
                    $scope.ListOrders = Orders.ListItems;
                    /* Order info */
                    $scope.OrderInfo = {
                        SubTotal: Orders.SubTotal,
                        Tax: Orders.Tax,
                        Total: Orders.Total,
                        TotalDiscount: Orders.TotalDiscount,
                        DiscountType: Orders.DiscountType,
                        TotalPromotion: Orders.TotalPromotion,
                        sTaxName: Orders.sTaxName,
                        ID: Orders.ID,
                        CustomerID: Orders.CustomerID,
                        HasMerchant: data.HasMerchant
                    }
                }
                $scope.OrderID = OrderID;
                var orderIDcookies = $scope.OrderID;
                $cookies.putObject('orderIDcookies', orderIDcookies);
                $cookies.putObject('cusIDcookies', CusID);
            }
           
            window.location = _InputInformationForCusOfReseller;

        },
            function errorCallback(response) {

            }).finally(function () {
                $('.se-pre-con').hide();
            });
    }

    $scope.YourCartDoneForReseller = function (OrderID, CusID, HasMerchant) {
        $cookies.putObject('orderIDcookies', OrderID);
        $cookies.putObject('cusIDcookies', CusID);
        $cookies.putObject('hasmerchantCookies', HasMerchant);
        window.location = _InputInformationForCusOfReseller;
    }

    $scope.CheckOut = function (AmountPay, OrderID, CusID) {
        $cookies.putObject('orderIDcookies', OrderID);
        $cookies.putObject('cusIDcookies', CusID);
        window.location = _CustomerCheckOut;
    }

    /* event button update period */
    $scope.UpdatePeriodItem = function (item,OrderId) {
        $('.se-pre-con').show();
        $http({
            method: "POST",
            url: "/YourCart/UpdatePeriod",
            data: { item: item, OrderId: OrderId }
        }).then(function successCallback(response) {
            var data = response.data;
            if (data !== null && data !== undefined) {
                /* List Orders */
                if (data.ListItems !== null && data.ListItems !== undefined && data.ListItems.length > 0) {
                    $scope.ListOrders = data.ListItems;
                    /* Order info */
                    $scope.OrderInfo = {
                        SubTotal: data.SubTotal,
                        Tax: data.Tax,
                        Total: data.Total,
                        TotalDiscount: data.TotalDiscount,
                        DiscountType: data.DiscountType,
                        TotalPromotion: data.TotalPromotion,
                        sTaxName: data.sTaxName,
                        ID: data.ID,
                        CustomerID: data.CustomerID,
                        HasMerchant: data.HasMerchant
                    }
                }
            }
        },
            function errorCallback(response) {

            }).finally(function () {
                $('.se-pre-con').hide();
            });
    }

    /* event button apply coupon */
    $scope.CouponCode = function (OrderId,CusID)
    {
        $('.se-pre-con').show();
        var DiscountCode = $scope.DiscountCode;
        $http({
            method: "POST",
            url: "/YourCart/ApplyCouponCode",
            data: { OrderId: OrderId, DiscountCode: DiscountCode, CusID: CusID }
        }).then(function successCallback(response) {
            var data = response.data;
            if (data !== null && data !== undefined) {
                if (data.msg != undefined)
                {
                    $scope.Message = data.msg;
                    $scope.IsShow = true;
                }
                else {
                    $scope.Message = "";
                    $scope.IsShow = false;
                    /* List Orders */
                    if (data.ListItems !== null && data.ListItems !== undefined && data.ListItems.length > 0) {
                        $scope.ListOrders = data.ListItems;
                        /* Order info */
                        $scope.OrderInfo = {
                            SubTotal: data.SubTotal,
                            Tax: data.Tax,
                            Total: data.Total,
                            TotalDiscount: data.TotalDiscount,
                            DiscountType: data.DiscountType,
                            TotalPromotion: data.TotalPromotion,
                            sTaxName: data.sTaxName,
                            ID: data.ID,
                            CustomerID: data.CustomerID,
                            HasMerchant: data.HasMerchant
                        }
                    }
                }
            }
        },
            function errorCallback(response) {

            }).finally(function () {
                $('.se-pre-con').hide();
            });
    }
}]);
/* envent filter additon */
function FilterAddition() {
    var scope = angular.element($("#filterAddition")).scope();
    scope.$apply(function () {
        var _addition = $("#filterAddition").val();
        if (_addition == 0) {
            scope.ListAdditionType = scope.ListAdditionTypeCommon;
        }
        else {
            scope.ListAdditionType = $.grep(scope.ListAdditionTypeCommon, function (item) {
                return item.AdditionType == $("#filterAddition").val()
            });
        }
        setTimeout(function () {
            $(".Period").select2({
                placeholder: "Please choose period",
            });
        })
    });
}