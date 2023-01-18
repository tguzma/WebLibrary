$(document).on('click', '.exportDb', function () {
    $.ajax({
        url: "/Export",
        type: "GET",
        success: 
            alert("Database was exported into: D:\GitHub\WebLibrary\ExportDb.zip folder")
    });
});

$(document).on('click', '.importDb', function () {
    $.ajax({
        url: "/Import",
        type: "GET",
        success:
            alert("Database was imported successfuly")
    });
});