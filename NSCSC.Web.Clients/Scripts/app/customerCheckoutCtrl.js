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

cscApp.controller('customerCheckoutCtrl', function ($http, $scope, $sce, $rootScope, $cookies) {
    var dataCheckout = null;
    var paymentId = null;
    $scope.data = null;
    //orderId;
    $scope.orderIDcookies = $cookies.getObject('orderIDcookies');

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
    $scope.choosePayment = function (id) {
        paymentId = id;
        $(".imgpayment").removeClass("choosepayment");
        $("#img_" + id).addClass("choosepayment");
        $(".checkpayment").removeClass("fa fa-check");
        $("#span_" + id).addClass("fa fa-check");
    };
    $scope.SaveCreateOrEditStore = function () {
        var dataStore = $scope.dataStore;
        dataStore.MerchantID = $scope.data.MerchantID;
        dataStore.CusID = $scope.data.CustomerID;
        console.log(dataStore);
        var ListProductApply = $scope.dataStore.ListProductApply;
        $scope.dataStore.ListProductApply = jQuery.grep($scope.dataStore.ListProductApply, function (n, i) {
            return (n.IsApply == true);
        });
        var Country = $("[id=ddlCountryStore]").val();
        $scope.dataStore.Country = Country;
        $('.se-pre-con').show();
        $http({
            method: "POST",
            url: _CustomerCreateStoreTemp,
            data: { model: $scope.dataStore, OrderId: $scope.orderIDcookies },
        }).then(function successCallback(response) {
            var data = response.data;
            if (data.Success == true) {
                $("#modal-store-info-list-product").modal('hide');
            } else {

            }

        },
            function errorCallback(response) {

            }).finally(function () {
                $('.se-pre-con').hide();
            });
    }
    $scope.changeTimeZone = function () {
        var _CountryName = $('#ddlCountryStore').val();
        var _ListContry = $scope.dataStore.Countries;
        for (var i = 0; i < _ListContry.length; i++) {
            var _countryTemp = _ListContry[i];
            if(_countryTemp.Name == _CountryName)
                $scope.dataStore.ListTimeZone = _countryTemp.TimeZones;
        }

    }
    ////event
    $scope.event = {

        ///PartiviewView Personal Merchant
        getPartiviewViewPersonalMerchant: function () {


            $('.se-pre-con').show();
            $http({
                method: "POST",
                url: _CustomerInputStoreInf,
                data: { OrderId: $scope.orderIDcookies },
            }).then(function successCallback(response) {
                text = $sce.trustAsHtml(response.data);
                $scope.partiview = text;
                $('.se-pre-con').hide();
                ///get OrderDetail
                $scope.event.getOrderDetail();
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
                data: { OrderId: $scope.orderIDcookies, IsReseller: false },
            }).then(function successCallback(response) {
                var _data = JSON.stringify(response.data);
                _data = JSON.parse(_data);
                $scope.data = _data;
                console.log($scope.data);
                dataCheckout = _data;
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
                url: _CustomerMakePayment,
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

        createOrEditStore: function (ProductType, CusID, ProductID) {
            $('#datatable-responsive-list-product-create-store').DataTable().destroy();
            $('.se-pre-con').show();
            $http({
                method: "POST",
                url: _CreateOrEditStore,
                data: { ProductType: ProductType, CusID: CusID, ProductID: ProductID, isReseller: false},
            }).then(function successCallback(response) {
                var data = response.data;
                if (data != null && data != undefined) {
                    $scope.ApplyProductCount = data.ApplyProductCount;
                    console.log(data);
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

        //comleted: function () {
        //    $('.se-pre-con').show();
        //    $http({
        //        method: "POST",
        //        url: "/YourCart/Comleted",
        //    }).then(function successCallback(response) {
        //        text = $sce.trustAsHtml(response.data);
        //        $scope.view = text;
        //        $(".wizard_horizontal ul.wizard_steps li a#isDone1::before, #step_no1").addClass("fa fa-check");
        //        $(".wizard_horizontal ul.wizard_steps li a#isDone1::before, #step_no1").css("background-color", "#28acf7");
        //        $(".wizard_horizontal ul.wizard_steps li a#isDone2::before, #step_no2").addClass("fa fa-check");
        //        $(".wizard_horizontal ul.wizard_steps li a#isDone2::before, #step_no2").css("background-color", "#28acf7");
        //        $('.se-pre-con').hide();

        //    },
        //        function errorCallback(response) {

        //        }).finally(function () {
        //            $('.se-pre-con').hide();
        //        });

        //    $cookies.putObject('perSonalInformation', $scope.data.CustomerDetail.Name);

        //},

        next: function () {
            $scope.event.getPartiviewViewInputStore();
        },

        previous: function () {
            $scope.event.getPartiviewViewPersonalMerchant();
        },

        cancel: function () {
            window.location = _OrderPayCart;
        },

        canceled: function () {
            $scope.event.getPartiviewViewPersonalMerchant();
        },

        //finish: function () {
        //    $('.se-pre-con').show();
        //    $http({
        //        method: "POST",
        //        url: "/YourCart/Submit",
        //        data: { dataCheckout: dataCheckout },
        //    }).then(function successCallback(response) {

        //    },
        //        function errorCallback(response) {

        //        }).finally(function () {
        //            $('.se-pre-con').hide();
        //        });
        //},
        makepayment: function () {
            if (paymentId == null || paymentId == "") {
                alert("please choose payment");
                return false;
            }
            $('.se-pre-con').show();
            $http({
                method: "POST",
                url: _CustomerDonePayment,
                data: {
                    dataCheckout: dataCheckout,
                    paymentId: paymentId
                },
            }).then(function successCallback(response) {
                var _data = JSON.stringify(response.data);
                _data = JSON.parse(_data);
                if (_data.Success == true) {
                    //return my profile
                    window.location = _MyProfile;
                }
                else {

                }
                $('.se-pre-con').hide();
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
            $scope.event.getPartiviewViewPersonalMerchant();
            ///get OrderDetail
            $scope.event.getOrderDetail();
        }
    };

    ////init
    $scope.method.init();
});
