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
             data: { id:"id", lockoutEnd: "lockoutEnd" },
             title: "Actions",
             render: function (data) {
                 var today = new Date().getTime();
                 var lockout = new Date(data.lockoutEnd).getTime();

                 if (lockout > today) {
                     return `
                    <div class="text-center d-flex justify-content-center gap-2">
                     <a onclick=LockUnlock('${data.id}') class="btn btn-danger text-white" style="cursor:pointer; width:100px;">
                                <i class="bi bi-lock-fill"></i> Lock
                            </a>
                  
                            <a href="/admin/user/RoleManagment?userId=${data.id}" class="btn btn-danger text-white" style="cursor:pointer; width:150px;">
                                <i class="bi bi-pencil-square"></i> Permission
                            </a>
                    </div>
                         `
                 } else {
                     return `
                    <div class="text-center d-flex justify-content-center gap-2">
                             <a onclick=LockUnlock('${data.id}') class="btn btn-success text-white" style="cursor:pointer; width:100px;">
                                <i class="bi bi-unlock-fill"></i> Unlock
                            </a>
                            <a href="/admin/user/RoleManagment?userId=${data.id}" class="btn btn-danger text-white" style="cursor:pointer; width:150px;">
                                <i class="bi bi-pencil-square"></i> Permission
                            </a>
                    </div>
                         `
                 }

             },
             "width": "15%"
         }
        ],
        "autoWidth": false,  // ✅ Prevents width calculation errors
        "responsive": true   // ✅ Ensures table renders properly
    });
}



function LockUnlock(id) {
    $.ajax({
        type: "POST",
        url: '/Admin/User/LockUnlock',
        data: JSON.stringify(id),
        contentType: "application/json",
        success: function (data) {
            if (data.success) {
                toastr.success(data.message);
                dataTable.ajax.reload();
            }
        }
    })
}

