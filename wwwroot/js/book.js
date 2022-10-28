$(".deleteBook").click(function () {
    let btn = $(this);
    $.ajax({
        url: "Delete",
        type: "POST",
        data: { id: btn.attr("id") }
    });
});
