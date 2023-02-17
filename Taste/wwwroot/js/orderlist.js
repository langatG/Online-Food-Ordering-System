var dataTable;
$(document).ready(function () {
    var url = window.location.search;
    if (url.includes("cancelled")) {
        loadList("cancelled");
    } else {
        if (url.includes("completed")) {
            loadList("completed");
        } else {
            loadList("");
        }
    }
});

function loadList(param) {
    dataTable = $('#DT_lo').DataTable({
        "ajax": {
            "url": "/api/order?status=" + param,
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { "data": "orderHeader.id", "width": "20%" },
            { "data": "orderHeader.pickupName", "width": "20%" },
            { "data": "orderHeader.applicationUser.email", "width": "20%" },
            { "data": "orderHeader.orderTotal", "width": "20%" },
            {
                "data": "orderHeader.id",
                "render": function (data) {
                    return `<div class="text-center">
                                <a href="/Admin/Order/OrderDetails?id=${data}" class="btn btn-success text-white">
                                 <i class="fa-solid fa-pen-to-square"></i></i> Details</a>
                         </div>`;
                },
                "width": "20%",
            }
        ],
        "Language": {
            "emptyTable": "no data found."
        },
        "width": "100%"
    });
}