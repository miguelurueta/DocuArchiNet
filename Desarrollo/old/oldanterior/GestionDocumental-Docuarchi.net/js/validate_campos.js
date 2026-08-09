function validate_letra_numero(e, sender) {
    try {
        tecla = (document.all) ? e.keyCode : e.which;
        if (tecla == 32) {
            return false;
        }
        var se = sender;
        patron = /^[\ba-zA-Z\s-0-9]$/;
        var te = String.fromCharCode(tecla);
        var ret = patron.test(te);
        return patron.test(te);
    } catch (err) {
        alert(err.message + " funcion validate_letra_numero " + err.message);
    }
}
function validate_numero(e, sender) {
    try {
        tecla = (document.all) ? e.keyCode : e.which;
        if (tecla == 32) {
            return false;
        }
        
        patron = /^[0-9]$/;
        var te = String.fromCharCode(tecla);
        return patron.test(te);
    } catch (err) {
        alert(err.message + " funcion validate_numero " + err.message);
    }
}
function validate_fecha(e, sender) {
    try {

        tecla = (document.all) ? e.keyCode : e.which;
        if (tecla == 32) {
            return false;
        }
        patron = /^[0-9 ]$/;
        var te = String.fromCharCode(tecla);
        var res = patron.test(te);
        if (res) {
            if (sender.value.length == 4 || sender.value.length == 7) {
                sender.value = sender.value + "-";
                return patron.test(te);
            }
        } else {
            return patron.test(te);
        }

    } catch (err) {
        alert(err.message + " funcion validate_numero " + err.message);
    }
}
function caracter_especial(e, sender) {
    try {
        tecla = (document.all) ? e.keyCode : e.which;
        
        if (tecla == 39) {
            return false;
        }
        if (tecla == 47) {
            return false;
        }
       
    } catch (err) {
        alert(err.message + " funcion caracter_especial " + err.message);
    }
}
function caracter_especial_nombre(e, sender) {
    try {
        tecla = (document.all) ? e.keyCode : e.which;
       
        if (tecla == 59) {
            return false;
        }
        if (tecla == 96) {
            return false;
        }
        if (tecla == 91) {
            return false;
        }
        if (tecla == 92) {
            return false;
        }
        if (tecla == 93) {
            return false;
        }
        if (tecla == 39) {
            return false;
        }
        if (tecla == 44) {
            return false;
        }
        if (tecla == 47) {
            return false;
        }
    } catch (err) {
        alert(err.message + " funcion caracter_especial " + err.message);
    }
}
function validate_mayuscula(e,sender) {
    try {
        tecla = (document.all) ? e.keyCode : e.which;      
        var te = String.fromCharCode(tecla);
        te = te.toUpperCase();
        return te;
    } catch (err) {
        alert(err.message + " funcion validate_mayuscula " + err.message);
    }
}
function GetChar(event) {
    try {
        var chCode = ('charCode' in event) ? event.charCode : event.keyCode;
        if (chCode == 13) {

        }
    }
    catch (err) {
        alert(err.message + " Funcion auto_zise_popup_editar_radicados");
    }
}
function validateEmail(name_campo) {
    // Get our input reference.
    var emailField = document.getElementById(name_campo);
    // Define our regular expression.
    var validEmail = /^\w+([.-_+]?\w+)*@\w+([.-]?\w+)*(\.\w{2,10})+$/;
    // Using test we can check if the text match the pattern
    if (validEmail.test(emailField.value)) { 
        return "";
    } else {
        return "el email no es valido";
    }
}


