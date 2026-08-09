(function ($) {
    $.fn.Buscar = function (options) {
        var defaults = {
            contenido: "",
            chekd: true,
            id_selecion:"0"
            
        };
        var options = $.extend(defaults, options);
        return this.each(function () {
            var grid = $(this).get(0);
            var s = options.contenido.toLowerCase();
            // Limpia la seleccion anterior    
            $('#' + grid.id + ' tr[id!=' + options.id_selecion + ']').css({ "background-color": "White", "color": "Black" });
            $(grid).parent().scrollTop(50);
            $(grid).find('tr').each(function () {
                $(this).children("td").each(function (idex) {
                    var tempotd = $(this).text().toLowerCase();                
                    if (options.chekd == true) {
                        if (idex >= 0) {
                            if (s == tempotd) {
                                $(this).parent().css({ "background-color": "green", "color": "White" });
                                $(grid).parent().scrollTop($(this).parent().offset().top);
                                return false;
                            }
                        }
                    }

                    if (options.chekd == false) {
                        if (idex >= 0 && s!="") {
                            var compare = tempotd;
                            var strcompre = compare.indexOf(s);
                            if (strcompre >= 0) {
                                $(this).parent().css({ "background-color": "green", "color": "White" });
                                $(grid).parent().scrollTop($(this).parent().offset().top);
                                return false;
                            }
                        }
                    }


                })
            })
        }
        )
    }

    $.fn.posicion_scroll = function (options) {
        var defaults = {     
            id_selecion: "0"
        };
        var options = $.extend(defaults, options);
       
        return this.each(function () {
            var grid = $(this).get(0);
            if (options.id_selecion !="0"){
            
                $(grid).parent().scrollTop(50); 
            }
            $(grid).parent().scrollTop($('#' + grid.id + ' tr[id!=' + options.id_selecion + ']').offset.top);
        })
    }

    $.fn.FiltroAvanced=function (options)
    {
        var defaults = {
            contenido: "",
            chekd: true,

        };
        var options = $.extend(defaults, options);
        return this.each(function () {
           
            var grid = $(this).get(0);
            var s = options.contenido.toLowerCase();
            //Vergica que el valor a filtrar se encuentre en la tabla
            //var compare = tempotd;
            //var strcompre = compare.indexOf(s);
            //if (strcompre >= 0) {
            var confirm = -1;
            $(grid).find('tr').each(function () {
                $(this).children("td").each(function (idex) {
                    var tempotd = $(this).text().toLowerCase();
                    if (idex >= 0) {
                        if (options.chekd === true) {
                           if (s == tempotd) {
                            confirm = 1;
                            return false;
                           }
                        } else {
                            var strcompre = tempotd.indexOf(s);
                            if (strcompre >= 0) {
                                confirm = 1;
                                return false;
                            }
                        }
                    }
                })
            })
           
            if (confirm==1){
            $(".filtrar tr:has(td)").each(function() { 
                var t = $(this).text().toLowerCase();  
                $("<td class='indexColumn'></td>") 
                .hide().text(t).appendTo(this); 
            });
            //Agregar el comportamiento al texto (se selecciona por el ID)     
            var s1 = options.contenido.toLowerCase().split(" ");
                $(".filtrar tr:hidden").show(); 
                $.each(s1, function() { 
                    $(".filtrar tr:visible .indexColumn:not(:contains('" 
                    + this + "'))").parent().hide(); 
                });  

            }

            if (confirm == -1) {
                //alert("No se encontraron concordancias para filtrar se restablece la consulta para el contenido" + options.contenido);
            }
            
        })
    }
})(jQuery);