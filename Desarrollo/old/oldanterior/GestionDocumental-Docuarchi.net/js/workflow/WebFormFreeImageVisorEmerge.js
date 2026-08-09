$(document).ready(function () {
    $.fn.inicio = function () {
        auto_zise_visor();

    }
    $("#noaming").bind("contextmenu", function (e) {
        e.preventDefault();
    });
    function auto_zise_visor() {
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


        $(document).ready(bodyResize);
        $(window).resize(bodyResize);
        function bodyResize() {


            $('#ContentGeneral').css("height", (espacio_iframe - 20) + "px");
            
            $("#tollimage").css("height", ($("#ImageButtonAnterior").height() + 5) + "px");
            
            $("#content").css("height", (espacio_iframe - 20) - ($("#tollimage").height() + 10) + "px");

        }


    }
    var left;
    var top;
    $("#draggable").css({ opacity: 0.5 });
    left = $("#draggable").position.left;
    top = $("#draggable").position.top;
    var heigimage, withimage;
    //$("#zona").size=$("#noaming").size

    $("#ImageFirma").click(function (e) {
        var elmentonoa = $("#noaming").offset();
        var tempo = $("#Hiddenintercambio2").val();
        $("#img").attr("src", tempo);
        $("#draggable").css("with", "100");
        $("#draggable").css("height", "70");
        $("#draggable").css("display", "block");
        $("#img").imageResize();
        var ofsetconten = $("#content").offset();
        //Posiciona la firma con el scroll
        //$("#content").scrollTop(elmentonoa.top);
        var contenido = $("#content");
        var scrolltop = contenido.scrollTop();
        var scrolleft = contenido.scrollLeft();
        var topconten = contenido.scrollTop();
        var lefconten = contenido.scrollLeft();
        //if (lefconten = 0) {

        //  lefconten = 20;
        //} else { lefconten = lefconten + 20 }

        if (topconten = 0) {

            topconten = 20;
        } else {
            topconten = topconten + 20;
        }

        //if (scrolltop > 30) {
        //    topconten = (ofsetconten.top + scrolltop) / 2;
        //    lefconten = (ofsetconten.left + scrolleft) / 2;
        //}
        $("#draggable").offset({ top: topconten, left: lefconten });



    });


    var sues = $('#noaming.ClientID');
    $("#draggable").draggable({
        containment: $("#zona"),
        stop: function (event, ui) {
            var elemento = $("#draggable");
            var posicion = elemento.position();
            left = posicion.left;
            top = posicion.top;
            var dragab = $("#draggable");
            var contenido = $("#content");
            var scr = contenido.scrollTop();
            var scrolleft = contenido.scrollLeft();
            left = (scrolleft) + left;
            //var posicfinal = (top + scr) - (im.height() / 2);
            var posicfinal = (top + scr) - 10;
            $("#Hiddenintercambio").val(top + "-" + left + "-" + dragab.height() + "-" + dragab.width() + "-" + dragab.height() + "-" + scr + "-" + posicfinal + "-" + heigimage + "-" + withimage);

        }


    }
    );

    $("#draggable").resizable({
        maxHeight: 80, maxWidth: 100, minWidth: 50, minHeight: 50,
        start: function (event, ui) {
            //$("#draggable").offset({ top: top, left: left });
        },
        stop: function (event, ui) {
            var conta = $("#draggable");
            //$("#img").imageResize();
            var im = $("#img");
            $("#draggable").css('position', 'relative');
            var dragab = $("#draggable");
            var contenido = $("#content");
            var scr = contenido.scrollTop();
            var scrolleft = contenido.scrollLeft();
            left = left - scrolleft;
            var posicfinal = (top + scr) - 10;
            //$("#draggable").offset({ top: top, left: left });
            $("#Hiddenintercambio").val(top + "-" + left + "-" + dragab.height() + "-" + dragab.width() + "-" + dragab.height() + "-" + scr + "-" + posicfinal);

        },
        resize: function (event, ui) {

            $("#img").imageResize();
            var contenido = $("#content");
            var scroltop = contenido.scrollTop();
            var scroleft = contenido.scrollLeft();
            $("#draggable").offset({ top: top, left: left - scroleft });

        }
    }
    );

    $('#draggable').contextMenu('context-menu-1', {
        'Guardar': {
            click: function (element) {  // element is the jquery obj clicked on when context menu launched
                __doPostBack('#ImageButtonguardar.ClientID>', 'to');
                $("#draggable").css("display", "none");
            }
        },
        'Limpiar': {
            click: function (element) {  // element is the jquery obj clicked on when context menu launched

                $("#draggable").css("display", "none");
            }
        },
        'Cancelar': {
            click: function (element) {  // element is the jquery obj clicked on when context menu launched

                //$(element).css("display", "none");
            }
        }
    }


         );
    $.fn.firmavisible = function () {
        // $('#Image1').ccs("Dispaly", "block");
        //$('#Image1').style.display = 'block';

    };

    
})
function progres_hiden(progres) {
    try {
        $("#progres_bar").css("display", "none");
    }
    catch (err) {
        alert(err.message + " Funcion progres_hiden");
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