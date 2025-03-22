var dataTable;


$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({

        "ajax": {
            url: '/admin/company/getall',
            type: "GET",
            datatype: "json",
            dataSrc: "data"
        },
     "columns": [
         { data: 'name', title: "name", "width": "15%" },  // ✅ Matches API response
         { data: 'streetAddress', title: "streetAddress", "width": "15%" },  // ✅ Matches API response
         { data: 'city', title: "city", "width": "15%" },  // ✅ Matches API response
         { data: 'state', title: "state", "width": "15%" },
         { data: 'phoneNumber', title: "phoneNumber", "width": "15%" },


         {
             data: 'id',
             title: "Actions",
             render: function (data) {
                 return `<div class="w-75 btn-group" role="group">
                            <a href="/admin/company/upsert?id=${data}" class="btn btn-primary mx-2">
                                <i class="bi bi-pencil-square"></i> Edit
                            </a>

                            <a onClick=Delete("/admin/company/Delete/${data}") class="btn btn-danger mx-2">
                                <i class="bi bi-trash-fill"></i> delete
                            </a>

                         </div>
`;
             },
             "width": "15%"
         }
        ],
        "autoWidth": false,  // ✅ Prevents width calculation errors
        "responsive": true   // ✅ Ensures table renders properly
    });
}



function Delete(url) {
    Swal.fire({
        title: "Are you sure?",
        text: "You won't be able to revert this!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Yes, delete it!"
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: "DELETE",
                url: url,
                success: function (data) {
                    if (data.success) {
                        toastr.success(data.message);
                        dataTable.ajax.reload();
                    } else {
                        toastr.error(data.message);
                    }
                },
                error: function () {
                    toastr.error("An error occurred while deleting.");
                }
            });
        }
    });
}
