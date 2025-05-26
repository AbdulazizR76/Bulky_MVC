var dataTable;


$(document).ready(function () {
    var url = window.location.search;
    if (url.includes("inproccess")) {
        loadDataTable("inproccess");
    }
    else {
        if (url.includes("completed")) {
            loadDataTable("completed");
        }
        else {
            if (url.includes("pending")) {
                loadDataTable("pending");
            }
            else {
                if (url.includes("approved")) {
                    loadDataTable("approved");
                }
                else {
                    loadDataTable("all");
                }
            }
        }
    }
});

function loadDataTable(status) {
    dataTable = $('#tblData').DataTable({

        "ajax": {
            url: '/admin/order/getall?status='+status,
            type: "GET",
            datatype: "json",
            dataSrc: "data"
        },
     "columns": [
         { data: 'id', title: "Id", "width": "5%" },  
         { data: 'name', title: "Nane", "width": "20%" },  
         { data: 'phoneNumber', title: "Phone Number", "width": "5%" },  
         { data: 'applicationUser.email', title: "Email", "width": "15%" }, 
         { data: 'orderStatus', title: "Status", "width": "15%" },
         { data: 'orderTotal', title: "Total", "width": "10%" },
         

         {
             data: 'id',
             title: "Actions",
             render: function (data) {
                 return `<div class="w-75 btn-group text-start"dir="ltr" role="group" >
                            <a href="/admin/order/details?orderId=${data}" class="btn btn-primary mx-2">
                                <i class="bi bi-pencil-square"></i> 
                            </a>
                         </div>`;
             },
             "width": "20%"
         }
        ],
        "autoWidth": false,  
        "responsive": true   
    });
}




