var dataTable;
$(document).ready(function () {
    loadList();
});

function loadList() {
    dataTable = $('#DT_lo').DataTable({
        "ajax": {
            "url": "/api/category",
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { "data": "name", "width": "40%" },
            { "data": "displayOrder", "width": "30%" },
            {
                "data": "id",
                "render": function (data) {
                    return `<div class="text-center">
                                <a href="/Admin/Category/Upsert?id=${data}" class="btn btn-success text-white">
                                 <i class="fa-solid fa-pen-to-square"></i></i> Edit</a>
                                <a onClick=Delete('/api/category/'+${data}) class="btn btn-danger text-white" style="cursor:pointer;width:100px;">
                                 <i class="fa-solid fa-trash-can"></i> Delete</a>
                         </div>`;
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
function Delete(url) {
    swal({
        title: "Are you sure?",
        text: "You won't be able to revert this!",
        icon: "warning",
        buttons: true,
        dangerMode: true
    }).then((willDelete) => {
        if (willDelete) {
            $.ajax({
                type: 'DELETE',
                url: url,
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
    });

}