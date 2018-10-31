$.fn.extend({
    ChatSocket: function (opciones) {
        var ChatSocket = this;
        var idChat = $(ChatSocket).attr('id');
        defaults = {
            ws,

            Room: "RoomNewstead",    // important  - room or user

            pass: "1234",                       // important - pass of room or user

            lblTitulChat: "Chat [Admin CSC] ",    //Chat Name

            lblCampoEntrada: "Type a message...",

            lblEnviar: "Send",

            textoAyuda: "Develoteca",           // Help button

            FullName: "James Cobs",               // default Name.

            urlImg: "http://placehold.it/50/55C1E7/fff&text=Name",    // Avatar chat

            btnEntrar: "btnEntrar",

            btnEnviar: "btnEnviar",

            lblBtnEnviar: "Send",

            lblTxtEntrar: "txtEntrar",

            lblTxtEnviar: "txtMensaje",

            lblBtnEntrar: "Enter",       // Joined button

            idDialogo: "DialogoEntrada",

            classChat: "",              // add class chat

            idOnline: "ListaOnline",    // id control users joined

            lblUsuariosOnline: "Users joined",   // text users online 

            lblEntradaNombre: "Name:",

            panelColor: "success",

            userLoginUrlImg: "http://placehold.it/50/55C1E7/fff&text=Name",    // Avatar chat

            userName: "James Cobs",
        }

        var opciones = $.extend({}, defaults, opciones);

        var ws;
        var Room = opciones.Room;
        var pass = opciones.pass;
        var lblTitulChat = opciones.lblTitulChat;
        var lblCampoEntrada = opciones.lblCampoEntrada;
        var lblEnviar = opciones.lblBtnEnviar;
        var textoAyuda = opciones.textoAyuda;
        var FullName = opciones.FullName;

        var userName = opciones.userName;
        var userLoginUrlImg = opciones.userLoginUrlImg;
        var urlImg = opciones.urlImg;
        var btnEntrar = opciones.btnEntrar;
        var btnEnviar = opciones.btnEnviar;
        var lblBtnEnviar = opciones.lblBtnEnviar;
        var lblTxtEntrar = opciones.lblTxtEntrar;
        var lblTxtEnviar = opciones.lblTxtEnviar;
        var lblBtnEntrar = opciones.lblBtnEntrar;
        var idDialogo = opciones.idDialogo;
        var classChat = opciones.classChat;
        var idOnline = opciones.idOnline;
        var lblUsuariosOnline = opciones.lblUsuariosOnline;
        var lblEntradaNombre = opciones.lblEntradaNombre;
        var panelColor = opciones.panelColor;
        if ($('#' + idOnline).length == 0) {
            idOnline = idChat + "listaOnline";
            $('#' + idChat).append('<br/><div id="' + idOnline + '"></div>');
        }

        function StartConnection() {
            //conex = '{"setID":"' + Room + '","passwd":"' + pass + '"}';
            conex = {
                setID: Room,
                passwd: pass
            }
            // Create WebSocket connection.
            ws = new WebSocket("ws://achex.ca:4010");
            ws.onopen = function () {
                console.log("Opening a connection...[" + Room + "]");
                $('#ltrNetworkingSuccess').html('Opening a connection...ROOM[' + Room + ']');
                //ws.send(conex);
                ws.send(JSON.stringify(conex));
            }
            ws.onmessage = function (Messages) {
                var data = Messages.data;
                var obj = jQuery.parseJSON(data);
                alert(data);
                getItemMsg(obj);
                if (obj.sID != null) {
                    if ($('#' + obj.sID).length == 0) {

                        $('#listaOnline').append('<li class="list-group-item" id="' + obj.sID
                                            + '"><span class="label label-success">&#9679;</span> <span class="titleNickName">' + obj.FullName + '</span></li>');
                    }
                }
            }
            // Show a disconnected message when the WebSocket is closed.
            ws.onclose = function () {
                console.log("I'm sorry. Bye! => [" + FullName + "]");
            }
            ws.onerror = function (evt) {
                console.log("ERR: " + evt.data);
            };
        }

        StartConnection();

        function StartChat() {
            //FullName = $('#' + lblTxtEntrar).val();
            //if (FullName == '') {
            //    alert('Please enter a user name in the field below for the chat room');
            //    $('#' + lblTxtEntrar).focus();
            //    return false;
            //}

            $('#' + idDialogo).hide();
            $('#' + idOnline).show();

            CreatChat();
            UserOnline();
            getOnline();
        }

        function CreatEntrada() {
            $('#' + idChat).append('<div id="' + idDialogo + '" class="' + classChat + '" id="InputNombre"><div class="panel-footer" style="margin-top:100px;"><div class="input-group"><input id="' + lblTxtEntrar + '" type="text" class="form-control input-sm" placeholder="' + lblEntradaNombre + '"><span class="input-group-btn"><button id="' + btnEntrar + '" class="btn btn-success btn-sm" >' + lblBtnEntrar + '</button></span></div></div></div>');

            $('#' + idOnline).append(' <div class="panel panel-' + panelColor + '"><div class="panel-heading"><span class="glyphicon glyphicon-user"></span> ' + lblUsuariosOnline + '</div><div class="panel-body"><ul class="list-group" id="listaOnline"></ul></div><div class="panel-footer"><div class="input-group"><div><a href="#">Admin CSC</a></div></span></div></div></div>');

            $("#" + lblTxtEntrar).keyup(function (e) {
                if (e.keyCode == 13) {
                    StartChat();
                }
            });
            $("#" + btnEntrar).click(function () {
                StartChat();
            });
        }

        function CreatChat() {
            //$('#' + idChat).append('<div class="' + classChat + '"><div class="panel panel-' + panelColor + '"><div class="panel-heading"><span class="glyphicon glyphicon-comment"></span>' + lblTitulChat + " : [" + FullName + ']<div class="btn-group pull-right"> <button type="button" onclick="alert(\'' + textoAyuda + '\')" class="btn btn-default btn-xs dropdown-toggle" data-toggle="dropdown"><span class="glyphicon glyphicon-chevron-down"></span></button> </div> </div> <div class="panel-body"><ul class="chatpluginchat"></ul></div><div class="panel-footer"><div class="input-group"><input id="' + lblTxtEnviar + '" type="text" class="form-control input-sm" placeholder="' + lblCampoEntrada + '" /><span class="input-group-btn"><button  class="btn btn-success btn-sm" id="' + btnEnviar + '"><i class="fa fa-paper-plane"></i>&nbsp;&nbsp;' + lblEnviar + '</button></span></div></div></div></div><li class="left clearfix itemtemplate" style="display:none"><span class="chat-img pull-left"><img src="' + urlImg + '" alt="User Avatar" class="img-circle" id="Foto" width="50" height="50"/></span><div class="chat-body clearfix"><div class="header"><strong class="primary-font" id="FullName">FullName</strong><small class="pull-right text-muted"><span class="fa fa-calendar" style="font-size:11px;"></span>&nbsp;&nbsp;<span id="Tiempo">12 mins ago</span></small></div> <p id="Content">Content</p></div></li>');


            $('#' + idChat).append('<div class="' + classChat + '"><div class="panel panel-' + panelColor + '"><!--heading--><div class="panel-heading" style="padding:0px 10px;"> <div class="mail_heading row botPadding" style="background-color: #dff0d8; border-color: #d6e9c6; color: #3c763d; margin-bottom:0px;"><div class="col-md-8"><a href="javascript:void(0);" class="user-profile"><img src="' + urlImg + '" alt="">&nbsp;&nbsp;<span style="color:#0475B9">' + FullName + '</span> <span style="font-style:italic;">(California, USA)</span>&nbsp;&nbsp;<small><i class="fa fa-circle" style="color:#1DC251;"></i></small></a></div><div class="col-md-4 text-right"><div class="btn-group listIconUserChat"><button class="btn btn-sm btn-default" type="button" data-placement="top" data-toggle="tooltip" data-original-title="Forward" onclick="VideoCall();"><i class="fa fa-video-camera"></i></button> <button class="btn btn-sm btn-default" type="button" data-placement="top" data-toggle="tooltip" data-original-title="Print" onclick="AudioCall();"><i class="fa fa-phone"></i></button> <button class="btn btn-sm btn-default" type="button" data-placement="top" data-toggle="tooltip" data-original-title="Trash" onclick="AddContact();"><i class="fa fa-plus-circle"></i></button> <button class="btn btn-sm btn-default" type="button" data-placement="top" data-toggle="tooltip" data-original-title="Trash" onclick="ViewProfile();"><i class="fa fa-user"></i></button> <button class="btn btn-sm btn-default" type="button" data-placement="top" data-toggle="tooltip" data-original-title="Trash" onclick="MovetoTrash();"><i class="fa fa-trash"></i></button> </div></div> </div> </div> <!--body--><div class="panel-body"><ul class="chatpluginchat"></ul></div> <!--footer--><div class="panel-footer"><div class="input-group"><input id="' + lblTxtEnviar + '" type="text" class="form-control input-sm" placeholder="' + lblCampoEntrada + '" /><span class="input-group-btn"><button  class="btn btn-success btn-sm" id="' + btnEnviar + '"><i class="fa fa-paper-plane"></i>&nbsp;&nbsp;' + lblEnviar + '</button></span></div></div></div></div><li class="left clearfix itemtemplate" style="display:none"><span class="chat-img pull-left"><img src="' + urlImg + '" alt="User Avatar" class="img-circle" id="Foto" width="50" height="50"/></span><div class="chat-body clearfix"><div class="header"><strong class="primary-font" id="FullName">FullName</strong><small class="pull-right text-muted"><span class="fa fa-calendar" style="font-size:11px;"></span>&nbsp;&nbsp;<span id="Tiempo">12 mins ago</span></small></div> <p id="Content">Content</p></div></li>');

            $("#" + lblTxtEnviar).keyup(function (e) { if (e.keyCode == 13) { SendMessage(); } });
            $("#" + btnEnviar).click(function () { SendMessage(); });
        }

        // Send text to all users through the server
        function SendMessage() {

            var _msg = $('#' + lblTxtEnviar).val();
            if (_msg == '') {
                return false;
            }
            // Send the msg object as a JSON-formatted string.
            var msg = {
                to: Room,
                userName: userName,
                Content: _msg,
                Image: userLoginUrlImg,
                date: Date.now()
            };
            //ws.send('{"to":"' + Room + '","userName":"' + userName + '","Content":"' + _msg + '","Image":"' + userLoginUrlImg + '"}');
            ws.send(JSON.stringify(msg));

            $("#" + lblTxtEnviar).val('');
        };

        function UserOnline() {
            //var _user = {
            //    to: Room,
            //    FullName: FullName
            //}
            //ws.send(JSON.stringify(_user));
            //ws.send('{"to":"' + Room + '","FullName":"' + FullName + '"}');
        }

        function getItemMsg(Obj) {

            if ((Obj.Content != null) && (Obj.userName != null)) {

                $(".itemtemplate").clone().appendTo(".chatpluginchat");
                $('.chatpluginchat .itemtemplate').show(10);
                $('.chatpluginchat .itemtemplate #Foto').attr("src", Obj.Image);
                $('.chatpluginchat .itemtemplate #FullName').html(Obj.userName);
                $('.chatpluginchat .itemtemplate #Content').html(Obj.Content);

                var formattedDate = new Date();
                var d = formattedDate.getUTCDate();
                var m = formattedDate.getMonth() + 1;
                var y = formattedDate.getFullYear();
                var h = formattedDate.getHours();
                var min = formattedDate.getMinutes();

                Fecha = d + "/" + m + "/" + y + " " + h + ":" + min;

                $('.chatpluginchat .itemtemplate #Tiempo').html(Fecha);
                $('.chatpluginchat .itemtemplate').removeClass("itemtemplate");

                $('#Elchat .panel-body').animate({ scrollTop: $('#Elchat .panel-body').prop("scrollHeight") }, 250);
            }
        }

        function getOnline() {
            setInterval(UserOnline, 3000);
        }

        CreatEntrada();
        StartChat();
        // Fun
    }
});