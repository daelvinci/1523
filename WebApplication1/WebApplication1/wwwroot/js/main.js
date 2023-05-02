$(document).ready(function () {
    $(document).on("click", '.open-book-modal', function (e) {

        e.preventDefault();
        var url = $(this).attr("href");

        fetch(url)
            .then(response => response.text())
            .then(modalHtml => {
                $("#quickModal .modal-dialog").html(modalHtml)
            });


        $("#quickModal").modal("show")
    })

    $(document).on("click", '.add-to-basket', function(e){
        e.preventDefault();
        var basketUrl = $(this).attr("href");

        fetch(basketUrl)
            .then(response => response.text())
            .then(html=> 
               $("#basket-cart").html(html))

    })

    $(document).on("click", '.delete-from-basket', function (e) {
        e.preventDefault();
        var url =`https://localhost:7105/book/deletefrombasket/${$(this).attr("data")}`;

        fetch(url)
            .then(response => response.text())
            .then(html =>
                $("#basket-cart").html(html))

    })


    $(document).on("click", ".remove-img-box", function (e) {
        console.log($(this).parent().remove())
    })

    $(document).on("click", ".delete-btn", function (e) {
        e.preventDefault();

        let url = $(this).attr("href");

        Swal.fire({
            title: 'Are you sure?',
            text: "You won't be able to revert this!",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Yes, delete it!'
        }).then((result) => {
            if (result.isConfirmed) {
                fetch(url).then(response => {
                    if (response.ok) {
                        Swal.fire(
                            'Deleted!',
                            'Your file has been deleted.',
                            'success'
                        ).then(() => window.location.reload())
                    }
                    else {
                        Swal.fire({
                            icon: 'error',
                            title: 'Oops...',
                            text: 'Something went wrong!',
                            footer: '<a href="">Why do I have this issue?</a>'
                        })
                    }
                })
            }
        })
    })
})


   

