var dataTable;


$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({

        "ajax": {
            url: '/admin/product/getall',
            type: "GET",
            datatype: "json",
            dataSrc: "data"
        },
     "columns": [
         { data: 'title', title: "Title", "width": "15%" },  // ✅ Matches API response
         { data: 'isbn', title: "ISBN", "width": "15%" },  // ✅ Matches API response
         { data: 'author', title: "Author", "width": "15%" },  // ✅ Matches API response
         { data: 'price', title: "Price", "width": "15%" },  // ✅ Matches API response
         {
             data: 'category',
             title: "Category",
             render: function (data) {
                 return data ? data.name : 'No Category';
             },
             "width": "15%"
         },

         {
             data: 'id',
             title: "Actions",
             render: function (data) {
                 return `<div class="w-75 btn-group" role="group">
                            <a href="/admin/product/upsert?id=${data}" class="btn btn-primary mx-2">
                                <i class="bi bi-pencil-square"></i> Edit
                            </a>

                            <a onClick=Delete("/admin/product/Delete/${data}") class="btn btn-danger mx-2">
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
