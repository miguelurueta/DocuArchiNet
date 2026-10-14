var pdf;
var url_response = ""

const PDFJS_WORKER_SRC = "https://cdnjs.cloudflare.com/ajax/libs/pdf.js/3.4.120/pdf.worker.min.js";
if (window.pdfjsLib && window.pdfjsLib.GlobalWorkerOptions) {
    window.pdfjsLib.GlobalWorkerOptions.workerSrc = PDFJS_WORKER_SRC;
}

const viewerCapabilityPromises = Object.create(null);
const viewerCapabilities = Object.freeze({
    graph: {
        styles: [
            "../../js/jquery-ui-1.12.1.custom/jquery-ui.min.css",
            "../../js/jquery.contextMenu.css"
        ],
        scripts: [
            "../../js/jquery-ui-1.12.1.custom/jquery-ui.min.js",
            "../../js/jquery.contextMenu.js",
            { src: "https://cdnjs.cloudflare.com/ajax/libs/jqueryui-touch-punch/0.2.3/jquery.ui.touch-punch.min.js", optional: true }
        ],
        ready: function () {
            return Boolean(window.jQuery && window.jQuery.fn && window.jQuery.fn.draggable &&
                window.jQuery.fn.resizable && window.jQuery.fn.contextMenu);
        }
    },
    wacom: {
        styles: ["../wacom/demoButtons.css"],
        scripts: [
            "../wacom/q.js",
            "../wacom/BigInt.js",
            "../wacom/wgssStuSdk.js",
            "../wacom/sjcl.js",
            "../wacom/demoButtons_encryption.js",
            "../wacom/signature_sdk.js",
            "../wacom/sigCaptDialog.js",
            "../wacom/stuCaptDialog.js",
            "../wacom/wacom.js"
        ],
        ready: function () { return typeof window.captureFromSTU === "function"; }
    },
    print: {
        styles: ["../../printer_js/print.min.css"],
        scripts: ["../../printer_js/print.min.js"],
        ready: function () { return typeof window.printJS === "function"; }
    },
    fabric: {
        styles: [],
        scripts: ["https://cdnjs.cloudflare.com/ajax/libs/fabric.js/4.3.0/fabric.min.js"],
        ready: function () { return Boolean(window.fabric && window.fabric.Canvas); }
    },
    prettify: {
        styles: [],
        scripts: ["https://cdnjs.cloudflare.com/ajax/libs/prettify/r298/prettify.min.js"],
        ready: function () { return Boolean(window.PR && typeof window.PR.prettyPrint === "function"); }
    }
});

function viewerResourceUrl(value) {
    return new URL(value, document.baseURI).href;
}

function findViewerResource(selector, attribute, url) {
    const expected = viewerResourceUrl(url);
    return Array.from(document.querySelectorAll(selector)).find(function (element) {
        return viewerResourceUrl(element.getAttribute(attribute)) === expected;
    });
}

function loadViewerStyle(url) {
    const existing = findViewerResource("link[rel='stylesheet'][href]", "href", url);
    if (existing) return Promise.resolve(existing);
    return new Promise(function (resolve, reject) {
        const link = document.createElement("link");
        link.rel = "stylesheet";
        link.href = url;
        link.setAttribute("data-viewer-resource", url);
        link.onload = function () { resolve(link); };
        link.onerror = function () { link.remove(); reject(new Error("VIEWER_STYLE_LOAD_FAILED")); };
        document.head.appendChild(link);
    });
}

function loadViewerScript(resource) {
    const descriptor = typeof resource === "string" ? { src: resource, optional: false } : resource;
    const existing = findViewerResource("script[src]", "src", descriptor.src);
    if (existing) return Promise.resolve(existing);
    return new Promise(function (resolve, reject) {
        const script = document.createElement("script");
        script.src = descriptor.src;
        script.async = false;
        script.setAttribute("data-viewer-resource", descriptor.src);
        script.onload = function () { resolve(script); };
        script.onerror = function () {
            script.remove();
            if (descriptor.optional) resolve(null);
            else reject(new Error("VIEWER_SCRIPT_LOAD_FAILED"));
        };
        document.head.appendChild(script);
    });
}

function loadViewerCapability(name) {
    const capability = viewerCapabilities[name];
    if (!capability) return Promise.reject(new Error("VIEWER_CAPABILITY_UNKNOWN"));
    if (capability.ready()) return Promise.resolve(name);
    if (viewerCapabilityPromises[name]) return viewerCapabilityPromises[name];

    viewerCapabilityPromises[name] = Promise.all(capability.styles.map(loadViewerStyle))
        .then(function () {
            return capability.scripts.reduce(function (promise, resource) {
                return promise.then(function () { return loadViewerScript(resource); });
            }, Promise.resolve());
        })
        .then(function () {
            if (!capability.ready()) throw new Error("VIEWER_CAPABILITY_SYMBOL_MISSING");
            return name;
        })
        .catch(function (error) {
            delete viewerCapabilityPromises[name];
            throw error;
        });
    return viewerCapabilityPromises[name];
}

function optionalCapabilityControl(event) {
    const target = event && event.currentTarget;
    return target && typeof target.closest === "function" ? target.closest("button") : null;
}

function reportOptionalCapabilityError(label) {
    const message = "No fue posible cargar " + label + ". Intente nuevamente.";
    if (pdfViewer && typeof pdfViewer.PdfViwerAlert === "function") {
        pdfViewer.PdfViwerAlert(message, "warning", "container-general");
    } else {
        console.warn(message);
    }
}

async function runOptionalCapability(event, name, label, action) {
    const control = optionalCapabilityControl(event);
    if (control) control.disabled = true;
    try {
        await loadViewerCapability(name);
        return await action();
    } catch (error) {
        reportOptionalCapabilityError(label);
        return false;
    } finally {
        if (control) control.disabled = false;
    }
}

window.viewerCapabilities = viewerCapabilities;
window.loadViewerCapability = loadViewerCapability;

function buildAuthorizedPdfUrl(urlParams) {
    const handlerPath = urlParams.get("ash");
    const documentReference = urlParams.get("doc_ref");
    if (!handlerPath || !documentReference) return null;
    const separator = handlerPath.indexOf("?") >= 0 ? "&" : "?";
    return handlerPath + separator + "doc_ref=" + encodeURIComponent(documentReference);
}

function reportDocumentLoadError() {
    const message = "No fue posible abrir el documento autorizado.";
    if (typeof pdfViewer !== "undefined" && pdfViewer && typeof pdfViewer.PdfViwerAlert === "function") {
        pdfViewer.PdfViwerAlert(message, "warning", "container-general");
    } else {
        console.warn(message);
    }
}

function normalizePdfLoadingMode(value) {
    return value === "range" ? "range" : "stream";
}

window.buildAuthorizedPdfUrl = buildAuthorizedPdfUrl;
function load_pdf_anotate(url_response, url_src, url_firma_) {
    pdf = new PDFAnnotate("pdf-container_", url_response, url_src, url_firma_, {
        onPageUpdated(page, oldData, newData) {
            //console.log(page, oldData, newData);
        },
        ready() {
            //console.log("Plugin initialized successfully");
        },
        scale: 1.5,
        pageImageCompression: "FAST", // FAST, MEDIUM, SLOW(Helps to control the new PDF file size)

    });
}
$(window).on("load", function () {
    window.addEventListener("resize", rezize_event);
    configToolbarPdfView();
    auto_size_pdf_anotate();
    const valores = window.location.search;
    const urlParams = new URLSearchParams(valores);
    let url_src = buildAuthorizedPdfUrl(urlParams);
    let url_src_fromat = urlParams.get("ash") + "?rut_image=" + urlParams.get("urimage_format");
    let ur_firma_user = urlParams.get("url_firma");
    let url_id_imagen = 0;
    if (urlParams.get("url_id_imagen")) { url_id_imagen = urlParams.get("url_id_imagen"); };
    let url_cabinete_imagen = "";
    if (urlParams.get("url_cabinete_imagen")) { url_cabinete_imagen = urlParams.get("url_cabinete_imagen"); };
    let url_radicado = "";
    if (urlParams.get("url_radicado")) { url_radicado = urlParams.get("url_radicado"); };
    let url_id_workflow = 0;
    if (urlParams.get("url_id_workflow")) { url_id_workflow = urlParams.get("url_id_workflow"); };
    let url_desc_transacion = "";
    if (urlParams.get("url_desc_transacion")) { url_desc_transacion = urlParams.get("url_desc_transacion"); };
    let url_printer = 0;
    if (urlParams.get("url_printer")) { url_printer = urlParams.get("url_printer"); };
    let url_save = 0;
    if (urlParams.get("url_save")) { url_save = urlParams.get("url_save"); };
    let url_add_stamp = 0;
    if (urlParams.get("url_add_stamp")) { url_add_stamp = urlParams.get("url_add_stamp"); };
    let url_add_firma = 0;
    if (urlParams.get("url_add_firma")) { url_add_firma = urlParams.get("url_add_firma"); };
    let pdf_load_mode = normalizePdfLoadingMode(urlParams.get("pdf_load_mode"));
    let paramenter_pdf_view;
    paramenter_pdf_view = new Array ({
        "url_id_imagen": url_id_imagen,
        "url_cabinete_imagen": url_cabinete_imagen,
        "url_radicado": url_radicado,
        "url_id_workflow": url_id_workflow,
        "url_desc_transacion" : url_desc_transacion,
        "url_printer": url_printer,
        "url_save": url_save,
        "url_add_stamp": url_add_stamp,
        "url_add_firma": url_add_firma,
        "pdf_load_mode": pdf_load_mode
                        });

    if (!url_src) {
        reportDocumentLoadError();
        return;
    }
    load_pdf_viwer_anotation(url_src, url_src_fromat, ur_firma_user, paramenter_pdf_view);
    
    function rezize_event() {
        try {
            auto_size_pdf_anotate();

        } catch (ex) {
            alert(ex.message + " Función rezize_event")
        }
    }
})
//------------------ZONA PDFVIWER-2-----------------------------------------------
let pdfViewer;
let stampConfigurationPromise = null;

function ensureStampConfiguration(showInterface) {
    if (!stampConfigurationPromise) {
        stampConfigurationPromise = Promise.resolve(pdfViewer.config_stamp()).then(function () {
            const config = pdfViewer.itex_config_stamp;
            if (!config || !config[0] || config[0].error_sistema !== "YES") {
                throw new Error("VIEWER_STAMP_CONFIGURATION_FAILED");
            }
            return config;
        }).catch(function (error) {
            stampConfigurationPromise = null;
            throw error;
        });
    }
    return stampConfigurationPromise.then(function (config) {
        if (showInterface) asign_connfig_itext_stamp(config);
        return config;
    });
}

function initializeViewerContextMenus() {
    if (typeof window.context_menu !== "function") return;
    document.querySelectorAll("#pdf-container_ canvas[id]").forEach(function (canvas) {
        if (canvas.getAttribute("data-viewer-context-menu") === "ready") return;
        window.context_menu(canvas.id);
        canvas.setAttribute("data-viewer-context-menu", "ready");
    });
}

async function activateViewerContextMenu(event) {
    if (window.jQuery && window.jQuery.fn && window.jQuery.fn.contextMenu) return;
    const canvas = event.target && event.target.closest ? event.target.closest("#pdf-container_ canvas[id]") : null;
    if (!canvas) return;
    event.preventDefault();
    event.stopImmediatePropagation();
    await runOptionalCapability(event, "graph", "el menú contextual", async function () {
        await ensureStampConfiguration(false);
        initializeViewerContextMenus();
        const repeated = new MouseEvent("contextmenu", {
            bubbles: true,
            cancelable: true,
            clientX: event.clientX,
            clientY: event.clientY
        });
        canvas.dispatchEvent(repeated);
    });
}
function setHorizontal() {
    document.querySelector(".maindoc").classList.add("horizontal-scroll");
    pdfViewer.refreshAll();
}
/** Toggles the visibility of the thumbnails */
function togglethumbs(el) {
    if (el.classList.contains('pushed')) {
        el.classList.remove('pushed');
        document.querySelector('.thumbnails').classList.add('hide');
    } else {
        el.classList.add('pushed');
        document.querySelector('.thumbnails').classList.remove('hide');
    }
}
function load_pdf_viwer_anotation(document, url_src_fromat, ur_firma_user, paramenter_pdf_view) {
    /** Now create the PDFjsViewer object in the DIV */
    pdfViewer = new PDFjsViewer($('.maindoc'), {
        zoomValues: [0.5, 0.75, 1, 1.25, 1.5, 2, 3, 4],
        /** Update the zoom value in the toolbar */
        onZoomChange: function (zoom) {
            zoom = parseInt(zoom * 10000) / 100;
            $('.zoomval').text(zoom + '%');
        },

        /** Update the active page */
        onActivePageChanged: function (page) {
            let pageno = $(page).data('page');
            let pagetotal = this.getPageCount();

            //pdfThumbnails.setActivePage(pageno);
            $('#pdf_page_find').val(pageno);
            $('#pdf_page_find').attr('max', pagetotal);
            $('#num_page_total').val(pagetotal);
        },

        /** zoom to fit when the document is loaded and create the object if wanted to be downloaded */
        onDocumentReady: function () {
            pdfViewer.setZoom('');
            pdfViewer.pdf.getData().then(function (data) {
                //document.querySelector('#filedownload').href = URL.createObjectURL(new Blob([data], { type: 'application/pdf' }));
                //document.querySelector('#filedownload').target = '_blank';
            });
        }
    });

    /** Load the initial PDF file */
    return pdfViewer.loadDocument(document, url_src_fromat, ur_firma_user, paramenter_pdf_view).then(function () {
        //document.querySelector('#filedownload').download = PDFFILE;
    }).catch(function () {
        reportDocumentLoadError();
        return false;
    });
}


/** Create the thumbnails */
/*let pdfThumbnails = new PDFjsViewer($('.thumbnails'), {
    zoomFillArea: 0.7,
    onNewPage: function (page) {
        page.on('click', function () {
            if (!pdfViewer.isPageVisible(page.data('page'))) {
                pdfViewer.scrollToPage(page.data('page'));
            }
        })
    },
    onDocumentReady: function () {
        this.setZoom('fit');
    }
});

pdfThumbnails.setActivePage = function (pageno) {
    this.$container.find('.pdfpage').removeClass('selected');
    let $npage = this.$container.find('.pdfpage[data-page="' + pageno + '"]').addClass('selected');
    if (!this.isPageVisible(pageno)) {
        this.scrollToPage(pageno);
    }
}.bind(pdfThumbnails);

pdfThumbnails.loadDocument(PDFFILE);*/
const configToolbarPdfView = () => {
    let zon_plus = document.getElementById("pdf_btn_zon_plus");
    let zon_minus = document.getElementById("pdf_btn_zon_minus");
    let btn_tbl_sing = document.getElementById("pdf_btn_tbl_sing");
    let pdf_btn_down_load = document.getElementById("pdf_btn_down_load");
    let pdf_btn_tbl_desk_redo = document.getElementById("pdf_btn_tbl_desk_redo");
    let pdf_btn_tbl_desk_undo = document.getElementById("pdf_btn_tbl_desk_undo");
    let tbl_digital = document.getElementById("pdf_btn_tbl_digital");
    let pdf_btn_printer_page = document.getElementById("pdf_btn_printer_page");
    let pdf_a_paramneter_grafo = document.getElementById("pdf_a_paramneter_grafo");
    let Pdf_btn_update_insert = document.getElementById("Pdf_btn_update_insert");
    zon_plus.addEventListener("click", scale_pdfTool_viwer_plus, false);
    zon_minus.addEventListener("click", scale_pdfTool_viwer_minus, false);
    btn_tbl_sing.addEventListener("click", add_pdfTool_viwer_stamp, false);
    pdf_btn_down_load.addEventListener("click", donwload_pdfTool_viwer, false);
    pdf_btn_tbl_desk_redo.addEventListener("click", pdfTool_viwer_rotate_redo, false);
    pdf_btn_tbl_desk_undo.addEventListener("click", pdfTool_viwer_rotate_undo, false);
    tbl_digital.addEventListener("click", pdfTool_viwer_captureFromSTU, false);
    pdf_btn_printer_page.addEventListener("click", pdfTool_viwer_Printer, false);
    pdf_a_paramneter_grafo.addEventListener("click", pdfTool_parameter_grafo, false);
    Pdf_btn_update_insert.addEventListener("click", pdfBtn_update_insert, false);
    document.getElementById("pdf-container_").addEventListener("contextmenu", activateViewerContextMenu, true);
}
const pdfTool_parameter_grafo = async (event) => {
    return runOptionalCapability(event, "graph", "los parámetros del grafo", function () {
        initializeViewerContextMenus();
        return ensureStampConfiguration(true);
    });
}
const pdfBtn_update_insert = (event) => {
    pdfViewer.UpdateInstConfigStamp();
}
function scale_pdfTool_viwer_plus(event) {
    pdfViewer.setZoom('in');
}
function scale_pdfTool_viwer_minus(event) {
    pdfViewer.setZoom('out');
}
async function add_pdfTool_viwer_stamp(event) {
    return runOptionalCapability(event, "graph", "la firma personal", async function () {
        await ensureStampConfiguration(false);
        initializeViewerContextMenus();
        return pdfViewer._Add_interface_anotate(1,0,0);
    });
}
function donwload_pdfTool_viwer(event) {
    pdfViewer._DonwloadPage();
}
function pdfTool_viwer_rotate_redo(event) {
    pdfViewer.rotate(90, true);
}
function pdfTool_viwer_rotate_undo(event) {
    pdfViewer.rotate(-90, true);
}
async function pdfTool_viwer_captureFromSTU(event) {
    if (pdfViewer.anotate_add_firma == 0) return pdfViewer._Add_captureFromSTU();
    return runOptionalCapability(event, "wacom", "la tableta digital", function () {
        return pdfViewer._Add_captureFromSTU();
    });
}
async function pdfTool_viwer_Printer(event) {
    if (pdfViewer.anotate_printer == 0) return pdfViewer._PrinterPdf();
    return runOptionalCapability(event, "print", "la impresión", function () {
        return pdfViewer._PrinterPdf();
    });
}
//------------------FIN----------------------------------------------------
function configToolbar() {
    let pag_find = document.getElementById("pdf_page_find");
    let zon_plus = document.getElementById("pdf_btn_zon_plus");
    let zon_minus = document.getElementById("pdf_btn_zon_minus");
    let tbl_digital = document.getElementById("pdf_btn_tbl_digital");
    //let btn_add_imagen = document.getElementById("pdf_btn_add_imagen");
    let btn_tbl_sing = document.getElementById("pdf_btn_tbl_sing");
    //let btn_delete_anotation = document.getElementById("pdf_btn_delete_anotation");
    let pdf_btn_down_load = document.getElementById("pdf_btn_down_load");
    let pdf_btn_tbl_desk_redo = document.getElementById("pdf_btn_tbl_desk_redo");
    let pdf_btn_tbl_desk_undo = document.getElementById("pdf_btn_tbl_desk_undo");
    pag_find.addEventListener("keypress", Active_find_page, false);
    zon_plus.addEventListener("click", scale_pdf_viwer_plus, false);
    zon_minus.addEventListener("click", scale_pdf_viwer_minus, false);
    tbl_digital.addEventListener("click", captureFromSTU, false);
    //btn_add_imagen.addEventListener("click", addImage, false);
    btn_tbl_sing.addEventListener("click", add_image_sing, false);
    //btn_delete_anotation.addEventListener("click", deleteSelectedObject, false); 
    pdf_btn_down_load.addEventListener("click", donload_pdf_viwer_plus, false);
    pdf_btn_tbl_desk_redo.addEventListener("click", pdf_viwer_rotate_redo, false);
    pdf_btn_tbl_desk_undo.addEventListener("click", pdf_viwer_rotate_undo, false);
}
function Active_find_page(e) {    
    if (e.code == "Enter") {
        let element_src = document.getElementById(e.srcElement.id);
        if (element_src) {
            if (element_src.value > COUNT_PAGE) {
                element_src.value = COUNT_PAGE;
            }
            Pdfanotate_control_page_find(element_src.value);
        } else {
            alert("Imposible encontrar el control " + element_src);
        }
    }
}
function scale_pdf_viwer_plus(event) {   
    pdf.setScaleSizePlus();
}
function scale_pdf_viwer_minus(event) {
    pdf.setScaleSizeMinus();
}
function pdf_viwer_rotate_redo(event) {
    pdf.rotate_page(90);
}
function pdf_viwer_rotate_undo(event) {
    pdf.rotate_page(-90);
}
function pdf_addwacom(event) {
    LEFT_WACOM = 0;
    TOP_WACOM = 0;
    captureFromSTU();
}
function donload_pdf_viwer_plus(event) {
    pdf.donwdload();
}
function changeActiveTool(event) {
    /*var element = $(event.target).hasClass("tool-button")
      ? $(event.target)
      : $(event.target).parents(".tool-button").first();
    $(".tool-button.active").removeClass("active");
    $(element).addClass("active");*/
}

function enableSelector(event) {
    event.preventDefault();
    changeActiveTool(event);
    pdf.enableSelector();
}

async function enablePencil(event) {
    event.preventDefault();
    changeActiveTool(event);
    var t = await render_page();
    pdf.enablePencil();
}

function enableAddText(event) {
    event.preventDefault();
    changeActiveTool(event);
    pdf.enableAddText();
}

function enableAddArrow(event) {
    event.preventDefault();
    changeActiveTool(event);
    pdf.enableAddArrow();
}
function add_image_sing() {
    pdf.addImageToCavasSing();
}
//Evento agrega anotación
function addImage(event) {
    event.preventDefault();
    pdf.addImageToCanvas();
}
function addImagewacom(imagsrc) {
    event.preventDefault();
    pdf.addwacom(imagsrc);
}

function enableRectangle(event) {
    event.preventDefault();
    changeActiveTool(event);
    pdf.setColor('rgba(255, 0, 0, 0.3)');
    pdf.setBorderColor('blue');
    pdf.enableRectangle();
}

function deleteSelectedObject(event) {
  event.preventDefault();
  pdf.deleteSelectedObject();
}

function savePDF() {
    // pdf.savePdf();
    pdf.savePdf('sample.pdf'); // save with given file name
}

function clearPage() {
    pdf.clearActivePage();
}

async function showPdfData(event) {
    var string = pdf.serializePdf();
    $('#dataModal .modal-body pre').first().text(string);
    return runOptionalCapability(event, "prettify", "el detalle del PDF", function () {
        PR.prettyPrint();
        $('#dataModal').modal('show');
    });
}

$(function () {
    $('.color-tool').click(function () {
        $('.color-tool.active').removeClass('active');
        $(this).addClass('active');
        color = $(this).get(0).style.backgroundColor;
        pdf.setColor(color);
    });

    $('#brush-size').change(function () {
        var width = $(this).val();
        pdf.setBrushSize(width);
    });

    $('#font-size').change(function () {
        var font_size = $(this).val();
        pdf.setScaleSize(font_size);
        //pdf.setFontSize(font_size);
    });
});
function auto_size_pdf_anotate() {
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
        //corrige el posicionamiento horizontal del modalpopuextender, la propiedad width del panel debe estar en auto
        //var widtth_procent_left_rigth = (with_frame - document.getElementById("Panel_agregar_expediente_carpeta").clientHeight) / 2;
        //$('#Panel_agregar_expediente_carpeta').css("left", (Math.round(widtth_procent_left_rigth)) + "px");
        var heig_porcent = espacio_iframe - ((espacio_iframe * 1) / 100);  // Indica el porcentaje de espacio vertical del elemento
        $('#pdf-container_').css("height", (heig_porcent - document.getElementById("tool_pdf").clientHeight) + "px"); // Asigna altura del contenedor bootstraf
        //Asgina el valor del contenido central del modal  contenedor bootstraf  menos la suma del footer y la cabecera
       
    }
    catch (err) {
        alert(err.message + " funcion auto_size_pdf_anotate " + err.message);
    }
}
