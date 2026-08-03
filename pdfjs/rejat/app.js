/* eslint-disable */

/**
 * More work need to be done to make it npmable, so that other projects might also use this
 * 
 * THis code is only working because we have added follwing code snippets here in index.html
 *  <script src="https://cdn.jsdelivr.net/npm/pdfjs-dist@2.5.207/build/pdf.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/pdfjs-dist@2.1.266/web/pdf_viewer.min.js"></script>
 */
/**
 * Code source: https://github.com/mozilla/pdf.js/blob/master/examples/mobile-viewer/viewer.js
 */

/**
  * Why so much effort ?

  Pdf preview doesnot works constitently for the browsers across mobile and desktop clients.
  Mozilla have done a great job of wrting a pdf previewer based on readable steam.
  There are no good ports available. There was one(https://github.com/mikecousins/react-pdf-js/blob/master/README.md) , but was very restrictive in the way you can use it.
  Hence I have to write my own port of mozilla pdf.js
  */
import React, { useState, useEffect } from "react";
import styled, { css } from "styled-components";

// import './previewtest.css';

function getCustomIdname(props) {
    return props.uniqueContainerId ? `-${props.uniqueContainerId}` : "-";
}

export const isDesktop = () => `(min-width: 640px)`;

export const isMobile = () => `(min-width: 300px)`;

export const PDFViewCustomContainer = styled.div`
  header {
    background-color: rgba(244, 244, 244, 1);
  }

  #title${getCustomIdname} {
    display: none;
  }
  header h1 {
    border-bottom: 1px solid rgba(216, 216, 216, 1);
    color: rgba(133, 133, 133, 1);
    font-size: 23px;
    font-style: italic;
    font-weight: normal;
    overflow: hidden;
    padding: 10px;
    text-align: center;
    text-overflow: ellipsis;
    white-space: nowrap;
  }

  body {
    /* background: url(/images/document_bg.png);
   color: rgba(255, 255, 255, 1);
   font-family: sans-serif;
   font-size: 10px;
   height: 100%;
   width: 100%;
   overflow: hidden;
   padding-bottom: 5rem; */
  }

  section {
    position: absolute;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    overflow: hidden;
    font-size: 2rem;
  }

  footer {
    background-image: url(/images/toolbar_background.png);
    height: 4rem;
    position: absolute;
    bottom: 90px;
    left: 0;
    right: 0;
    z-index: 1;
    box-shadow: 0 -0.2rem 0.5rem rgba(50, 50, 50, 0.75);
  }

  .toolbarButton {
    display: block;
    padding: 0;
    margin: 0;
    border-width: 0;
    background-position: center center;
    background-repeat: no-repeat;
    background-color: rgba(0, 0, 0, 0);
  }

  .toolbarButton.pageUp {
    position: absolute;
    width: 18%;
    height: 100%;
    left: 0;
    background-image: url(/images/icon_previous_page.png);
    // background-size: 2rem;
  }

  .toolbarButton.pageDown {
    position: absolute;
    width: 18%;
    height: 100%;
    left: 18%;
    background-image: url(/images/icon_next_page.png);
    // background-size: 2rem;
  }

  #pageNumber${getCustomIdname} {
    -moz-appearance: textfield; /* hides the spinner in moz */
    position: absolute;
    width: 28%;
    height: 100%;
    left: 36%;
    text-align: center;
    border: 0;
    background-color: rgba(0, 0, 0, 0);
    font-size: 1.2rem;
    color: rgba(255, 255, 255, 1);
    background-image: url(/images/div_line_left.png),
      url(/images/div_line_right.png);
    background-repeat: no-repeat;
    background-position: left, right;
    background-size: 0.2rem, 0.2rem;
  }

  .toolbarButton.zoomOut {
    position: absolute;
    width: 18%;
    height: 100%;
    left: 64%;
    background-image: url(/images/icon_zoom_out.png);
    // background-size: 2.4rem;
  }

  .toolbarButton.zoomIn {
    position: absolute;
    width: 18%;
    height: 100%;
    left: 82%;
    background-image: url(/images/icon_zoom_in.png);
    // background-size: 2.4rem;
  }

  .toolbarButton[disabled] {
    opacity: 0.3;
  }

  .hidden {
    display: none;
  }
  [hidden] {
    display: none !important;
  }

  #viewerContainer${getCustomIdname} {
    position: absolute;
    overflow: auto;
    width: 100%;
    top: 5rem;
    bottom: 4rem;
    left: 0;
    right: 0;
  }

  canvas {
    margin: auto;
    display: block;
  }

  .pdfViewer .page .loadingIcon {
    width: 2.9rem;
    height: 2.9rem;
    background: url("/images/spinner.png") no-repeat left top / 38rem;
    border: medium none;
    animation: 1s steps(10, end) 0s normal none infinite moveDefault;
    display: block;
    position: absolute;
    top: calc((100% - 2.9rem) / 2);
    left: calc((100% - 2.9rem) / 2);
    display: none;
  }

  @keyframes moveDefault {
    from {
      background-position: 0 top;
    }

    to {
      background-position: -39rem top;
    }
  }

  #loadingBar${getCustomIdname} {
    position: relative;
    height: 0.6rem;
    background-color: rgba(51, 51, 51, 1);
    border-bottom: 1px solid rgba(51, 51, 51, 1);
    margin-top: 5rem;
  }

  #loadingBar${getCustomIdname} .progress {
    position: absolute;
    left: 0;
    width: 0;
    height: 100%;
    background-color: rgba(221, 221, 221, 1);
    overflow: hidden;
    transition: width 200ms;
  }

  @keyframes progressIndeterminate {
    0% {
      left: 0;
    }
    50% {
      left: 100%;
    }
    100% {
      left: 100%;
    }
  }

  #loadingBar${getCustomIdname} .progress.indeterminate {
    background-color: rgba(153, 153, 153, 1);
    transition: none;
  }

  #loadingBar${getCustomIdname} .indeterminate .glimmer {
    position: absolute;
    top: 0;
    left: 0;
    height: 100%;
    width: 5rem;
    background-image: linear-gradient(
      to right,
      rgba(153, 153, 153, 1) 0%,
      rgba(255, 255, 255, 1) 50%,
      rgba(153, 153, 153, 1) 100%
    );
    background-size: 100% 100%;
    background-repeat: no-repeat;
    animation: progressIndeterminate 2s linear infinite;
  }

  #errorWrapper${getCustomIdname} {
    background: none repeat scroll 0 0 rgba(255, 85, 85, 1);
    color: rgba(255, 255, 255, 1);
    left: 0;
    position: absolute;
    right: 0;
    top: 3.2rem;
    z-index: 1000;
    padding: 0.3rem;
    font-size: 0.8em;
  }

  #errorMessageLeft${getCustomIdname} {
    float: left;
  }

  #errorMessageRight${getCustomIdname} {
    float: right;
  }

  #errorMoreInfo${getCustomIdname} {
    background-color: rgba(255, 255, 255, 1);
    color: rgba(0, 0, 0, 1);
    padding: 0.3rem;
    margin: 0.3rem;
    width: 98%;
  }
  ${(props) => props.styles || ""}
`;
function PdfPreview(props) {
    const [loaded, setLoaded] = React.useState(false);

    const {
        docUrl,
        uniqueContainerId = "pdf-preview",
        footerStyles,
        styles
    } = props;

    useEffect(() => {
        /**
         * [description]
         */
        const pdfjsLib = window.pdfjsLib;
        const pdfjsViewer = window.pdfjsViewer;
        const USE_ONLY_CSS_ZOOM = true;
        const TEXT_LAYER_MODE = 0; // DISABLE
        const MAX_IMAGE_SIZE = 1024 * 1024;
        const CMAP_URL = "../node_modules/pdfjs-dist/cmaps/";
        const CMAP_PACKED = true;

        // pdfjsLib.GlobalWorkerOptions.workerSrc =
        //   "../../node_modules/pdfjs-dist/build/pdf.worker.js";

        const DEFAULT_SCALE_DELTA = 1.1;
        const MIN_SCALE = 0.25;
        const MAX_SCALE = 10.0;
        const DEFAULT_SCALE_VALUE = "auto";
        const PDFViewerApplication = {
            pdfLoadingTask: null,
            pdfDocument: null,
            pdfViewer: null,
            pdfHistory: null,
            pdfLinkService: null,
            eventBus: null,

            /**
             * Opens PDF document specified by URL.
             * @returns {Promise} - Returns the promise, which is resolved when document
             *                      is opened.
             */
            open: function (params) {
                if (this.pdfLoadingTask) {
                    // We need to destroy already opened document
                    return this.close().then(
                        function () {
                            // ... and repeat the open() call.
                            return this.open(params);
                        }.bind(this)
                    );
                }

                const url = params.url;
                const self = this;
                this.setTitleUsingUrl(url);

                // Loading document.
                const loadingTask = pdfjsLib.getDocument({
                    url: url,
                    maxImageSize: MAX_IMAGE_SIZE,
                    cMapUrl: CMAP_URL,
                    cMapPacked: CMAP_PACKED
                });
                this.pdfLoadingTask = loadingTask;

                loadingTask.onProgress = function (progressData) {
                    self.progress(progressData.loaded / progressData.total);
                };

                return loadingTask.promise.then(
                    function (pdfDocument) {
                        // if(pdfDocument) {

                        // }
                        // Document loaded, specifying document for the viewer.
                        self.pdfDocument = pdfDocument;
                        self.pdfViewer.setDocument(pdfDocument);
                        self.pdfLinkService.setDocument(pdfDocument);
                        self.pdfHistory.initialize({
                            fingerprint: pdfDocument.fingerprint
                        });

                        self.loadingBar.hide();
                        setLoaded(true);

                        self.setTitleUsingMetadata(pdfDocument);
                    },
                    function (exception) {
                        const message = exception && exception.message;

                        // const l10n = self.l10n;
                        // let loadingErrorMessage;

                        // if (exception instanceof pdfjsLib.InvalidPDFException) {
                        //  // change error message also for other builds
                        //  loadingErrorMessage = l10n.get(
                        //    'invalid_file_error',
                        //    null,
                        //    'Invalid or corrupted PDF file.',
                        //  );
                        // } else if (exception instanceof pdfjsLib.MissingPDFException) {
                        //  // special message for missing PDFs
                        //  loadingErrorMessage = l10n.get(
                        //    'missing_file_error',
                        //    null,
                        //    'Missing PDF file.',
                        //  );
                        // } else if (exception instanceof pdfjsLib.UnexpectedResponseException) {
                        //  loadingErrorMessage = l10n.get(
                        //    'unexpected_response_error',
                        //    null,
                        //    'Unexpected server response.',
                        //  );
                        // } else {
                        //  loadingErrorMessage = l10n.get(
                        //    'loading_error',
                        //    null,
                        //    'An error occurred while loading the PDF.',
                        //  );
                        // }

                        // loadingErrorMessage.then(function (msg) {
                        //  self.error(msg, { message: message });
                        // });
                        self.loadingBar.hide();
                    }
                );
            },

            /**
             * Closes opened PDF document.
             * @returns {Promise} - Returns the promise, which is resolved when all
             *                      destruction is completed.
             */
            close: function () {
                const errorWrapper = document.getElementById(
                    `errorWrapper-${uniqueContainerId}`
                );
                errorWrapper.setAttribute("hidden", "true");

                if (!this.pdfLoadingTask) {
                    return Promise.resolve();
                }

                const promise = this.pdfLoadingTask.destroy();
                this.pdfLoadingTask = null;

                if (this.pdfDocument) {
                    this.pdfDocument = null;

                    this.pdfViewer.setDocument(null);
                    this.pdfLinkService.setDocument(null, null);

                    if (this.pdfHistory) {
                        this.pdfHistory.reset();
                    }
                }

                return promise;
            },

            get loadingBar() {
                const bar = new pdfjsViewer.ProgressBar(
                    `#loadingBar${getCustomIdname({
                        uniqueContainerId
                    })}`,
                    {}
                );

                return pdfjsLib.shadow(this, "loadingBar", bar);
            },

            setTitleUsingUrl: function pdfViewSetTitleUsingUrl(url) {
                this.url = url;
                const title = pdfjsLib.getFilenameFromUrl(url) || url;
                try {
                    title = decodeURIComponent(title);
                } catch (e) {
                    // decodeURIComponent may throw URIError,
                    // fall back to using the unprocessed url in that case
                }
                this.setTitle(title);
            },

            setTitleUsingMetadata: function (pdfDocument) {
                const self = this;
                pdfDocument.getMetadata().then(function (data) {
                    const info = data.info,
                        metadata = data.metadata;
                    self.documentInfo = info;
                    self.metadata = metadata;

                    // Provides some basic debug information
                    console.log(
                        "PDF " +
                        pdfDocument.fingerprint +
                        " [" +
                        info.PDFFormatVersion +
                        " " +
                        (info.Producer || "-").trim() +
                        " / " +
                        (info.Creator || "-").trim() +
                        "]" +
                        " (PDF.js: " +
                        (pdfjsLib.version || "-") +
                        ")"
                    );

                    let pdfTitle;
                    if (metadata && metadata.has("dc:title")) {
                        const title = metadata.get("dc:title");
                        // Ghostscript sometimes returns 'Untitled', so prevent setting the
                        // title to 'Untitled.
                        if (title !== "Untitled") {
                            pdfTitle = title;
                        }
                    }

                    if (!pdfTitle && info && info.Title) {
                        pdfTitle = info.Title;
                    }

                    if (pdfTitle) {
                        self.setTitle(pdfTitle + " - " + document.title);
                    }
                });
            },

            setTitle: function pdfViewSetTitle(title) {
                document.title = title;
                document.getElementById(
                    `title-${uniqueContainerId}`
                ).textContent = title;
            },

            error: function pdfViewError(message, moreInfo) {
                const l10n = this.l10n;
                const moreInfoText = [
                    l10n.get(
                        "error_version_info",
                        { version: pdfjsLib.version || "?", build: pdfjsLib.build || "?" },
                        "PDF.js v{{version}} (build: {{build}})"
                    )
                ];

                if (moreInfo) {
                    moreInfoText.push(
                        l10n.get(
                            "error_message",
                            { message: moreInfo.message },
                            "Message: {{message}}"
                        )
                    );
                    if (moreInfo.stack) {
                        moreInfoText.push(
                            l10n.get(
                                "error_stack",
                                { stack: moreInfo.stack },
                                "Stack: {{stack}}"
                            )
                        );
                    } else {
                        if (moreInfo.filename) {
                            moreInfoText.push(
                                l10n.get(
                                    "error_file",
                                    { file: moreInfo.filename },
                                    "File: {{file}}"
                                )
                            );
                        }
                        if (moreInfo.lineNumber) {
                            moreInfoText.push(
                                l10n.get(
                                    "error_line",
                                    { line: moreInfo.lineNumber },
                                    "Line: {{line}}"
                                )
                            );
                        }
                    }
                }

                const errorWrapper = document.getElementById(
                    `errorWrapper-${uniqueContainerId}`
                );
                errorWrapper.removeAttribute("hidden");

                const errorMessage = document.getElementById(
                    `errorMessage-${uniqueContainerId}`
                );
                errorMessage.textContent = message;

                const closeButton = document.getElementById(
                    `errorClose-${uniqueContainerId}`
                );
                closeButton.onclick = function () {
                    errorWrapper.setAttribute("hidden", "true");
                };

                const errorMoreInfo = document.getElementById(
                    `errorMoreInfo-${uniqueContainerId}`
                );
                const moreInfoButton = document.getElementById(
                    `errorShowMore-${uniqueContainerId}`
                );
                const lessInfoButton = document.getElementById(
                    `errorShowLess-${uniqueContainerId}`
                );
                moreInfoButton.onclick = function () {
                    errorMoreInfo.removeAttribute("hidden");
                    moreInfoButton.setAttribute("hidden", "true");
                    lessInfoButton.removeAttribute("hidden");
                    errorMoreInfo.style.height = errorMoreInfo.scrollHeight + "px";
                };
                lessInfoButton.onclick = function () {
                    errorMoreInfo.setAttribute("hidden", "true");
                    moreInfoButton.removeAttribute("hidden");
                    lessInfoButton.setAttribute("hidden", "true");
                };
                moreInfoButton.removeAttribute("hidden");
                lessInfoButton.setAttribute("hidden", "true");
                Promise.all(moreInfoText).then(function (parts) {
                    errorMoreInfo.value = parts.join("\n");
                });
            },

            progress: function pdfViewProgress(level) {
                const percent = Math.round(level * 100);
                // Updating the bar if value increases.
                if (percent > this.loadingBar.percent || isNaN(percent)) {
                    this.loadingBar.percent = percent;
                }
            },

            get pagesCount() {
                return this.pdfDocument.numPages;
            },

            get page() {
                return this.pdfViewer.currentPageNumber;
            },

            set page(val) {
                this.pdfViewer.currentPageNumber = val;
            },

            zoomIn: function pdfViewZoomIn(ticks) {
                let newScale = this.pdfViewer.currentScale;
                do {
                    newScale = (newScale * DEFAULT_SCALE_DELTA).toFixed(2);
                    newScale = Math.ceil(newScale * 10) / 10;
                    newScale = Math.min(MAX_SCALE, newScale);
                } while (--ticks && newScale < MAX_SCALE);
                this.pdfViewer.currentScaleValue = newScale;
            },

            zoomOut: function pdfViewZoomOut(ticks) {
                let newScale = this.pdfViewer.currentScale;
                do {
                    newScale = (newScale / DEFAULT_SCALE_DELTA).toFixed(2);
                    newScale = Math.floor(newScale * 10) / 10;
                    newScale = Math.max(MIN_SCALE, newScale);
                } while (--ticks && newScale > MIN_SCALE);
                this.pdfViewer.currentScaleValue = newScale;
            },

            initUI: function pdfViewInitUI() {
                const eventBus = new pdfjsViewer.EventBus();
                this.eventBus = eventBus;

                const linkService = new pdfjsViewer.PDFLinkService({
                    eventBus: eventBus
                });
                this.pdfLinkService = linkService;

                this.l10n = pdfjsViewer.NullL10n;

                const container = document.getElementById(
                    `viewerContainer-${uniqueContainerId}`
                );
                const pdfViewer = new pdfjsViewer.PDFViewer({
                    container: container,
                    eventBus: eventBus,
                    linkService: linkService,
                    l10n: this.l10n,
                    useOnlyCssZoom: USE_ONLY_CSS_ZOOM,
                    textLayerMode: TEXT_LAYER_MODE
                });
                this.pdfViewer = pdfViewer;

                linkService.setViewer(pdfViewer);

                this.pdfHistory = new pdfjsViewer.PDFHistory({
                    eventBus: eventBus,
                    linkService: linkService
                });
                linkService.setHistory(this.pdfHistory);

                document
                    .getElementById(`previous-${uniqueContainerId}`)
                    .addEventListener("click", function () {
                        PDFViewerApplication.page--;
                    });

                document
                    .getElementById(`next-${uniqueContainerId}`)
                    .addEventListener("click", function () {
                        PDFViewerApplication.page++;
                    });

                document
                    .getElementById(`zoomIn-${uniqueContainerId}`)
                    .addEventListener("click", function () {
                        PDFViewerApplication.zoomIn();
                    });

                document
                    .getElementById(`zoomOut-${uniqueContainerId}`)
                    .addEventListener("click", function () {
                        PDFViewerApplication.zoomOut();
                    });

                document
                    .getElementById(`pageNumber-${uniqueContainerId}`)
                    .addEventListener("click", function () {
                        this.select();
                    });

                document
                    .getElementById(`pageNumber-${uniqueContainerId}`)
                    .addEventListener("change", function () {
                        PDFViewerApplication.page = this.value | 0;

                        // Ensure that the page number input displays the correct value,
                        // even if the value entered by the user was invalid
                        // (e.g. a floating point number).
                        if (this.value !== PDFViewerApplication.page.toString()) {
                            this.value = PDFViewerApplication.page;
                        }
                    });

                eventBus.on("pagesinit", function () {
                    // We can use pdfViewer now, e.g. let's change default scale.
                    pdfViewer.currentScaleValue = DEFAULT_SCALE_VALUE;
                });

                eventBus.on(
                    "pagechanging",
                    function (evt) {
                        const page = evt.pageNumber;
                        const numPages = PDFViewerApplication.pagesCount;

                        document.getElementById(
                            `pageNumber-${uniqueContainerId}`
                        ).value = page;
                        document.getElementById(`previous-${uniqueContainerId}`).disabled =
                            page <= 1;
                        document.getElementById(`next-${uniqueContainerId}`).disabled =
                            page >= numPages;
                    },
                    true
                );
            }
        };

        // document.addEventListener(
        //  'DOMContentLoaded',
        //  function () {
        //    PDFViewerApplication.initUI();
        //  },
        //  true,
        // );

        (function animationStartedClosure() {
            // The offsetParent is not set until the PDF.js iframe or object is visible.
            // Waiting for first animation.
            PDFViewerApplication.animationStartedPromise = new Promise(function (
                resolve
            ) {
                window.requestAnimationFrame(resolve);
            });
        })();

        // We need to delay opening until all HTML is loaded.
        if (docUrl) {
            PDFViewerApplication.animationStartedPromise.then(function () {
                const loadingIconArr = document.querySelectorAll(".loadingIcon");
                // console.log(
                //  '🚀 ~ file: PreviewTest.js ~ line 690 ~ loadingIconArr',
                //  loadingIconArr,
                // );

                // if (loadingIconArr && loadingIconArr[0]) {
                //  loadingIconArr[0].style.display = 'none';
                // }

                PDFViewerApplication.open({
                    url: docUrl
                });
            });
        }

        /**
         * [des
         * cription]
         */

        if (docUrl) {
            PDFViewerApplication.initUI();
        }

        return () => { };
    }, [docUrl]);
    return (
        <PDFViewCustomContainer
            styles={styles}
            uniqueContainerId={uniqueContainerId}
            id={`preview-container-${uniqueContainerId ? `${uniqueContainerId}` : ""
                }`}
        >
            {/* {!loaded && <Loader dark />} */}
            <header>
                <h1 id={`title-${uniqueContainerId}`} />
            </header>
            <div id={`viewerContainer-${uniqueContainerId}`}>
                <div id="viewer" className="pdfViewer" />
            </div>
            <div id={`loadingBar-${uniqueContainerId}`}>
                <div className="progress" />
                <div className="glimmer" />
            </div>
            <div id={`errorWrapper-${uniqueContainerId}`} hidden="true">
                <div id={`errorMessageLeft-${uniqueContainerId}`}>
                    <span id={`errorMessage-${uniqueContainerId}`} />
                    <button id={`errorShowMore-${uniqueContainerId}`}>
                        More Information
                    </button>
                    <button id={`errorShowLess-${uniqueContainerId}`}>
                        Less Information
                    </button>
                </div>
                <div id={`errorMessageRight-${uniqueContainerId}`}>
                    <button id={`errorClose-${uniqueContainerId}`}>Close</button>
                </div>
                <div className="clearBoth" />
                <textarea
                    id={`errorMoreInfo-${uniqueContainerId}`}
                    hidden="true"
                    readOnly="readonly"
                    defaultValue={""}
                />
            </div>
            <footer>
                <button
                    className="toolbarButton pageUp"
                    title="Previous Page"
                    id={`previous-${uniqueContainerId}`}
                />
                <button
                    className="toolbarButton pageDown"
                    title="Next Page"
                    id={`next-${uniqueContainerId}`}
                />
                <input
                    type="number"
                    id={`pageNumber-${uniqueContainerId}`}
                    className="toolbarField pageNumber"
                    defaultValue={1}
                    size={4}
                    min={1}
                />
                <button
                    className="toolbarButton zoomOut"
                    title="Zoom Out"
                    id={`zoomOut-${uniqueContainerId}`}
                />
                <button
                    className="toolbarButton zoomIn"
                    title="Zoom In"
                    id={`zoomIn-${uniqueContainerId}`}
                />
            </footer>
        </PDFViewCustomContainer>
    );
}

function App() {
    const url = `https://file-examples-com.github.io/uploads/2017/10/file-sample_150kB.pdf`;
    return (
        <div>
            <PdfPreview
                docUrl={url}
                styles={css`
          #loadingBar-pdf-preview {
            width: 80%;
            margin: 0px auto;
            margin-top: 50px;
          }

          footer {
            height: 30px;
            bottom: 0px;
            @media ${isDesktop} {
              height: 4rem;
              bottom: 0px;
            }
            button {
              transform: scale(0.8);
            }
          }
        `}
            />
        </div>
    );
}

/**
 * Feels like this code is lot complicated. Yes, You are right. Read more at the top of the file
 * to get complete context of what and whys.
 */

export default App;