let canBeBorrowed = $('.canBeBorrowed');
let count = canBeBorrowed.data("count");

$(document).ready(function () {
    changeCountText();
});

$(document).on('click', '.borrowBtn', function () {
    let btn = $(this);
    let card = btn.parent();
    $.ajax({
        url: "/Borrow",
        type: "POST",
        data: { bookId: card.attr("id") },
        success: function (printsLeft) {

                btn.addClass('returnBtn').removeClass('borrowBtn')
                btn.text('Return');
                card.find(".printsLeft").text(`Prints left: ${printsLeft}`);
                count--;
                changeCountText();

        }
    });
});

$(document).on('click', '.returnBtn', function () {
    let btn = $(this);
    let card = btn.parent();
    $.ajax({
        url: "/Return",
        type: "POST",
        data: { bookId: card.attr("id") },
        success: function (printsLeft) {
            btn.addClass('borrowBtn').removeClass('returnBtn')
            btn.text('Borrow');
            card.find(".printsLeft").text(`Prints left: ${printsLeft}`);
            count++;
            changeCountText();
        }
    });
});

$(document).on('click', '.borrowOrReturnBtn', function () {
    let bookId = $(this).parent().attr("id");
    let username = $(".userNameInput").val();

    $.ajax({
        url: "/BorrowOrReturn",
        type: "POST",
        data: {
            bookId: bookId,
            username: username
        },
        success: function (response) {
            if (response.error) {
                alert("Book cannot be borrowed");
                return;
            }
            $(".amt-avalib").next().text(response.amountAvalible);
            if (response.wasBorrowed) {
                alert("The book was returned.");
            } else {
                alert("The book was borrowed.");
            }
        }
    });
});

function changeCountText() {
    canBeBorrowed.text(`You can borrow ${count} more books.`);
}