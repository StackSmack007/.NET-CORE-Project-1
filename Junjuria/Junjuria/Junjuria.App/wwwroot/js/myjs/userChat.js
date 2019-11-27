var connection =
    new signalR.HubConnectionBuilder()
        .withUrl("/chat")
        .build();

window.onload = (function () {
    connection.start().then(function () {
        connection.invoke("NewCommer");
    })
        .catch(function (err) {
            return console.error(err.message)
        });;
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

$("#chatBoardA").on("click", "button", (function () {
    if (this.id.endsWith('-sendBTN')) {
        var userName = this.id.replace('-sendBTN', '');
        var textBoxId = '#' + this.id.replace('-sendBTN', '-responseTXT');
        var message = $(textBoxId).val();
        connection.invoke("StaffMessagingUser", message, userName);
        $(textBoxId).val('');
    }
}));

connection.on("AddUserNameToStaffPanel", function (userName) {
    $('#usersOnlineNames').append(`<li>${userName}</li>`)
});

connection.on("AddStaffNameToStaffPanel", function (staffName) {
    $('#staffOnlineNames').append(`<li>${staffName}</li>`)
});

$('#admPanel').on("change", "input", (function () {
    var idName = this.id;
    var targetContainerName = idName == "admSelectAllUsers" ? "#usersOnlineNames" : "#staffOnlineNames";
    if (this.checked) {
        $(targetContainerName).css("font-weight", "bold");
    }
    else {
        $(targetContainerName).css("font-weight", "normal");
    }
}));

$('#massSendBtn').click(function () {
    var message = $('#massMessageTXT').val();
    var sentToUsers = $('#admSelectAllUsers').prop("checked");
    var sentToStaff = $('#admSelectAllStaff').prop("checked");
    if (message != '' && (sentToStaff || sentToUsers)) {
        connection.invoke("Broadcast", message, sentToUsers, sentToStaff);
        $('#massMessageTXT').val('');
    }
});

connection.on("AdminInstructStaff", function (comment) {
    var chatBoxName = '.miniMessageBoxAssistance';
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

window.addEventListener("beforeunload",function () {
    connection.invoke("MemberExit");
});

connection.on("unPopUserChatInStaffWindows", function (name) {
    let element = document.getElementById(`${name}-commentTab`);
    if (typeof element === "undefined") {
        console.log("cant remove member from non existing tab")
        return;
    }

    element.parentNode.parentNode.removeChild(element.parentNode);
    let userLi = [...document.querySelectorAll("#usersOnlineNames > li")].find(x => x.innerText ===name);
    if (typeof userLi === "undefined") {
        console.log("no user in mass mail panel!")
        return;
    }

    userLi.parentNode.removeChild(userLi)
})