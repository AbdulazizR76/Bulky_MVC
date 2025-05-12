var dataTable;


$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({

        "ajax": {
            url: '/admin/user/getall',
            type: "GET",
            datatype: "json",
            dataSrc: "data"
        },
     "columns": [
         { data: 'name', title: "name", "width": "15%" },  // ✅ Matches API response
         { data: 'email', title: "email", "width": "15%" },  // ✅ Matches API response
         { data: 'phoneNumber', title: "PhoneNumber", "width": "15%" },  // ✅ Matches API response
         { data: 'company.name', title: "company", "width": "15%" },
         { data: 'role', title: "role", "width": "15%" },


         {
             data: 'id',
             title: "Actions",
             render: function (data) {
                 return `<div class="w-75 btn-group" role="group">
                            <a href="/admin/company/upsert?id=${data}" class="btn btn-primary mx-2">
                                <i class="bi bi-pencil-square"></i> Edit
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



