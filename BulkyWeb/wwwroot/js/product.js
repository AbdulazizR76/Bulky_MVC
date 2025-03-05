//$(document).ready(function () {
//    loadDataTable();
//});

document.addEventListener("DOMContentLoaded", function () {
    loadDataTable();
})


function loadDataTable() {
    dataTable = $('#tblData').DataTable({

     "ajax": { url:'/admin/product/getall'},
     "columns": [
         { data: 'title',"width":"15%" },
         { data: 'position', "width": "15%" },
         { data: 'salary', "width": "15%" },
         { data: 'office', "width": "15%" }
    ]
    });
}
