/**
 * PDFAnnotate v1.0.1
 * Author: Ravisha Heshan   <script src="extend_fabric.js"></script>
<script src="./script.js"></script>
 */
var loadin_cahche;
var cache_inst;
var loadingTask;
var conta_ner;
var page_render;
var WIDTH_VIEWPORT;
var HEIG_VIEWPORT;
var COUNT_PAGE = 1;
var inst_fabric = 0;
var active_canvas = 1;
var IMGEN_WACOM;
var TOP_WACOM=0;
var LEFT_WACOM=0;
var render_page;
var SCALE_FACTOR = 1.5;
var currentX = 0;
var currentY = 0;
var isDraggable = false;
var star_img = new Image();
var canvas_page = "";
var conten_canvas_page = "";
var url_src_pdf;
var url_firma;
var STRU_ANOTATE_IMAGE = new Array();
let PDFAnnotate = function (container_id, url, url_src, url_firma_, options = {}) {
	//zona nueva version
	this._pagesLoading = [];
	this._loading = false;
	this._visibles = [];
	this._activePage = null;
	this.pdf = null;
	this.number_of_pages = 0;
	this.pages_rendered = 0;
	this.active_tool = 1; // 1 - Free hand, 2 - Text, 3 - Arrow, 4 - Rectangle
	this.fabricObjects = [];
	this.fabricObjectsData = [];
	this.pages = [];
	this.zoom_page = 1.5;
	this.zoom_page_max = 3;
	this.color = '#212121';
	this.borderColor = '#000000';
	this.borderSize = 1;
	this.font_size = 16;
	this.active_canvas = 0;
	this.container = document.getElementById(container_id);
	this.container_id = container_id;
	conta_ner = container_id;
	this.url = url;
	url_src_pdf = url_src;
	this.url_src = url_src;
	this.canvas_ = [];
	this.data_image = [];
	this.marcado = [];
	this.rotate = [];
	url_firma = url_firma_;
	this.pageImageCompression = options.pageImageCompression
		? options.pageImageCompression.toUpperCase()
		: "NONE";
	var inst = this;
	cache_inst = this;
	var pdf_;
	var page;
	var nu_page = 1;
	var id_canvas;
	/*load_document(this.url);
	async function load_document(document) {
		this.pages = [];
		this.pdf = null;
		// Load the task and return the promise to load the document
		let loadingTask = pdfjsLib.getDocument(document);
		return loadingTask.promise.then(function (pdf) {
			// Store the pdf file and get the 
			this.pdf = pdf;
			this.pageCount = pdf.numPages;
			this._rotation = 0;
			return forceViewerInitialization();
		})
	}
  const forceViewerInitialization = () => {
		// Store the pdf file
		// Now prepare a placeholder for the pages
		this.pages = [];
		// Remove all the pages
		//this.$container.find(`.${this.settings.pageClass}`).remove();
		this._pagesLoading = [];
		this._loading = false;
		this._visibles = [];
		this._activePage = null;
		return this.pdf.getPage(1).then(function (page) {
			//_createSkeletons(page);
			//this._visiblePages();
			//this._setActivePage(1);
		});
	}
 const 	_createSkeletons = (pageinfo) => {
		for (let i = 1; i <= this.pageCount; i++) {
			if (this.pages[i] === undefined) {

				// Create the pageinfo structure, store it and place it in the appropriate place (the next page will be created similar to the previous one)
				pageinfo = this._createSkeleton(pageinfo, i);
				this.pages[i] = pageinfo;
				this._placeSkeleton(pageinfo, i);

				// Call the callback function (if provided)
				//if (typeof this.settings.onNewPage === "function") {
				//	this.settings.onNewPage.call(this, pageinfo.$div, i);
				//}
			}
		}
	}

const _createSkeleton = (page, i) => {
		let pageinfo = {
			$div: null,
			width: 0,
			height: 0,
			loaded: false,
		};

		// If it is a page, the size will be obtained from the viewport; otherwise, it will be copied from the provided pageinfo
		if (page.getViewport !== undefined) {
			let viewport = page.getViewport({ rotation: this._rotation, scale: 1 });
			pageinfo.width = viewport.width;
			pageinfo.height = viewport.height;
			pageinfo.loaded = true;
		} else {
			pageinfo.width = page.width;
			pageinfo.height = page.height;
		}
		console.assert(((pageinfo.width > 0) && (pageinfo.height > 0)), "Page width and height must be greater than 0");

		// Now create the skeleton for the divs
		pageinfo.$div = $(`<div id="page-${i}">`)
			.attr('data-page', i)
			.data('width', pageinfo.width)
			.data('height', pageinfo.height)
			.data('zoom', this._zoom.current)
			.addClass(this.settings.pageClass)
			.width(pageinfo.width)
			.height(pageinfo.height);

		let $content = $(`<div class="${this.settings.contentClass}">`)
			.width(pageinfo.width)
			.height(pageinfo.height);

		pageinfo.$div.append($content);

		// Clean the page (i.e. put the empty content, etc.)
		//this._cleanPage(pageinfo.$div);

		return pageinfo;
	}*/

	const loadingTask = pdfjsLib.getDocument(this.url);
	load_file();
	async function load_file() {
		let this_ = this;
		pdf_ = await loadingTask.promise;
		COUNT_PAGE = pdf_.numPages;
		document.getElementById("num_page_total").value = pdf_.numPages;
		document.getElementById("pdf_page_find").value = 1;
		canvas_page = 'page-' + (1) + '-canvas';
		conten_canvas_page = 'content_' + (1) + '_apge'
		for await (let num of lod_page()) {

		}
	};
	async function* lod_page() {
		try {
			create_bar("pdf-container_", 0, pdf_.numPages);
			while (nu_page <= pdf_.numPages) {
				set_bar(nu_page);
				page = await pdf_.getPage(nu_page);
				inst.pages.push(page);
				let div_page = document.createElement("div");
				div_page.classList.add("pdfpage");
				let viewport = page.getViewport({ rotation: 0, scale: inst.zoom_page });
				var canvas = document.createElement('canvas');
				div_page.appendChild(canvas);
				document.getElementById(inst.container_id).appendChild(div_page);
				canvas.className = 'content-wrapper';
				canvas.height = viewport.height;
				canvas.width = viewport.width;
				HEIG_VIEWPORT = canvas.height;
				WIDTH_VIEWPORT = canvas.width;
				canvas.style.width = (canvas.width) + "px";
				canvas.style.height = (canvas.height) + "px";
				div_page.style.width = (canvas.width) + "px";
				div_page.style.height = (canvas.height) + "px";
				div_page.setAttribute("id", 'content_' + (nu_page) + '_apge');
				canvas.setAttribute("id", 'page-' + (nu_page) + '-canvas');
				canvas.setAttribute("pg", (nu_page));
				id_canvas = 'page-' + (nu_page) + '-canvas';
				inst.canvas_.push(canvas);
				inst.marcado.push(0);
				inst.rotate.push(0)
				context = canvas.getContext('2d');
				var renderContext = {
					canvasContext: context,
					intent: 'print',
					viewport: viewport,
				};
				pdf_anotate_mouse('content_' + (nu_page) + '_apge');
				pdf_anotate_onclik('content_' + (nu_page) + '_apge');
				context_menu('content_' + (nu_page) + '_apge');	
				let renderTask = await page.render(renderContext);
				inst_fabric++;
				nu_page++;
				if (nu_page == pdf_.numPages) {
					close_bar();
				}
			}
		} catch (ex) {
			close_bar();
			//alert(ex.message);
		} finally {
			close_bar();
		}
	};	
}

function pdf_anotate_onclik(id_div_dag) {
	$("#" + id_div_dag).click(function () {
		let elment = document.getElementById(this.id);
		active_canvas = elment.childNodes[0].getAttribute("pg");
		canvas_page = elment.childNodes[0].id;
		conten_canvas_page = elment.childNodes[0].parentElement.id;
		document.getElementById("pdf_page_find").value = active_canvas;
	});
}

function pdf_anotate_mouse(id_div_dag) {
	$("#" + id_div_dag).mouseup(function () {	
		let elment = document.getElementById(this.id);
		active_canvas = elment.childNodes[0].getAttribute("pg");
		canvas_page = elment.childNodes[0].id;
		conten_canvas_page = elment.childNodes[0].parentElement.id;
		document.getElementById("pdf_page_find").value = active_canvas;
	});
}

function context_menu(id_div_dag) {
	try {
		
		$('#' + id_div_dag).contextMenu('context-menu-1' + id_div_dag, {
		'Tableta digital': {
			click: function (element) {
				var currentY = document.getElementById("context-menu-1" + id_div_dag).style.top.replace("px", "");
				var currentX = document.getElementById("context-menu-1" + id_div_dag).style.left.replace("px", "");
				var currentW = document.getElementById("context-menu-1" + id_div_dag).clientWidth;
				let num_active_page = active_canvas;
				let name_element = "content_" + num_active_page + "_apge";
				let elemen = document.getElementById(name_element);
				let distance;
				if (elemen) {
					distance = elemen.getBoundingClientRect();
				}

				if (currentY > distance.top) {
					currentY = currentY - distance.top;
				} else {
					currentY = currentY + distance.top;
				}
				if (currentY > distance.bottom) {
					currentY = currentY - 100;
				}
				if (currentX > parseInt(distance.left)) {
					currentX = currentX - parseInt(distance.left);
				} else {
					//currentX = currentX + parseInt(distance.left);
				}
				if (((currentX + currentW)) > parseInt(distance.right)) {
					currentX = currentX - currentW;
				}
				LEFT_WACOM = currentX;
				TOP_WACOM = currentY;
				captureFromSTU();
				},
				klass: "fad fa-edit"
		},
		'firma personal': {
			click: function (element) {  // element is the jquery obj clicked on when context menu launched
				var currentY = document.getElementById("context-menu-1" + id_div_dag).style.top.replace("px", "");
				var currentX = document.getElementById("context-menu-1" + id_div_dag).style.left.replace("px", "");
				var currentW = document.getElementById("context-menu-1" + id_div_dag).clientWidth;
				let num_active_page = active_canvas;
				let name_element = "content_" + num_active_page + "_apge";
				let elemen = document.getElementById(name_element);
				let distance;
				if (elemen) {	
				  distance = elemen.getBoundingClientRect();	
				}
				if (currentY > parseInt(distance.top)) {
					currentY = currentY - parseInt(distance.top);
				} else {
					currentY = currentY + parseInt(distance.top);
				}
				if (currentY > parseInt(distance.bottom)) {
					currentY = currentY - 100;
				}
				if (currentX > parseInt(distance.left)) {
					currentX = currentX - parseInt(distance.left);
				} else {
					//currentX = currentX + parseInt(distance.left);
				}
				if (((currentX + currentW)) > parseInt(distance.right)) {
					currentX = currentX - currentW;
				}
				
				anotate_firma(conten_canvas_page, url_firma, "", 1, currentY, currentX);
			},
			klass: "fad fa-signature"
		},
		'Cancelar': {
			click: function (element) {  

				//$(element).css("display", "none");  
			},
			klass: "far fa-times"
		}
	}


	);
	}
	catch (ex) {
	}
}
async function  Pdfanotate_control_page_find(pag_find) {
	let Element_canvas = document.getElementById("page-" + pag_find + "-canvas");
	if (Element_canvas) {
		Element_canvas.parentElement.scrollIntoView();
		active_canvas = pag_find;		
	}
}
function isInViewport(elem) {
	if (elem) {
		var distance = elem.getBoundingClientRect();
		return (	
			distance.top < (window.innerHeight  || document.documentElement.clientHeight) && distance.bottom > 10
		);
	}
}

function validarURL(miurl) {
	try {
		new URL(miurl);
		return "YES";
	} catch (ex) {
		return ex.message;
	}
}
function get_distance_top() {
	let num_active_page = active_canvas;
	let name_element = "content_" + num_active_page + "_apge";
	let elemen = document.getElementById(name_element);
	if (elemen) {
		if (isInViewport(elemen)) {
			var distance = elemen.getBoundingClientRect();
		}
	}
}
let distance_ = 0;

$('#pdf-container_').scroll(function () {	
	for (var i = 0; i <= document.getElementsByClassName("pdfpage").length; i++) {
		let container = document.getElementsByClassName("pdfpage");
		let elment = container[i];
		if (isInViewport(elment)) {
			let content_scroll = document.getElementById("pdf-container_");
			active_canvas = elment.childNodes[0].getAttribute("pg");
			var distance = elment.getBoundingClientRect();
			var dis_total = (distance.top + distance.bottom) + elment.scrollTop;
			var rec_container = content_scroll.getBoundingClientRect();
			document.getElementById("pdf_page_find").value = active_canvas;
			canvas_page = elment.childNodes[0].id;
			conten_canvas_page = elment.childNodes[0].parentElement.id;
			return true;	
		}
	}
	
});
PDFAnnotate.prototype.scale_pdf = function () {
	return this.zoom_page;
}
PDFAnnotate.prototype.setScaleSizePlus = async function (size) {
	try {
		
		if (this.zoom_page > this.zoom_page_max) {
			return true;
		}
		create_bar("pdf-container_", 0, COUNT_PAGE);
		this.zoom_page = this.zoom_page + 0.5;
		for (var i = 1; i <= (COUNT_PAGE); i++) {
			set_bar(i);
			var id_canvas = 'page-' + (i) + '-canvas';
			var div_page = document.getElementById('content_' + (i) + '_apge');
			var div_canvas_container = div_page.childNodes[0];
			var canvas = document.getElementById(id_canvas);
			const ctx = canvas.getContext("2d");
			var page = this.pages[i - 1];
			let rota_te = this.rotate[i - 1]
			let viewport = page.getViewport({ rotation: rota_te, scale: this.zoom_page });
			canvas.height = viewport.height;
			canvas.width = viewport.width;
			canvas.style.width = (canvas.width) + "px";
			canvas.style.height = (canvas.height) + "px";
			HEIG_VIEWPORT = canvas.height;
			WIDTH_VIEWPORT = canvas.width;
			div_page.style.width = (canvas.width) + "px";
			div_page.style.height = (canvas.height) + "px";
			div_canvas_container.style.width = (canvas.width) + "px";
			div_canvas_container.style.height = (canvas.height) + "px";
			var renderContext = {
				canvasContext: ctx,
				intent: 'print',
				viewport: viewport
			};
			var renderTask = await page.render(renderContext);

		}
	} catch (ex) {
		alert(ex.message);
	} finally {
		close_bar();
	}
}
PDFAnnotate.prototype.setScaleSizeMinus = async function (size) {
	try {	
		if (this.zoom_page <= 1) {
			return true;
		}
		create_bar("pdf-container_", 0, COUNT_PAGE);
		this.zoom_page = this.zoom_page - 0.5;
		for (var i = 1; i <= (COUNT_PAGE); i++) {
			set_bar(i);
			var id_canvas = 'page-' + (i) + '-canvas';
			var div_page = document.getElementById('content_' + (i) + '_apge');
			var div_canvas_container = div_page.childNodes[0];
			var canvas = document.getElementById(id_canvas);
			const ctx = canvas.getContext("2d");
			var page = this.pages[i - 1];
			let rota_te = this.rotate[i - 1]
			let viewport = page.getViewport({ rotation: rota_te, scale: this.zoom_page });
			canvas.height = viewport.height;
			canvas.width = viewport.width;
			canvas.style.width = (canvas.width) + "px";
			canvas.style.height = (canvas.height) + "px";
			HEIG_VIEWPORT = canvas.height;
			WIDTH_VIEWPORT = canvas.width;
			div_page.style.width = (canvas.width) + "px";
			div_page.style.height = (canvas.height) + "px";
			div_canvas_container.style.width = (canvas.width) + "px";
			div_canvas_container.style.height = (canvas.height) + "px";
			var renderContext = {
				canvasContext: ctx,
				intent: 'print',
				viewport: viewport
			};
			var renderTask = await page.render(renderContext);
		}
	} catch (ex) {
		alert(ex.message);
	} finally {
		close_bar();
	}	
}
PDFAnnotate.prototype.rotate_page = async function (rotate) {
	var inst = this;
	let rotation = inst.rotate[active_canvas - 1];
	rotate = rotate + rotation;
	rotation = rotate;
	inst.rotate[active_canvas - 1] = rotation;
	var id_canvas = 'page-' + (active_canvas) + '-canvas';
	var div_page = document.getElementById('content_' + (active_canvas) + '_apge');
	var div_canvas_container = div_page.childNodes[0];
	var canvas = document.getElementById(id_canvas);
	const ctx = canvas.getContext("2d");
	var page = this.pages[active_canvas - 1];
	let viewport = page.getViewport({ rotation: inst.rotate[active_canvas - 1], scale: inst.zoom_page });
	canvas.height = viewport.height;
	canvas.width = viewport.width;
	canvas.style.width = (canvas.width) + "px";
	canvas.style.height = (canvas.height) + "px";
	HEIG_VIEWPORT = canvas.height;
	WIDTH_VIEWPORT = canvas.width;
	div_page.style.width = (canvas.width) + "px";
	div_page.style.height = (canvas.height) + "px";
	div_canvas_container.style.width = (canvas.width) + "px";
	div_canvas_container.style.height = (canvas.height) + "px";
	var renderContext = {
		canvasContext: ctx,
		intent: 'print',
		viewport: viewport
	}
	var renderTask = await page.render(renderContext);
	inst.pages[active_canvas - 1] = page;
}
PDFAnnotate.prototype.enableSelector = function () {
	var inst = this;
	inst.active_tool = 0;
	if (inst.fabricObjects.length > 0) {
	    $.each(inst.fabricObjects, function (index, fabricObj) {
	        fabricObj.isDrawingMode = false;
	    });
	}
}

PDFAnnotate.prototype.enablePencil =  async function () {
	var inst = this;
	inst.active_tool = 1;
	let res = await render_page();
	if (inst.fabricObjects.length > 0  ) {
			$.each(inst.fabricObjects, function (index, fabricObj) {
				fabricObj.isDrawingMode = true;
			});

		}
	
}

PDFAnnotate.prototype.enableAddText = function () {
	var inst = this;
	inst.active_tool = 2;
	if (inst.fabricObjects.length > 0) {
	    $.each(inst.fabricObjects, function (index, fabricObj) {
	        fabricObj.isDrawingMode = false;
	    });
	}
}

PDFAnnotate.prototype.enableRectangle = function () {
	var inst = this;
	var fabricObj = inst.fabricObjects[inst.active_canvas];
	inst.active_tool = 4;
	if (inst.fabricObjects.length > 0) {
		$.each(inst.fabricObjects, function (index, fabricObj) {
			fabricObj.isDrawingMode = false;
		});
	}

	var rect = new fabric.Rect({
		width: 100,
		height: 100,
		fill: inst.color,
		stroke: inst.borderColor,
		strokeSize: inst.borderSize
	});
	fabricObj.add(rect);
}

PDFAnnotate.prototype.enableAddArrow = function () {
	var inst = this;
	inst.active_tool = 3;
	if (inst.fabricObjects.length > 0) {
	    $.each(inst.fabricObjects, function (index, fabricObj) {
	        fabricObj.isDrawingMode = false;
	        new Arrow(fabricObj, inst.color, function () {
	            inst.active_tool = 0;
	        });
	    });
	}
}
PDFAnnotate.prototype.addwacom = async function (imagsrc)  {
	var inst = this;
	anotate_firma(conten_canvas_page, "", IMGEN_WACOM, 1, TOP_WACOM, LEFT_WACOM);
	
}
PDFAnnotate.prototype.addImageToCavasSing = async function () {
	
	anotate_firma(conten_canvas_page, url_firma, "" ,1,0,0);
}

PDFAnnotate.prototype.addImageToCanvas = async function () {
	var inst = this;
	inst.active_tool = 0;	
	var fabricObj = inst.fabricObjects[active_canvas - 1];
	if (fabricObj) {
		var inputElement = document.createElement("input");
		inputElement.type = 'file'
		inputElement.accept = ".jpg,.jpeg,.png,.PNG,.JPG,.JPEG";
		inputElement.onchange = async function() {
			var reader = new FileReader();
			reader.addEventListener("load", function () {
				inputElement.remove();
				var image = new Image();
				image.onload = function () {
					fabricObj.add(new fabric.Image(image, {			
					}))
				}
				if (inst.marcado[active_canvas - 1] == 0) {
					var background = inst.canvas_[active_canvas - 1].toDataURL("image/png");
					var image_ = new Image();
					image_.src = background;
					var oImg = new fabric.Image(image_);
					oImg.scale(0.8, 0.8);
					oImg.height = (HEIG_VIEWPORT * 10);
					oImg.width = (WIDTH_VIEWPORT * 10);
					fabricObj.setBackgroundImage(oImg, fabricObj.renderAll.bind(fabricObj));
					inst.marcado[active_canvas - 1] = 1;
				}
				image.src = this.result;
			}, false);
			reader.readAsDataURL(inputElement.files[0]);
		}
		//var t = await render_page();
		document.getElementsByTagName('body')[0].appendChild(inputElement)
		inputElement.click()
	}
}

PDFAnnotate.prototype.deleteSelectedObject = function () {
	var inst = this;
	var activeObject = inst.fabricObjects[active_canvas - 1].getActiveObject();
	if (activeObject)
	{
		if (confirm('Desea eliminar la anotacion ?')) inst.fabricObjects[active_canvas - 1].remove(activeObject);
	}
}
PDFAnnotate.prototype.replace_page = async function (fileName, pdf_file_page) {
	var inst = this;
	var id_canvas = 'page-' + (pdf_file_page) + '-canvas';
	var canvas = document.getElementById(id_canvas);
	const ctx = canvas.getContext("2d");
	let loadingTask = pdfjsLib.getDocument(fileName);
	let pdf_ = await  loadingTask.promise;
	let page = await pdf_.getPage(1);
	let rotation = inst.rotate[active_canvas - 1];
	let viewport = page.getViewport({ rotation: rotation, scale: inst.zoom_page });
	var renderContext = {
		canvasContext: ctx,
		intent: 'print',
		viewport: viewport
	}
	var renderTask = await page.render(renderContext);
	inst.pages[0] = page;
}

PDFAnnotate.prototype.savePdf = function (fileName) {
	try {
		var inst = this;
		var doc = new jspdf.jsPDF();
		if (typeof fileName === 'undefined') {
			fileName = `${new Date().getTime()}.pdf`;
		}
		create_bar_cont_title("pdf-container_", 0, inst.canvas_.length,"");
		for (var i = 0; i < inst.canvas_.length; i++) {
			set_bar_cont_title(i + 1);
			if (i != 0) {
				doc.addPage();
				doc.setPage(i + 1);
			}
			doc.addImage(
				inst.canvas_[i].toDataURL(),
				inst.pageImageCompression == "NONE" ? "PNG" : "JPEG",
				0,
				0,
				doc.internal.pageSize.getWidth(),
				doc.internal.pageSize.getHeight(),
				`page-${i + 1}`,
				["FAST", "MEDIUM", "SLOW"].indexOf(inst.pageImageCompression) >= 0
					? inst.pageImageCompression
					: undefined
			);
			if (i == (inst.canvas_.length - 1)) {
				doc.save(fileName);
			}
		}
	} catch (ex) {
		alert(ex.message);
	} finally {
		close_bar_cont_title();
	}
	
}

PDFAnnotate.prototype.setBrushSize = function (size) {
	var inst = this;
	$.each(inst.fabricObjects, function (index, fabricObj) {
	    fabricObj.freeDrawingBrush.width = size;
	});
}

PDFAnnotate.prototype.setColor = function (color) {
	var inst = this;
	inst.color = color;
	$.each(inst.fabricObjects, function (index, fabricObj) {
        fabricObj.freeDrawingBrush.color = color;
    });
}

PDFAnnotate.prototype.setBorderColor = function (color) {
	var inst = this;
	inst.borderColor = color;
}

PDFAnnotate.prototype.setFontSize = function (size) {
	this.font_size = size;
}

PDFAnnotate.prototype.setBorderSize = function (size) {
	this.borderSize = size;
}

PDFAnnotate.prototype.clearActivePage = function () {
	var inst = this;
	var fabricObj = inst.fabricObjects[inst.active_canvas];
	var bg = fabricObj.backgroundImage;
	if (confirm('Are you sure?')) {
	    fabricObj.clear();
	    fabricObj.setBackgroundImage(bg, fabricObj.renderAll.bind(fabricObj));
	}
}

PDFAnnotate.prototype.serializePdf = function() {
	var inst = this;
	return JSON.stringify(inst.fabricObjects, null, 4);
}

PDFAnnotate.prototype.loadFromJSON = function(jsonData) {
	var inst = this;
	$.each(inst.fabricObjects, function (index, fabricObj) {
		if (jsonData.length > index) {
			fabricObj.loadFromJSON(jsonData[index], function () {
				inst.fabricObjectsData[index] = fabricObj.toJSON()
			})
		}
	})
}
PDFAnnotate.prototype.donwdload = async function () {
	var inst = this;
	service_download_pdf_visor_plus(inst.url_src);
	
}
//zona drgable
let contador_dag=1;
function anotate_firma(content, ur_image, ur_data64, utili_elimina, topconten_, lefconten_) {
	let content_canvas = document.getElementById(content);
	if (content_canvas == null) {
		alert("Imposible encontar el contendor " + content);
		return true;
	}
	let id_div_dag = "div-drav-image-" + contador_dag;
	if (utili_elimina == 1) {
		let div_drag = document.getElementById(id_div_dag);
		if (div_drag) {
			div_drag.parentElement.removeChild(div_drag);
		}	
	}
	//Creando bara de botonoes
	let div_barra = document.createElement("div");
	div_barra.style.backgroundColor = "#5b6dcd";
	div_barra.classList.add("modal-header_");
	div_barra.id = id_div_dag + "_bar";
	//add buton cerrar ventana
	var ihtml = document.createElement("i");
	ihtml.style.color = "black";
	ihtml.classList.add("fas");
	ihtml.classList.add("fa-times");
	var ahtml = document.createElement("a");
	ahtml.classList.add("btn");
	ahtml.classList.add("btn-link");
	ahtml.classList.add("btn-sm");
	ahtml.setAttribute("onclick", "event_drag_anotate(event,this);");
	ahtml.setAttribute("title", "Cerrar");
	ahtml.setAttribute("parent_id", id_div_dag);
	ahtml.setAttribute("event_drag", "close");
	
	ahtml.appendChild(ihtml);
	div_barra.appendChild(ahtml);
	//add boton salvar anotacion
	ihtml = document.createElement("i");
	ihtml.style.color = "black";
	ihtml.classList.add("fas");
	ihtml.classList.add("fa-save");
	ahtml = document.createElement("a");
	ahtml.classList.add("btn");
	ahtml.classList.add("btn-link");
	ahtml.classList.add("btn-sm");
	ahtml.setAttribute("onclick", "event_drag_anotate(event,this);");
	ahtml.setAttribute("title", "Guardar");
	ahtml.setAttribute("parent_id", id_div_dag);
	ahtml.setAttribute("parent_page", active_canvas);
	ahtml.setAttribute("ur_image_drag", ur_image);
	ahtml.setAttribute("ur_image_drag64", ur_data64);
	ahtml.setAttribute("event_drag", "save");
	ahtml.appendChild(ihtml);
	div_barra.appendChild(ahtml);
	//creandon el elemento drop and drop  
	let div_drad = document.createElement("div");
	div_drad.style.border = "1px solid rgba(0,0,0,.2)";
	div_drad.style.borderRadius = "10px";
	div_drad.classList.add("ui-widget-content");	
	div_drad.id = id_div_dag;
	div_drad.style.position = "absolute";
	let img_drag = document.createElement("img");
	img_drag.id = id_div_dag + "img";
	img_drag.style.maxWidth = "100px";
	img_drag.style.maxHeight = "100px";
	div_drad.appendChild(img_drag);
	div_drad.appendChild(div_barra);
	let image_sr = new Image();
	if (ur_image !== "") {
		image_sr.src = ur_image;
	} else {
		image_sr.src = ur_data64;
	}
	
	content_canvas.appendChild(div_drad);
	if (utili_elimina !== 1) {
		contador_dag++;
	}
	image_sr.onload = function () {
		img_drag.src = image_sr.src;	
	};
	let parent = document.getElementById("pdf-container_");
	let container_ = document.getElementById("container-general");
	var topconten = 0;
	var lefconten = 0;
	if (topconten_ !== 0) {
		topconten = topconten_;
		lefconten = lefconten_;
	} else {
		topconten = container_.clientHeight / 2;
		lefconten = content_canvas.clientWidth / 2;
	}
	
	//parent.scrollTop = topconten;
	div_drad.style.top = topconten + "px";
	div_drad.style.left = lefconten + "px";	
	if (isInViewport(div_drad) == false) {
		div_drad.scrollIntoView();
	}
	$("#" + id_div_dag).css({ opacity: 0.5 });
	left = $("#" + id_div_dag).position.left;
	top = $("#" + id_div_dag).position.top;
	$("#" + id_div_dag).draggable({
		containment: $("#" + content),
		stop: function (event, ui) {
			
		}
	}
	);

	$("#" + id_div_dag).resizable({
		maxHeight: 80, maxWidth: 100, minWidth: 50, minHeight: 50,
		start: function (event, ui) {
			//$("#draggable").offset({ top: top, left: left });
		},
		stop: function (event, ui) {
			
			$("#" + id_div_dag).css('position', 'relative');
			var dragab = $("#" + id_div_dag);
			var contenido = $("#" + content);
			var scr = contenido.scrollTop();
			var scrolleft = contenido.scrollLeft();
			left = left - scrolleft;
			var posicfinal = (top + scr) - 10;
			//$("#draggable").offset({ top: top, left: left });
			//$("#Hiddenintercambio").val(top + "-" + left + "-" + dragab.height() + "-" + dragab.width() + "-" + dragab.height() + "-" + scr + "-" + posicfinal);

		},
		resize: function (event, ui) {
			//$("#img").imageResize();
			//var contenido = $("#" + content);
			//var scroltop = contenido.scrollTop();
			//var scroleft = contenido.scrollLeft();
			//$("#" + id_div_dag).offset({ top: top, left: left - scroleft });
		}
	}
	);

	
}
async function  event_drag_anotate (event, element) {
	let value_id_parent = $(element).attr("parent_id");
	let page_drag = $(element).attr("parent_page");
	let even_drag = $(element).attr("event_drag");
	let ur_image_drag = $(element).attr("ur_image_drag");
	let ur_image_drag_bit64 = $(element).attr("ur_image_drag64");
	if (even_drag == "close") {
		let drag_div_parent = document.getElementById(value_id_parent);
		if (drag_div_parent) {
			drag_div_parent.parentElement.removeChild(drag_div_parent);
		}
	}
	if (even_drag == "save") {
		let drag_canvas = document.getElementById('page-' + (page_drag) + '-canvas')
		let drag_div_parent = document.getElementById(value_id_parent);
		let div_imag = document.getElementById(value_id_parent + "img");
		let div_bar = document.getElementById(value_id_parent + "_bar");
		if (drag_div_parent) {	
			let currentY = drag_div_parent.style.top.replace("px", "");
			let bar_heigt = div_bar.style.height.replace("px", "");
			let currentX = drag_div_parent.style.left.replace("px", "");
			//let drag_div_heigth = drag_div_parent.clientHeight;
			let canvas_heigth = drag_canvas.style.height.replace("px", "");
			currentY = canvas_heigth - (currentY);
			let anotate_width = div_imag.clientWidth;
			let anotate_height = div_imag.clientHeight;
			let pdf_file_width = drag_canvas.clientWidth;
			let pdf_file_height = drag_canvas.clientHeight;
			let pdf_file_page = page_drag;
			let anotate_file_src = ur_image_drag;
			let anotate_scale = pdf.scale_pdf();
			let anotate_file_bit = ur_image_drag_bit64;
			let res = confirm("Desea guadar la anotación de firma");
			if (res == false) {
				return "";
			}
			let res_sav = save_anotate_firma(currentX, currentY, anotate_width, anotate_height, pdf_file_page, anotate_file_src, anotate_file_bit, anotate_scale, pdf_file_width, pdf_file_height);
			if (res_sav == "YES") {
				drag_div_parent.parentElement.removeChild(drag_div_parent);
			}
			
		}
		
	}
}

 function save_anotate_firma(currentX, currentY, anotate_width, anotate_height, pdf_file_page, anotate_file_src, anotate_file_bit, anotate_scale, pdf_file_width, pdf_file_height) {
	try {
		var canvas = document.getElementById("page-" + pdf_file_page + "-canvas");
		var ctx = canvas.getContext("2d");
		star_img = new Image();
		if (anotate_file_src !== "") {
			star_img.src = anotate_file_src;
		} else {
			star_img.src = anotate_file_bit;
		}
		
		star_img.onload = function () {
			_Go(star_img);
		}
		function _Go(star_img) {
			_Anotate_server();
		
		}
		function _Anotate_server() {	
			STRU_ANOTATE_IMAGE = new Array();
			STRU_ANOTATE_IMAGE.push({
				"pdf_file_src": url_src_pdf, "pdf_file_page": pdf_file_page, "anotate_file_src": anotate_file_src,
				"anotate_file_bit": anotate_file_bit, "anotate_width": anotate_width, "anotate_heigth": anotate_height,
				"anotate_x": currentX, "anotate_y": currentY, "anotate_scale": anotate_scale, "anotate_type": "img",
				"pdf_file_width": pdf_file_width, "pdf_file_heigth": pdf_file_height
			});
			service_anotate_pdf_image(pdf_file_page);
		}
		function _ResetCanvas() {
			ctx.fillStyle = 'transparent';
			ctx.fillRect(0, 0, canvas.width, canvas.height);
		}
		function _DrawImage(star_img) {
			ctx.drawImage(star_img, currentX, currentY);
			cache_inst.replace_page();
			
		}	
		return "YES";
	} catch (ex) {
		return ex.message;
	}
}
//zona de web service
function service_anotate_pdf_image(pdf_file_page) {
	try {
		var serialice = JSON.stringify(STRU_ANOTATE_IMAGE);
		$.ajax('../../webservice/WebService_itext.asmx/service_anotate_pdf_image', {
			data: "{'anotate':" + "'" + serialice + "'}",
			dataType: 'json',
			type: "POST",
			traditional: true,
			processData: false,
			contentType: "application/json; charset=utf-8",
			success: function (data) {
				if (data.d !== "YES") {
					if (data.d[0].error_sistema != "YES") {
						alert(data.d[0].error_sistema);
						return "";
					} else {
						cache_inst.replace_page(data.d[0].url_salida, pdf_file_page);
						return true;
					}
					
				}
			}, error: function (xception, textStatus, errorThrown) {

				if (xception.status === 0) {

					alert('Not connect: Verify Network.');

				} else if (xception.status == 404) {

					alert('Requested page not found [404]');

				} else if (xception.status == 500) {

					alert('Internal Server Error [500].' + xception.responseText);

				} else if (textStatus === 'parsererror') {

					alert('Requested JSON parse failed.');

				} else if (textStatus === 'timeout') {

					alert('Time out error.');

				} else if (textStatus === 'abort') {

					alert('Ajax request aborted.');

				} else {

					alert('Uncaught Error: ' + xception.responseText);

				}
			}
		});
	}
	catch (ex) {
		//ESTADO_EVENT_GENERAL = "out";
		alert('service_anotate_pdf_image  ' + ex.message);
	}
}
function service_download_pdf_visor_plus(url_pdf) {
	try {
		
		$.ajax('../../webservice/WebService_itext.asmx/service_download_pdf_visor_plus', {
			data: "{'anotate':" + "'" + url_pdf + "'}",
			dataType: 'json',
			type: "POST",
			traditional: true,
			processData: false,
			contentType: "application/json; charset=utf-8",
			success: function (data) {
				if (data.d !== "YES") {
					if (data.d[0].error_sistema != "YES") {
						alert(data.d[0].error_sistema);
						return "";
					} else {
						dowload_file(data.d[0].url_salida, data.d[0].name_file);
						return true;
					}

				}
			}, error: function (xception, textStatus, errorThrown) {

				if (xception.status === 0) {

					alert('Not connect: Verify Network.');

				} else if (xception.status == 404) {

					alert('Requested page not found [404]');

				} else if (xception.status == 500) {

					alert('Internal Server Error [500].' + xception.responseText);

				} else if (textStatus === 'parsererror') {

					alert('Requested JSON parse failed.');

				} else if (textStatus === 'timeout') {

					alert('Time out error.');

				} else if (textStatus === 'abort') {

					alert('Ajax request aborted.');

				} else {

					alert('Uncaught Error: ' + xception.responseText);

				}
			}
		});
	}
	catch (ex) {
		//ESTADO_EVENT_GENERAL = "out";
		alert('service_download_pdf_visor_plus  ' + ex.message);
	}
}
let MAX_CONTABAR = 0;
function create_bar(parent_bar, conta_bar, max_conta_bar) {
	MAX_CONTABAR = max_conta_bar;
	let div_progres = document.createElement("div");
	div_progres.id = "div_progres_element";
	div_progres.style.width = "100%";
	div_progres.style.backgroundColor = '#ddd';
	div_progres.style.top = "0px";
	div_progres.style.left = "0px";
	let div_bar_progres = document.createElement("div");
	div_bar_progres.style.height = "5px";
	div_bar_progres.style.width = conta_bar + "%";
	div_bar_progres.id = "div_progres_bar_element";
	div_bar_progres.style.backgroundColor = '#04AA6D';
	div_progres.appendChild(div_bar_progres);
	let div_parent_progres = document.getElementById(parent_bar);
	div_parent_progres.appendChild(div_progres);
}
function close_bar() {
	MAX_CONTABAR = 0;
	let div_drag = document.getElementById("div_progres_element");
	if (div_drag) {
		div_drag.parentElement.removeChild(div_drag);
	}
}	
function set_bar(valor_incre) {
	let porcent = (100 * valor_incre) / MAX_CONTABAR;
	porcent = Math.round(porcent);
	let div_bar_progres = document.getElementById("div_progres_bar_element");
	if (div_bar_progres) {
		div_bar_progres.style.width = porcent + "%";
	}
}
function create_bar_cont_title(parent_bar, conta_bar, max_conta_bar, title) {
	MAX_CONTABAR = max_conta_bar;
	let div_progres = document.createElement("div");
	div_progres.id = "div_progres_element_";
	div_progres.style.width = "50%";
	div_progres.style.height = "30%";
	div_progres.style.border = "1px solid rgba(0,0,0,.2)";
	div_progres.style.borderRadius = "10px";
	div_progres.style.position = "absolute";
	div_progres.style.zIndex="9000000"
	let div_bar_progres = document.createElement("div");
	div_bar_progres.style.height = "5px";
	div_bar_progres.style.width = conta_bar + "%";
	div_bar_progres.id = "div_progres_bar_element_";
	div_bar_progres.style.backgroundColor = '#04AA6D';
	div_progres.appendChild(div_bar_progres);
	let div_parent_progres = document.getElementById(parent_bar);
	if (div_parent_progres) {
		div_parent_progres.appendChild(div_progres);
	}
	
}
function set_bar_cont_title(valor_incre) {
	let porcent = (100 * valor_incre) / MAX_CONTABAR;
	porcent = Math.round(porcent);
	let div_bar_progres = document.getElementById("div_progres_bar_element_");
	if (div_bar_progres) {
		div_bar_progres.style.width = porcent + "%";
	}
}
function close_bar_cont_title() {
	MAX_CONTABAR = 0;
	let div_drag = document.getElementById("div_progres_element_");
	if (div_drag) {
		div_drag.parentElement.removeChild(div_drag);
	}
}