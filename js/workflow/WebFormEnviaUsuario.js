$(document).ready(function () {
    $.fn.inicio = function () {
        //****************************************VALIDACION RADICACION**********************************************************************************
        //FUNCION ACTIVA SELECCION CLIK EN EL DATAGREDVIEW DE VALIDACION RADICACION
        $('#GridViewlista tr[id]').click(function () {
            $('#GridViewlista tr[id]').css({ "background": "White", "color": "Black" });
            $(this).css({ "background-color": "#E7EDF5", "color": "Black" });
            var fer = $(this).attr("id");
            $('#hdnEmailID').val(fer);
            $('#hdnEmailID', window.parent.document).val(fer)
          
  
        });

        //ASIGNA EL CURSOR DE SELECCION CUANDO PASA EL CURSOR EN EL DATAGREDVIEW DE VALIDACION RADICACION
        $('#GridViewlista tr[id]').mouseover(function () {
            $(this).css({ cursor: "hand", cursor: "pointer" });
        });
        if (document.getElementById("Hidden_resultado_gred").value == "YES") {
            document.getElementById("Hidden_resultado_gred").value = "";
            auto_zise_popup_pendinetes();
        }
        $(document).ready(bodyResize);
        $(window).resize(bodyResize);
        function bodyResize() {
            auto_zise_popup_pendinetes();
        }
        //******************************************FIN****************************************************************************************************
    }
});

function onDataShown(sender, args) {
    sender._popupBehavior._element.style.zIndex = 1000001;

}
//MUEVE EL SCCROL AL ID SELECCIONADO
function mueve_scroll_data_gred(data_grid, HiddenSeleccion) {
    if ($("#" + HiddenSeleccion).val() != "-1" || $("#" + HiddenSeleccion).val() != "0") {
        var scrollableDiv = $("#" + data_grid).parent();
        //limpia todos los seleccionados
        $("#" + data_grid + " tr[id]").css({ "background-color": "transparent", "color": "Black" });
        $("#" + data_grid + " tr[id=" + $("#" + HiddenSeleccion).val() + "]").css({ "background-color": "LightSkyBlue", "color": "Red" });
        $("#" + data_grid + " tr[id= " + $("#" + HiddenSeleccion).val() + "]").each(function () {
            $(scrollableDiv).scrollTop(70);
            $(scrollableDiv).scrollTop(($(this).offset().top));
            return true;
        });
    }
}

function progres_hiden(progres) {
    $("#progres_bar").css("display", "none");
}
function posicion_update_pogres(progres) {
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

function auto_zise_popup_pendinetes() {
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


   
    $('#contenido_label').css("height", (document.getElementById("Buttonbuscar").clientHeight + 5) + "px");
    $('#contenido_botonoes').css("height", (document.getElementById("Buttonbuscar").clientHeight + 5) + "px");
    var total = document.getElementById("contenido_label").clientHeight + document.getElementById("contenido_botonoes").clientHeight;
    $('#Lista').css("height", ((espacio_iframe - 5) - (total + 10)) + "px");
    var gridwith = document.getElementById("Lista").clientWidth;
    var gridheihg = document.getElementById("Lista").clientHeight - 3;
        //LLAMA PLUGIN FIJA HIDER O TITULOS   
    if ($('#GridViewlista td').children.length > 0 && $('#GridViewlista tr:visible').length > 0) {
        $(document).ready(function () { $('#GridViewlista').gridviewScroll({ width: gridwith, height: (gridheihg) }); })
    }

}
        catch (err) {
    alert(err.message + " funcion auto_zise_popup_pendinetes " + err.message);
}
}
function activa_busqueda() {
    try {
       
        
        busqueda_gred('HiddenSeleccion', 'GridViewlista', 'contenidobusqueda', 'CheckboxBusqueda');
        
    }
    catch (err) {
        alert(err.message + " funcion activa_busqueda " + err.message);
    }
}
function busqueda_gred(HiddenSeleccion, data_grid, contenido_busqueda, CheckboxBusqueda) {
    try {
        if ($("#" + contenido_busqueda).val() == "") {
            $("#" + data_grid + " tr[id]").css({ "background-color": "transparent", "color": "Black" });
            $("#" + HiddenSeleccion).val("-1");
            return false;
        }
        $("#" + HiddenSeleccion).val("-1");
        var refgrid;
        var filtro;
        $("#" + data_grid + " tr[id]").css({ "background-color": "transparent", "color": "Black" });
        var s = $("#" + contenido_busqueda).val().toLowerCase();
        var grid = $("#" + data_grid);
        var confirma_ok = "0";
        var cel_indes = 0;
        $("#" + data_grid + " tr:has(td)").each(function () {
            cel_indes = cel_indes + 1;
            var rowtd = $(this);
            var scrollableDiv = grid.parent();
            $(this).children("td").each(function (idex) {
                var tempotd = $(this).text().toLowerCase()
                var check = document.getElementById(CheckboxBusqueda).checked;
                if (check == true) {
                    if (idex >= 0) {
                        if (s == tempotd) {
                            $(this).parent().css({ "background-color": "LightSkyBlue", "color": "green" });
                            //var id_ref = $(this).parent();
                            //confirma_ok = $(id_ref).offset().top;
                            var id_ref = $(this).parent();
                            if (cel_indes == 2) {
                                $(scrollableDiv).scrollTop(($(id_ref).offset().top - id_ref[0].offsetHeight));
                            }
                            if (cel_indes !== 2) {
                                $(scrollableDiv).scrollTop(rowtd[0].offsetTop - id_ref[0].offsetHeight);
                            }


                        }
                    }
                }

                if (check == false) {
                    if (idex >= 0) {
                        var compare = tempotd;
                        var strcompre = compare.indexOf(s);
                        if (strcompre >= 0) {
                            $(this).parent().css({ "background-color": "LightSkyBlue", "color": "green" });
                            //var id_ref = $(this).parent();
                            //confirma_ok = $(id_ref).offset().top;
                            var id_ref = $(this).parent();
                            if (cel_indes == 2) {
                                $(scrollableDiv).scrollTop(($(id_ref).offset().top - id_ref[0].offsetHeight));
                            }
                            if (cel_indes !== 2) {
                                $(scrollableDiv).scrollTop(rowtd[0].offsetTop - id_ref[0].offsetHeight);
                            }


                        }
                    }
                }


            })

        });
        
    }
    catch (err) {
        alert(err.message + " funcion busqueda_gred " + err.message);
    }
}
function busqueda_gred1(HiddenSeleccion, data_grid, contenido_busqueda, CheckboxBusqueda) {
    if ($("#" + contenido_busqueda).val() == "") {
        return false;
    }
    $("#" + HiddenSeleccion).val("-1");
    var refgrid;
    var filtro;
    $("#" + data_grid + " tr[id]").css({ "background-color": "transparent", "color": "Black" });
    var s = $("#" + contenido_busqueda).val().toLowerCase();
    var grid = $("#" + data_grid);

    $("#" + data_grid + " tr:has(td)").each(function () {
        var scrollableDiv = grid.parent();
        $(this).children("td").each(function (idex) {
            var tempotd = $(this).text().toLowerCase()
            var check = document.getElementById(CheckboxBusqueda).checked;
            if (check == true) {

                if (idex >= 0) {
                    if (s == tempotd) {
                        $(this).parent().css({ "background-color": "LightSkyBlue", "color": "orange" });
                        $(scrollableDiv).scrollTop(70);
                        var id_ref = $(this).parent();
                        $(scrollableDiv).scrollTop($(id_ref).offset().top);

                    }
                }
            }

            if (check == false) {
                if (idex >= 0) {
                    var compare = tempotd;
                    var strcompre = compare.indexOf(s);
                    if (strcompre >= 0) {
                        $(this).parent().css({ "background-color": "LightSkyBlue", "color": "orange" });
                        $(scrollableDiv).scrollTop(70);
                        var id_ref = $(this).parent();
                        $(scrollableDiv).scrollTop($(id_ref).offset().top);

                    }
                }
            }


        })
    });

}

function filtro_gred(HiddenSeleccion, data_grid, contenido_busqueda, CheckboxBusqueda) {
    $("#" + HiddenSeleccion).val("-1");
    var refgrid;
    var filtro;
    var ito = 0;
    var confirma_hidem_fila = 0;
    var showtr;
    $("#" + data_grid + " tr[id]").css({ "background-color": "transparent", "color": "Black" });
    var s = $("#" + contenido_busqueda).val().toLowerCase();
    var grid = $("#" + data_grid);
    $("#" + data_grid + " tr:hidden").show();
    //$('#data_grid_auxiliar_listaHeader').hide();
    $("#" + data_grid + " tr:has(td)").each(function () {

        var refdif = $(this);
        var confirm = -1;
        $(this).children("td").each(function (idex) {

            var tempotd = $(this).text().toLowerCase()
            var check = document.getElementById(CheckboxBusqueda).checked;
            if (check == true) {

                if (idex >= 0) {
                    if (s == tempotd) {
                        (this).parent().show();


                        confirm = 1;
                    } else {


                    }
                }
            }

            if (check == false) {
                if (idex >= 0) {
                    var compare = tempotd;
                    var strcompre = compare.indexOf(s);
                    if (strcompre >= 0) {
                        refdif.show();

                        confirm = 1;
                    } else {

                    }
                }
            }

        })
        ito++;
        if (confirm == -1 && ito != 1) {
            refdif.hide();
            $("#" + data_grid).append(refdif.clone());
            refdif.remove();
        }
        if (confirm == -1 && ito == 1) {
            refdif.hide();
            $("#" + data_grid).append(refdif.clone());
            refdif.remove();
        }
    });



}