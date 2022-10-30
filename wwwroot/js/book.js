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

function changeCountText() {
    canBeBorrowed.text(`You can borrow ${count} more books.`);
}