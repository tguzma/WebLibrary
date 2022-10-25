$(".banBtn").click(function () {
    let btn = $(this);
    $.ajax({
        url: "/User/Ban",
        type: "POST",
        data: { userId: btn.attr("id") },
        success: function (isBanned) {
            if (isBanned) {
                btn.addClass("btn-success");
                btn.removeClass("btn-danger");
            } else {
                btn.addClass("btn-danger");
                btn.removeClass("btn-success");
            }
            btn.closest("tr").find(".isBanned").text(String(isBanned).charAt(0).toUpperCase() + String(isBanned).slice(1));
        }
    });
});

$(".approveBtn").click(function () {
    let btn = $(this);
    $.ajax({
        url: "/User/Approve",
        type: "POST",
        data: { userId: btn.attr("id") },
        success: function () {
            btn.remove();
        }
    });
});
