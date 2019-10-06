var token = document.getElementsByName("__RequestVerificationToken")[0].value;
console.log(token);
$(document).ready(function () {
    var button = $("#FavourizeButton")
    $("#FavourizeButton").change(function () {

        console.log(choise);
        $.ajax({
            type: 'POST',
            url: '/Products/Favourize',
            data: { dto, __RequestVerificationToken: token },
            success: function (data) {
                alert(data);
            }
        });
    });
});