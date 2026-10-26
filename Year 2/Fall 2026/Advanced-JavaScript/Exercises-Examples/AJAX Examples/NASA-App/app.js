'use strict'

const getDateString = date => 
    `${date.getFullYear()}-${date.getMonth() + 1}-${date.getDate()}`;

const displayPicture = data => {
    let html = "";
    if (data.error) { 
        html += `<span class="error">${data.error.message}</span`;
    }
    else if (data.code) {
        html += `<span class="error">${data.message}</span`;
    }
    else {
        html += `<h3>${data.title}</h3>`;
        const width = 700;
        switch (data.media_type) {
            case "image":
                html += `<img src="${data.url}" width="${width}" alt="NASA Photo">`;
            break;

            case "video":
                html += `<iframe src="${data.url}" frameborder=0 allowfullscreen></iframe>`
            break;

            default:
                html += `<img src="images/notavailable.png" width="${width}" alt="NASA-Photo>`;
        }

        // Date and Copy Rigth
        html += `<div>${data.date}`;
        if (data.copyright) {
            html += `<span class="right">&copy; ${data.copyright}</span>`;
        }
        html += `</div>`;
        html += `<p>${data.explanation}</p>`; 
    }

    //Display Error
    $("#display").html(html);
};

const displayError = error => {
    let html = `<span class="error">${error.message}</span>`;
    $("#display").html(html);
}

$(document).ready( () => {
    // On Load, get todays date in YYYY-MM-DD format
    const today = new Date();
    let dateString = getDateString(today);
    const dateTxtBox = $("#date");
    dateTxtBox.val(dateString);
    dateTxtBox.focus();

    $("#view_button").click( () => {
        dateString = $("#date").val();
        const dateObj = new Date(dateString);

        if (dateObj == "Invalid Date") {
            const msg = "Please Enter valid date in YYYY-MM-DD format.";
            $("#display").html(`<span class="error">${msg}</span>`);
        }
        else {
            dateString = getDateString(dateObj);

            const domain = `https://api.nasa.gov/planetary/apod`;
            const request = `?api_key=DEMO_KEY&date=${dateString}`;
            const url = domain + request;

            // Example of AJAX
            fetch(url)
                .then(response => response.json())
                .then(json => displayPicture(json))
                .catch(error => displayError(error));
        }
        $("#date").focus();
    });

    // Set Todays date in the textbox
});