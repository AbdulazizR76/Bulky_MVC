$(document).ready(function () {
    loadDataTable();
});

//document.addEventListener("DOMContentLoaded", function () {
//    loadDataTable();
//})


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
        <a href="/admin/product/delete?id=${data}" class="btn btn-danger mx-2">
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
