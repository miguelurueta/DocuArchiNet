// Archivo JScript

var NAV=-1;
function ConfirmationWindow()
{

var result = confirm("Are you sure"); 

if(result)
{ 

alert('You selected YES'); 

}
else
{

alert('You selected NO'); 

}

}
function A(e)
{
var i=true;
var url;
if(NAV==0 || NAV==1)
 {
 if(e.target.nodeName!="IMG") {return i;}
 url=e.target.alt;
 i=false;
 }
else
 {
 e=window.event;
 if(e.srcElement.tagName!="IMG") {return i;}
 url=e.srcElement.alt;
 i=false;
 }
if(!i) {alert("DocuArchi-FreeImage"); document.getElementById("TextBox5").focus();}
return i;
}

function Menu()
{

if(document.layers) {NAV=1;}
else if(document.all) {NAV=2;}
else {NAV=0;}
switch(NAV)
 {
 case 0: {document.captureEvents(Event.CONTEXTMENU);document.oncontextmenu=A;} 
     break;
 case 1: {document.captureEvents(Event.MOUSEDOWN);document.onmousedown=A;} 
     break;
 case 2: {document.onmousedown=A;} 
     break;
 }
 
  }
//funcion captura eventos para validacion de caracteres
  function valida()
  { 
  var NAV2=-1;
     if(document.layers) {NAV2=1;}
          else if(document.all) {NAV2=2;}
          else {NAV2=0;}
          switch(NAV2)
 {
          case 0: {document.captureEvents(Event.CONTEXTMENU);document.oncontextmenu=validacar;} 
           break;
         case 1: {document.captureEvents(Event.MOUSEDOWN);document.onmousedown=validacar;} 
           break;
         case 2: {document.onmousedown=validacar;} 
          break;
  } 
   }
  //funcion para la validacion de caracteres
   function validacar(){
    var salida = ""
    var caja = document.getElementById("TextBox5");
    if(caja.value == ""){
        caja.value = "";
        return "";
    }
    if(caja.value == "0"){
        caja.value = "1";
        return "";
    }
    var dias = ["1", "2", "3", "4", "5" , "6", "7" , "8" , "9" , "0"];
    var len = caja.value.length;
    for (var e=0; e<len; e++ ){
      var text=caja.value.charAt(e); 
        for(var i=0; i<10; i++) {
         
         if(text == dias[i])
         { 
                 
                 salida=salida.concat(text);
         }
      }
    }
    
    caja.value=salida;
    caja.focus;
    return "";
  }
  
  //funcion para imprimir
function IMPRI(e)
{
var i=true;
var url;
if(NAV==0 || NAV==1)
 {
 if(e.target.nodeName!="IMG") {return i;}
 url=e.target.alt;
 i=false;
 }
else
 {
 e=window.event;
 if(e.srcElement.tagName!="IMG") {return i;}
 url=e.srcElement.alt;
 i=false;
 }
if(!i) {print();}
return i;
}

function MenuImpri()
{

if(document.layers) {NAV=1;}
else if(document.all) {NAV=2;}
else {NAV=0;}
switch(NAV)
 {
 case 0: {document.captureEvents(Event.CONTEXTMENU);document.oncontextmenu=IMPRI;} 
     break;
 case 1: {document.captureEvents(Event.MOUSEDOWN);document.onmousedown=IMPRI;} 
     break;
 case 2: {document.onmousedown=IMPRI;} 
     break;
 }
 
 
 }
 
 function diligencia(caracteristica)

{

 window.open("image.aspx?caracteristica="+caracteristica,'mensajes',"width=350px,height=200px,toolbar=no,location=no,directories=no,status=no,menubar=no,scrollbars=no,resizable=yes");
//docprint.document.open();
}

///funcion para llamar ventana emergente
function presionBoton(ident)
{
  var semilla =" ";
  var datos=ident.split("-");
  var arc1 = datos[0];
  var cony;
  var tempo;
  
  for (i=0;i<document.forms[0].length;i++)
    {
        //document.forms[i].name
        cony = cony + "-" + document.forms[0].elements[i].id;
      if  (document.forms[0].elements[i].name == datos[0]) {

          // alert(arc1);
          //"<%=ModalPopupTexto.ClientID %>"
          if (document.forms[0].elements[i].value != "") {
              tempo = datos[0] + "|" + document.forms[0].elements[i].value;
              
              var valor = 'hdnConsult'
              //$find('hdnConsult').value();
              for (i2 = 0; i2 < document.forms[0].length; i2++) {
                  if (document.forms[0].elements[i2].name == valor) {
                      //$find('Labelconsulta').text = tempo;
                      document.forms[0].elements[i2].value = tempo;
                      //$find('ModalPopupTexto').show();
                      __doPostBack('Button5', '');
                     
                      
                      
                } 
                 
              }
            
          }
      }

     
   
  }
  //alert(cony);
}
////FUNCION ASIGNACION DE DATOS A CONSULTA
function asignadatos(ident)
{
       var datos=ident.split("|");
       var caja = document.getElementById("Droupdatos");

       for (i = 0; i < document.forms[0].length; i++)
       {

          if (document.forms[0].elements[i].name == datos[0])
         {
             document.forms[0].elements[i].value = caja.value;
             document.forms[0].elements[i].focus();
         }
       }
      
     //window.close();
  
 }
 
  function MensajeError(ident)
{
   
   if (ident!="" )
   {
    alert(ident);
   document.captureEvents(Event.MOUSEDOWN);
   document.onmousedown=false
  
     
     
     
   }
}

  function MensajeErrorLogin(ident)
{
   
   if (ident!="" )
   {
     alert(ident);
     document.captureEvents(Event.MOUSEDOWN);
     document.onmousedown=false
     document.getElementById("TextBox2").focus();
   }
}


 function alerta(numero) {
            alert(' ' + numero);
        }

//Funcncion confirmacion
 
(function ($) {
   
    $.fn.format_date= function()
    {
        
        $(this.get(0)).on('keydown', function (e) {
        
            if (e.which == 9) {
               
                var salidadato;
                
                    var dato = e.srcElement.value;

                    if (dato != "") {

                       
                   

                    if (salidadato == "Formato fecha no cumple") {
                        alert(salidadato);
                        e.preventDefault();
                        return false;
                    }
                    var BisestA;
                    var Año_F, Mes_f, Dia_f, tip;
                    var numerocaracter = dato.length;
                    if (numerocaracter == 10 || numerocaracter == 8) {

                    }
                    else {
                        alert("Formato fecha no cumple demaciados caracteres");
                        e.srcElement.value="";
                        e.preventDefault();
                        return false;
                    }

                    if (numerocaracter == 10) {

                        Año_F = dato.substring(0, 4);
                        Mes_f = dato.substring(0, 7);
                        Mes_f = Mes_f.substring(7, 5);
                        Dia_f = dato.substring(8, 10);
                    }
                    else {
                        Año_F = dato.substring(0, 4);
                        Mes_f = dato.substring(0, 6);
                        Mes_f = Mes_f.substring(6, 4);
                        Dia_f = dato.substring(6, 8);
                    }

                    //Verifica el formato del dia
                    if (Dia_f > 31 || Dia_f == 0) {

                        alert("ED_" + Año_F + "" + Mes_f + "(" + Dia_f + ")");
                        e.preventDefault();
                        return false;
                    }

                    //verifica el formato del mes
                    if (Mes_f > 12 || Mes_f < 1) {
                        alert("EM_" + Año_F + "(" + Mes_f + ")" + Dia_f);
                        e.preventDefault();
                        return false;
                    }

                    switch (Mes_f) {
                        case "01":
                            if (Dia_f > 31) {
                                alert("ED_" + Año_F + "" + Mes_f + "(" + Dia_f + ")");
                                e.preventDefault();
                            }
                            break;

                        case "02":
                            if (Dia_f % 4 == 0) {

                                BisestA = 29;
                            }
                            else {
                                BisestA = 28;
                            }
                            if (Dia_f > BisestA) {
                                alert("ED_" + Año_F + "" + Mes_f + "(" + Dia_f + ")");
                                e.preventDefault();
                            }
                            break;
                        case "03":
                            if (Dia_f > 31) {
                                alert("ED_" + Año_F + "" + Mes_f + "(" + Dia_f + ")");
                                e.preventDefault();
                            }
                            break;

                        case "04":
                            if (Dia_f > 30) {
                                alert("ED_" + Año_F + "" + Mes_f + "(" + Dia_f + ")");
                                e.preventDefault();
                            }
                            break;

                        case "05":
                            if (Dia_f > 31) {
                                alert("ED_" + Año_F + "" + Mes_f + "(" + Dia_f + ")");
                                e.preventDefault();
                            }
                            break;

                        case "06":
                            if (Dia_f > 30) {
                                alert("ED_" + Año_F + "" + Mes_f + "(" + Dia_f + ")");
                                e.preventDefault();
                            }
                            break;

                        case "07":
                            if (Dia_f > 31) {
                                alert("ED_" + Año_F + "" + Mes_f + "(" + Dia_f + ")");
                                e.preventDefault();
                            }
                            break;

                        case "08":
                            if (Dia_f > 31) {
                                alert("ED_" + Año_F + "" + Mes_f + "(" + Dia_f + ")");
                                e.preventDefault();
                            }
                            break;

                        case "09":
                            if (Dia_f > 30) {
                                alert("ED_" + Año_F + "" + Mes_f + "(" + Dia_f + ")");
                                e.preventDefault();
                            }
                            break;

                        case "10":
                            if (Dia_f > 31) {
                                alert("ED_" + Año_F + "" + Mes_f + "(" + Dia_f + ")");
                                e.preventDefault();
                            }
                            break;

                        case "11":
                            if (Dia_f > 30) {
                                alert("ED_" + Año_F + "" + Mes_f + "(" + Dia_f + ")");
                                e.preventDefault();
                            }
                            break;

                        case "12":
                            if (Dia_f > 31) {
                                alert("ED_" + Año_F + "" + Mes_f + "(" + Dia_f + ")");
                                e.preventDefault();
                            }
                            break;
                    }

                    if (numerocaracter == 8) {
                        salidadato = Año_F + "/" + Mes_f + "/" + Dia_f;
                        e.srcElement.value = salidadato;
                    }

                    if (numerocaracter == 10) {
                        salidadato = Año_F + "/" + Mes_f + "/" + Dia_f;
                        e.srcElement.value = salidadato;
                    }

                    }
            }
        });


    };
})(jQuery);

