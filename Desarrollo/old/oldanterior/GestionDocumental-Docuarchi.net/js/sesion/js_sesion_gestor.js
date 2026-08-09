
let INTERVAL_SESION_ITEM_MANTENT_SSION_GESTOR;
const Service_REST_validate_sesion_gestor = () => {
    let dat = "";
    try {
        $.ajax('../webservice/WebServiceInicioGestor.asmx/web_service_validate_sesion_active', {
            data: "{'DName':'" + dat + "'}",
            dataType: 'json',
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.d !== "YES") {
                    $("#modal_alert_sesion_time_out").modal("show");
                    clearInterval(INTERVAL_SESION_ITEM_MANTENT_SSION_GESTOR);
                }
            }, error: function (xception, textStatus, errorThrown) {

                if (xception.status === 0) {
                   


                } else if (xception.status == 404) {
                   


                } else if (xception.status == 500) {
                    


                } else if (textStatus === 'parsererror') {
                    


                } else if (textStatus === 'timeout') {
                   


                } else if (textStatus === 'abort') {
                   


                } else {
                   


                }
            }
        });
    }
    catch (ex) {
        
    }
}
const Service_REST_validate_sesion_tab = () => {
    let dat = "";
    try {
        $.ajax('../webservice/WebServiceInicioGestor.asmx/web_service_validate_sesion_active', {
            data: "{'DName':'" + dat + "'}",
            dataType: 'json',
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.d !== "YES") {
                    window.location.assign("../sesionend/sesion_end.html");
                    clearInterval(INTERVAL_SESION_ITEM_MANTENT_SSION_GESTOR);
                }
            }, error: function (xception, textStatus, errorThrown) {

                if (xception.status === 0) {



                } else if (xception.status == 404) {



                } else if (xception.status == 500) {



                } else if (textStatus === 'parsererror') {



                } else if (textStatus === 'timeout') {



                } else if (textStatus === 'abort') {



                } else {



                }
            }
        });
    }
    catch (ex) {

    }
}
const Service_REST_sesion_end_gestor = (dat) => {
    try {
        $.ajax('../webservice/WebServiceInicioGestor.asmx/web_service_sesion_end', {
            data: "{" + "'DName':'" + dat + "'}",
            dataType: 'json',
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data.d == "YES") {
                    //alert("Sin sesion ");
                    //window.location.assign("../gestor.aspx");
                    //clearInterval(INTERVAL_SESION_ITEM_MANTENT);
                }
            }, error: function (xception, textStatus, errorThrown) {

                if (xception.status === 0) {
                    //resolve("Not connect: Verify Network.");


                } else if (xception.status == 404) {
                    //resolve("Requested page not found [404]");


                } else if (xception.status == 500) {
                    resolve("Internal Server Error [500]." + xception.responseText);


                } else if (textStatus === 'parsererror') {
                    //resolve("Requested JSON parse failed.");


                } else if (textStatus === 'timeout') {
                    //return "Time out error.";


                } else if (textStatus === 'abort') {
                    //resolve("Ajax request aborted.");


                } else {
                    //resolve("Ajax request aborted." + xception.responseText);


                }
            }
        });
    }
    catch (ex) {
        //resolve(ex.message);
    }
}
const Service_REST_sesion_end_gestor_async = async (dat) => {
    let myPromise = new Promise(function (resolve) {
    try {
        $.ajax('../webservice/WebServiceInicioGestor.asmx/web_service_sesion_end', {
            data: "{" + "'DName':'" + dat + "'}",
            dataType: 'json',
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                resolve(data.d);
               
            }, error: function (xception, textStatus, errorThrown) {

                if (xception.status === 0) {
                    resolve("Not connect: Verify Network.");


                } else if (xception.status == 404) {
                    resolve("Requested page not found [404]");


                } else if (xception.status == 500) {
                    resolve("Internal Server Error [500]." + xception.responseText);


                } else if (textStatus === 'parsererror') {
                    resolve("Requested JSON parse failed.");


                } else if (textStatus === 'timeout') {
                    return "Time out error.";


                } else if (textStatus === 'abort') {
                    resolve("Ajax request aborted.");


                } else {
                    resolve("Ajax request aborted." + xception.responseText);


                }
            }
        });
    }
    catch (ex) {
        resolve(ex.message);
        }
    })
    let result = await myPromise;
    return result;
}