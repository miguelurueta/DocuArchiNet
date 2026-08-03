var _iLeft, _iTop, _iRight, _iBottom; //These variables are used to remember the selected area
var _dwtParam = {
    'productKey': 'C98699013E55B9B40151CD473F390B6B6EBDCFB7665CABD954CEC5BF857B4C47CAD263C66696FE277EB945FA828D845B2CE410BAA0BA9CF62957FF51BA66CADFFCA4F8749BF437B1BE50AFECD538E0F519EDA0D3D877A93AE3717997B9568F0BB52DC7F6533683FC0CA70ECE',
    'containerID': 'dwtcontrolContainer',   //The ID of Dynamic Web TWAIN control div in HTML.This value is required.
    /*
    'isTrial': 'true',  
    isTrial is used to judge whether Dynamic Web TWAIN control is trial or full. This value is optional.
    The default value is 'TRUE', which means the control is a trial one. The value of isTrial is 'TRUE' or 'FALSE'.
    */

    /*
    'version': '9,2',   
    The version of Dynamic Web TWAIN control, which is used to judge the version when downloading CAB.
    This value is optional. The default value is '9,2'.
    */                      
    
    /*
    'resourcesPath': 'Resources',   
    The relative path of MSI, CAB and PKG.
    This value is optional. The default value is 'Resources'.
    */    
   
    /*
    'width': 580,       //The width of Dynamic Web TWAIN control
     This value is optional. The default value is '580'.
     */  
       
    /*
    'height': 600       //The height of  Dynamic Web TWAIN control
    This value is optional. The default value is '600'.
     */   
      
     
    /*  These are events. The name of 'OnPostAllTransfer' shouldn't be changed, but the name of 'Dynamsoft_OnPostAllTransfers' can be modified. 
        Please pay attention, the name of 'Dynamsoft_OnPostAllTransfers' and 'function Dynamsoft_OnPostAllTransfers()' event must be coincident.
        
        Events are as follows. You can choose one or many according to you needs.
        Once an event is added, you must make sure the corresponding function is defined in your code.
        
        'onPostTransfer':Dynamsoft_OnPostTransfer,
        'onPostAllTransfers':Dynamsoft_OnPostAllTransfers,  
        'onMouseClick':Dynamsoft_OnMouseClick,   
        'onPostLoad':Dynamsoft_OnPostLoadfunction,    
        'onImageAreaSelected':Dynamsoft_OnImageAreaSelected,   
        'onMouseDoubleClick':Dynamsoft_OnMouseDoubleClick,   
        'onMouseRightClick':Dynamsoft_OnMouseRightClick,   
        'onTopImageInTheViewChanged':Dynamsoft_OnTopImageInTheViewChanged,   
        'onImageAreaDeSelected':Dynamsoft_OnImageAreaDeselected,    
        'onGetFilePath':Dynamsoft_OnGetFilePath  
    */
    'onPostTransfer': Dynamsoft_OnPostTransfer,
    'onTopImageInTheViewChanged':Dynamsoft_OnTopImageInTheViewChanged,
    'onImageAreaSelected':Dynamsoft_OnImageAreaSelected,
    'onImageAreaDeSelected':Dynamsoft_OnImageAreaDeselected                                                        
};


var gWebTwain;
(function() {
	gWebTwain = new Dynamsoft.WebTwain(_dwtParam);
})();

var seed;
function onPageLoad() {
    initInfo();            //Add guide info

    _iLeft = 0;
    _iTop = 0;
    _iRight = 0;
    _iBottom = 0;

    var varInterpolationMethod = document.getElementById("InterpolationMethod");
    if (varInterpolationMethod) {
        varInterpolationMethod.options.length = 0;
        varInterpolationMethod.options.add(new Option("NearestNeighbor", 1));
        varInterpolationMethod.options.add(new Option("Bilinear", 2));
        varInterpolationMethod.options.add(new Option("Bicubic", 3));
    }

    seed = setInterval(initControl, 500);
}


function initControl() {
    var DWObject = gWebTwain.getInstance();
    if (DWObject) {
        if (DWObject.ErrorCode == 0) {
            clearInterval(seed);
            DWObject.BrokerProcessType = 1;
        }
    }

   
    if (DWObject) {
        if (DWObject.ErrorCode == 0) {
            clearInterval(seed);
            DWObject.BrokerProcessType = 1;

            var vDWTSource = document.getElementById("source");
            if (vDWTSource) {
                vDWTSource.options.length = 0;
                // fill in the source items.
                for (var i = 0; i < DWObject.SourceCount; i++) {
                    vDWTSource.options.add(new Option(DWObject.GetSourceNameItems(i), i));
                }

                if (DWObject.SourceCount > 0) {
                    source_onchange();
                }
            }

            // Fill the init data for preview mode selection
            var vResolution = document.getElementById("Resolution");
            if (vResolution) {
                vResolution.options.length = 0;
                vResolution.options.add(new Option("100", 100));
                vResolution.options.add(new Option("150", 150));
                vResolution.options.add(new Option("200", 200));
                vResolution.options.add(new Option("300", 300));
                vResolution[2].selected = true;
            }

            var vGray = document.getElementById("BW");
            if (vGray)
                vGray.checked = true;
            var aDF = document.getElementById("ADF");
            if (aDF)
                aDF.checked = true;

            
        }
    }
}

function source_onchange() {
    var DWObject = gWebTwain.getInstance();
    if (DWObject) {
        var vDWTSource = document.getElementById("source");
        if (vDWTSource) {

            if (vDWTSource)
                DWObject.SelectSourceByIndex(vDWTSource.selectedIndex);
            else
                DWObject.SelectSource();
        }

        DWObject.CloseSource();
    }
}
/*
function acquireImage() {
    var DWObject = gWebTwain.getInstance();
    if (DWObject) {
        if (DWObject.SourceCount > 0) {
            DWObject.SelectSource();
            DWObject.AcquireImage();
        }
        else
            alert("No TWAIN compatible drivers detected.");
    }
}*/
var iDocumentCounter = 0;
var ControBufer = 0;
function mas_view_image() {
    try {
        var DWObject = gWebTwain.getInstance();
        if (DWObject) {
            //DWObject.SetViewMode(-1, -1);
            //DWObject.EnableInteractiveZoom = true;
            //alert(DWObject.Zoom);
       
            //DWObject.Zoom = DWObject.Zoom * 1.1;
            //DWObject.Zoom = DWObject.Zoom * 0.9;
            DWObject.Zoom = DWObject.Zoom + 0.11111111111;
            //alert(DWObject.Zoom);
            //if (DW_CheckErrorString()) {
             //   return;
            //}
        }
    }
    catch (err) {
        //alert(err.message + " ConfirmMensajeGeneral");
    }
}

function menos_view_image() {
    try {
    var DWObject = gWebTwain.getInstance();
    if (DWObject) {
       // DWObject.SetViewMode(-1, -1);
       // DWObject.EnableInteractiveZoom = true;
        //alert(DWObject.Zoom);

        //DWObject.Zoom = DWObject.Zoom * 1.1;
        //DWObject.Zoom = DWObject.Zoom * 0.9;
        DWObject.Zoom = DWObject.Zoom - 0.11111111111;
        //alert(DWObject.Zoom);
        //if (DW_CheckErrorString()) {
        //    return;
        //}
    }
}
    catch (err) {
        //alert(err.message + " ConfirmMensajeGeneral");
}
}
function todas_view_image() {
    var DWObject = gWebTwain.getInstance();
    if (DWObject) {
        DWObject.SetViewMode(1, 1);
        DWObject.EnableInteractiveZoom = false;
        
    }
}
function NextImage_Image() {
    var DWObject = gWebTwain.getInstance();
    if (DWObject.HowManyImagesInBuffer == 0) 
        alert("There is no image in buffer");
    else
          //var ideximage = DWObject.CurrentImageIndexInBuffer + 1;
          DWObject.CurrentImageIndexInBuffer =
          DWObject.CurrentImageIndexInBuffer + 1;
          document.getElementById('Paginador').innerHTML = (DWObject.CurrentImageIndexInBuffer+1) + " de ";
        
    
  
}

function LasImage_Image() {
    var DWObject = gWebTwain.getInstance();
    if(DWObject.HowManyImagesInBuffer == 0)
    alert("There is no image in buffer");
    else
        
        //var ideximage = DWObject.CurrentImageIndexInBuffer - 1;
        DWObject.CurrentImageIndexInBuffer =
        DWObject.CurrentImageIndexInBuffer - 1;
        document.getElementById('Paginador').innerHTML = (DWObject.CurrentImageIndexInBuffer+1) + " de ";
}
function acquireImage() {
    iDocumentCounter = 0;
    ControBufer = 0;
    var DWObject = gWebTwain.getInstance();
    if (DWObject) {
        document.getElementById('Paginador').innerHTML = "";
        var zonn = 0.22222226858139038;
        DWObject.SetViewMode(-1, -1);
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
                window.parent.document.getElementById('ButtonEliminarArchivos').click();
            }

            DWObject.AcquireImage();
           
        }
        else
            alert("No TWAIN compatible drivers detected.");
    }
}
function addacquireImage() {
    var DWObject = gWebTwain.getInstance();
    if (DWObject) {
        //DWObject.SetViewMode(-1,- 1);
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
            DWObject.IfShowUI = document.getElementById("ShowUI").checked;
            var i;
            for (i = 0; i < 3; i++) {
                if (document.getElementsByName("PixelType").item(i).checked == true)
                    DWObject.PixelType = i;
            }
            DWObject.Resolution = Resolution.value;
            DWObject.IfFeederEnabled = document.getElementById("ADF").checked;
            DWObject.IfDuplexEnabled = document.getElementById("Duplex").checked;
            DWObject.IfDisableSourceAfterAcquire = true;
            DWObject.AcquireImage();
        }
        else
            alert("No TWAIN compatible drivers detected.");
    }
}
function Guardar_Documento() {
    try {

        var doc;
        var exte;
        var DWObject = gWebTwain.getInstance();
        if (DWObject) {
            if (DWObject.HowManyImagesInBuffer = !0) {

                for (var i = 0; i < 900; i++){
                if (i == 0) {

                    var setval = window.parent.document.getElementById("HiddenIdFlujo").value;
                    var spl = setval.split("|");
                    var numer = spl[0];
                    var rut = window.parent.document.getElementById("HiddenRuta").value;
                    doc = Ceros_Doc(numer.toString());
                    DWObject.SaveAsTIFF(rut + "/" + doc + ".TIF", i)

                }
                else {
                    var setval = window.parent.document.getElementById("HiddenIdFlujo").value;
                    var spl = setval.split("|");
                    var numer = spl[0];
                    doc = Ceros_Doc(numer.toString());
                    var rut = window.parent.document.getElementById("HiddenRuta").value;
                    var reconteo = i;
                    exte = cero_exten(reconteo.toString());
                    DWObject.SaveAsTIFF(rut + "/" + doc + "." + exte, i)

                }
                }
            }


            //bot.click();
        }
        if (DWObject.HowManyImagesInBuffer = !0) {
        DWObject.RemoveAllImages();
        }
        document.getElementById('Contador').innerHTML = "";
        document.getElementById('Paginador').innerHTML = "";
        window.parent.document.getElementById('ButtonAlmacenar').click(); 
    }
    catch (err) {
        alert(err.toString());
    }

}
function Guardar_Documento_file(i,indexbufer) {
    try {
  
    var doc;
    var exte;
    //var conteo;
    //conteo = 0;
    //var ideximage = 1;
    var DWObject = gWebTwain.getInstance();
    
        if (DWObject) {
          if (DWObject.HowManyImagesInBuffer !== 0) {
            

                if (i== 0)
                {
                   
                    var setval = window.parent.document.getElementById("HiddenIdFlujo").value;
                    var spl = setval.split("|");
                    var numer = spl[0];
                    var rut = window.parent.document.getElementById("HiddenRuta").value;
                    doc = Ceros_Doc(numer.toString());
                    DWObject.IfShowFileDialog = false;
                    //DWObject.SaveAsTIFF(rut + "/" + doc + ".TIF", indexbufer)
                    alert(rut + "/" + doc + ".TIF");
                   
                }
              else {
                    var setval = window.parent.document.getElementById("HiddenIdFlujo").value;
                    var spl = setval.split("|");
                    var numer = spl[0];
                    doc = Ceros_Doc(numer.toString());
                    var rut = window.parent.document.getElementById("HiddenRuta").value;
                    var reconteo = i;
                    DWObject.IfShowFileDialog = false;
                    exte = cero_exten(reconteo.toString());
                    //DWObject.SaveAsTIFF(rut + "/" + doc + "." + exte, indexbufer)
                    alert(rut + "/" + doc + ".TIF");
                   
                }
            
          }
        
        
        //bot.click();
       }
           //if (DWObject.HowManyImagesInBuffer = !0) {
                  //DWObject.RemoveAllImages();
          // }
        //window.parent.document.getElementById('ButtonAlmacenar').click(); ButtonEliminarArchivos
    }
    catch(err)
           {
               alert (err.toString());
           }

}
function Dynamsoft_OnPostTransfer() {
    // Add your code here
    //var i = WebTWAIN.CurrentImageIndexInBuffer;
    var DWObject = gWebTwain.getInstance();
    document.getElementById('Contador').innerHTML = DWObject.HowManyImagesInBuffer;
    //Guardar_Documento_file(iDocumentCounter, DWObject.CurrentImageIndexInBuffer);
    //iDocumentCounter = iDocumentCounter + 1; 
}
function cero_exten(numeral)
{
    
    var kp = numeral.length;
    var ext;
    //alert(kp);
    switch (kp)
    {
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
function Ceros_Doc(numero)
{
    var n = numero.length;
    var doc;
    switch(n)
    {
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


function Dynamsoft_OnTopImageInTheViewChanged(index) {
    var DWObject = gWebTwain.getInstance();
    if (DWObject) {
        DWObject.CurrentImageIndexInBuffer = index;
    }
}

//--------------------------------------------------------------------------------------
//************************** Edit Image ******************************
//--------------------------------------------------------------------------------------
function btnShowImageEditor_onclick() {
     var DWObject = gWebTwain.getInstance();
     if (DWObject) {
         if (DWObject.HowManyImagesInBuffer == 0) {
             return;
         }
         DWObject.ShowImageEditor();
     }
}

function btnRotateRight_onclick() {
    var DWObject = gWebTwain.getInstance();
    if (DWObject) {
        if (DWObject.HowManyImagesInBuffer == 0) {
            return;
        }
        DWObject.RotateRight(DWObject.CurrentImageIndexInBuffer);
    }
}
function btnRotateLeft_onclick() {
    var DWObject = gWebTwain.getInstance();
    if (DWObject) {
        if (DWObject.HowManyImagesInBuffer == 0) {
            return;
        }
        DWObject.RotateLeft(DWObject.CurrentImageIndexInBuffer);
    }
}

function btnMirror_onclick() {
    var DWObject = gWebTwain.getInstance();
    if (DWObject) {
        if (DWObject.HowManyImagesInBuffer == 0) {
            return;
        }
        DWObject.Mirror(DWObject.CurrentImageIndexInBuffer);
    }
}
function btnFlip_onclick() {
    var DWObject = gWebTwain.getInstance();
    if (DWObject) {
        if (DWObject.HowManyImagesInBuffer == 0) {
            return;
        }
        DWObject.Flip(DWObject.CurrentImageIndexInBuffer);
    }
}


function btnCrop_onclick() {
    var DWObject = gWebTwain.getInstance();
    if (DWObject) {
        if (DWObject.HowManyImagesInBuffer == 0) {
            return;
        }
        if (_iLeft != 0 || _iTop != 0 || _iRight != 0 || _iBottom != 0) {
            DWObject.Crop(
            DWObject.CurrentImageIndexInBuffer,
            _iLeft, _iTop, _iRight, _iBottom);
        _iLeft = 0;
        _iTop = 0;
        _iRight = 0;
        _iBottom = 0;
        return;
    }
        switch (document.getElementById("Crop").style.visibility) {
            case "visible": document.getElementById("Crop").style.visibility = "hidden"; break;
            case "hidden": document.getElementById("Crop").style.visibility = "visible"; break;
            default: break;
        }
        document.getElementById("Crop").style.top = ds_gettop(document.getElementById("btnCrop")) + document.getElementById("btnCrop").offsetHeight + "px";
        document.getElementById("Crop").style.left = ds_getleft(document.getElementById("btnCrop")) - 80 + "px";
    }
}

function btnCropCancel_onclick() {
    document.getElementById("Crop").style.visibility = "hidden";
}
function btnCropOK_onclick() {
  var DWObject = gWebTwain.getInstance();
  if (DWObject) {
      var re = /^\d+$/;
      document.getElementById("img_left").className = "";
      document.getElementById("img_top").className = "";
      document.getElementById("img_right").className = "";
      document.getElementById("img_bottom").className = "";
      if (!re.test(document.getElementById("img_left").value)) {
          document.getElementById("img_left").className += " invalid";
          document.getElementById("img_left").focus();
          alert("Please input a valid left value.");
          return;
      }
      if (!re.test(document.getElementById("img_top").value)) {
          document.getElementById("img_top").className += " invalid";
          document.getElementById("img_top").focus();
          alert("Please input a valid top value.");
          return;
      }
      if (!re.test(document.getElementById("img_right").value)) {
          document.getElementById("img_right").className += " invalid";
          document.getElementById("img_right").focus();
          alert("Please input a valid right value.");
          return;
      }
      if (!re.test(document.getElementById("img_bottom").value)) {
          document.getElementById("img_bottom").className += " invalid";
          document.getElementById("img_bottom").focus();
          alert("Please input a valid bottom value.");
          return;
      }
      DWObject.Crop(
        DWObject.CurrentImageIndexInBuffer,
        document.getElementById("img_left").value,
        document.getElementById("img_top").value,
        document.getElementById("img_right").value,
        document.getElementById("img_bottom").value);
      document.getElementById("Crop").style.visibility = "hidden";
  }
}

function btnChangeImageSize_onclick() {
    var DWObject = gWebTwain.getInstance();
    if (DWObject) {
        if (DWObject.HowManyImagesInBuffer == 0) {
            return;
        }
        switch (document.getElementById("ImgSizeEditor").style.visibility) {
            case "visible": document.getElementById("ImgSizeEditor").style.visibility = "hidden"; break;
            case "hidden": document.getElementById("ImgSizeEditor").style.visibility = "visible"; break;
            default: break;
        }
        document.getElementById("ImgSizeEditor").style.top = ds_gettop(document.getElementById("btnChangeImageSize")) + document.getElementById("btnChangeImageSize").offsetHeight + "px";
        document.getElementById("ImgSizeEditor").style.left = ds_getleft(document.getElementById("btnChangeImageSize")) - 30 + "px";
    }
}

function btnCancelChange_onclick() {
    document.getElementById("ImgSizeEditor").style.visibility = "hidden";
}

function btnChangeImageSizeOK_onclick() {
  var DWObject = gWebTwain.getInstance();
  if (DWObject) {
      var re = /^\d+$/;
      document.getElementById("img_height").className = "";
      document.getElementById("img_width").className = "";
      if (!re.test(document.getElementById("img_height").value)) {
          document.getElementById("img_height").className += " invalid";
          document.getElementById("img_height").focus();
          alert("Please input a valid height.");
          return;
      }
      if (!re.test(document.getElementById("img_width").value)) {
          document.getElementById("img_width").className += " invalid";
          document.getElementById("img_width").focus();
          alert("Please input a valid width.");
          return;
      }
      
      DWObject.ChangeImageSize(
        DWObject.CurrentImageIndexInBuffer,
        document.getElementById("img_width").value,
        document.getElementById("img_height").value,
        document.getElementById("InterpolationMethod").selectedIndex + 1);
     document.getElementById("ImgSizeEditor").style.visibility = "hidden";

  }
}


//--------------------------------------------------------------------------------------
//************************** Used a lot *****************************
//--------------------------------------------------------------------------------------
function ds_getleft(el) {
    var tmp = el.offsetLeft;
    el = el.offsetParent
    while (el) {
        tmp += el.offsetLeft;
        el = el.offsetParent;
    }
    return tmp;
}
function ds_gettop(el) {
    var tmp = el.offsetTop;
    el = el.offsetParent
    while (el) {
        tmp += el.offsetTop;
        el = el.offsetParent;
    }
    return tmp;
}


//******************Instructions*******************//
function initInfo() {
    return true;
    var MessageBody = document.getElementById("divInfo");
    if (MessageBody) {
        var ObjString = "<div>";
        ObjString += "This sample demonstrates how to scan documents and edit them using Dynamic Web TWAIN.<br />";
        ObjString += "<br />";
        ObjString += "<b>Steps to try:</b><br />";
        ObjString += "1. Connect your scanner<br />";
        ObjString += "2. Click the \"Scan\" button<br />";
        ObjString += "3. Edit the image(s)<br />";
        ObjString += "<br />";
        ObjString += "<b>Note:</b>";
        ObjString += "<br />";
        ObjString += "By clicking  the \"Show Image Editor\" button, you are able to use the built-in Image Editor dialog to edit the image.";
        ObjString += "<br />";
        ObjString += "<br />";
        ObjString += "Any questions? <a target='blank' href='mailto:support@dynamsoft.com'>Please let us know!</a>";
        ObjString += "<br />";
        ObjString += "</div>";
        MessageBody.innerHTML = ObjString;
    }
}
