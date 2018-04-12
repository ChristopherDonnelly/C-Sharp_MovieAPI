// Write your JavaScript code.
$(document).ready(function(){
    $('form').on('submit', function(e){

        $.ajax({
            headers: { 
                'Accept': 'application/json',
                'Content-Type': 'application/json' 
            },
            type: "POST",
            url: "/movies",
            data: JSON.stringify({ "query": $(this).find('[name=query]').val() }),
            dataType: "json",
            success: function(data){
                console.log(data);
                $('tbody').append('<tr><td>'+data.title+'</td><td>'+data.vote_average+'</td><td>'+data.release_date+'</td></tr>');
            }
        });
        e.preventDefault();
    });

    getAllMovies();
});

function getAllMovies(){
    $.ajax({
        headers: { 
            'Accept': 'application/json',
            'Content-Type': 'application/json' 
        },
        type: "GET",
        url: "/movies",
        dataType: "json",
        success: function(data){
            console.log(data);
            for(let i=0; i<data.length; i++){
                $('tbody').append('<tr><td>'+data[i].title+'</td><td>'+data[i].vote_average+'</td><td>'+data[i].release_date+'</td></tr>');
            }
        }
    });
}