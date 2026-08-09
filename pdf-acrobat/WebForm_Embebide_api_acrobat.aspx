<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="WebForm_Embebide_api_acrobat.aspx.vb" Inherits="GestionDocumental_Docuarchi.net.WebForm_Embebide_api_acrobat" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">

<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
     <script src="https://documentservices.adobe.com/view-sdk/viewer.js"></script>

    <meta charset="utf-8"/>
    <meta http-equiv="X-UA-Compatible" content="IE=edge,chrome=1"/>
    <meta id="viewport" name="viewport" content="width=device-width, initial-scale=1"/>
</head>
<body>
    <form id="form1" style="width:100%; height:100%" runat="server">
       
            <div style="width:100%; height:700px" id="adobe-dc-view"></div>
 
    </form>
</body>
    <script type="text/javascript">
       
        const previewConfig = {
            embedMode: "FULL_WINDOW",
            showDownloadPDF: false,
            showZoomControl: false,
            enableAnnotationAPIs: false,
            includePDFAnnotations: false,
            showToolbar: false,
            showCommentsPanel: false,
            downloadWithAnnotations: true,
            showToolsOnTextSelection: true,
            printWithAnnotations: false,
            showAnnotationTools: true,
            enableFormFilling: true,
            enableAnnotationAPIs: true,
            includePDFAnnotations:true
        }
        document.addEventListener("adobe_dc_view_sdk.ready", function () {
            var adobeDCView = new AdobeDC.View({ clientId: "04193acf863c457382f6eff77117a5f5", divId: "adobe-dc-view" });
            adobeDCView.previewFile(
                {
                    content: { location: { url: "https://documentservices.adobe.com/view-sdk-demo/PDFs/Bodea Brochure.pdf" } },
                    metaData: { fileName: "Brochure.pdf", id: "6d07d124-ac85-43b3-a867-36930f502ac6"},
                }, previewConfig);
        });
        var saveApiHandler = function (metaData, content, options) {
            console.log(metaData, content, options);
            return new Promise(function (resolve, reject) {
                /* Dummy implementation of Save API, replace with your business logic */
                setTimeout(function () {
                    var response = {
                        code: AdobeDC.View.Enum.ApiResponseCode.SUCCESS,
                        data: {
                            metaData: Object.assign(metaData, { updatedAt: new Date().getTime() })
                        },
                    };
                    resolve(response);
                }, 2000);
            });
        };
        function add_src_image() {
            document.getElementById("Icons_Expanded_Shapes");
            var elment_stik = document.getElementsByClassName("StickyNoteCommentView__modernStickyNoteSelected___yw8Tb");
            var element_img;
            element_img = document.createElement("IMG");
            elment_stik[0].appendChild(element_img)
        }
        adobeDCView.registerCallback(
            AdobeDC.View.Enum.CallbackType.SAVE_API,
            saveApiHandler,
            {}
        );
        /* Use the annotation manager interface to invoke the commenting APIs*/
        previewFilePromise.then(function (adobeViewer) {
            adobeViewer.getAnnotationManager().then(function (annotationManager) {
                /* API to add annotations */
                annotationManager.addAnnotations(annotations)
                    .then(function () {
                        alert("Annotations added through API successfully");
                    })
                    .catch(function (error) {
                        console.log(error)
                    });

                /* API to get all annotations */
                annotationManager.getAnnotations()
                    .then(function (result) {
                        console.log("GET all annotations", result);
                    })
                    .catch(function (error) {
                        console.log(error)
                    });

                /* API to delete annotations based on annotation ID filter */
                var filter = {
                    annotationIds: ["3adeae16-a868-4653-960e-613c048dddc5", "079d66a4-5ec2-4703-ae9d-30ccbb1aa84c"]
                };
                annotationManager.deleteAnnotations(filter)
                    .then(function () {
                        console.log("Deleted annotations based on annotation ID filter.")
                    })
                    .catch(function (error) {
                        console.log(error)
                    });

                /* API to delete annotations based on page range filter */
                filter = {
                    pageRange: {
                        startPage: 4,
                        endPage: 6
                    }
                };
                annotationManager.deleteAnnotations(filter)
                    .then(function () {
                        console.log("Deleted annotations based on page range filter")
                    })
                    .catch(function (error) {
                        console.log(error)
                    });

                /* API to get annotations after deletion */
                annotationManager.getAnnotations()
                    .then(function (result) {
                        console.log("GET annotations result after deleting annotations", result)
                    })
                    .catch(function (error) {
                        console.log(error)
                    });

                /* API to update a single annotation */
                const newComment = "Preserving your legacy with Bodea life insurance plans.";
                setTimeout(function () {
                    annotations[3].bodyValue = newComment;
                    const updatedAnnotation = annotations[3];
                    annotationManager.updateAnnotation(updatedAnnotation)
                        .then(function () {
                            console.log("Annotation updated through API successfully")
                        })
                        .catch(function (error) {
                            console.log(error)
                        });
                }, 3000);
            });
        });
    </script>
</html>
