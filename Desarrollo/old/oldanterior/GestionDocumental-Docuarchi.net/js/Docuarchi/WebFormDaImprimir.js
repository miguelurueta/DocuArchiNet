$(document).ready(function () {
    $.fn.inicio = function () {

        $('.solo-numero').keyup(function () {
            this.value = (this.value + '').replace(/[^0-9 $ ,-]/g, '');
        });

    }
});
function progres_hiden(progres) {
    try {
        $("#progres_bar").css("display", "none");
    }
    catch (err) {
        alert(err.message + " Funcion progres_hiden");
    }
}
function ChangeCarList() {
    try {
        var carList = document.getElementById("pid");
        var selCar = carList.options[carList.selectedIndex].value;
        if (selCar == "1") {
            document.getElementById("Table_selecion_pagina").style.display = "none";
            //document.getElementById("cel_option").style.display = "none";
        }
        if (selCar == "0") {
            document.getElementById("Table_selecion_pagina").style.display = "block";
            //document.getElementById("cel_option").style.display = "block";
        }
    }
    catch (err) {
        alert(err.message + " Función ChangeCarList");
    }
   
    
}
function posicion_update_pogres(progres) {
    try {
        var espacio_iframe = 420;
        var hidenpadre = 0;
        var with_frame = 420;
        if (window.innerHeight) {
            //navegadores basados en mozilla 
            espacio_iframe = window.innerHeight;
            with_frame = window.innerWidth;
        } else {
            if (document.body.clientHeight) {
                //Navegadores basados en IExplorer, es que no tengo innerheight 
                espacio_iframe = document.body.clientHeight;
                with_frame = document.body.clientWidth;
            } else {
                //otros navegadores y iframe
                //hidenpadre = $('#Hiddenheigpaginapopup', window.parent.document).val();

            }
        }
        var prog = document.getElementById(progres);
        var widtop = (espacio_iframe / 2);
        var heitop = (with_frame / 2);
        prog.style.top = widtop + "px";
        prog.style.left = heitop + "px";
        prog.style.zIndex = "1000009";
        $("#progres_bar").css("display", "block");
        prog.style.position = "fixed";

    }
    catch (err) {
        alert(err.message + " Funcion posicion_update_pogres");
    }

}