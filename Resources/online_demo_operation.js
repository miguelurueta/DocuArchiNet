/// <reference path="../Resources/dynamsoft.webtwain.intellisense.js" />
/// <reference path="../Resources/addon/dynamsoft.webtwain.addon.barcode.intellisense.js" />
/// <reference path="../Resources/addon/dynamsoft.webtwain.addon.webcam.intellisense.js" />
/// <reference path="../Resources/addon/dynamsoft.webtwain.addon.pdf.intellisense.js" />

//--------------------------------------------------------------------------------------
//************************** Import Image*****************************
//--------------------------------------------------------------------------------------
/*-----------------select source---------------------*/
var estado_replace = 0;
var vufer_index = 0;
var numer_pagina_actual = 0;
var numero_pagina_digitaliza = 0;
var tipo_visor = 1;
var response_sevice_java_selecion = ""; //Gaurda el resultado del setvicio web
var ESTADO_SEVICE_UPLOAD = "";
var interval_sevice_uopload;
function get_solicita_documento_seleccionado(id_imagen_) {
    $(document).ready(function () {
        $.ajax({
            async: false,
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: "../webservice/WebServiceDocuarchi.asmx/Get_solicita_documento_seleccionado",
            data: "{'id_imagen':'" + id_imagen_ + "'}",
            dataType: "json",
            success: function (data) {
                if (data.d) {
                    response_sevice_java_selecion = data.d;
                    ESTADO_SEVICE_UPLOAD = "yes";
                } else {
                    response_sevice_java_selecion = data.d;
                    ESTADO_SEVICE_UPLOAD = "yes";
                }
            },
            error: function (result) {
                ESTADO_SEVICE_UPLOAD = "yes";
                response_sevice_java_selcion = result;
                alert("Error......" + result);
                clearInterval(interval_sevice_uopload);
                //event.preventDefault();
            }, compelete: function () {
                ESTADO_SEVICE_UPLOAD="yes";
            }
        });

    })
    
}

function http_upload_server(event,e) {
    try {
       
        if (DWObject.HowManyImagesInBuffer > 0) {
            var x;
            var r = confirm("Se detectaron (" + DWObject.HowManyImagesInBuffer + ") pagina(s), las desea remplazar por la imagen seleccionada en el sistema ? ");
            if (r == false) {
                return true;
            }
        }
        DWObject.RemoveAllImages();
        document.getElementById('Paginador').innerHTML = (0) + " de " + DWObject.HowManyImagesInBuffer;
        strHTTPServer = location.hostname;
        DWObject.IfSSL = Dynamsoft.Lib.detect.ssl;
        var _strPort = location.port == "" ? 80 : location.port;
        if (Dynamsoft.Lib.detect.ssl == true)
            _strPort = location.port == "" ? 443 : location.port;
        DWObject.HTTPPort = _strPort;
        ESTADO_SEVICE_UPLOAD = "";
        response_sevice_java_selecion = "";
        posicion_update_pogres('progres_bar');
        interval_sevice_uopload = setInterval(frame_upload, 400);
        function frame_upload() {
            if (ESTADO_SEVICE_UPLOAD == "") {
                get_solicita_documento_seleccionado("1");              
            } 
            if (response_sevice_java_selecion !== "") {              
                upload_complete();
            }
        }
        Sevalue_tumb_a(-1);
            
    } catch (ex) {
        alert("http_upload_server" + ex.message)
    } finally {
        progres_hiden('progres_bar');
    }

}
function upload_complete() {
    clearInterval(interval_sevice_uopload);
    if (response_sevice_java_selecion == "") {
        alert("Seleccione el archivo a cargar ");
        return;
    }
    var split = [];
    split = response_sevice_java_selecion.split("|");
    if (split[0] !== "YES") {
        alert(split[1]);
        return;
    }
    var rut = split[2];
    var file_ = split[1];
    function optionalAsyncSuccessFunc() {
        document.getElementById('Paginador').innerHTML = (DWObject.CurrentImageIndexInBuffer + 1) + " de " + DWObject.HowManyImagesInBuffer;
        Sevalue_tumb_a(-1);
    }

    function optionalAsyncFailureFunc(errorCode, errorString) {
        alert("ErrorCode: " + errorCode + " ErrorString: " +
        errorString + " url : " + rut + file_);
    }

    DWObject.HTTPDownload(
                 rut,
                 file_,
                 optionalAsyncSuccessFunc,
                 optionalAsyncFailureFunc
             );

}
function source_onchange(bWebcam) {
    if(bWebcam)
    {
         if (document.getElementById("divWebcamType"))
            document.getElementById("divWebcamType").style.display = "";
            
        DWObject.Addon.Webcam.SelectSource(document.getElementById("webcamsource").options[document.getElementById("webcamsource").selectedIndex].text);
       
       var countMediaType = -1;
       var countResolution = -1;
        var MediaType = document.getElementById("MediaType");
        if (MediaType) {
            MediaType.options.length = 0;
            var aryMediaType = DWObject.Addon.Webcam.GetMediaType();
            countMediaType = aryMediaType.GetCount();
            var i;
            var value;
            for (i = 0; i < countMediaType; i++) {
                value = aryMediaType.Get(i);
                MediaType.options.add(new Option(value, value));
            }
        }

        var ResolutionWebcam = document.getElementById("ResolutionWebcam");
        if (ResolutionWebcam) {
            ResolutionWebcam.options.length = 0;
            var aryResolution = DWObject.Addon.Webcam.GetResolution();
            countResolution = aryResolution.GetCount();
            for (i = 0; i < countResolution; i++) {
                value = aryResolution.Get(i);
                ResolutionWebcam.options.add(new Option(value, value));
            }
        }
        
        if(Dynamsoft.Lib.env.bWin)
        {
            if(countMediaType <=0 || countResolution <= 0)
            {
                appendMessage('<b>Webcam source is currently occupied by other program.</b>');
            }
        }
        
        DWObject.Addon.Webcam.CloseSource();
    }
    else
    {
        if (document.getElementById("divTwainType"))
            document.getElementById("divTwainType").style.display = "";

        if (document.getElementById("source"))
            DWObject.SelectSourceByIndex(document.getElementById("source").selectedIndex);
    }
}

function mediaType_onchange() {
   var MediaType = document.getElementById("MediaType");
    if(MediaType && MediaType.options.length > 0)
    {
        valueMediaType = MediaType.options[MediaType.selectedIndex].text;
        if(valueMediaType != "")
            if(!DWObject.Addon.Webcam.SetMediaType(valueMediaType))
            {
                appendMessage('<b>Error setting MediaType value: </b>');
		        appendMessage("<span style='color:#cE5E04'><b>" + DWObject.ErrorString + "</b></span><br />");
                return;
            }
    }

    var ResolutionWebcam = document.getElementById("ResolutionWebcam");
    if (ResolutionWebcam) {
        ResolutionWebcam.options.length = 0;
        var aryResolution = DWObject.Addon.Webcam.GetResolution();
        countResolution = aryResolution.GetCount();
        for (i = 0; i < countResolution; i++) {
            value = aryResolution.Get(i);
            ResolutionWebcam.options.add(new Option(value, value));
        }
    }
}

///*******************Guarda el archivo en servidor por medio de
function Gurdar_documento_htpp_server(extencion_file) { 
    var doc;
    var exte;
    var extension_documento = "";
    var i;
    var tempo_item = document.getElementsByName("TypeFormato");
    for (iz = 0; iz < tempo_item.length; iz++) {
        if (tempo_item[iz].checked == true)
            extension_documento = tempo_item[iz].value;
    }
    if (extencion_file !== "") {
        extension_documento = extencion_file;
    }
    if (extension_documento == "") {
        alert("Imposible encontrar la extensión " + extension_documento)
        return false;
    }
    if (DWObject) {
        if (DWObject.HowManyImagesInBuffer !== 0) {
            var protocol = window.location.protocol;
            var ssl_state = false;
            if (protocol == "https:") {
                ssl_state = true;
            }
            for (var i = 0; i < DWObject.HowManyImagesInBuffer; i++) {
                if (i == 0) {
                    var setval = window.parent.document.getElementById("HiddenIdFlujo").value;
                    var spl = setval.split("|");
                    var numer = spl[0];
                    doc = Ceros_Doc(numer.toString());
                    DWObject.IfShowFileDialog = false;
                    if (extension_documento == "TIF") {
                        docu_save_html("Webform_save_digital_image.aspx", ssl_state, doc + i + ".TIF", 2, i);
                        return;
                    }
                    if (extension_documento == "JPG") {
                        //DWObject.SaveAsJPEG(rut + "/" + doc + ".JPG", i)
                        docu_save_html_Individual("Webform_save_digital_image.aspx", ssl_state, doc + ".JPG", 1, i);
                        //return;
                    }
                    if (extension_documento == "BMP") {
                        //DWObject.SaveAsBMP(rut + "/" + doc + ".BMP", i)
                        docu_save_html_Individual("Webform_save_digital_image.aspx", ssl_state, doc + ".BMP", 0, i);
                        //return;
                    }
                    if (extension_documento == "PDF") {                       
                        docu_save_html("Webform_save_digital_image.aspx", ssl_state, doc + ".PDF", 4, i);
                        return;
                    }
                }
                else {
                    var setval = window.parent.document.getElementById("HiddenIdFlujo").value;
                    var spl = setval.split("|");
                    var numer = spl[0];
                    doc = Ceros_Doc(numer.toString());
                    //var rut = ruta_server;
                    var reconteo = i;
                    exte = cero_exten(reconteo.toString());
                    //DWObject.IfShowFileDialog = false;
                    if (extension_documento == "TIF") {
                        //DWObject.SaveAsTIFF(rut + "/" + doc + "." + exte, i)
                        setTimeout(docu_save_html_Individual("Webform_save_digital_image.aspx", ssl_state, doc + i + ".TIF", 2, i),30000);
                        //alert(doc + i + ".TIF");
                    }
                    if (extension_documento == "JPG") {
                        //DWObject.SaveAsJPEG(rut + "/" + doc + exte, i)
                        docu_save_html_Individual("Webform_save_digital_image.aspx", ssl_state, doc + "." + exte, 1, i);
                    }
                    if (extension_documento == "BMP") {
                        //DWObject.SaveAsBMP(rut + "/" + doc + exte, i)
                        docu_save_html_Individual("Webform_save_digital_image.aspx", ssl_state, doc + "." + exte, 0, i);
                    }
                    if (extension_documento == "PDF") {
                        //DWObject.SaveAllAsPDF(rut + "/" + doc + exte, i)
                        return;
                    }
                }
            }
            
        }

    }
}
function docu_save_html(page_aspx, ssl_option, nombre_archivo_image, tipo_image_numero, index_image) {
    try {
        var strHTTPServer = location.hostname; //The name of the HTTP server. 
        var CurrentPathName = unescape(location.pathname);
        var CurrentPath = CurrentPathName.substring(0, CurrentPathName.lastIndexOf("/") + 1);
        var strActionPage = CurrentPath + page_aspx;
        console.log(strActionPage);
        console.log(strHTTPServer);
        console.log(nombre_archivo_image);
        console.log("OPTIONSSL " + ssl_option);
        DWObject.IfSSL = ssl_option; // Set whether SSL is used 
        //guarda tipo pdf
        if (tipo_image_numero == 4) {
            DWObject.HTTPUploadAllThroughPostAsPDF(strHTTPServer, strActionPage, nombre_archivo_image, OnHttpUploadSuccess, OnHttpUploadFailure);
        }
        //guarda tipo tif
        if (tipo_image_numero == 2) {
            DWObject.HTTPUploadAllThroughPostAsMultiPageTIFF(strHTTPServer, strActionPage, nombre_archivo_image, OnHttpUploadSuccess, OnHttpUploadFailure);
        }
    } catch (ex) {
        aler("Error guardando archivo en el servidor funcion docu_save_html detalle : (" + ex.message + ")");
    }

}
function docu_save_html_Individual(page_aspx, ssl_option, nombre_archivo_image, tipo_image_numero,index_image) {
    var strHTTPServer = location.hostname; //The name of the HTTP server. 
    var CurrentPathName = unescape(location.pathname);
    var CurrentPath = CurrentPathName.substring(0, CurrentPathName.lastIndexOf("/") + 1);
    var strActionPage = CurrentPath + page_aspx;
    DWObject.IfSSL = ssl_option; // Set whether SSL is used  HTTPUploadThroughPostEx   HTTPUploadThroughPost
    //DWObject.HTTPPort = location.port == "" ? 80 : location.port;
    DWObject.HTTPUploadThroughPostEx(
      strHTTPServer,
      index_image,
      strActionPage,
      nombre_archivo_image,
      tipo_image_numero,
      OnHttpUploadSuccess,
      OnHttpUploadFailure
    );
}
function OnHttpUploadSuccess() {
    //console.log('successful');
    //activa_document_save();
    //hiden_modal();
}
//fuccion que activa la carga del archivo en el servidor
function OnHttpUploadFailure(errorCode, errorString, sHttpResponse) {
    try {
        if (errorCode != "-2003") {
            alert("Codigo error " + errorCode + " Detalle " + errorString + " reguest " + sHttpResponse);
            return;
        } else {
            activa_document_save();
            hiden_modal();
        }
    } catch (ex) {
        alert(ex.message);
    }
}
//guardar documento local
function Guardar_Documento_server() {
    try {
        
        var doc;
        var exte;
        var extension_documento = "";
        var i;
        var tempo_item = document.getElementsByName("TypeFormato");
        for (iz = 0; iz < tempo_item.length; iz++) {
            if (tempo_item[iz].checked == true)
                extension_documento = tempo_item[iz].value;
        }
        if (extension_documento == "") {
            alert("Imposible encontrar la extensión " + extension_documento)
            return false;
        }
        if (DWObject) {
            if (DWObject.HowManyImagesInBuffer !== 0) {
                var valid_hiden_flujo = window.parent.document.getElementById("HiddenIdFlujo");
                var vali_hiden_ruta = window.parent.document.getElementById("HiddenRuta");
                if (valid_hiden_flujo == undefined) {
                    return false;
                }
                if (vali_hiden_ruta == undefined) {
                    return false;
                }
                for (var i = 0; i < DWObject.HowManyImagesInBuffer; i++) {
                    if (i == 0) {
                        var setval = window.parent.document.getElementById("HiddenIdFlujo").value;
                        var spl = setval.split("|");
                        var numer = spl[0];
                        var rut = window.parent.document.getElementById("HiddenRuta").value;
                        doc = Ceros_Doc(numer.toString());
                        DWObject.IfShowFileDialog = false;
                        if (extension_documento == "TIF") {
                            DWObject.SaveAsTIFF(rut + "/" + doc + ".TIF", i)
                        }
                        if (extension_documento == "JPG") {
                            DWObject.SaveAsJPEG(rut + "/" + doc + ".JPG", i)
                        }
                        if (extension_documento == "BMP") {
                            DWObject.SaveAsBMP(rut + "/" + doc + ".BMP", i)
                        }
                        if (extension_documento == "PDF") {
                            DWObject.SaveAllAsPDF(rut + "/" + doc + ".PDF", i)
                            return;
                        }
                    }
                    else {
                        var setval = window.parent.document.getElementById("HiddenIdFlujo").value;
                        var spl = setval.split("|");
                        var numer = spl[0];
                        doc = Ceros_Doc(numer.toString());
                        var rut = window.parent.document.getElementById("HiddenRuta").value;
                        var reconteo = i;
                        exte = cero_exten(reconteo.toString());
                        DWObject.IfShowFileDialog = false;
                        if (extension_documento == "TIF") {
                            DWObject.SaveAsTIFF(rut + "/" + doc + "." + exte, i)
                        }
                        if (extension_documento == "JPG") {
                            DWObject.SaveAsJPEG(rut + "/" + doc + exte, i)
                        }
                        if (extension_documento == "BMP") {
                            DWObject.SaveAsBMP(rut + "/" + doc + exte, i)
                        }
                        if (extension_documento == "PDF") {
                            DWObject.SaveAllAsPDF(rut + "/" + doc + exte, i)
                            return;
                        }
                    }
                }
              
            }
        }     
    }
    catch (err) {
        alert(err.toString());
    }

}
function cero_exten(numeral) {

    var kp = numeral.length;
    var ext;
    //alert(kp);
    switch (kp) {
        case 0:
            ext = "0000" + numeral;
            break;
        case 1:
            ext = "0000" + numeral;
            break;
        case 2:
            ext = "000" + numeral;
            break;
        case 3:
            ext = "00" + numeral;
            break;

        case 4:
            ext = "0" + numeral;
            break;

        case 5:
            ext = numeral;
            break;

    }
    return ext;
}
function Ceros_Doc(numero) {
    var n = numero.length;
    var doc;
    switch (n) {
        case 1:
            doc = "DIG000000000" + numero;
            break;
        case 2:
            doc = "DIG00000000" + numero;
            break;
        case 3:
            doc = "DIG0000000" + numero;
            break;

        case 4:
            doc = "DIG000000" + numero;
            break;

        case 5:
            doc = "DIG00000" + numero;
            break;

        case 6:
            doc = "DIG0000" + numero;
            break;

        case 7:
            doc = "DIG000" + numero;
            break;

        case 8:
            doc = "DIG00" + numero;
            break;

        case 9:
            doc = "DIG0" + numero;
            break;

        case 10:
            doc = "DIG" + numero;
            break;


    }

    return doc;
}
/*-----------------Acquire Image---------------------*/

function acquireImageByWebcam()
{
    DWObject.Addon.Webcam.SelectSource(document.getElementById("webcamsource").options[document.getElementById("webcamsource").selectedIndex].text);
    var valueMediaType = "";
    var MediaType = document.getElementById("MediaType");
    if(MediaType && MediaType.options.length > 0)
    {
        valueMediaType = MediaType.options[MediaType.selectedIndex].text;
        if(valueMediaType != "")
            if(!DWObject.Addon.Webcam.SetMediaType(valueMediaType))
            {
        	    appendMessage('<b>Error setting MediaType value: </b>');
		        appendMessage("<span style='color:#cE5E04'><b>" + DWObject.ErrorString + "</b></span><br />");
            }
    }

    var valueResolution = "";
    var ResolutionWebcam = document.getElementById("ResolutionWebcam");
    if(ResolutionWebcam && ResolutionWebcam.options.length > 0)
    {
        valueResolution = ResolutionWebcam.options[ResolutionWebcam.selectedIndex].text;
        if(valueResolution != "")
        DWObject.Addon.Webcam.SetResolution(valueResolution);
        var aryResolution = DWObject.Addon.Webcam.GetResolution();
        if(valueResolution != aryResolution.GetCurrent())
        {
    	    appendMessage('<b>Error setting Resolution value: </b>');
		    appendMessage("<span style='color:#cE5E04'><b>" + DWObject.ErrorString + "</b></span><br />");
        }
    }
    
    var showUI = document.getElementById("ShowUIForWebcam").checked;

    // optional
    var OnCaptureStart = function () {
    }
    // optional
    var OnCaptureSuccess = function () {
    }
    // optional
    var OnCaptureError = function (error, errorstr) {
        alert(errorstr);
    }
    // optional
    var OnCaptureEnd = function () {
        //Call DWObject.Addon.Webcam.CloseSource() to release webcam.
        DWObject.Addon.Webcam.CloseSource();
        updatePageInfo();
    }

    DWObject.Addon.Webcam.CaptureImage(showUI, OnCaptureStart, OnCaptureSuccess, OnCaptureError, OnCaptureEnd);
    
}
function acquireImage_() {
    iDocumentCounter = 0;
    ControBufer = 0;
    //var DWObject = gWebTwain.getInstance();
    if (DWObject) {
        document.getElementById('Paginador').innerHTML = "";
        var zonn = 0.22222226858139038;
        //DWObject.SetViewMode(-1, -1);
        //$("#a_tumb_pagina").text("-1" + " x " + "-1");
        tipo_visor = 1;
        DWObject.Zoom = zonn;
        DWObject.EnableInteractiveZoom = true;
        if (DWObject.SourceCount > 0) {
            var vDWTSource = document.getElementById("source");
            if (vDWTSource) {

                if (vDWTSource)
                    DWObject.SelectSourceByIndex(vDWTSource.selectedIndex);
                else
                    DWObject.SelectSource();
            }
            DWObject.CloseSource();
            DWObject.OpenSource();
            DWObject.IfAutomaticBorderDetection = true;
            DWObject.IfShowUI = document.getElementById("ShowUI").checked;
            var i;
            for (i = 0; i < 3; i++) {
                if (document.getElementsByName("PixelType").item(i).checked == true)
                    DWObject.PixelType = i;
            }
            //DWObject.XferCount = -1;
            DWObject.Resolution = Resolution.value;
            DWObject.IfFeederEnabled = document.getElementById("ADF").checked;
            DWObject.IfDuplexEnabled = document.getElementById("Duplex").checked;
            DWObject.IfDisableSourceAfterAcquire = true;
            if (DWObject.HowManyImagesInBuffer != 0) {
                DWObject.RemoveAllImages();
                //window.parent.document.getElementById('ButtonEliminarArchivos').click();
                if (window.parent.document.getElementById('ButtonEliminarArchivos') === undefined) {

                } else { //window.parent.document.getElementById('ButtonEliminarArchivos').click(); }
                }
                DWObject.IfDisableSourceAfterAcquire = true;
                DWObject.AcquireImage();

            }
            else
                alert("No TWAIN compatible drivers detected.");
        }

    }
}
var ARQUIRE = 0; 
function acquireImage(estate_new_page)
{
    try {
        if (DWObject.HowManyImagesInBuffer > 0 && estate_new_page==1) {
            var x;
            var r = confirm("Se detectaron (" + DWObject.HowManyImagesInBuffer + ") pagina(s), las desea remplazar por un nuevo documento ? ") ;
            if (r == false) {
                return true;
            }       
        }
        if (ARQUIRE == 0) {
            //DWObject.Viewer.setViewMode(-1, -1);
            //Sevalue_tumb_a(-1);
            ARQUIRE = 1;
        }
        estado_replace = estate_new_page
        //var zonn = 0.22222226858139038;
        //DWObject.SetViewMode(-1, -1);
        
        tipo_visor = 1;
        DWObject.EnableInteractiveZoom = true;
        DWObject.SelectSourceByIndex(document.getElementById("source").selectedIndex);
        DWObject.CloseSource();
        DWObject.IfFeederEnabled = true;
        DWObject.OpenSource();
        //DWObject.IfShowUI = false;
        //DWObject.IfShowIndicator = true;
        //DWObject.IfShowProgressBar = true;
        //DWObject.IfShowFileDialog = true;
        //DWObject.IfAutomaticBorderDetection = true;
        DWObject.IfAutomaticBorderDetection = document.getElementById("border_detect").checked;
        DWObject.IfShowUI = document.getElementById("ShowUI").checked;
        document.getElementById('Paginador').innerHTML = "";
        var i;
        for (i = 0; i < 3; i++) {
            if (document.getElementsByName("PixelType").item(i).checked == true)
                DWObject.PixelType = i;
        }
        if(DWObject.ErrorCode != 0)
        {
            appendMessage('<b>Error setting PixelType value: </b>');
            appendMessage("<span style='color:#cE5E04'><b>" + DWObject.ErrorString + "</b></span><br />");
        }
        DWObject.Resolution = document.getElementById("Resolution").value;
        if(DWObject.ErrorCode != 0)
        {
            appendMessage('<b>Error setting Resolution value: </b>');
            appendMessage("<span style='color:#cE5E04'><b>" + DWObject.ErrorString + "</b></span><br />");
        }
	
        var bADFChecked = document.getElementById("ADF").checked;
        DWObject.IfFeederEnabled = bADFChecked;
        if(bADFChecked == true && DWObject.ErrorCode != 0)
        {
            appendMessage('<b>Error setting ADF value: </b>');
            appendMessage("<span style='color:#cE5E04'><b>" + DWObject.ErrorString + "</b></span><br />");
        }
	
        var bDuplexChecked = document.getElementById("Duplex").checked;
        DWObject.IfDuplexEnabled = bDuplexChecked;
        if(bDuplexChecked == true && DWObject.ErrorCode != 0)
        {
            appendMessage('<b>Error setting Duplex value: </b>');
            appendMessage("<span style='color:#cE5E04'><b>" + DWObject.ErrorString + "</b></span><br />");
        }
        if (Dynamsoft.Lib.env.bWin || (!Dynamsoft.Lib.env.bWin && DWObject.ImageCaptureDriverType == 0))
            appendMessage("Pixel Type: " + DWObject.PixelType + "<br />Resolution: " + DWObject.Resolution + "<br />");
        //Detecta pagina en blanco
        if (DWObject.IfShowUI == false) {
            decar_page_blank_harware();
        }
        
        DWObject.IfDisableSourceAfterAcquire = true;
        if (estate_new_page == "1") {
            if (DWObject.HowManyImagesInBuffer != 0) {
                DWObject.RemoveAllImages();
            }
            document.getElementById('Paginador').innerHTML = "0" + " de " + DWObject.HowManyImagesInBuffer;
            DWObject.CurrentImageIndexInBuffer = 0;
            DWObject.IfAppendImage = true;        
            DWObject.AcquireImage();
            document.getElementById('Paginador').innerHTML = (DWObject.CurrentImageIndexInBuffer + 1) + " de " + DWObject.HowManyImagesInBuffer;
            if (DWObject.HowManyImagesInBuffer == 0) {
                //document.getElementById('Paginador').innerHTML = "0" + " de " + DWObject.HowManyImagesInBuffer;
               
            } else {       
                document.getElementById('Paginador').innerHTML = (DWObject.CurrentImageIndexInBuffer + 1) + " de " + DWObject.HowManyImagesInBuffer;
                
            }
           
       
        }
        //Agregar imagen
        if (estate_new_page == "0") {
            if (DWObject.HowManyImagesInBuffer == 0) {
                return;
            }
            DWObject.IfAppendImage = true;
            DWObject.AcquireImage();
            document.getElementById('Paginador').innerHTML = (DWObject.CurrentImageIndexInBuffer + 1) + " de " + DWObject.HowManyImagesInBuffer;
            
        }
        //Insertar imagen
        if (estate_new_page == "2") {
            if (DWObject.HowManyImagesInBuffer == 0) {
                return;
            } 
            DWObject.IfAppendImage = false;
            DWObject.AcquireImage();
            document.getElementById('Paginador').innerHTML = (DWObject.CurrentImageIndexInBuffer + 1) + " de " + DWObject.HowManyImagesInBuffer;
            
        }
        //Remplazar imagen
        if (estate_new_page == "3") {
            if (DWObject.HowManyImagesInBuffer == 0) {
                return;
            }
            if (bDuplexChecked == true) {
                alert("Está intentando remplazar la página actual con la opción a doble cara en estado activo, por favor inactívela");
                document.getElementById('Paginador').innerHTML = (DWObject.CurrentImageIndexInBuffer + 1) + " de " + DWObject.HowManyImagesInBuffer;
                
            }
            vufer_index = DWObject.CurrentImageIndexInBuffer;
            numer_pagina_actual = DWObject.HowManyImagesInBuffer;         
            DWObject.IfAppendImage = false;
            DWObject.AcquireImage();
            
            
        }
        //DWObject.Zoom = ZOON_VISOR_DA;
    }
    catch (err) {
        alert(" funcion acquireImage " + err.message);
    }
}

function asig_property_scaner() {
    if (DWObject) {
        DWObject.Resolution = Resolution.value;
        DWObject.IfFeederEnabled = document.getElementById("ADF").checked;
        DWObject.IfDuplexEnabled = document.getElementById("Duplex").checked;
        DWObject.IfShowUI = document.getElementById("ShowUI").checked;
    }
}
function des_kew_harware() {
    try {
        if (document.getElementById("des_kew").checked == true && DWObject.IfShowUI == false) {
            DWObject.Capability = Dynamsoft.DWT.EnumDWT_Cap.ICAP_AUTOMATICDESKEW;
            DWObject.CapType = 5;
            DWObject.CapValue = 0;
        } else {
           
        }
    }
    catch (err) {
        alert(" funcion des_kew_harware " + err.message);
    }
}
function des_kew_software() {
    try {
        if (document.getElementById("des_kew").checked == true && DWObject.IfShowUI == false) {
            if (DWObject.Capability !== Dynamsoft.DWT.EnumDWT_Cap.ICAP_AUTOMATICDESKEW) {
                var Angle = DWObject.GetSkewAngle(DWObject.CurrentImageIndexInBuffer);
                DWObject.Rotate(DWObject.CurrentImageIndexInBuffer, -Angle, 0);           
            }
           
        } else {

        }
    }
    catch (err) {
        alert(" funcion des_kew_software " + err.message);
    }
}

var ANGLE;
var TRUE_DESK;
var INTERVAL;

function des_kew() { 
    if (DWObject.CurrentImageIndexInBuffer != -1) {
        DWObject.CopyToClipboard(DWObject.CurrentImageIndexInBuffer);
        DWObject.GetSkewAngle(
       DWObject.CurrentImageIndexInBuffer,
       function (ANGLE) {
           DWObject.Rotate(DWObject.CurrentImageIndexInBuffer, ANGLE, true,
               function () {
                   TRUE_DESK = "YES";
                   setTimeout(confirma_desk, 500);
               },
               function (errorCode, errorString) {

                   alert(errorString);

               }
           );
       },
       function (errorCode, errorString) {

           alert(errorString);

       }
      );
    }
   
    
}
function confirma_desk() {
    if (TRUE_DESK == "YES") {
        var r = confirm("La imagen se endrezó corectamente, desea conservar el cambio ? ");
        if (r == true) {
            TRUE_DESK = "YES";
            return true;
        } else {
            TRUE_DESK = "NO";
            DWObject.RemoveImage(DWObject.CurrentImageIndexInBuffer);
            DWObject.IfAppendImage = false;
            DWObject.LoadDibFromClipboard();
           
        }
        
    }
}


function decar_page_blank_harware() {

    try {
        if (document.getElementById("Radio_pag_blank").checked == true && DWObject.IfShowUI == false) {
            DWObject.IfAutoDiscardBlankpages = true;
            DWObject.Capability = Dynamsoft.DWT.EnumDWT_Cap.ICAP_AUTODISCARDBLANKPAGES;
            DWObject.CapType = Dynamsoft.DWT.EnumDWT_CapType.TWON_ONEVALUE;
            DWObject.CapValue = -1;
        } else {
            DWObject.IfAutoDiscardBlankpages = false;
        }
    }
    catch (err) {
        alert(" funcion decar_page_blank_harware " + err.message);
    }
}
function detecta_pagina_blanco() {
    try {
        if (document.getElementById("Radio_pag_blank").checked == true) {
            DWObject.BlankImageMaxStdDev = 1;
            if (DWObject.IsBlankImageExpress(DWObject.CurrentImageIndexInBuffer)) {
                DWObject.RemoveImage(DWObject.CurrentImageIndexInBuffer);
            }
            
            
        }
    } catch (ex) {
        alert(ex.message);
    }

}
//primera imagen
function ini_lasImage_Image(event, e) {
    try {
       
        //var DWObject = gWebTwain.getInstance();
        if (DWObject.HowManyImagesInBuffer == 0) {
            //alert("There is no image in buffer");
        }
        else {

            //var ideximage = DWObject.CurrentImageIndexInBuffer - 1;
            //DWObject.CurrentImageIndexInBuffer =
            DWObject.CurrentImageIndexInBuffer = 0;
            document.getElementById('Paginador').innerHTML = (DWObject.CurrentImageIndexInBuffer + 1) + " de " + DWObject.HowManyImagesInBuffer;
            //updatePageInfo();
        }
    } catch (err) {
        alert(" funcion LasImage_Image " + err.message)
    } finally {
        
    }
}
//imagen anterior
function LasImage_Image(event, e) {
    try {
        
        //var DWObject = gWebTwain.getInstance();
        if (DWObject.HowManyImagesInBuffer == 0) {
            //alert("There is no image in buffer");
        }
        else {

            //var ideximage = DWObject.CurrentImageIndexInBuffer - 1;
            DWObject.CurrentImageIndexInBuffer = DWObject.CurrentImageIndexInBuffer - 1;
            document.getElementById('Paginador').innerHTML = (DWObject.CurrentImageIndexInBuffer + 1) + " de " + DWObject.HowManyImagesInBuffer;
            //updatePageInfo();
        }
    } catch (err) {
        alert(" funcion LasImage_Image " + err.message)
    } finally {
       
    }
}
//siguiente imagen
function NextImage_Image() {
    try {
        //var DWObject = gWebTwain.getInstance();
        if (DWObject.HowManyImagesInBuffer == 0) {
            //alert("There is no image in buffer");
        }
        else {
            //var ideximage = DWObject.CurrentImageIndexInBuffer + 1;
            DWObject.CurrentImageIndexInBuffer = DWObject.CurrentImageIndexInBuffer + 1;      
            document.getElementById('Paginador').innerHTML = (DWObject.CurrentImageIndexInBuffer + 1) + " de " + DWObject.HowManyImagesInBuffer;
            //updatePageInfo();
        }
            

    } catch (err) {
        alert(" funcion NextImage_Image " + err.message)
    }

}
//utima imagen
function Fin_NextImage_Image(event, e) {
    try {
        
        //var DWObject = gWebTwain.getInstance();
        if (DWObject.HowManyImagesInBuffer == 0) {
            //alert("There is no image in buffer");
        }
        else {
            //var ideximage = DWObject.CurrentImageIndexInBuffer + 1;
            //DWObject.CurrentImageIndexInBuffer =
            var numero_image = DWObject.HowManyImagesInBuffer;
            DWObject.CurrentImageIndexInBuffer = numero_image - 1;
            document.getElementById('Paginador').innerHTML = (DWObject.CurrentImageIndexInBuffer + 1) + " de " + DWObject.HowManyImagesInBuffer;
            //updatePageInfo();
        }


    } catch (err) {
        alert(" funcion NextImage_Image " + err.message)
    } finally {
        
    }

}
//activa vista normal de minuiatura
function pagina_entera(index) {
    try {
        var numero_image = DWObject.HowManyImagesInBuffer;
        DWObject.CurrentImageIndexInBuffer = index;
        document.getElementById('Paginador').innerHTML = (DWObject.CurrentImageIndexInBuffer + 1) + " de " + DWObject.HowManyImagesInBuffer;
        //DWObject.Viewer.setViewMode(-1, -1);
        //$("#a_tumb_pagina").text("-1" + " x " + "-1");
        Sevalue_tumb_a(-1);
        tipo_visor = 2;
    } catch (err) {
        alert(" pagina_entera " + err.message)
    }
}
//busqueda pagina
function Find_page_imagen(event, e) {
    try {
       
        if (DWObject.HowManyImagesInBuffer == 0) {
            return;
        }
        var valu_pag = document.getElementById("Text_buequeda").value;
        if (valu_pag == "") {
            return;
        }
        if (valu_pag == 0) {
            return;
        }
        if (valu_pag > (DWObject.HowManyImagesInBuffer + 1)) {
            return;
        }
        var numero_image = DWObject.HowManyImagesInBuffer;
        DWObject.CurrentImageIndexInBuffer = valu_pag - 1;
        document.getElementById('Paginador').innerHTML = (DWObject.CurrentImageIndexInBuffer + 1) + " de " + DWObject.HowManyImagesInBuffer;
        //updatePageInfo();
    } catch (err) {
        alert(" Find_page_imagen " + err.message)
    } finally {
       
    }
}
function btnRemoveCurrentImage_onclick(event, e) {
    try {
    
    if (!checkIfImagesInBuffer()) {
        return;
    }
    if (DWObject.SelectedImagesCount == 0) {
        alert("Debe seleccionar una imagen para digitalzar " )
        return;
    }
    var r;
    if (tipo_visor == 1) {
        r = confirm("Desea eliminar la imagen " + (DWObject.CurrentImageIndexInBuffer + 1));
    } else {
        r = confirm("Desea eliminar las imagenes selecionadas " );
    }
    
    if (r == false) {
        return;
    }
    if (tipo_visor == 1) {
        DWObject.RemoveImage(DWObject.CurrentImageIndexInBuffer);
    } else {
        DWObject.RemoveAllSelectedImages();
    }
    
    if (DWObject.HowManyImagesInBuffer == 0) {
        document.getElementById('Paginador').innerHTML = (0) + " de " + DWObject.HowManyImagesInBuffer;
        return;
    }
    else {

        document.getElementById('Paginador').innerHTML = (DWObject.CurrentImageIndexInBuffer + 1) + " de " + DWObject.HowManyImagesInBuffer;
    }
} catch (err) {
    alert(" btnRemoveCurrentImage " + err.message)
} finally {
    
}
}


function btnRemoveAllImages_onclick(event, e) {
    try {
        
    if (!checkIfImagesInBuffer()) {
        return;
    }
    var r = confirm("Desea eliminar todas las imagenes ");
    if (r == false) {
        return;
    }
    DWObject.RemoveAllImages();
    document.getElementById('Paginador').innerHTML = (0) + " de " + DWObject.HowManyImagesInBuffer;
} catch (err) {
    alert(" btnRemoveCurrentImage " + err.message)
} finally {
   
}
}

function mas_view_image(event, e) {
    try {
       
        //var DWObject = gWebTwain.getInstance();
        if (DWObject) {
            //DWObject.SetViewMode(-1, -1);
            //DWObject.EnableInteractiveZoom = true;
            //alert(DWObject.Zoom);
            
            
            DWObject.Zoom = DWObject.Zoom * 1.1;
            
            //alert(DWObject.Zoom);
            //DWObject.Zoom = DWObject.Zoom * 0.9;
            //DWObject.Zoom = DWObject.Zoom + 0.11111111111;
            //alert(DWObject.Zoom);
            //if (DW_CheckErrorString()) {
            //   return;
            //}
        }
    }
    catch (err) {
        alert(err.message);
    } finally {
       
    }
}

function menos_view_image(event, e) {
    try {
       
        //var DWObject = gWebTwain.getInstance();
        if (DWObject) {
            // DWObject.SetViewMode(-1, -1);
            // DWObject.EnableInteractiveZoom = true;
            //alert(DWObject.Zoom);

            //DWObject.Zoom = DWObject.Zoom * 1.1;
            DWObject.Zoom = DWObject.Zoom * 0.9;
            
            //DWObject.Zoom = DWObject.Zoom - 0.11111111111;
            //alert(DWObject.Zoom);
            //if (DW_CheckErrorString()) {
            //    return;
            //}
        }
    }
    catch (err) {
        alert(err.message );
    } finally {
        
    }
}
function todas_view_image() {
    try {
    if (DWObject) {
        //DWObject.Viewer.setViewMode(1, 1);
        //$("#a_tumb_pagina").text("1" + " x " + "1");
        Sevalue_tumb_a(-1);
        DWObject.EnableInteractiveZoom = false;

    }
    } catch (err) {
        alert( " funcion todas_view_image " + err.message)
    }
}
function Dynamsoft_OnReady_(event, e) {
    try {
        DWObject.Viewer.sho
        e.classList.toggle("fa-spin");
        if (document.getElementById("Select_tumb").value == -1) {
            DWObject.Viewer.setViewMode(5, 5);
            document.getElementById("Select_tumb").value = 5;
        } else {
            DWObject.Viewer.setViewMode(document.getElementById("Select_tumb").value, document.getElementById("Select_tumb").value);
        }
       
        tipo_visor = 2;
    } catch (err) {
        alert(" Dynamsoft_OnReady_ " + err.message)
    } finally {
        e.classList.toggle("fa-spin");
    }
}
function Sevalue_tumb(event, e) {
    try {
       
        Sevalue_tumb_a(document.getElementById("Select_tumb").value);
        tipo_visor = 2;
    } catch (err) {
        alert(" Sevalue_tumb " + err.message)
    } finally {
        
    }
}
function Sevalue_tumb_a(val) {
    try { 
        $("#a_tumb_pagina").text(val);
        DWObject.Viewer.setViewMode(val, val);
        //DWObject.Viewer.showCheckbox = true;
        if (val == -1) {
            tipo_visor = 1;
            if (ESTADO_TUMB == 1) {
                ESTADO_TUMB = 0;
                Thumbnail_();
            }
           
        } else {
            
            tipo_visor = 2;
        }
        
    } catch (err) {
        alert(" Sevalue_tumb " + err.message)
    } 
}
var ESTADO_TUMB = 0;
function state_Thumbnail() {
    try {
        
        if (TUMBAIL_VISOR_DA == 1) {
            var thumbnail = DWObject.Viewer.createThumbnailViewer();
            thumbnail.on("click", function (viewerEvent) {
                document.getElementById('Paginador').innerHTML = (viewerEvent.index + 1) + " de " + DWObject.HowManyImagesInBuffer;
            });
            thumbnail.showPageNumber = true;
            thumbnail.allowResizing = true;
            thumbnail.show();
            $("#a_tumb_pagina").text("-1" + " x " + "-1");
            ESTADO_TUMB = 1;
        } else {
            ESTADO_TUMB = 0;
            if (thumbnail) {
                thumbnail.hide();
            }
           
        }
    } catch (ex) {
        alert("Error funcion state_Thumbnail " + ex.message);
    }
}

function Thumbnail_() {
    try {
        
        var thumbnail = DWObject.Viewer.createThumbnailViewer();  
        thumbnail.on("click", function (viewerEvent) {
            document.getElementById('Paginador').innerHTML = (viewerEvent.index + 1) + " de " + DWObject.HowManyImagesInBuffer;          
        });
        if (ESTADO_TUMB == 0) {
            thumbnail.showPageNumber = true;
            thumbnail.allowResizing = true;
            thumbnail.show();
            $("#a_tumb_pagina").text("-1" + " x " + "-1");
            ESTADO_TUMB = 1;        
        } else {
            ESTADO_TUMB = 0;
            thumbnail.hide();
        }       
    } catch (err) {
        alert(" Thumbnail_ " + err.message)
    }
}

function load_file(event, e) {
    try {
       
        if (DWObject) {
            if (DWObject.HowManyImagesInBuffer > 0) {
                var x;
                var r = confirm("Se detectaron (" + DWObject.HowManyImagesInBuffer + ") pagina(s), las desea remplazar por la imagen a cargar ? ");
                if (r == false) {
                    return true;
                }
            }
            DWObject.IfShowFileDialog = true; // Open the system's file dialog to load image
            function OnSuccess() {
                document.getElementById('Paginador').innerHTML = (DWObject.CurrentImageIndexInBuffer + 1) + " de " + DWObject.HowManyImagesInBuffer;
                //DWObject.Viewer.setViewMode(-1, -1);
                //$("#a_tumb_pagina").text("-1" + " x " + "-1");
                Sevalue_tumb_a(-1);
            }
            function OnFailure(errorCode, errorString) {
                if (errorCode != -2326)
                    alert(errorString);
            }    
            DWObject.LoadImageEx("", Dynamsoft.DWT.EnumDWT_ImageType.IT_ALL, OnSuccess, OnFailure);
       
        }
    } catch (err) {
        alert(" load_file " + err.message)
    } 
}
function save_file(event, e) {
    try {
       
        if (DWObject.HowManyImagesInBuffer == 0) {
            return;
        }
        function OnSuccess() {
         
            //console.log('successful');
        }
        function OnFailure(errorCode, errorString) {
            alert(errorString);
        }
        DWObject.SaveAllAsPDF("", OnSuccess, OnFailure);
    } catch (ex) {
        alert("save_file" + ex.message)
    } finally {
       
    }
}
Dynamsoft.DWT.RegisterEvent('OnWebTwainReady', Dynamsoft_OnReady);

/*-----------------Load Image---------------------*/
function btnLoad_onclick() {
    var OnSuccess = function() {
        appendMessage("Loaded an image successfully.<br/>");
        updatePageInfo();
    };

    var OnFailure = function(errorCode, errorString) {
        checkErrorStringWithErrorCode(errorCode, errorString);
    };
    
    DWObject.IfShowFileDialog = true;
    DWObject.LoadImageEx("", Dynamsoft.DWT.EnumDWT_ImageType, OnSuccess, OnFailure);
}

function btnLoadPDF_onclick() {     
      var OnPDFSuccess = function() {
            appendMessage("Loaded an image successfully.<br/>");
            updatePageInfo();
      };

      var OnPDFFailure = function(errorCode, errorString) {
            checkErrorStringWithErrorCode(errorCode, errorString);
      };
        
     
     var OnSuccess = function () {  
        DWObject.IfShowFileDialog = true;
        DWObject.Addon.PDF.SetResolution(300);
        DWObject.Addon.PDF.SetConvertMode(EnumDWT_ConverMode.CM_RENDERALL);
        DWObject.LoadImageEx("", EnumDWT_ImageType.IT_PDF, OnPDFSuccess, OnPDFFailure);

    };

    var OnFailure = function(errorCode, errorString) {
        appendMessage(errorString);
    };

    if(Dynamsoft.Lib.env.bMac)
    {
        DWObject.IfShowFileDialog = true;
        DWObject.Addon.PDF.SetResolution(300);
        DWObject.Addon.PDF.SetConvertMode(EnumDWT_ConverMode.CM_RENDERALL);
        DWObject.LoadImageEx("", EnumDWT_ImageType.IT_PDF, OnPDFSuccess, OnPDFFailure);
    }
    else
    {
        var strhttp = "http:";
	    if("https:" == document.location.protocol) 
		    strhttp = "https:";
        DWObject.IfSSL = Dynamsoft.Lib.detect.ssl;
        var _strPort = location.port == "" ? 80 : location.port;
        if (Dynamsoft.Lib.detect.ssl == true)
            _strPort = location.port == "" ? 443 : location.port;
        DWObject.HTTPPort = _strPort;
        DWObject.Addon.PDF.Download(strhttp + "//www.dynamsoft.com/Demo/DWT/Resources/addon/Pdf.zip", OnSuccess, OnFailure); 
    }  
   }

function loadSampleImage(nIndex) {
    var ImgArr;

    switch (nIndex) {
        case 1:
            ImgArr = "/Images/twain_associate1.png";
            break;
        case 2:
            ImgArr = "/Images/twain_associate2.png";
            break;
        case 3:
            ImgArr = "/Images/twain_associate3.png";
            break;
    }

    if (location.hostname != '') {

        var OnSuccess = function() {
            appendMessage('Loaded a demo image successfully. (Http Download)<br/>');
            updatePageInfo();
        };

        var OnFailure = function(errorCode, errorString) {
            checkErrorStringWithErrorCode(errorCode, errorString);
        };
        
        DWObject.IfSSL = Dynamsoft.Lib.detect.ssl;
        var _strPort = location.port == "" ? 80 : location.port;
        if (Dynamsoft.Lib.detect.ssl == true)
            _strPort = location.port == "" ? 443 : location.port;
        DWObject.HTTPPort = _strPort;

        DWObject.HTTPDownload(location.hostname, Dynamsoft.Lib.getRealPath(ImgArr), OnSuccess, OnFailure);
    }
    else {
        var OnSuccess = function() {
            DWObject.IfShowFileDialog = true;

            appendMessage('Loaded a demo image successfully.');
            updatePageInfo();
        };

        var OnFailure = function(errorCode, errorString) {
            DWObject.IfShowFileDialog = true;
            checkErrorStringWithErrorCode(errorCode, errorString);
        };
        
        DWObject.IfShowFileDialog = false;
        DWObject.LoadImage(Dynamsoft.Lib.getRealPath(ImgArr), OnSuccess, OnFailure);
    }
}

//--------------------------------------------------------------------------------------
//************************** Edit Image ******************************

//--------------------------------------------------------------------------------------
function btnShowImageEditor_onclick() {
    if (!checkIfImagesInBuffer()) {
        return;
    }
    DWObject.ShowImageEditor();
}
function ini_rotate_paginas_miniaturas(event,value) {
    try {
        if (DWObject.SelectedImagesIndices.length > 1) {
            if (DWObject.SelectedImagesIndices.length == 0) {
                alert("Debe seleccionar imagenes para rotar");
                return true;
            }
            if (DWObject.SelectedImagesIndices.length == 1) {
                rotate_paginas_miniaturas(value, DWObject.SelectedImagesIndices[0]);
            }
            if (DWObject.SelectedImagesIndices.length > 1) {
                event_multiple_row(event, value, "rotate_dinamic");
            }
        } else {
            if (!checkIfImagesInBuffer()) {
                return;
            }
            rotate_paginas_miniaturas(value, DWObject.CurrentImageIndexInBuffer,0);
        }

    } catch (err) {
        alert("ini_rotate_paginas_miniaturas" + err.message);
    }
}
function rotate_paginas_miniaturas(value, index, multiple) {
    try {
       
        if (value == 180) {
            DWObject.Rotate(index, 180, true);
        }
        if (value == 90) {
            DWObject.RotateLeft(index);
        }
        if (value == -90) {
            DWObject.RotateRight(index);
        }
        if (multiple == 1) {
            ROW_MULTIPLE_ESTATUS_SERVICE = "yes";
        }
        
    } catch (err) {
        if (multiple == 1) {
            myStopFunction_Event("rotate_paginas_miniaturas " + err.message);
        } else {
            alert("rotate_paginas_miniaturas" + err.message);
        }
       
    }
}
function btnRotateRight_onclick(event, e) {
    try {
        
    if (!checkIfImagesInBuffer()) {
        return;
     }
    DWObject.RotateRight(DWObject.CurrentImageIndexInBuffer);
    appendMessage('<b>Rotate right: </b>');
    if (checkErrorString()) {
        return;
    }
    } catch (err) {
        alert("btnRotateRight_onclick" + err.message);
    } finally {
       
    }
}
function btnRotateLeft_onclick(event, e) {
    try {
       
    if (!checkIfImagesInBuffer()) {
        return;
    }
    DWObject.RotateLeft(DWObject.CurrentImageIndexInBuffer);
    appendMessage('<b>Rotate left: </b>');
    if (checkErrorString()) {
        return;
    }
    } catch (err) {
        alert("btnRotateLeft_onclick" + err.message);
     } finally {
        
     }
}

function btnRotate180_onclick(event, e) {
    try {
       
    if (!checkIfImagesInBuffer()) {
        return;
    }
    DWObject.Rotate(DWObject.CurrentImageIndexInBuffer, 180, true);
    appendMessage('<b>Rotate 180: </b>');
    if (checkErrorString()) {
        return;
    }
      } catch (err) {
        alert("btnRotateLeft_onclick" + err.message);
      } finally {
       
     }
}

function btnMirror_onclick() {
    if (!checkIfImagesInBuffer()) {
        return;
    }
    DWObject.Mirror(DWObject.CurrentImageIndexInBuffer);
    appendMessage('<b>Mirror: </b>');
    if (checkErrorString()) {
        return;
    }
}
function btnFlip_onclick() {
    if (!checkIfImagesInBuffer()) {
        return;
    }
    DWObject.Flip(DWObject.CurrentImageIndexInBuffer);
    appendMessage('<b>Flip: </b>');
    if (checkErrorString()) {
        return;
    }
}
function btn_cursor(event, e) {
    try {
        if (DWObject.Viewer.cursor == 'default') {
            DWObject.Viewer.cursor = 'crosshair';
            document.getElementById("cursor_select").style.border = "inset";
        } else {
            DWObject.Viewer.cursor = 'default';
            document.getElementById("cursor_select").style.border = "none";
        }
        
    } catch (err) {
        alert("btn_cursor" + err.message);
    }
}
/*----------------------Crop Method---------------------*/
function btnCrop_onclick(event, e) {
    try {    
        
        if (!checkIfImagesInBuffer()) {
            return;
        }
        if (_iLeft != 0 || _iTop != 0 || _iRight != 0 || _iBottom != 0) {
            DWObject.Crop(
                DWObject.CurrentImageIndexInBuffer,
                _iLeft, _iTop, _iRight, _iBottom
            );
            _iLeft = 0;
            _iTop = 0;
            _iRight = 0;
            _iBottom = 0;
            appendMessage('<b>Crop: </b>');
            if (checkErrorString()) {
                return;
            }
            return;
        } else {
            appendMessage("<b>Crop: </b>failed. Please first select the area you'd like to crop.<br />");
        }
    } catch (err) {
        alert("btnCrop_onclick" + err.message);
    } finally {
        
    }
}
/*----------------Change Image Size--------------------*/
function btnChangeImageSize_onclick() {
    if (!checkIfImagesInBuffer()) {
        return;
    }
    switch (document.getElementById("ImgSizeEditor").style.visibility) {
        case "visible": document.getElementById("ImgSizeEditor").style.visibility = "hidden"; break;
        case "hidden": document.getElementById("ImgSizeEditor").style.visibility = "visible"; break;
        default: break;
    }
    document.getElementById("ImgSizeEditor").style.top = ds_gettop(document.getElementById("btnChangeImageSize")) + document.getElementById("btnChangeImageSize").offsetHeight + 15 + "px";
    document.getElementById("ImgSizeEditor").style.left = ds_getleft(document.getElementById("btnChangeImageSize")) - 14 + "px";

    var iWidth = DWObject.GetImageWidth(DWObject.CurrentImageIndexInBuffer);
    if (iWidth != -1)
        document.getElementById("img_width").value = iWidth;
    var iHeight = DWObject.GetImageHeight(DWObject.CurrentImageIndexInBuffer);
    if (iHeight != -1)
        document.getElementById("img_height").value = iHeight;
}
function btnCancelChange_onclick() {
    document.getElementById("ImgSizeEditor").style.visibility = "hidden";
}

function btnChangeImageSizeOK_onclick() {
    document.getElementById("img_height").className = "";
    document.getElementById("img_width").className = "";
    if (!re.test(document.getElementById("img_height").value)) {
        document.getElementById("img_height").className += " invalid";
        document.getElementById("img_height").focus();
        appendMessage("Please input a valid <b>height</b>.<br />");
        return;
    }
    if (!re.test(document.getElementById("img_width").value)) {
        document.getElementById("img_width").className += " invalid";
        document.getElementById("img_width").focus();
        appendMessage("Please input a valid <b>width</b>.<br />");
        return;
    }
    DWObject.ChangeImageSize(
        DWObject.CurrentImageIndexInBuffer,
        document.getElementById("img_width").value,
        document.getElementById("img_height").value,
        document.getElementById("InterpolationMethod").selectedIndex + 1
    );
    appendMessage('<b>Change Image Size: </b>');
    if (checkErrorString()) {
        document.getElementById("ImgSizeEditor").style.visibility = "hidden";
        return;
    }
}
//--------------------------------------------------------------------------------------
//************************** Save Image***********************************
//--------------------------------------------------------------------------------------
function saveUploadImage(type){
	if(type=='local'){
		btnSave_onclick();
		}else if(type=='server'){
			btnUpload_onclick()
			}
	}
function btnSave_onclick() {
    if (!checkIfImagesInBuffer()) {
        return;
    }
    var i, strimgType_save;
    var NM_imgType_save = document.getElementsByName("ImageType");
    for (i = 0; i < 5; i++) {
        if (NM_imgType_save.item(i).checked == true) {
            strimgType_save = NM_imgType_save.item(i).value;
            break;
        }
    }
    DWObject.IfShowFileDialog = true;
    var _txtFileNameforSave = document.getElementById("txt_fileName");
    if(_txtFileNameforSave)
        _txtFileNameforSave.className = "";
    var bSave = false;

    var strFilePath = _txtFileNameforSave.value + "." + strimgType_save;

    var OnSuccess = function() {
        appendMessage('<b>Save Image: </b>');
        checkErrorStringWithErrorCode(0, "Successful.");
    };

    var OnFailure = function(errorCode, errorString) {
        checkErrorStringWithErrorCode(errorCode, errorString);
    };

    var _chkMultiPageTIFF_save = document.getElementById("MultiPageTIFF");
    var vAsyn = false;
    if (strimgType_save == "tif" && _chkMultiPageTIFF_save && _chkMultiPageTIFF_save.checked) {
        vAsyn = true;
        if ((DWObject.SelectedImagesCount == 1) || (DWObject.SelectedImagesCount == DWObject.HowManyImagesInBuffer)) {
            bSave = DWObject.SaveAllAsMultiPageTIFF(strFilePath, OnSuccess, OnFailure);
        }
        else {
            bSave = DWObject.SaveSelectedImagesAsMultiPageTIFF(strFilePath, OnSuccess, OnFailure);
        }
    }
    else if (strimgType_save == "pdf" && document.getElementById("MultiPagePDF").checked) {
        vAsyn = true;
        if ((DWObject.SelectedImagesCount == 1) || (DWObject.SelectedImagesCount == DWObject.HowManyImagesInBuffer)) {
            bSave = DWObject.SaveAllAsPDF(strFilePath, OnSuccess, OnFailure);
        }
        else {
            bSave = DWObject.SaveSelectedImagesAsMultiPagePDF(strFilePath, OnSuccess, OnFailure);
        }
    }
    else {
        switch (i) {
            case 0: bSave = DWObject.SaveAsBMP(strFilePath, DWObject.CurrentImageIndexInBuffer); break;
            case 1: bSave = DWObject.SaveAsJPEG(strFilePath, DWObject.CurrentImageIndexInBuffer); break;
            case 2: bSave = DWObject.SaveAsTIFF(strFilePath, DWObject.CurrentImageIndexInBuffer); break;
            case 3: bSave = DWObject.SaveAsPNG(strFilePath, DWObject.CurrentImageIndexInBuffer); break;
            case 4: bSave = DWObject.SaveAsPDF(strFilePath, DWObject.CurrentImageIndexInBuffer); break;
        }
    }

    if (vAsyn == false) {
        if (bSave)
            appendMessage('<b>Save Image: </b>');
        if (checkErrorString()) {
            return;
        }
    }
}
//--------------------------------------------------------------------------------------
//************************** Upload Image***********************************
//--------------------------------------------------------------------------------------



function btnUpload_onclick() {
    if (!checkIfImagesInBuffer()) {
        return;
    }
    var i, strHTTPServer, strActionPage, strImageType;

    var _txtFileName = document.getElementById("txt_fileName");
    if(_txtFileName)
        _txtFileName.className = "";
  
    //DWObject.MaxInternetTransferThreads = 5;
    strHTTPServer = location.hostname;
    DWObject.IfSSL = Dynamsoft.Lib.detect.ssl;
    var _strPort = location.port == "" ? 80 : location.port;
    if (Dynamsoft.Lib.detect.ssl == true)
        _strPort = location.port == "" ? 443 : location.port;
    DWObject.HTTPPort = _strPort; 
    var CurrentPathName = unescape(location.pathname); // get current PathName in plain ASCII	
    var CurrentPath = CurrentPathName.substring(0, CurrentPathName.lastIndexOf("/") + 1);
    strActionPage = CurrentPath + "SaveToFile.aspx"; //the ActionPage's file path , Online Demo:"SaveToDB.aspx" ;Sample: "SaveToFile.aspx";
    var redirectURLifOK = CurrentPath + "online_demo_list.aspx";
    for (i = 0; i < 5; i++) {
        if (document.getElementsByName("ImageType").item(i).checked == true) {
            strImageType = i;
            break;
        }
    }

	var fileName = _txtFileName.value;
	var replaceStr = "<";
	fileName = fileName.replace(new RegExp(replaceStr,'gm'),'&lt;');
    var uploadfilename = fileName + "." + document.getElementsByName("ImageType").item(i).value;

    var OnSuccess = function(httpResponse) {
        appendMessage('<b>Upload: </b>');
        checkErrorStringWithErrorCode(0, "Successful.");
        if (strActionPage.indexOf("SaveToFile") != -1) {
            alert("Successful")//if save to file.
        } else {
            window.location.href = redirectURLifOK;
        }
    };

    var OnFailure = function(errorCode, errorString, httpResponse) {
        checkErrorStringWithErrorCode(errorCode, errorString, httpResponse);
    };
    
    if (strImageType == 2 && document.getElementById("MultiPageTIFF").checked) {
        if ((DWObject.SelectedImagesCount == 1) || (DWObject.SelectedImagesCount == DWObject.HowManyImagesInBuffer)) {
            DWObject.HTTPUploadAllThroughPostAsMultiPageTIFF(
                strHTTPServer,
                strActionPage,
                uploadfilename,
                OnSuccess, OnFailure
            );
        }
        else {
            DWObject.HTTPUploadThroughPostAsMultiPageTIFF(
                strHTTPServer,
                strActionPage,
                uploadfilename,
                OnSuccess, OnFailure
            );
        }
    }
    else if (strImageType == 4 && document.getElementById("MultiPagePDF").checked) {
    if ((DWObject.SelectedImagesCount == 1) || (DWObject.SelectedImagesCount == DWObject.HowManyImagesInBuffer)) {
            DWObject.HTTPUploadAllThroughPostAsPDF(
                strHTTPServer,
                strActionPage,
                uploadfilename,
                OnSuccess, OnFailure
            );
        }
        else {
            DWObject.HTTPUploadThroughPostAsMultiPagePDF(
                strHTTPServer,
                strActionPage,
                uploadfilename,
                OnSuccess, OnFailure
            );
        }
    }
    else {
        DWObject.HTTPUploadThroughPostEx(
            strHTTPServer,
            DWObject.CurrentImageIndexInBuffer,
            strActionPage,
            uploadfilename,
            strImageType,
            OnSuccess, OnFailure
        );
    }
}

//--------------------------------------------------------------------------------------
//************************** Navigator functions***********************************
//--------------------------------------------------------------------------------------

function btnFirstImage_onclick() {
    if (!checkIfImagesInBuffer()) {
        return;
    }
    DWObject.CurrentImageIndexInBuffer = 0;
    updatePageInfo();
}

function btnPreImage_wheel() {
    if (DWObject.HowManyImagesInBuffer != 0)
        btnPreImage_onclick()
}

function btnNextImage_wheel() {
    if (DWObject.HowManyImagesInBuffer != 0)
        btnNextImage_onclick()
}

function btnPreImage_onclick() {
    if (!checkIfImagesInBuffer()) {
        return;
    }
    else if (DWObject.CurrentImageIndexInBuffer == 0) {
        return;
    }
    DWObject.CurrentImageIndexInBuffer = DWObject.CurrentImageIndexInBuffer - 1;
    updatePageInfo();
}
function btnNextImage_onclick() {
    if (!checkIfImagesInBuffer()) {
        return;
    }
    else if (DWObject.CurrentImageIndexInBuffer == DWObject.HowManyImagesInBuffer - 1) {
        return;
    }
    DWObject.CurrentImageIndexInBuffer = DWObject.CurrentImageIndexInBuffer + 1;
    updatePageInfo();
}


function btnLastImage_onclick() {
    if (!checkIfImagesInBuffer()) {
        return;
    }
    DWObject.CurrentImageIndexInBuffer = DWObject.HowManyImagesInBuffer - 1;
    updatePageInfo();
}


function setlPreviewMode() {
    var varNum = parseInt(document.getElementById("DW_PreviewMode").selectedIndex + 1);
    var btnCrop = document.getElementById("btnCrop");
    if (btnCrop) {
        var tmpstr = btnCrop.src;
        if (varNum > 1) {
            tmpstr = tmpstr.replace('Crop.', 'Crop_gray.');
            btnCrop.src = tmpstr;
            btnCrop.onclick = function() { };
        }
        else {
            tmpstr = tmpstr.replace('Crop_gray.', 'Crop.');
            btnCrop.src = tmpstr;
            btnCrop.onclick = function() { btnCrop_onclick(); };
        }
    }
    //$("#a_tumb_pagina").text(varNum + " x " + varNum);
    //DWObject.Viewer.setViewMode(varNum, varNum);
    Sevalue_tumb_a(varNum);
    if (Dynamsoft.Lib.env.bMac) {
        return;
    }
    else if (document.getElementById("DW_PreviewMode").selectedIndex != 0) {
        DWObject.MouseShape = true;
    }
    else {
        DWObject.MouseShape = false;
    }
}

//--------------------------------------------------------------------------------------
//*********************************radio response***************************************
//--------------------------------------------------------------------------------------
function rdTIFF_onclick() {
    var _chkMultiPageTIFF = document.getElementById("MultiPageTIFF");
    _chkMultiPageTIFF.disabled = false;
    _chkMultiPageTIFF.checked = false;

    var _chkMultiPagePDF = document.getElementById("MultiPagePDF");
    _chkMultiPagePDF.checked = false;
    _chkMultiPagePDF.disabled = true;
}

function rdPDF_onclick() {
    var _chkMultiPageTIFF = document.getElementById("MultiPageTIFF");
    _chkMultiPageTIFF.checked = false;
    _chkMultiPageTIFF.disabled = true;
    
    var _chkMultiPagePDF = document.getElementById("MultiPagePDF");
    _chkMultiPagePDF.disabled = false;
    _chkMultiPagePDF.checked = false;

}

function rd_onclick() {
    var _chkMultiPageTIFF = document.getElementById("MultiPageTIFF");
    _chkMultiPageTIFF.checked = false;
    _chkMultiPageTIFF.disabled = true;
    
    var _chkMultiPagePDF = document.getElementById("MultiPagePDF");
    _chkMultiPagePDF.checked = false;
    _chkMultiPagePDF.disabled = true;
}


//--------------------------------------------------------------------------------------
//************************** Dynamic Web TWAIN Events***********************************
//--------------------------------------------------------------------------------------

function Dynamsoft_OnPostTransfer() {
    updatePageInfo();
    if (document.getElementById("Radio_pag_blank").checked == true) {
        detecta_pagina_blanco();
    }
    if (document.getElementById("des_kew").checked == true) {
        des_kew_software();
    }
    
    if (DWObject.IfAppendImage == false ) {
        //DWObject.IfAppendImage = true;
        var ref_buffer = DWObject.CurrentImageIndexInBuffer + 1;
        DWObject.CurrentImageIndexInBuffer = ref_buffer;
    }
    if (DWObject.HowManyImagesInBuffer != 0) {
        
        document.getElementById('Paginador').innerHTML = (DWObject.CurrentImageIndexInBuffer + 1) + " de " + DWObject.HowManyImagesInBuffer;
        //alert(document.getElementById('Paginador').innerHTML);
    }
    
}

function Dynamsoft_OnPostLoadfunction(path, name, type) {
    updatePageInfo();
}

function Dynamsoft_OnPostAllTransfers() {
   
    DWObject.CloseSource();
    updatePageInfo();
    checkErrorString();
    if (estado_replace == 3) {
        if (DWObject.HowManyImagesInBuffer != 0 && numer_pagina_actual != DWObject.HowManyImagesInBuffer) {
            var difer_pagina;
            var ref_image_bufer = DWObject.HowManyImagesInBuffer; //numero pagina actual
            difer_pagina = ref_image_bufer - numer_pagina_actual;// diferencia 
            var value_elimina_index = vufer_index + difer_pagina; // pagina a eliminar
            //alert(value_elimina_index + "-numero pagina actual" + ref_image_bufer + " numero pagina inicial " + numer_pagina_actual);
            DWObject.RemoveImage(value_elimina_index);
            DWObject.CurrentImageIndexInBuffer = value_elimina_index - 1;
            var curren_img_bufer = DWObject.CurrentImageIndexInBuffer;
            curren_img_bufer = curren_img_bufer + 1;
            document.getElementById('Paginador').innerHTML = (DWObject.CurrentImageIndexInBuffer + 1) + " de " + DWObject.HowManyImagesInBuffer;
        } else {
            document.getElementById('Paginador').innerHTML = (DWObject.CurrentImageIndexInBuffer + 1) + " de " + DWObject.HowManyImagesInBuffer;
        }
    }
    
    
    
}

function Dynamsoft_OnMouseClick(index) {
    updatePageInfo();
}

function Dynamsoft_OnMouseRightClick(index) {
    // To add
}


function Dynamsoft_OnImageAreaSelected(index, left, top, right, bottom) {
    _iLeft = left;
    _iTop = top;
    _iRight = right;
    _iBottom = bottom;
}

function Dynamsoft_OnImageAreaDeselected(index) {
    _iLeft = 0;
    _iTop = 0;
    _iRight = 0;
    _iBottom = 0;
}

function Dynamsoft_OnMouseDoubleClick() {
    return;
}


function Dynamsoft_OnTopImageInTheViewChanged(index) {
    _iLeft = 0;
    _iTop = 0;
    _iRight = 0;
    _iBottom = 0;
    DWObject.CurrentImageIndexInBuffer = index;
    updatePageInfo();
}

function Dynamsoft_OnGetFilePath(bSave, count, index, path, name) {

}

//--------------------------------------------------------------------------------------
//************************** Barcode Addon***********************************
//--------------------------------------------------------------------------------------
function LoadBarcodeDemoImage(nIndex) {
    var ImgArr;

    switch (nIndex) {
        case 1:
            ImgArr = "/Images/code-39.png";
            break;
        case 2:
            ImgArr = "/Images/code-128.png";
            break;
        case 3:
            ImgArr = "/Images/qrcode.png";
            break;
        case 4:
            ImgArr = "/Images/UPC-A.png";
            break;
    }
    
   if (location.hostname != '') {

        var OnSuccess = function() {
            appendMessage('Loaded a demo image successfully. (Http Download)<br/>');
            updatePageInfo();
        };

        var OnFailure = function(errorCode, errorString) {
            checkErrorStringWithErrorCode(errorCode, errorString);
        };
        
        DWObject.IfSSL = Dynamsoft.Lib.detect.ssl;
        var _strPort = location.port == "" ? 80 : location.port;
        if (Dynamsoft.Lib.detect.ssl == true)
            _strPort = location.port == "" ? 443 : location.port;
        DWObject.HTTPPort = _strPort;

        DWObject.HTTPDownload(location.hostname, Dynamsoft.Lib.getRealPath(ImgArr), OnSuccess, OnFailure);
    }
    else {
        var OnSuccess = function() {
            DWObject.IfShowFileDialog = true;

            appendMessage('Loaded a demo image successfully.');
            updatePageInfo();
        };

        var OnFailure = function(errorCode, errorString) {
            DWObject.IfShowFileDialog = true;
            checkErrorStringWithErrorCode(errorCode, errorString);
        };
        
        DWObject.IfShowFileDialog = false;
        DWObject.LoadImage(Dynamsoft.Lib.getRealPath(ImgArr), OnSuccess, OnFailure);
    }
}

function GetBacodeFormatDesc(format)
{
    for (var index = 0; index < BarcodeInfo.length; index ++)
    {
        if (BarcodeInfo[index].val == format)
            return BarcodeInfo[index].desc;
    }

    return "UNKNOWN";
}

function GetBarcodeInfo(sImageIndex, result) {//This is the function called when barcode is read successfully
    //Retrieve barcode details
    var count = result.GetCount();
     appendMessage('BarcodeCount: ' + count + '<br/>');
    if (count == 0) {
        alert("The barcode for the selected format is not found.");
        return;
    } else {
        for (i = 0; i < count; i++) {
            var text = result.GetContent(i);
            var x = result.GetX1(i);
            var y = result.GetY1(i);
            var format = result.GetFormat(i);
            var barcodeText = ("barcode[" + (i + 1) + "]: " + text + "<br/>");
            barcodeText += ("format:" + GetBacodeFormatDesc(format) + "<br/>");
            barcodeText += ("x: " + x + " y:" + y + "<br/>");
            appendMessage(barcodeText);
            
            var strBarcodeString = text + "\r\n" + GetBacodeFormatDesc(format);
            DWObject.AddText(DWObject.CurrentImageIndexInBuffer, x, y, strBarcodeString, 255, 4894463, 0, 1);
        }
    }
}

function GetErrorInfo (errorcode, errorstring) {//This is the function called when barcode reading fails
    alert(errorstring);
}
        
function btnScanReadBarcode_onclick() {
    if (!checkIfImagesInBuffer()) {
        return;
    }
    
     var OnSuccess = function () {
         //Get barcode result.
        DWObject.Addon.Barcode.Read(DWObject.CurrentImageIndexInBuffer, 
        BarcodeInfo[document.getElementById("ddl_barcodeFormat").selectedIndex].val, GetBarcodeInfo, GetErrorInfo);
    };

    var OnFailure = function(errorCode, errorString) {
        appendMessage(errorString);
    };

    var strhttp = "http:";
	if("https:" == document.location.protocol) 
		strhttp = "https:";
	
	DWObject.IfSSL = Dynamsoft.Lib.detect.ssl;
    var _strPort = location.port == "" ? 80 : location.port;
    if (Dynamsoft.Lib.detect.ssl == true)
        _strPort = location.port == "" ? 443 : location.port;
    DWObject.HTTPPort = _strPort;

    if(Dynamsoft.Lib.env.bMac)
        DWObject.Addon.Barcode.Download(strhttp + "//www.dynamsoft.com/Demo/DWT/Resources/addon/MacBarcode.zip", OnSuccess, OnFailure);  
    else
    {
        DWObject.Addon.Barcode.Download(strhttp + "//www.dynamsoft.com/Demo/DWT/Resources/addon/Barcode.zip", OnSuccess, OnFailure); 
    }  
}
