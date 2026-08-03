function valida_navegador_compatible() {
    if (window.File && window.FileReader && window.FileList && window.Blob) {
        // Great success! All the File APIs are supported.
    } else {
        alert('The File APIs are not fully supported in this browser.');
    }
}
function inicializa_file_api(drap_zone) {
    var dropZone = document.getElementById(drap_zone);
    dropZone.addEventListener('dragover', handleDragOver, false);
    dropZone.addEventListener('drop', handleFileSelect, false);
}

function handleFileSelect(evt) {
    evt.stopPropagation();
    evt.preventDefault();
    var data = new FormData();
    var files = evt.dataTransfer.files; // FileList object.
   
    // Make Ajax request with the contentType = false, and procesDate = false
    $.ajax({
        type: 'POST',
        url: '../webservice/WebServiceRadicacion.asmx/desarcaga_archivo_respuesta',
        data:  data  ,
        contentType: 'application/json; utf-8',
        dataType: 'json',
        success: function (data) {
            if (data.d != null) {
                //$("#Hidden_resultado_web_service").val(data.d);
                alert(data.d);
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            //$("#Hidden_resultado_web_service").val(jqXHR.responseText);
            //alert($("#Hidden_resultado_web_service").val());
            alert("ojo");
        }

    });

    ajaxRequest.done(function (xhr, textStatus) {
        // Do other operation
    });

    // files is a FileList of File objects. List some properties.
    var output = [];
    for (var i = 0, f; f = files[i]; i++) {
        output.push('<li><strong>', escape(f.name), '</strong> (', f.type || 'n/a', ') - ',
                    f.size, ' bytes, last modified: ',
                    f.lastModifiedDate ? f.lastModifiedDate.toLocaleDateString() : 'n/a',
                    '</li>');
    }
    document.getElementById('list').innerHTML = '<ul>' + output.join('') + '</ul>';
}

function handleDragOver(evt) {
    evt.stopPropagation();
    evt.preventDefault();
    evt.dataTransfer.dropEffect = 'copy'; // Explicitly show this is a copy.
}

function descar_manual() {
    var data = new FormData();

    var files = $("#fileUpload").get(0).files;

    // Add the uploaded image content to the form data collection
    if (files.length > 0) {
        data.append("UploadedImage", files[0]);
    }
    var ajaxRequest = $.ajax({
        type: "POST",
        url: "../webservice/WebServiceRadicacion.asmx/desarcaga_archivo_respuesta",
        contentType: false,
        processData: false,
        data: data
    });

    ajaxRequest.done(function (xhr, textStatus) {
        // Do other operation
    });
}