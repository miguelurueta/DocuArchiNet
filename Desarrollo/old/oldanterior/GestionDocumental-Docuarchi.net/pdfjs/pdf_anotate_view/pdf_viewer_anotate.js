//const { resolve } = require("path/posix");
//const { promise } = require("../wacom/q");

/*
   Copyright 2020 Carlos de Alfonso (https://github.com/dealfonso)

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/
(function (exports, $) {
    'use strict';

    // Class used to help in zoom management; probably it can be moved to the main class, but it is used to group methods
    class Zoomer {
        /**
         * Construct the helper class
         * @param {PDFjsViewer} viewer - the viewer object
         * @param {*} options - the options object
         */
        constructor(viewer, options = {}) {
            let defaults = {
                // The possible zoom values to iterate through using "in" and "out"
                zoomValues: [0.25, 0.5, 0.75, 1, 1.25, 1.50, 2, 4, 8],
                // The area to fill the container with the zoomed pages
                fillArea: 0.9,
            }

            // The current zooom value
            this.current = 1.25;
            // The viewer instance whose pages may be zoomed
            this.viewer = viewer;
            // The settings
            this.settings = $.extend(defaults, options);

            // Need having the zoom values in order
            this.settings.zoomValues = this.settings.zoomValues.sort();
        }

        /** Translates a zoom value into a float value; possible values:
         * - a float value
         * - a string with a keyword (e.g. "width", "height", "fit", "in", "out")
         * @param {number} zoom - the zoom value to be translated
         * @return {number} The zoom value
        */
        get(zoom = null) {
            // If no zoom is specified, return the current one
            if (zoom === null) {
                return this.current;
            }
            // If it is a number, return it
            if (parseFloat(zoom) == zoom) {
                return zoom;
            }
            let $activepage = this.viewer.getActivePage();
            let zoomValues = [];
            // If it is a keyword, return the corresponding value
            switch (zoom) {
                case "in":
                    zoom = this.current;
                    zoomValues = this.settings.zoomValues.filter((x) => x > zoom);
                    if (zoomValues.length > 0) {
                        zoom = Math.min(...zoomValues);
                    }
                    break;
                case "out":
                    zoom = this.current;
                    zoomValues = this.settings.zoomValues.filter((x) => x < zoom);
                    if (zoomValues.length > 0) {
                        zoom = Math.max(...zoomValues);
                    }
                    break;
                case "fit":
                    zoom = Math.min(this.get("width"), this.get("height"));
                    break;
                case "width":
                    zoom = this.settings.fillArea * this.viewer.$container.width() / $activepage.data("width");
                    break;
                case "height":
                    zoom = this.settings.fillArea * this.viewer.$container.height() / $activepage.data("height");
                    break;
                default:
                    zoom = this.current;
                    break;
            }
            return zoom;
        }

        /**
         * Sets the zoom value to each page (changes both the page and the content div); relies on the data-values for the page
         * @param {number} zoom - the zoom value to be set
         */
        zoomPages(zoom) {
            zoom = this.get(zoom);
            this.viewer.getPages().forEach(function (page) {
                let $page = page.$div;
                let c_width = $page.data("width");
                let c_height = $page.data("height");
                $page.width(c_width * zoom).height(c_height * zoom);
                $page.data('zoom', zoom);
                $page.find(`.${this.viewer.settings.contentClass}`).width(c_width * zoom).height(c_height * zoom);
            }.bind(this));
            this.current = zoom;
        }
    }

    class PDFjsViewer {
        /**
         * Constructs the object, and initializes actions:
         *   - add the scroll handler to the container
         *   - set the first adjusting action when the page is loaded
         *   - creates the zoom helper
         * @param {jQuery} $container the jQuery value that will hold the pages
         * @param {dictionary} options options for the viewer
         */
        constructor($container, options = {}) {

            let defaults = {
                visibleThreshold: 0.5,
                extraPagesToLoad: 3,
                // The class used for each page (the div that wraps the content of the page)
                pageClass: "pdfpage",
                // The class used for the content of each page (the div that contains the page)
                contentClass: "content-wrapper",
                // Function called when a document has been loaded and its structure has been created
                onDocumentReady: () => { },
                // Function called when a new page is created (it is binded to the object, and receives a jQuery object as parameter)
                onNewPage: (page, i) => { },
                // Function called when a page is rendered
                onPageRender: (page, i) => { },
                // Function called to obtain a page that shows an error when the document could not be loaded (returns a jQuery object)
                errorPage: () => {
                    $(`<div class="placeholder"></div>`).addClass(this.settings.pageClass).append($(`<p class="m-auto"></p>`).text("could not load document"))
                },
                // Posible zoom values to iterate over using "in" and "out"
                zoomValues: [0.25, 0.5, 0.75, 1, 1.25, 1.50, 2, 4, 8],
                // Function called when the zoom level changes (it receives the zoom level)
                onZoomChange: (zoomlevel) => { },
                // Function called whenever the active page is changed (the active page is the one that is shown in the viewer)
                onActivePageChanged: (page, i) => { },
                // Percentage of the container that will be filled with the page
                zoomFillArea: 0.95,
                // Function called to get the content of an empty page
                emptyContent: () => $('<div class="loader"></div>'),
                // Contador de inaterface de firmas
                Inconter_Stamp:1

            }

            this.settings = $.extend(defaults, options);

            // Create the zoomer helper
            this._zoom = new Zoomer(this, {
                zoomValues: this.settings.zoomValues,
                fillArea: this.settings.zoomFillArea,
            });

            // Store the container
            this.$container = $container;

            // Add a reference to this object to the container
            $container.get(0)._pdfjsViewer = this;

            // Add the event listeners
            this._setScrollListener();

            // Initialize some variables
            this.pages = [];
            this.pdf = null;
            //Aidición para las anotaciones
            //this.fabricObjects = [];
            //this.fabricObjectsData = [];
            this.url_firma = "";
            this.url_block;
            this.url_document = "";
            this.anotate_id_imagen = 0;
            this.anotate_cabinete_imagen = "";
            this.anotate_radicado = "";
            this.anotate_id_workflow = 0;
            this.anotate_desc_transacion = "";
            this.anotate_printer = 0;
            this.anotate_save = 0;
            this.anotate_add_firma = 0;
            this.anotate_add_stamp = 0;
            this.estate_anotate = 0;
            this.pages_anotate = [];
            this.rotate_page = 0;
            this.itex_config_stamp = null;
            
        }

        /**
         * Sets the current zoom level and applies it to all the pages
         * @param {number} zoom the desired zoom level, which will be a value (1 equals to 100%), or the keywords 'in', 'out', 'width', 'height' or 'fit'
         */
        setZoom(zoom) {
            let container = this.$container.get(0);

            // Get the previous zoom and scroll position
            let prevzoom = this._zoom.current;
            let prevScroll = {
                top: container.scrollTop,
                left: container.scrollLeft
            };

            // Now zoom the pages
            this._zoom.zoomPages(zoom);

            // Update the scroll position (to match the previous one), according to the new relationship of zoom
            container.scrollLeft = prevScroll.left * this._zoom.current / prevzoom;
            container.scrollTop = prevScroll.top * this._zoom.current / prevzoom;

            // Force to redraw the visible pages to upgrade the resolution
            if (this.estate_anotate == 0) {
                this._visiblePages(true);
            } else {
                this._visiblePages(true);
            }
            
            // Call the callback (if provided)
            if (typeof this.settings.onZoomChange === "function")
                this.settings.onZoomChange.call(this, this._zoom.current);
        }

        /**
         * Obtain the current zoom level
         * @returns {number} the current zoom level
         */
        getZoom() {
            return this._zoom.current;
        }

        PdfViwerAlert(message, type, name_control) {
            if (document.getElementById("viwer_alert_document")) {
                let element = document.getElementById("viwer_alert_document");
                element.remove();
            }
            const wrapper = document.createElement('div');
            wrapper.style.position = "absolute";
            wrapper.style.width = "100%";
            wrapper.id="viwer_alert_document";
            wrapper.innerHTML = [
                `<div class="alert alert-${type} alert-dismissible row" role="alert">`,
                `   <div class="col-11">${message}</div>`,
                ' <div class="col-1">  <button type="button" title="close" class="btn-close-person" onclick="pdfViewer.PdfViwerAlertClose()" ></button>  </div>',
                '</div>'
            ].join('')
           
            let alertPlaceholder = document.getElementById(name_control);
            if (alertPlaceholder) {
                alertPlaceholder.append(wrapper);
            }
        }
        PdfViwerAlertClose() {
            if (document.getElementById("viwer_alert_document")) {
                let element = document.getElementById("viwer_alert_document");
                element.remove();
            }
        }
        /**
         * Function that removes the content of a page and replaces it with the empty content (i.e. a content generated by function emptyContent)
         *   such content will not be visible except for the time that the 
         * @param {jQuery} $page the page to be emptied
         */
        _cleanPage($page) {
            let $emptyContent = this.settings.emptyContent();
            $page.find(`.${this.settings.contentClass}`).empty().append($emptyContent)
        }

        /**
         * Function that replaces the content with the empty class in a page with a new content
         * @param {*} $page the page to be modified
         * @param {*} $content the new content that will be set in the page
         */
        _setPageContent($page, $content) {
            $page.find(`.${this.settings.contentClass}`).empty().append($content)
        }

        /**
         *  Recalculates which pages are now visible and forces redrawing them (moreover it cleans those not visible) 
        */
        refreshAll() {
            this._visiblePages(true);
        }

        /** Function that creates a scroll handler to update the active page and to load more pages as the scroll position changes */
        _setScrollListener() {
            // Create a scroll handler that prevents reentrance if called multiple times and the loading of pages is not finished
            let scrollLock = false;
            let scrollPos = { top: 0, left: 0 };

            this.__scrollHandler = function (e) {
                // Avoid re-entrance for the same event while loading pages
                if (scrollLock === true) {
                    return;
                }
                scrollLock = true;
                let container = this.$container.get(0);
                if ((Math.abs(container.scrollTop - scrollPos.top) > (container.clientHeight * 0.2 * this._zoom.current)) ||
                    (Math.abs(container.scrollLeft - scrollPos.left) > (container.clientWidth * 0.2 * this._zoom.current))) {
                    scrollPos = {
                        top: container.scrollTop,
                        left: container.scrollLeft
                    }
                    this._visiblePages();
                }

                scrollLock = false;
            }.bind(this);

            // Set the scroll handler
            this.$container.off('scroll');
            this.$container.on('scroll', this.__scrollHandler);
        }
        /**
         * Function that creates the pageinfo structure for one page, along with the skeleton to host the page (i.e. <div class="page"><div class="content-wrapper"></div></div>)
         *   If the page is a pageinfo, the new pageinfo structure will not rely on the size (it will copy it, but it won't be marked as loaded). If it is a page, the size will
         *   be calculated from the viewport and it will be marked as loaded.
         *   This is done in this way, because when creating the pages in the first time, they will be created assuming that they are of the same size than the first one. If they
         *   are not, the size will be adjusted later, when the pages are loaded.
         * 
         * @param {*} page - the pageinfo (or the page) from which to create the pageinfo structure
         * @param {*} i - the number of the page to be created
         * @returns pageinfo - the pageinfo structure for the page
         */
        _createSkeleton(page, i) {
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
            this._cleanPage(pageinfo.$div);
            return pageinfo;
        }

        /**
         * This function places the page.$div in the container, according to its page number (i.e. it searches for the previous page and puts this page after)
         *   * in principle, this method sould not be needed because all the pages are put in order; but this is created just in case it is needed in further versions
         * @param {*} pageinfo - the pageinfo structure for the page (needs a valid $div)
         * @param {*} i - the number of the page
         */
        _placeSkeleton(pageinfo, i) {
            let prevpage = i - 1;
            let $prevpage = null;
            while ((prevpage > 0) && (($prevpage = this.$container.find(`.${this.settings.pageClass}[data-page="${prevpage}"]`)).length === 0)) {
                prevpage--;
            }
            if (prevpage === 0) {
                this.$container.append(pageinfo.$div);
            }
            else {
                $prevpage.after(pageinfo.$div);
            }
        }

        /**
         * Creates the initial skeletons for all the pages, and places them into the container
         * @param {page/pageinfo} pageinfo - the initial pageinfo (or page) structure
         */
        _createSkeletons(pageinfo) {
            for (let i = 1; i <= this.pageCount; i++) {
                if (this.pages[i] === undefined) {

                    // Create the pageinfo structure, store it and place it in the appropriate place (the next page will be created similar to the previous one)
                    pageinfo = this._createSkeleton(pageinfo, i);
                    this.pages[i] = pageinfo;
                    this._placeSkeleton(pageinfo, i);

                    // Call the callback function (if provided)
                    if (typeof this.settings.onNewPage === "function") {
                        this.settings.onNewPage.call(this, pageinfo.$div, i);
                    }
                }
            }
        }

        /**
         * Function to set the active page, and calling the callback (if provided)
         * @param {*} i - the number of the page to set active
         */
        _setActivePage(i) {
            if (this._activePage !== i) {
                this._activePage = i;
                if (typeof this.settings.onActivePageChanged === "function")
                    this.settings.onActivePageChanged.call(this, this.getActivePage(), i);
            }
        }

        /**
         * Obtains the area of a div that falls in the viewer
         * @param {*} $page - div whose area is to be calculated
         * @returns the visible area
         */
        _areaOfPageVisible($page) {
            if ($page === undefined) {
                return 0;
            }
            let c_offset = this.$container.offset();
            let c_width = this.$container.width();
            let c_height = this.$container.height();
            let position = $page.offset();
            position.top -= c_offset.top;
            position.left -= c_offset.left;
            position.bottom = position.top + $page.outerHeight();
            position.right = position.left + $page.outerWidth();
            let page_y0 = Math.min(Math.max(position.top, 0), c_height);
            let page_y1 = Math.min(Math.max($page.outerHeight() + position.top, 0), c_height);
            let page_x0 = Math.min(Math.max(position.left, 0), c_width);
            let page_x1 = Math.min(Math.max($page.outerWidth() + position.left, 0), c_width);
            let vis_x = page_x1 - page_x0;
            let vis_y = page_y1 - page_y0;
            return (vis_x * vis_y);
        }

        /**
         * Function that returns true if the page is considered to be visible (the amount of visible area is greater than the threshold)
         * @param {*} i - the number of page to check
         * @returns true if the page is visible
         */
        isPageVisible(i) {
            if ((this.pdf === null) || (i === undefined) || (i === null) || (i < 1) || (i > this.pdf.numPages)) {
                return false;
            }
            let $page = i;
            if (typeof i === "number") {
                if (this.pages[i] === undefined)
                    return false;
                $page = this.pages[i].$div;
            }
            return this._areaOfPageVisible($page) > ($page.outerWidth() * $page.outerHeight() * this.settings.visibleThreshold);
        }

        /**
         * Function that calculates which pages are visible in the viewer, draws them (if not already drawn), and clears those not visible
         * @param {*} forceRedraw - if true, the visible pages will be redrawn regardless of whether they are already drawn (useful for zoom changes)
         */
        _visiblePages(forceRedraw = false) {
            // Will grab the page with the greater visible area to set it as active
            //Tomará la página con el área visible más grande para configurarla como activa.
            let max_area = 0;
            let i_page = null;

            // If there are no visible pages, return -- Si no hay páginas visibles, regrese.
            if (this.pages.length === 0) {
                this._visibles = [];
                this._setActivePage(0);
                return;
            }

            // Calculate the visible area for each page and consider it visible if the visible area is greater than 0
            //Calcule el área visible para cada página y considérela visible si el área visible es mayor que 0.
            let $visibles = this.pages.filter(function (pageinfo) {
                let areaVisible = this._areaOfPageVisible(pageinfo.$div);
                if (areaVisible > max_area) {
                    max_area = areaVisible;
                    i_page = pageinfo.$div.data('page');
                }
                return areaVisible > 0;
            }.bind(this)).map((x) => x.$div);

            // Set the active page --Establece la página activa.
            this._setActivePage(i_page);

            // Now get the visible pages
            //Ahora obtenga las páginas visibles.
            let visibles = $visibles.map((x) => $(x).data('page'));
            if (visibles.length > 0) {
                // Now will add some extra pages (before and after) the visible ones, to have them prepared in case of scroll
                //Ahora añadiremos algunas páginas extra (antes y después) de las visibles, para tenerlas preparadas en caso de desplazamiento.
                let minVisible = Math.min(...visibles);
                let maxVisible = Math.max(...visibles);
                for (let i = Math.max(1, minVisible - this.settings.extraPagesToLoad); i < minVisible; i++) {
                    if (!visibles.includes(i))
                        visibles.push(i)
                }
                for (let i = maxVisible + 1; i <= Math.min(maxVisible + this.settings.extraPagesToLoad, this.pdf.numPages); i++) {
                    if (!visibles.includes(i))
                        visibles.push(i)
                }
            }

            // Now will draw the visible pages, but if not forcing, will only draw those that were not visible before
            //Ahora dibujará las páginas visibles, pero si no las fuerza, solo dibujará aquellas que antes no eran visibles.
            let nowVisibles = visibles;
            if (!forceRedraw) {
                nowVisibles = visibles.filter(function (x) {
                    return !this._visibles.includes(x)
                }.bind(this));
            }

            // Get the pages that were visible before, that are not visible now, and clear them -
            //Obtenga las páginas que eran visibles antes, que no lo son ahora y elimínelas.
            this._visibles.filter(function (x) {
                return !visibles.includes(x)
            }).forEach(function (i) {
                this._cleanPage(this.pages[i].$div);
            }.bind(this))

            // Store the new visible pages - Almacene las nuevas páginas visibles.
            this._visibles = visibles;

            // And now we'll queue the pages to load -Y ahora pondremos en cola las páginas para cargar.
            this.loadPages(...nowVisibles);
        }

        /**
         * Function queue a set of pages to be loaded; if not loading, the function starts the loading worker
         * @param  {...pageinfo} pages - the pages to load
         */
        loadPages(...pages) {
            this._pagesLoading.push(...pages);
            if (this._loading) {
                return;
            }
            this._loadingTask();
        }

        /**
         * Function that gets the pages pending to load and renders them sequentially (to avoid multiple rendering promises)
         */
        _loadingTask() {
            this._loading = true;
            if (this._pagesLoading.length > 0) {
                let pagei = this._pagesLoading.shift();
                this.pdf.getPage(pagei).then(function (page) {
                    // Render the page and update the information about the page with the loaded values  if (this.pages_anotate.push({ "page ": page, "page_number": i }))
                    let swuit_page = -1;     
                    if (this.pages_anotate.length > 0) {
                        for (let k = 0; k < this.pages_anotate.length; k++)  {
                            if (page._pageIndex == this.pages_anotate[k]._pageIndex) {
                                swuit_page = k;             
                            }
                        }
                    }
                    if (swuit_page != -1) {      
                        this._renderPage(this.pages_anotate[swuit_page].page, pagei);
                    } else {
                        this._renderPage(page, pagei);
                    }
                    
                }.bind(this)).then(function (pageinfo) {
                    // Once loaded, we are not loading anymore
                    if (this._pagesLoading.length > 0) {
                        this._loadingTask();
                    }
                }.bind(this));
            }
            // Free the loading state
            this._loading = false;
        }

        /**
         * Function that sets the scroll position of the container to the specified page
         * @param {*} i - the number of the page to set the scroll position
         */
        scrollToPage(i) {
            if ((this.pages.length === 0) || (this.pages[i] === undefined)) {
                return;
            }
            let $page = this.pages[i].$div;
            if ($page.length === 0) {
                console.warn(`Page ${i} not found`);
                return;
            }
            let position = $page.position();
            if (position !== undefined) {
                this.$container.get(0).scrollTop = this.$container.get(0).scrollTop + position.top;
                this.$container.get(0).scrollLeft = this.$container.get(0).scrollLeft + position.left;
            }
            this._setActivePage(i);
        }
        _page_anotate() {
            let page_brus = this.pages[this._activePage].$div;
            let k = page_brus[0].firstElementChild;
            let intance = this;
            if (page_brus) {
                let fabricObj;
                let imagefabre;
                var inputElement = document.createElement("input");
                inputElement.type = 'file'
                inputElement.accept = ".jpg,.jpeg,.png,.PNG,.JPG,.JPEG";
                inputElement.onchange = async function () {
                    var reader = new FileReader();
                    reader.addEventListener("load", function () {
                        inputElement.remove();
                        var image = new Image();
                        image.onload = function () {
                            var background = k.firstElementChild.toDataURL("image/png");
                            fabricObj = intance.fabricObjects[0];
                            var frabicimage = new fabric.Image(image, {})

                            //fabricObj.add(new fabric.Image(image, {

                            //}))
                            //const rect = new fabric.Rect({
                            //   height: 280,
                            //   width: 200,
                            //    fill: 'yellow'
                            //});
                            //fabricObj.add(frabicimage).renderAll();
                            var image_ = new Image();
                            image_.src = background;
                            var oImg = new fabric.Image(image_);
                            fabricObj.setBackgroundImage(oImg, fabricObj.renderAll.bind(fabricObj));
                            //document.getElementById("rr").src = background;
                            var t = 1;
                            //var background = k.firstElementChild.toDataURL("image/png");
                            //var image_ = new Image();
                            //image_.src = background;
                            //document.getElementById("rr").src = background;
                            //var oImg = new fabric.Image(image_);
                            //fabricObj.setBackgroundImage(oImg, fabricObj.renderAll.bind(fabricObj));
                            //var z = k.firstElementChild;
                            //var image_ = new Image();
                            //image_.src = background;
                            //document.getElementById("rr").src = background;
                            //var oImg = new fabric.Image(image_);
                            //fabricObj.setBackgroundImage(oImg, fabricObj.renderAll.bind(fabricObj));
                            /*var background = k.firstElementChild.toDataURL("image/png");
                            fabricObj = intance._page_fabric(k);
                            fabricObj.add(new fabric.Image(image, {
                            }))
                            var z = k.firstElementChild;
                            
                            var image_ = new Image();
                            image_.src = background;
                            document.getElementById("rr").src = background;
                            var oImg = new fabric.Image(image_);
                            fabricObj.setBackgroundImage(oImg, fabricObj.renderAll.bind(fabricObj));*/
                        }



                        //var background = k.firstElementChild.toDataURL("image/png");
                        //   var image_ = new Image();
                        //    image_.src = background;
                        //    var oImg = new fabric.Image(image_);
                        //oImg.scale(0.8, 0.8);
                        //oImg.height = (HEIG_VIEWPORT * 10);
                        //oImg.width = (WIDTH_VIEWPORT * 10);
                        //    fabricObj.setBackgroundImage(oImg, fabricObj.renderAll.bind(fabricObj));
                        //inst.marcado[active_canvas - 1] = 1;

                        image.src = this.result;
                    }, false);
                    reader.readAsDataURL(inputElement.files[0]);
                }
                //var t = await render_page();
                document.getElementsByTagName('body')[0].appendChild(inputElement)
                inputElement.click()
            }
        }
        _page_fabric(i) {
            let id_canvas = 'page-' + (1) + '-canvas';
            var background = i.firstElementChild.toDataURL("image/png");
            var rl = i.firstElementChild;
            var fabricObj = new fabric.Canvas(i.firstElementChild, {
                freeDrawingBrush: {
                    width: 10,
                    color: '#212121'
                }
            });
            var image_ = new Image();
            image_.src = background;
            document.getElementById("rr").src = background;
            var oImg = new fabric.Image(image_);
            fabricObj.setBackgroundImage(oImg, fabricObj.renderAll.bind(fabricObj));
            this.fabricObjects.push(fabricObj);
            return fabricObj;
        }
        _Add_captureFromSTU() {
            if (this.anotate_add_firma == 0) {
                this.PdfViwerAlert("Usuario sin permiso para adjuntar firma de tableta digital", "warning", "container-general");
                return true;
            }
            captureFromSTU();
        }
        /**
         * 
         * @param {any} utili_elimina
         * @param {any} topconten_
         * @param {any} lefconten_
         */
        /** Agrega la interface de anotacion */
        _Add_interface_anotate(utili_elimina, topconten_, lefconten_) {
            if (this.anotate_add_firma == 0) {
                this.PdfViwerAlert("Usuario sin permiso para adjuntar firma de grafo", "warning","container-general");
                return true;
            }
            //Solicita el div de la pagina activa
            let content_canvas_ = this.getActivePage();
            let content_canvas = content_canvas_[0];
            if (content_canvas == null) {
                return true;
            }
            let id_div_dag = "div-drav-image-" + this.settings.Inconter_Stamp;
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
            //ahtml.setAttribute("parent_page", active_canvas);
            ahtml.setAttribute("ur_image_drag", this.ur_firma_user);
            ahtml.setAttribute("ur_image_drag64", this.ur_data64);
            ahtml.setAttribute("event_drag", "save");
            ahtml.appendChild(ihtml);
            div_barra.appendChild(ahtml);
          
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
            if (this.ur_firma_user !== "") {
                image_sr.src = this.ur_firma_user;
            } else {
                image_sr.src = this.ur_data64;
            }
            content_canvas.appendChild(div_drad);
            if (utili_elimina !== 1) {
                this.settings.Inconter_Stamp ++;
            }
            image_sr.onload = function () {
                img_drag.src = image_sr.src;
            };
           
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
            div_drad.style.top = topconten + "px";
            div_drad.style.left = lefconten + "px";
            if (this.isInViewport(div_drad) == false) {
                div_drad.scrollIntoView();
            }
            $("#" + id_div_dag).css({ opacity: 0.5 });
            //left = $("#" + id_div_dag).position.left;
            //top = $("#" + id_div_dag).position.top;
            $("#" + id_div_dag).draggable({
                containment: $("#" + content_canvas.id),
                stop: function (event, ui) {

                }
            }
            );
            $("#" + id_div_dag).resizable({
                maxHeight: 80, maxWidth: 100, minWidth: 50, minHeight: 50,
                start: function (event, ui) {
                    
                },
                stop: function (event, ui) {
                    /*$("#" + id_div_dag).css('position', 'relative');
                    var dragab = $("#" + id_div_dag);
                    var contenido = $("#" + content_canvas.id);
                    var scr = contenido.scrollTop();
                    var scrolleft = contenido.scrollLeft();
                    left = left - scrolleft;
                    var posicfinal = (top + scr) - 10;*/
                },
                resize: function (event, ui) {
                 
                }
            }
            );
        }
        isInViewport(elem) {
        if (elem) {
            var distance = elem.getBoundingClientRect();
            return (
                distance.top < (window.innerHeight || document.documentElement.clientHeight) && distance.bottom > 10
            );
        }
        }
        /**
         * Descarga pdf
         * 
        */
        _PropertiTransac() {
            let currentX = 0;
            let currentY = 0;
            let anotate_width = 0;
            let anotate_height = 0;
            let pdf_file_width = 0;
            let pdf_file_height = 0;
            let pdf_file_page = "";
            let anotate_file_src = "";
            let anotate_scale = pdfViewer.getZoom();
            let anotate_file_bit = "";
            let pdf_anotate_id_imagen = this.anotate_id_imagen;
            let pdf_anotate_cabinete_imagen = this.anotate_cabinete_imagen;
            let pdf_anotate_radicado = this.anotate_radicado;
            let pdf_anotate_id_workflow = this.anotate_id_workflow;
            let pdf_anotate_desc_transacion = this.anotate_desc_transacion;
            let array_parameter = new Array();
            array_parameter.push({
                "pdf_file_src": pdfViewer.getUrlPage(), "pdf_file_page": pdf_file_page, "anotate_file_src": anotate_file_src,
                "anotate_file_bit": anotate_file_bit, "anotate_width": anotate_width, "anotate_heigth": anotate_height,
                "anotate_x": currentX, "anotate_y": currentY, "anotate_scale": anotate_scale, "anotate_type": "img",
                "pdf_file_width": pdf_file_width, "pdf_file_heigth": pdf_file_height, "anotate_id_imagen": pdf_anotate_id_imagen,
                "anotate_cabinete_imagen": pdf_anotate_cabinete_imagen, "anotate_radicado": pdf_anotate_radicado, "anotate_id_workflow": pdf_anotate_id_workflow,
                "anotate_desc_transacion": pdf_anotate_desc_transacion
            });
            return array_parameter;
        }
        _DonwloadPage() {
            if (this.anotate_save == 0) {
                this.PdfViwerAlert("Usuario sin permisos para descargar el documento", "warning", "container-general");
                return true;
            }
            let array_parameter = pdfViewer._PropertiTransac()
            let url_page = pdfViewer.getUrlPage();
            service_download_pdf_visor_plus(url_page, array_parameter,"Descarga");
        }
        /**
         * 
        Printer pdf
         */
          _PrinterPdf() {
            if (this.anotate_printer == 0) {
                this.PdfViwerAlert("Usuario sin permisos para imprimir el documento", "warning", "container-general");
                return true;
            }
            let array_parameter = pdfViewer._PropertiTransac()
            let url_page = pdfViewer.getUrlPage();
            service_printer_pdf_visor_plus(url_page, array_parameter, "Imprime");
            
            
        }
        /**
        _
         updatte page width url  this.pdf.getPage(pagei)
         */
        async  _updatePage(i,document) {
            let loadingTask = pdfjsLib.getDocument(document);
            let pdf_ = await loadingTask.promise;
            let page = await pdf_.getPage(1);
            this._renderPage(page, i);
            this._add_anotate_page(page, i);      
        }
        //Agrega las paginas con anotacion
        async  _add_anotate_page(page, i) {
            let state = -1;
            if (this.pages_anotate.length > 0) {
                for (let k = 0; k < this.pages_anotate.length; k++) {
                    if (this.pages_anotate[k].page_number == i) {
                        state = k;
                    }
                }
            }
            if (state == -1) {
                let page_ = await this.pdf.getPage(i);
                this.pages_anotate.push({
                    "page": page, "page_number": i, "_pageIndex": page_._pageIndex
                });        
            } else {
                let page_ = await this.pdf.getPage(i);
                this.pages_anotate[state].page = page;
            }
            
            return true;
        }
        /**
         * Function that renders the page in a canvas, and sets the canvas into the $div
         * @param {*} page - the page to be rendered
         * @param {*} i - the number of the page to be rendered
         * @returns a promise to render the page (the result of the promise will be the pageinfo)
         */
        _renderPage(page, i) {
            // Get the pageinfo structure
            let pageinfo = this.pages[i];
            // Calculate the pixel ratio of the device (we'll use a minimum of 1)
            let pixel_ratio = Math.max(window.devicePixelRatio || 1, 1);

            // Update the information that we know about the page to the actually loaded page
            let viewport = page.getViewport({ rotation: this._rotation, scale: this._zoom.current * pixel_ratio });
            pageinfo.width = (viewport.width / this._zoom.current) / pixel_ratio;
            pageinfo.height = (viewport.height / this._zoom.current) / pixel_ratio;
            pageinfo.$div.data("width", pageinfo.width);
            pageinfo.$div.data("height", pageinfo.height);
            pageinfo.loaded = true;

            // Create the canvas and prepare the rendering context
            let $canvas = $('<canvas></canvas>');
            let canvas = $canvas.get(0);
            canvas.setAttribute("id", 'page-' + (i) + '-canvas');
            canvas.setAttribute("pg", (i));
            let context = canvas.getContext('2d');
            canvas.height = viewport.height; // * pixel_ratio;
            canvas.width = viewport.width; //  * pixel_ratio;    
            canvas.getContext("2d"); //.scale(pixel_ratio, pixel_ratio);
            var renderContext = {
                canvasContext: context,
                viewport: viewport
            };
          
            // Render the page and put the resulting rendered canvas into the page $div
            return page.render(renderContext).promise.then(function () {
                this._setPageContent(pageinfo.$div, $canvas);
                // Call the callback (if provided)
                if (typeof this.settings.onPageRender === "function") {
                    this.settings.onPageRender.call(this, pageinfo.$div, i);
                }
                 // Add contex menu page pageinfo.$div
                let div_conten = $canvas;
                div_conten[0].addEventListener("contextmenu", printMousePos);
                context_menu(div_conten[0].id);
                return pageinfo;
            }.bind(this));
        }

        /** Gets the div object corresponding to the active page */
        getActivePage() {
            if ((this._activePage === null) || (this.pdf === null)) {
                return null;
            }
            if ((this._activePage < 1) || (this._activePage > this.pdf.numPages)) {
                return null;
            }
            return this.pages[this._activePage].$div;
        }
        getUrlPage() {
            return this.url_document;
        }

        /** Gets all the pages of the document (the pageinfo structures) */
        getPages() {
            return this.pages;
        }

        /** Gets the number of pages of the document */
        getPageCount() {
            if (this.pdf === null) {
                return 0;
            }
            return this.pdf.numPages;
        }
        //** Gets number page active
        getActivePageNumber() {
            return this._activePage;
        }
        /** Scrolls to the next page (if any) */
        next() {
            if (this._activePage < this.pdf.numPages) {
                this.scrollToPage(this._activePage + 1);
            }
        }

        /** Scrolls to the previous page (if any) */
        prev() {
            if (this._activePage > 1) {
                this.scrollToPage(this._activePage - 1);
            }
        }

        first() {
            if (this._activePage !== 1) {
                this.scrollToPage(1);
            }
        }

        last() {
            if (this.pdf === null)
                return;
            if (this._activePage !== this.pdf.numPages) {
                this.scrollToPage(this.pdf.numPages);
            }
        }
        /**
         * Rotates the pages of the document
         * @param {*} deg - degrees to rotate the pages
         * @param {*} accumulate - whether the rotation is accumulated or not
         */
        rotate(deg, accumulate = false) {
            if (accumulate) {
                deg = deg + this._rotation;
            }

            this._rotation = deg;
            let container = this.$container.get(0);
            let prevScroll = {
                top: container.scrollTop,
                left: container.scrollLeft,
                height: container.scrollHeight,
                width: container.scrollWidth
            };
            return this.forceViewerInitialization().then(function () {
                let newScroll = {
                    top: container.scrollTop,
                    left: container.scrollLeft,
                    height: container.scrollHeight,
                    width: container.scrollWidth
                };
                container.scrollTop = prevScroll.top * (newScroll.height / prevScroll.height);
                container.scrollLeft = prevScroll.left * (newScroll.width / prevScroll.width);
            }.bind(this));

        }
        /**
         * This functions forces the creation of the whole content of the viewer (i.e. new divs, structures, etc.). It is usefull for full refresh of the viewer (e.g. when changes
         *   the rotation of the pages)
         * @returns a promise that is resolved when the viewer is fully initialized
         */
        forceViewerInitialization() {
            // Store the pdf file
            // Now prepare a placeholder for the pages
            this.pages = [];
            // Remove all the pages
            this.$container.find(`.${this.settings.pageClass}`).remove();
            this._pagesLoading = [];
            this._loading = false;
            this._visibles = [];
            this._activePage = null;
            return this.pdf.getPage(1).then(function (page) {
                this._createSkeletons(page);
                this._visiblePages();
                this._setActivePage(1);
            }.bind(this));
        }
        /** 
         * Loads the document and creates the pages
         * @param {string} document - the url of the document to load
         */
        load_config_stamp_graf() {
            service_config_itex_stamp_interface();
        }
        async loadDocument(document, url_src_fromat, ur_firma_user, array_parameter) {
            // Now prepare a placeholder for the pages       
            this.pages = [];
            this.url_document = url_src_fromat;
            this.url_block = document;
            // Remove all the pages
            this.$container.find(`.${this.settings.pageClass}`).remove();
            //parameter view
            this.anotate_id_imagen = array_parameter[0].url_id_imagen;
            this.anotate_cabinete_imagen = array_parameter[0].url_cabinete_imagen;
            this.anotate_radicado = array_parameter[0].url_radicado;
            this.anotate_id_workflow = array_parameter[0].url_id_workflow;
            this.anotate_desc_transacion = array_parameter[0].url_desc_transacion;
            this.anotate_save = array_parameter[0].url_save;
            this.anotate_add_stamp = array_parameter[0].url_add_stamp;
            this.anotate_printer = array_parameter[0].url_printer;
            this.anotate_add_firma = array_parameter[0].url_add_firma;
            // Let's free the pdf file (if there was one before), and rely on the garbage collector to free the memory
            this.pdf = null;
            // let' s asing url stamp user
            this.ur_firma_user = ur_firma_user;
            
            // Load the task and return the promise to load the document
            let loadingTask =  pdfjsLib.getDocument(document);
            
            return loadingTask.promise.then(function (pdf) {
                // Store the pdf file and get the 
                this.pdf = pdf;
                this.pageCount = pdf.numPages;
                this._rotation = 0;
                return this.forceViewerInitialization();
            }.bind(this)).then(function () {
                if (typeof this.settings.onDocumentReady === "function") {
                    this.settings.onDocumentReady.call(this);
                }
            }.bind(this));
        }
        /** acive windows config itex stamp */
        config_stamp() {
            service_configuracion_itex_stamp_user();
        }

        UpdateInstConfigStamp() {
            save_config_itex_stamp();
        }
    }
    exports.PDFjsViewer = PDFjsViewer;
})(window, jQuery)
//***********************************************************************
//Configuración de eventos 
//***********************************************************************
let STRU_ANOTATE_IMAGE = new Array();
async function event_drag_anotate(event, element) {
    let value_id_parent = $(element).attr("parent_id");
    let page_drag = pdfViewer.getActivePageNumber();
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
            let currentTop = drag_div_parent.style.top.replace("px", "");
            let bar_heigt = div_bar.style.height.replace("px", "");
            let currentX = drag_div_parent.style.left.replace("px", "");
            //let bar_heigt = div_bar.style.height.replace("px", "");
            //let currentX = lef_eventX;
            //let currentY = Top_eventY;
            //alto del canvas de la imagen
            let canvas_heigth = drag_canvas.clientHeight;
            let currentY = canvas_heigth - currentTop; // resta al ancho del lienzo del canvas a la posición para que se ajuste a la posición del de itex
            let canvas_width = drag_canvas.clientWidth;
            //currentX = canvas_width - currentX;
            let anotate_width = div_imag.clientWidth;
            let anotate_height = div_imag.clientHeight;
            let pdf_file_width = drag_canvas.clientWidth;
            let pdf_file_height = drag_canvas.clientHeight;
            let pdf_file_page = page_drag;
            let anotate_file_src = ur_image_drag;
            let anotate_scale = pdfViewer.getZoom();
            let anotate_file_bit = ur_image_drag_bit64;
            let pdf_anotate_id_imagen = pdfViewer.anotate_id_imagen;
            let pdf_anotate_cabinete_imagen = pdfViewer.anotate_cabinete_imagen;
            let pdf_anotate_radicado = pdfViewer.anotate_radicado;
            let pdf_anotate_id_workflow = pdfViewer.anotate_id_workflow;
            let pdf_anotate_desc_transacion = pdfViewer.anotate_desc_transacion;
            let pdf_rotate_ = pdfViewer._rotation;
            let itex_value_stamp_transparent = 100;
            let itex_aplicate_transparent_stamp = 1;
            if (pdfViewer.itex_config_stamp[0].error_sistema == "YES") {
                itex_value_stamp_transparent = pdfViewer.itex_config_stamp[0].itex_value_stamp_transparent;
                itex_aplicate_transparent_stamp = pdfViewer.itex_config_stamp[0].itex_aplicate_transparent_stamp;
            }
            let res = confirm("Desea guadar la anotación de grafo");
            if (res == false) {
                return "";
            }
            let res_sav = save_anotate_firma(currentX, currentY, anotate_width, anotate_height, pdf_file_page, anotate_file_src, anotate_file_bit,
                anotate_scale, pdf_file_width, pdf_file_height, pdf_anotate_id_imagen, pdf_anotate_cabinete_imagen, pdf_anotate_radicado,
                pdf_anotate_id_workflow, pdf_anotate_desc_transacion, pdf_rotate_, itex_aplicate_transparent_stamp,
                itex_value_stamp_transparent
            );
            if (res_sav == "YES") {
               drag_div_parent.parentElement.removeChild(drag_div_parent);
            }

        }

    }
}
function save_anotate_firma(currentX, currentY, anotate_width, anotate_height, pdf_file_page, anotate_file_src, anotate_file_bit, anotate_scale, pdf_file_width, pdf_file_height,
    pdf_anotate_id_imagen, pdf_anotate_cabinete_imagen, pdf_anotate_radicado, pdf_anotate_id_workflow,
    pdf_anotate_desc_transacion, _rotation, _aplica_transparente, _num_tranaparent) {
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
                "pdf_file_src": pdfViewer.getUrlPage(), "pdf_file_page": pdf_file_page, "anotate_file_src": anotate_file_src,
                "anotate_file_bit": anotate_file_bit, "anotate_width": anotate_width, "anotate_heigth": anotate_height,
                "anotate_x": currentX, "anotate_y": currentY, "anotate_scale": anotate_scale, "anotate_type": "img",
                "pdf_file_width": pdf_file_width, "pdf_file_heigth": pdf_file_height, "anotate_id_imagen": pdf_anotate_id_imagen,
                "anotate_cabinete_imagen": pdf_anotate_cabinete_imagen, "anotate_radicado": pdf_anotate_radicado, "anotate_id_workflow": pdf_anotate_id_workflow,
                "anotate_desc_transacion": pdf_anotate_desc_transacion, "rotation": _rotation, "aplica_transparente": _aplica_transparente,
                "num_tranaparent" : _num_tranaparent
            });
            service_anotate_pdf_image(pdf_file_page);
        }
        function _ResetCanvas() {
            ctx.fillStyle = 'transparent';
            ctx.fillRect(0, 0, canvas.width, canvas.height);
        }
        function _DrawImage(star_img) {
            //ctx.drawImage(star_img, currentX, currentY);
            //cache_inst.replace_page();

        }
        return "YES";
    } catch (ex) {
        return ex.message;
    }
}
let Top_event=0;
let lef_event=0;
function printMousePos(event) {    
    lef_event = event.offsetX;
    Top_event = event.offsetY;
}
function context_menu(id_div_dag) {
    try {
        $('#' + id_div_dag).contextMenu('context-menu-1' + id_div_dag, {
            'Tableta digital': {
                click: function (element) {
                    /*let currentY = document.getElementById("context-menu-1" + id_div_dag).style.top.replace("px", "");
                    let currentX = document.getElementById("context-menu-1" + id_div_dag).style.left.replace("px", "");
                    let currentW = document.getElementById("context-menu-1" + id_div_dag).clientWidth;
                    let num_active_page = pdfViewer.getActivePageNumber();
                    let name_element = "page-" + num_active_page + "-canvas";
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
                    }*/
                    LEFT_WACOM = lef_event;
                    TOP_WACOM = Top_event;
                    pdfViewer._Add_captureFromSTU();
                },
                klass: "fad fa-edit"
            },
            'firma personal': {
                click: function (element) {  
                    let currentX = lef_event;
                    let currentY = Top_event;
                    pdfViewer._Add_interface_anotate(1, Top_event, lef_event);
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
const asign_connfig_itext_stamp = (itex_config_stamp) => {
    let select_itext_stamp = document.getElementById("select_itext_stamp");
    for (let i = 0; i < itex_config_stamp[0].item_itext.length; i++) {
        let opt = document.createElement("OPTION");
        opt.text = itex_config_stamp[0].item_itext[i].text;
        opt.value = itex_config_stamp[0].item_itext[i].value;
        if (itex_config_stamp[0].itex_value_stamp_transparent == itex_config_stamp[0].item_itext[i].value) {
            opt.selected = true;
        }
        select_itext_stamp.add(opt);
    }   
    let checkbox_itext_stamp = document.getElementById("checkbox_itext_stamp");
    if (itex_config_stamp[0].itex_aplicate_transparent_stamp == 1) {
        checkbox_itext_stamp.checked = true;
    } else {
        checkbox_itext_stamp.checked = false;
    }
    $('#exampleModal').modal('show');
}
const save_config_itex_stamp = () => {
    let select_itext_stamp = document.getElementById("select_itext_stamp");
    let texto_campo = select_itext_stamp.options[select_itext_stamp.selectedIndex].text;
    let valor_campo = select_itext_stamp.options[select_itext_stamp.selectedIndex].value;
    let checkbox_itext_stamp = document.getElementById("checkbox_itext_stamp");
    let chek_itent = 1;
    if (checkbox_itext_stamp.checked == true) {
        chek_itent = 1;
    } else {
        chek_itent = 0;
    }
    let array_config_item_stamp = new Array();
    array_config_item_stamp.push({
        'id_user_config': 0, 'itex_value_stamp_transparent': valor_campo, 'itex_aplicate_transparent_stamp': chek_itent
        , 'item_itext': null, 'error_sistema' : "YES"}
    )
    service_update_insert_configuracion_itex_stamp_user(array_config_item_stamp);
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
                        pdfViewer.PdfViwerAlert(data.d[0].error_sistema, "warning", "container-general");
                        return "";
                    } else {
                        pdfViewer.estate_anotate = 1;
                        pdfViewer._updatePage(pdf_file_page, data.d[0].url_salida);
                        if (data.d[0].error_log != "YES") {
                            pdfViewer.PdfViwerAlert("El grafo se registro correctamente en el documento y se vera reflejado en el documento, pero el sistema no pudo registrar el log  por este error: " + data.d[0].error_log, "warning", "container-general");
                        }
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
function service_download_pdf_visor_plus(url_pdf, parameter_log, operation) {
    try {
        var serialice = JSON.stringify(parameter_log);
        $.ajax('../../webservice/WebService_itext.asmx/service_download_pdf_visor_plus', {
            data: "{'anotate':" + "'" + url_pdf + "','parameter_log':'" + serialice + "','operation':'" + operation + "'}",
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
                        if (data.d[0].error_log != "YES") {
                            alert("Se descargo el documento, con error en el log de descarga : " + data.d[0].error_log);
                        }
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
function service_printer_pdf_visor_plus(url_pdf, parameter_log, operation) {
    try {
        var serialice = JSON.stringify(parameter_log);
        $.ajax('../../webservice/WebService_itext.asmx/service_download_pdf_visor_plus', {
            data: "{'anotate':" + "'" + url_pdf + "','parameter_log':'" + serialice + "','operation':'" + operation + "'}",
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
                        var CurrentPathName = unescape(location.pathname);
                        var CurrentPath = CurrentPathName.substring(1, CurrentPathName.lastIndexOf("/") + 1);
                        let spli_ = CurrentPath.split("/");
                        let url_printer = location.origin +  data.d[0].url_path;
                        printJS({ printable: url_printer, type: 'pdf' });
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
        alert('service_printer_pdf_visor_plus  ' + ex.message);
    }
}
async function service_config_itex_stamp_interface () {
    let error_array = new Array();
    let error_;
    let myPromise = new Promise(function (resolve_) {
        $.ajax('../../webservice/WebService_itext.asmx/service_solicita_datos_configuracion_itex_stamp_user', {
            data: "{'anotate':" + "'" + 0 + "'}",
            dataType: 'json',
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                resolve_(data.d);

            }, error: function (xception, textStatus, errorThrown) {

                if (xception.status === 0) {

                    error = 'Not connect: Verify Network. ';

                } else if (xception.status == 404) {

                    error = 'Requested page not found [404]';

                } else if (xception.status == 500) {

                    error = 'Internal Server Error [500].' + xception.responseText;

                } else if (textStatus === 'parsererror') {

                    error = 'Requested JSON parse failed.';

                } else if (textStatus === 'timeout') {

                    error = 'Time out error.';

                } else if (textStatus === 'abort') {

                    error = 'Ajax request aborted.';

                } else {

                    error = 'Uncaught Error: ' + xception.responseText;

                }
                error_ = " funcion config " + error;
                error_array.push({ 'error_sistema': error_ });
                resolve_(error_array);
            }
        });
    })
    pdfViewer.itex_config_stamp = await myPromise;
    if (pdfViewer.itex_config_stamp[0].error_sistema != "YES") {
        pdfViewer.PdfViwerAlert(pdfViewer.itex_config_stamp[0].error_sistema, "warning", "container-general");
    } else {
        asign_connfig_itext_stamp(pdfViewer.itex_config_stamp);
    }
}
async function service_configuracion_itex_stamp_user() {
    let error_array = new Array();
    let error_;
    let myPromise = new Promise(function (resolve_) {
        $.ajax('../../webservice/WebService_itext.asmx/service_solicita_datos_configuracion_itex_stamp_user', {
            data: "{'anotate':" + "'" + 0 + "'}",
            dataType: 'json',
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                resolve_(data.d);
               
            }, error: function (xception, textStatus, errorThrown) {
               
                if (xception.status === 0) {
                    
                    error= 'Not connect: Verify Network. ';

                } else if (xception.status == 404) {

                    error = 'Requested page not found [404]';

                } else if (xception.status == 500) {

                    error = 'Internal Server Error [500].' + xception.responseText;

                } else if (textStatus === 'parsererror') {

                    error = 'Requested JSON parse failed.';

                } else if (textStatus === 'timeout') {

                    error = 'Time out error.';

                } else if (textStatus === 'abort') {

                    error = 'Ajax request aborted.';

                } else {

                    error = 'Uncaught Error: ' + xception.responseText;

                }
                error_ = " funcion config " + error;
                error_array.push({ 'error_sistema' : error_ });
                resolve_(error_array);
            }
        });
    })
    pdfViewer.itex_config_stamp = await myPromise;
    if (pdfViewer.itex_config_stamp[0].error_sistema != "YES") {
        pdfViewer.PdfViwerAlert(pdfViewer.itex_config_stamp[0].error_sistema, "warning", "container-general");
    }
    
}
async function service_update_insert_configuracion_itex_stamp_user(array_item) {
    let error_array = new Array();
    let error_;
    var serialice = JSON.stringify(array_item);
    let myPromise = new Promise(function (resolve_) {
        $.ajax('../../webservice/WebService_itext.asmx/service_update_insert_configuracion_itex_stamp_user', {
            data: "{'anotate':" + "'" + serialice + "'}",
            dataType: 'json',
            type: "POST",
            traditional: true,
            processData: false,
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                resolve_(data.d);

            }, error: function (xception, textStatus, errorThrown) {

                if (xception.status === 0) {

                    error = 'Not connect: Verify Network. ';

                } else if (xception.status == 404) {

                    error = 'Requested page not found [404]';

                } else if (xception.status == 500) {

                    error = 'Internal Server Error [500].' + xception.responseText;

                } else if (textStatus === 'parsererror') {

                    error = 'Requested JSON parse failed.';

                } else if (textStatus === 'timeout') {

                    error = 'Time out error.';

                } else if (textStatus === 'abort') {

                    error = 'Ajax request aborted.';

                } else {

                    error = 'Uncaught Error: ' + xception.responseText;

                }
                error_ = " funcion config " + error;
                error_array.push({ 'error_sistema': error_ });
                resolve_(error_array);
            }
        });
    })
    pdfViewer.itex_config_stamp = await myPromise;
    if (pdfViewer.itex_config_stamp[0].error_sistema != "YES") {
        pdfViewer.PdfViwerAlert(pdfViewer.itex_config_stamp[0].error_sistema, "warning", "exampleModal");
    } else {
        pdfViewer.itex_config_stamp[0].itex_value_stamp_transparent = array_item[0].itex_value_stamp_transparent;
        pdfViewer.itex_config_stamp[0].itex_aplicate_transparent_stamp = array_item[0].itex_aplicate_transparent_stamp;
        $('#exampleModal').modal('hide');
    }

}

