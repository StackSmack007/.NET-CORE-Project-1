var connection =
    new signalR.HubConnectionBuilder()
        .withUrl("/chat")
        .build();

window.onload = (function () {
    console.log("starting the connection......");
    connection.start().then(function () {
        connection.invoke("NewCommer");
    })
        .catch(function (err) {
            return console.error(err.toString())
        });
});

connection.on("PopChatTab", function (userName) {
    $.ajax({
        type: 'POST',
        url: '/Assistance/Home/MakeChatTab',
        data: { userName },
        success: function (data) {
            $('#chatBoardA').append(data);
        }
    });
});

$("#userSend").on("click", function () {
    var message = $('#userMessageContent').val();
    if (message != '') {
        connection.invoke("UserMessaging", message);
    }
    $('#userMessageContent').val('');
});

connection.on("ReceiveMessageU", function (comment) {
    $.ajax({
        type: 'POST',
        url: '/Home/MakeChatMessage',
        contentType: "application/json;charset=utf-8",
        data: JSON.stringify(comment),
        dataType: "Html",
        success: function (data) {
            $('#chatBoardU').append(data);
        }
    });
});

connection.on("ReceiveMessageA", function (comment) {
    var target = (comment.targetName == null) ? comment.userInfo.userName : comment.targetName;
    var chatBoxName = `#${target}-commentTab`;
    console.log(chatBoxName);
    $.ajax({
        type: 'POST',
        url: '/Assistance/Home/MakeChatMessage',
        contentType: "application/json;charset=utf-8",
        data: JSON.stringify(comment),
        dataType: "Html",
        success: function (data) {
            $(chatBoxName).append(data);
        }
    });
});

$("#chatBoardA").on("click","button",(function () {
    console.log(this.id);
    if (this.id.endsWith('-sendBTN')) {
        var userName = this.id.replace('-sendBTN','');
        var textBoxId = '#' + this.id.replace('-sendBTN', '-responseTXT');
        var message = $(textBoxId).val();
        connection.invoke("StaffMessagingUser", message, userName);
        $(textBoxId).val('');
    }
}));

connection.on("AddUserNameToAdminPanel", function (userName) {
    $('#usersOnlineNames').append(`<li>${userName}</li>`)
});

connection.on("AddStaffNameToAdminPanel", function (staffName) {
    $('#staffOnlineNames').append(`<li>${staffName}</li>`)
});

$('#admSelectAllUsers').change(function () {

    if (this.checked) {
        $('#usersOnlineNames').css("font-weight", "bold");
    }
    else {
        $('#usersOnlineNames').css("font-weight", "normal");
    }
});

$('#admSelectAllStaff').change(function () {

    if (this.checked) {
        $('#staffOnlineNames').css("font-weight", "bold");
    }
    else {
        $('#staffOnlineNames').css("font-weight", "normal");
    }
});
