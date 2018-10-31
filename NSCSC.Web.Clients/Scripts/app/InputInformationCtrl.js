var cscApp = angular.module('app', ['ngSanitize', 'angular-bind-html-compile', 'ngCookies']);
cscApp.directive('icheck', ['$timeout', '$parse', function ($timeout, $parse) {
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
}]);


cscApp.controller('mainFormmerchantController', function ($http, $scope, $sce, $cookies, $rootScope) {
    var dataCheckout = null;
    $scope.data = null;
    $scope.hasError = false;
    $scope._merchantId = '';
    $scope._customerId = '';
    $scope._dataStore = null;
    $scope.IsShowFeedBack = false;
    $scope.IsErrorCom = false;
    $scope.MsgComplete = "";
    //orderId;
    $scope.orderIDcookies = $cookies.getObject('orderIDcookies');
    $scope.ApplyProductCount = "0 Product(s)";

    $scope.ShowModalAppliedProducts = function () {
        $("#modal-store-info-list-product-create-store").modal({
            backdrop: 'static',
            keyboard: false
        });
    }


    $scope.dataStore = null;
    $scope.ApplyProductCount = "0 Product(s)";
    $scope.UpdatedNumberProductApplied = function () {
        if ($scope.dataStore != null && $scope.dataStore != undefined) {
            var ListProductApply = $scope.dataStore.ListProductApply;
            ListProductApply = jQuery.grep(ListProductApply, function (n, i) {
                return (n.IsApply == true);
            });
            $scope.ApplyProductCount = ListProductApply.length + " Product(s)";
        }
    }

    $scope.SaveCreateOrEditStore = function () {
        var dataStore = $scope.dataStore;
        dataStore.MerchantID = $scope._merchantId;
        dataStore.CusID = $scope._customerId;
       // console.log(dataStore);
        var ListProductApply = $scope.dataStore.ListProductApply;
        $scope.dataStore.ListProductApply = jQuery.grep($scope.dataStore.ListProductApply, function (n, i) {
            return (n.IsApply == true);
        });
        var Country = $("[id=ddlCountryStore]").val();
        $scope.dataStore.Country = Country;
        $('.se-pre-con').show();
        $http({
            method: "POST",
            url: _CreateStoreInfo,
            data: { model: dataStore, OrderId: $scope.orderIDcookies },
        }).then(function successCallback(response) {
            var data = response.data;
            if (data != null && data != undefined) {
            }

        },
            function errorCallback(response) {

            }).finally(function () {
                $('.se-pre-con').hide();
            });
    }
    ////event
    $scope.event = {

        ///PartiviewView Personal Merchant
        getPartiviewViewPersonalMerchant: function () {

            $('.se-pre-con').show();
            $http({
                method: "POST",
                url: _InputMerchantInfoForCusOfReseller,
                data: { OrderId: $scope.orderIDcookies },
            }).then(function successCallback(response) {
                text = $sce.trustAsHtml(response.data);
                $scope.partiview = text;
                $('.se-pre-con').hide();
            },
                function errorCallback(response) {

                }).finally(function () {
                    $('.se-pre-con').hide();
                });
        },

        ///get orderDetail      
        getOrderDetail: function () {
            $('.se-pre-con').show();
            $http({
                method: "POST",
                url: _GetOrderDetailJson,
                data: { OrderId: $scope.orderIDcookies, IsReseller: true},
            }).then(function successCallback(response) {
                var _data = JSON.stringify(response.data);
                _data = JSON.parse(_data);
                $scope.data = _data;
                console.log($scope.data);
                dataCheckout = _data;
                $scope.hasMerchant = _data.hasMerchant;
                $('.se-pre-con').hide();
            },
                function errorCallback(response) {

                }).finally(function () {
                    $('.se-pre-con').hide();
                });

        },


        ///PartiviewView InputStore
        getPartiviewViewInputStore: function () {

            $('.se-pre-con').show();
            $http({
                method: "POST",
                url: _InputStoreForCusOfReseller,
                data: { OrderId: $scope.orderIDcookies },
            }).then(function successCallback(response) {
                text = $sce.trustAsHtml(response.data);
                $scope.partiview = text;
                //$('.se-pre-con').hide();
                
            },
                function errorCallback(response) {

                }).finally(function () {
                    $('.se-pre-con').hide();
                    ///get OrderDetail
                    $scope.event.getOrderDetail();
                });
        },

        createOrEditStore: function (ProductType, CusID, ProductID) {
            $('#datatable-responsive-list-product-create-store').DataTable().destroy();
            $('.se-pre-con').show();
            $http({
                method: "POST",
                url: _CreateOrEditStore,
                data: { ProductType: ProductType, CusID: CusID, ProductID: ProductID, isReseller: true },
            }).then(function successCallback(response) {
                var data = response.data;
                if (data != null && data != undefined) {
                    $scope.ApplyProductCount = data.ApplyProductCount;
                    //console.log(data);
                    $scope.dataStore = data;
                    $("#modal-store-info-list-product").modal({
                        backdrop: 'static',
                        keyboard: false
                    });
                    setTimeout(function () {

                        $('#datatable-responsive-list-product-create-store').DataTable({
                            "columnDefs": [
                                { "orderable": false, "targets": 0 },
                                { "width": "1%", "targets": [0] },
                            ],
                            "order": [
                                [1, "asc"],
                            ],
                            "searching": true,
                            "info": false,
                            "lengthChange": false,
                            "paging": false,
                            "displayLength": 25,
                            "sPaginationType": "numbers",//hide Pre|Next on row footer
                            "ordering": true, //hide Order on row header
                            "destroy": true,
                        });
                    }, 0)
                }

            },
                function errorCallback(response) {

                }).finally(function () {
                    $('.se-pre-con').hide();
                });
        },

        comleted: function () {
            $('.se-pre-con').show();
            $http({
                method: "POST",
                url: _Comleted,
                data: { OrderId: $scope.orderIDcookies},
            }).then(function successCallback(response) {
                console.log(response.data);
                
                if (response.data.Result == true)
                {
                   // text = $sce.trustAsHtml(response.data);
                    $scope.IsErrorCom = false;
                    text = $sce.trustAsHtml(response.data.Html);
                    $scope.view = text;
                    $scope.data.ReceiptNo = response.data.ReceiptNo;
                    deleteOrderPay($scope.orderIDcookies);
                }
                else {
                    $scope.IsErrorCom = true;
                    $scope.MsgComplete = response.data.Msg;
                }
                $('.se-pre-con').hide();

            },
                function errorCallback(response) {

                }).finally(function () {
                    $('.se-pre-con').hide();
                });

            $cookies.putObject('perSonalInformation', $scope.data.CustomerDetail.Name);

        },

        next: function () {
            //var registerCusUrl = "/YourCart/ResellerRegisCustomer";
            //var cusid = $cookies.getObject('cusIDcookies');
            var _data = $scope.data;
            $scope._dataStore = _data;
             //_data.ResellerID = cusid;
            _data.MerchantID = $scope._merchantId;
            _data.CustomerID = $scope._customerId;
            $('.se-pre-con').show();
            $http({
                method: "POST",
                url: _ResellerRegisCustomer,
                data: { data: _data },
            }).then(function successCallback(response) {               
                var _data = JSON.stringify(response.data);
                _data = JSON.parse(_data);
                console.log(_data);
                if (_data.IsSuccess === true) {
                    $scope._merchantId = _data.MerchantID;
                    $scope._customerId = _data.CustomerID;
                   // $cookies.putObject('merchantIdResellerRegisCusmoter', _data.MerchantID);
                    //$cookies.putObject('customerIdResellerRegisCusmoter', _data.CustomerID);
                    $scope.event.getPartiviewViewInputStore();
                }
                if (_data.msg !== null || _data.msg !== '') {
                    $scope.error = _data.msg;
                    $scope.hasError = true;
                }

            },
                function errorCallback(response) {                    
                  

                }).finally(function () {
                    $('.se-pre-con').hide();
                });


        },

        previous: function () {
            if (!$scope.hasMerchant) {
                $scope.event.getPartiviewViewPersonalMerchant();
                $(".wizard_verticle ul.wizard_steps li a#isDone2::before, #step_no2").css("background-color", "#ccc");
                $(".wizard_verticle ul.wizard_steps li a#isDone1::before, #step_no1").removeClass("fa fa-check");
                $(".wizard_verticle ul.wizard_steps li a#isDone2::before, #step_no2").css("top", "auto");
                $("a#isDone2").css("top", "auto");
                $(".wizard_verticle ul.wizard_steps li a#isDone2_wizard_verticle::before, #step_no2").css("background-color", "#ccc");
                $(".wizard_verticle ul.wizard_steps li a#isDone1_wizard_verticle::before, #step_no1").removeClass("fa fa-check");
                $(".wizard_verticle ul.wizard_steps li a#isDone2_wizard_verticle::before, #step_no2").css("top", "auto");
            }
            else {
                window.location = _Cart;
            }
        },

        cancel: function () {
            window.location = _Cart;

        },

        canceled: function () {
            window.location = _Cart;
        },

        finish: function () {
            $('.se-pre-con').show();
            $http({
                method: "POST",
                url: _Submit,
                data: { dataCheckout: dataCheckout },
            }).then(function successCallback(response) {

            },
                function errorCallback(response) {

                }).finally(function () {
                    $('.se-pre-con').hide();
                });
        },

        submit: function () {
            $('.se-pre-con').show();
            var data = {
                CreatedUser: $scope.data.CustomerDetail.ID,
                CustomerID: $scope.data.CustomerDetail.ID,
                CustomerName: $scope.data.CustomerDetail.Name,
                CustomerEmail: $scope.data.CustomerDetail.Email,
                Message: $scope.orderdata.notefeelback
            };
            $http({
                method: "POST",
                url: _Feedback,
                data: { model: data },
            }).then(function successCallback(response) {
                if (response.data.Result)
                    $scope.orderdata.notefeelback = "";
                $scope.IsShowFeedBack = response.data.Result;
            },
                function errorCallback(response) {

                }).finally(function () {
                    $('.se-pre-con').hide();
                });
        }

    };    ////method
    $scope.method = {
        init: function () {
            ///get partivew
            if (!$scope.hasMerchant) {
                $scope.event.getPartiviewViewPersonalMerchant();
            }
            else {
                $scope.event.getPartiviewViewInputStore();
            }
        }
    };

    ////init
    $scope.method.init();
});


