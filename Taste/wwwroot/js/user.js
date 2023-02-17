var dataTable;
$(document).ready(function () {
    loadList();
});

function loadList() {
    dataTable = $('#DT_lo').DataTable({
        "ajax": {
            "url": "/api/user",
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { "data": "fullName", "width": "25%" },
            { "data": "email", "width": "25%" },
            { "data": "phoneNumber", "width": "25%" },
            {
                "data": { id: "id", lockoutEnd:"lockoutEnd" },
                "render": function (data) {
                    var today = new Date().getTime();
                    var lockout = new Date(data.lockoutEnd).getTime();
                    if (lockout > today) {
                        return`<div class="text-center">
                                <a onClick=lockunlock('${data.id}') class="btn btn-danger text-white" style="cursor:pointer;width:100px;">
                                 <i class="fa-solid fa-lock-open"></i> Unlock</a>
                         </div>`;
                    }
                    else {
                        return`<div class="text-center">
                                <a onClick=lockunlock('${data.id}') class="btn btn-success text-white" style="cursor:pointer;width:100px;">
                                 <i class="fa-solid fa-lock"></i> lock</a>
                         </div>`;
                    }
                },
                "width": "30%",
            }
        ],
        "Language": {
            "emptyTable":"no data found."
        },
        "width": "100%"
    });
}
function lockunlock(id) {
    $.ajax({
        type: 'POST',
        url: '/api/user',
        data: JSON.stringify(id),
        contentType:"application/json",
        success: function (data) {
            if (data.success) {
                //success notification
                toastr.success(data.message);
                dataTable.ajax.reload();
            }
            else {
                //failsure notification
                toastr.error(data.message);
            }
        }
    });
}