(function ($) {
    $.fn.Scrollable = function (options) {
        var defaults = {
            ScrollHeight: 300,
            Width: 500,
            contenido: "",
            chekd: true,
            emailid:"0"
        };
        var options = $.extend(defaults, options);
        return this.each(function () {
            
            var grid = $(this).get(0);
            if (options.contenido != "") {
                $(grid).FiltroAvanced(
                  {
                      contenido: options.contenido,
                      chekd: true
                  });
            }
            //grid.style.cssText = "height:" + options.ScrollHeight + "px;width:" + gridWidth + "px";
            var gridWidth = grid.offsetWidth;
            var gridHeight = grid.offsetHeight;
            var headerCellWidths = new Array();
            //$('#' + grid.id + ' td').css({ "white-space": "normal" });
            //$('#' + grid.id + ' th').css({ "white-space": "normal" });
           // $('#' + grid.id + ' th').css({ "width": "100%" });
          

            
            //asigna la celdas del grid
            for (var i = 0; i < grid.getElementsByTagName("TH").length; i++) {
               headerCellWidths[i] = grid.getElementsByTagName("TH")[i].offsetWidth;
            }
            grid.parentNode.appendChild(document.createElement("div"));
            var parentDiv = grid.parentNode;
            var table = document.createElement("table");
            for (i = 0; i < grid.attributes.length; i++) {
                if (grid.attributes[i].specified && grid.attributes[i].name != "id") {
                    table.setAttribute(grid.attributes[i].name, grid.attributes[i].value);
                }
            }
            
            table.style.cssText = grid.style.cssText;
            table.style.width = gridWidth + "px";
            table.appendChild(document.createElement("tbody"));
            //copia los titulos del grid
            table.getElementsByTagName("tbody")[0].appendChild(grid.getElementsByTagName("TR")[0]);
            var cells = table.getElementsByTagName("TH");
            var thgrid = grid.getElementsByTagName("TH");
            var gridRow = grid.getElementsByTagName("TR")[0];
            var celcunt = cells.length;
           





            parentDiv.removeChild(grid);

            var dummyHeader = document.createElement("div");
            dummyHeader.appendChild(table);
            parentDiv.appendChild(dummyHeader);

            if (options.Width > 0) {
                gridWidth = options.Width;
            }
            var scrolltable = 1;
            var scrollableDiv = document.createElement("div");
            if (parseInt(gridHeight) > options.ScrollHeight) {
                gridWidth = parseInt(gridWidth - 17);
                scrolltable=17
            }
            scrollableDiv.style.cssText = "overflow:auto;height:" + (options.ScrollHeight-17) + "px;width:" + gridWidth + "px";
            //table.height = options.ScrollHeight + "px";
            table.style.width = (gridWidth - scrolltable) + "px";
            //scrollableDiv.style.cssText = "overflow:auto;height:" + grid.offsetHeight + "px;width:" + grid.offsetWidth + "px";
            var ter_visible = $(".filtrar tr:visible");
            if (ter_visible != undefined) {
                if (ter_visible.length > 0) {
                    for (var i = 0; i < ter_visible[0].cells.length; i++) {
                        headerCellWidths[i] = ter_visible[0].cells[i].clientWidth;
                    }
                }
            }
            for (var i = 0; i < celcunt; i++) {
                //copia el tamaño de la celda
                var width = headerCellWidths[i];

                //cells[i].style.width = parseInt(width) + "px";
                //if (gridRow != undefined) {
                table.getElementsByTagName("TH")[i].style.width = parseInt(width + 40 ) + "px";
                //     gridRow.getElementsByTagName("TD")[i].style.width = parseInt(width) + "px";
                // }

            }
            
            scrollableDiv.appendChild(grid);
            parentDiv.appendChild(scrollableDiv);
            //Posiciona el scrool del div en la seleccion
            if (options.emailid != "0") {
               
                $(scrollableDiv).scrollTop(70);
                $(grid).find(' tr[id=' + options.emailid + ']').each(function () {
                    $(scrollableDiv).scrollTop($(this).offset().top);
                })
            }
           
           
        });
    };
   
})(jQuery);