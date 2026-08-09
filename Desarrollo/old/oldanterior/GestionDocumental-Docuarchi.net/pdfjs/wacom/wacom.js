let sigObj;
var mHash;



/*Module.onRuntimeInitialized = _ => {
	//document.getElementById("version_txt").innerHTML = Module.VERSION;
	mSigObj = new Module.SigObj();
	mHash = new Module.Hash(Module.HashType.SHA512);
	try {
		//mSigObj.setLicence("PUT HERE YOUR LICENCE STRING");
		mSigObj.setLicence("eyJhbGciOiJSUzUxMiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJMTVMiLCJleHAiOjE2NjczNzcxMTUsImlhdCI6MTYzNTg0NTEwOCwicmlnaHRzIjpbIlNJR19TREtfQ09SRSIsIkpTX0NPUkUiXSwiZGV2aWNlcyI6WyJXQUNPTV9BTlkiXSwidHlwZSI6ImV2YWwiLCJsaWNfbmFtZSI6IlRlc3QgbGljZW5zZSBbMTYzNTg0MTExNV0iLCJ3YWNvbV9pZCI6IjE1M2JlODlmLTk2MzctNGFiNC05OTk5LTk0ZTBjOGUwZDJhMyIsImxpY191aWQiOiI2M2IyNGU2YS05ZDI3LTQ3ZDMtODk0NS00OTkyM2FjMjA0ZTIiLCJhcHBzX3dpbmRvd3MiOltdLCJhcHBzX2lvcyI6W10sImFwcHNfYW5kcm9pZCI6W10sIm1hY2hpbmVfaWRzIjpbIjAwNTA1NkMwMDAwMSIsIjAwNTA1NkMwMDAwOCJdfQ.LKYsb6HR9K1M-69RNXhdZV_uSpxLyVgJHl0yjKlVRO0YfmNGB9sxGVIDE0ec7SDSV5417QikD8hxTyL6i5B97p7Pl99d_gvdJubW1k9oVpR1JEq3dws-whQggVpySIhBU0BGPhRQP1VzIvpsfrGcMP0-LTeOJoKCKBM9FwTbO98QAtIZq_xbqGyQjOkCQj3GXgRv8BdmGYtih7Antr7pCrVkSc3WtcDxQS3XoedNBOae4nUe2Op1Rgwhk4Oymjl_3q5z9hhoa5rYf7kwkpv5B78BbX6tGlUEFABLS0BgYdYNhUxsYuip3FnqoS543H7_q1s1CzhmREF7n1SZDS781A");
		
	} catch (e) {
		alert(e);
	}
}*/
/*async function loadFromFile() {
	const file = document.getElementById("myfile").files[0];
	if (file) {
		// check the type
		if ("text/plain" == file.type) {
			// read the file as string
			const reader = new FileReader();
			reader.onload = async function () {
				const data = reader.result;
				try {
					if (await mSigObj.setTextData(data)) {
						renderSignature();
					} else {
						alert("Incorrect signature data found");
					}
				} catch (e) {
					alert("Error loading signature as text " + e);
				}
			}
			reader.readAsText(file);
		} else if ((file.type == "image/png") ||
			(file.type == "image/jpeg")) {
			const reader = new FileReader();
			reader.onload = async function () {
				const data = reader.result;
				var img = new Image();
				img.addEventListener('load', async function () {
					//the image has been loaded
					const canvas = document.createElement("canvas");
					canvas.width = img.width;
					canvas.height = img.height;
					const ctx = canvas.getContext("2d");
					ctx.drawImage(img, 0, 0, img.width, img.height);
					const imageData = ctx.getImageData(0, 0, img.width, img.height);
					try {
						await mSigObj.readEncodedBitmapBinary(imageData.data, imageData.width, imageData.height);
						renderSignature();
					} catch (e) {
						alert("Error loading image " + e);
					}
				}, false);
				img.src = data;
			}
			reader.readAsDataURL(file);
		} else {
			// we assume is binary data
			const reader = new FileReader();
			reader.onload = async function () {
				const data = reader.result;
				try {
					if (await mSigObj.setSigData(new Uint8Array(data))) {
						renderSignature();
					} else {
						alert("Incorrect signature data found");
					}
				} catch (e) {
					alert("Error loading signature as binary " + e);
				}
			}
			reader.readAsArrayBuffer(file);
		}
	}
}
async function SigCaptDialog_() {
	// config it is a JSON object with some configuration data, when it is not present 
	// create a hash object. In this case an empty one.
	var documentHash = new Module.Hash(Module.HashType.None);

	// config it is a JSON object with some configuration data, when it is not present 
	// default values are taken.
	var sigCaptDialog = new SigCaptDialog();

	// adding listener for ok button
	sigCaptDialog.addEventListener("ok", function () {
		// here we has captured a signature
	});

	// open a new dialog for capturing the signature. Once the signature is captured it will be
	// in sigObj object.			  
	sigCaptDialog.open(mSigObj, "Signatory", "Reason for signing", null, Module.KeyType.SHA512, documentHash);

	// start listening for point data.
	sigCaptDialog.startCapture();
	
}
async function capture() {
	// create a sigObj object from Signature SDK and set its licence.
	//var sigObj = new Module.SigObj();
	//sigObj.setLicence("eyJhbGciOiJSUzUxMiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJMTVMiLCJleHAiOjE2NjczNzcxMTUsImlhdCI6MTYzNTg0NTEwOCwicmlnaHRzIjpbIlNJR19TREtfQ09SRSIsIkpTX0NPUkUiXSwiZGV2aWNlcyI6WyJXQUNPTV9BTlkiXSwidHlwZSI6ImV2YWwiLCJsaWNfbmFtZSI6IlRlc3QgbGljZW5zZSBbMTYzNTg0MTExNV0iLCJ3YWNvbV9pZCI6IjE1M2JlODlmLTk2MzctNGFiNC05OTk5LTk0ZTBjOGUwZDJhMyIsImxpY191aWQiOiI2M2IyNGU2YS05ZDI3LTQ3ZDMtODk0NS00OTkyM2FjMjA0ZTIiLCJhcHBzX3dpbmRvd3MiOltdLCJhcHBzX2lvcyI6W10sImFwcHNfYW5kcm9pZCI6W10sIm1hY2hpbmVfaWRzIjpbIjAwNTA1NkMwMDAwMSIsIjAwNTA1NkMwMDAwOCJdfQ.LKYsb6HR9K1M-69RNXhdZV_uSpxLyVgJHl0yjKlVRO0YfmNGB9sxGVIDE0ec7SDSV5417QikD8hxTyL6i5B97p7Pl99d_gvdJubW1k9oVpR1JEq3dws-whQggVpySIhBU0BGPhRQP1VzIvpsfrGcMP0-LTeOJoKCKBM9FwTbO98QAtIZq_xbqGyQjOkCQj3GXgRv8BdmGYtih7Antr7pCrVkSc3WtcDxQS3XoedNBOae4nUe2Op1Rgwhk4Oymjl_3q5z9hhoa5rYf7kwkpv5B78BbX6tGlUEFABLS0BgYdYNhUxsYuip3FnqoS543H7_q1s1CzhmREF7n1SZDS781A");

	// create a hash object. In this case an empty one.
	var documentHash = new Module.Hash(Module.HashType.None);

	// config it is a JSON object with some configuration data, when it is not present 
	// default values are taken.
	var stuCaptDialog = new StuCaptDialog();

	// adding listener for ok button
	stuCaptDialog.addEventListener("ok", function () {
		// here we has captured a signature
	});

	// open a new dialog for capturing the signature. Once the signature is captured it will be
	// in sigObj object.			  
	stuCaptDialog.open(mSigObj, "Signatory", "Reason for signing", null, Module.KeyType.SHA512, documentHash);

// Note: in this case it is not necessary starting the capture as it will start
// automatically when the STU device is ready.
}
const config = {
	width: 400,
	height: 300,
	left: 0,
	top: 0,
	centered: false,
	title: "My Tittle",
	borderColor: "#0097d4",
	borderWidth: "1",
	hasTitle: true,
	buttons: [{
		text: "*clear",
		textColor: "black",
		backgroundColor: "lightgrey",
		borderWidth: 0,
		borderColor: "black"
	},
	{
		text: "*cancel",
		textColor: "black",
		backgroundColor: "lightgrey",
		borderWidth: 0,
		borderColor: "black"
	},
	{
		text: "*ok",
		textColor: "black",
		backgroundColor: "lightgrey",
		borderWidth: 0,
		borderColor: "black"
	}
	],
	buttonsFont: "Arial",
	background: { alpha: 1.0, color: "white" },
	reason: { visible: true, fontFace: "Arial", fontSize: 16, color: "black", offsetY: 10, offsetX: 5 },
	signatory: { visible: true, fontFace: "Arial", fontSize: 16, color: "black", offsetY: 5, offsetX: 30 },
	date: { visible: true, fontFace: "Arial", fontSize: 16, color: "black", offsetY: 20, offsetX: 30 },
	signingLine: { visible: true, left: 30, right: 30, width: 2, color: "grey", offsetY: 5 },
	source: { mouse: true, touch: true, pen: true, stu: true },
	will: { tool: "pen", color: "#000F55" },
	modal: true,
	draggable: true
};
async function renderSignature() {
	//pixels = dpi*mm/25.4mm
	let width = Math.trunc((96 * mSigObj.getWidth(false) * 0.01) / 25.4);
	let height = Math.trunc((96 * mSigObj.getHeight(false) * 0.01) / 25.4);

	let scaleWidth = 300 / width;
	let scaleHeight = 200 / height;
	let scale = Math.min(scaleWidth, scaleHeight);

	let renderWidth = Math.trunc(width * scale);
	const renderHeight = Math.trunc(height * scale);

	// render with must be multiple of 4
	if (renderWidth % 4 != 0) {
		renderWidth += renderWidth % 4;
	}

	let canvas;
	const inkColor = "#000F55";
	try {
		const image_ = await mSigObj.renderBitmap();
		const inkTool = {
			brush: null,
			dynamics: {
				size: {
					value: {
						min: 0.5,
						max: 1.6,
						remap: v => ValueTransformer.sigmoid(v, 0.62)
					},
					velocity: {
						min: 5,
						max: 210
					}
				},
				rotation: {
					dependencies: [window.DigitalInk.SensorChannel.Type.ROTATION, window.DigitalInk.SensorChannel.Type.AZIMUTH]
				},
				scaleX: {
					dependencies: [window.DigitalInk.SensorChannel.Type.RADIUS_X, window.DigitalInk.SensorChannel.Type.ALTITUDE],
					value: {
						min: 1,
						max: 3
					}
				},
				scaleY: {
					dependencies: [window.DigitalInk.SensorChannel.Type.RADIUS_Y],
					value: {
						min: 1,
						max: 3
					}
				},
				offsetX: {
					dependencies: [window.DigitalInk.SensorChannel.Type.ALTITUDE],

					value: {
						min: 2,
						max: 5
					}
				}
			}
		};
		
		const image = await mSigObj.renderBitmap(renderWidth, renderHeight, "image/png", inkTool, inkColor, "white", 0, 0, 0x400000);
		document.getElementById("sig_image").src = image;
		document.getElementById("sig_text").value = await mSigObj.getTextData(Module.TextFormat.BASE64);
	} catch (e) {
		alert(e);
	}
}

function captureFromCanvas() {
	const config = {};
	config.source = {
		mouse: document.getElementById("allow_mouse_check").checked,
		touch: document.getElementById("allow_touch_check").checked,
		pen: document.getElementById("allow_pen_check").checked
	};

	const sigCaptDialog = new SigCaptDialog(config);

	sigCaptDialog.addEventListener("ok", function () {
		renderSignature();
	});

	sigCaptDialog.open(mSigObj, null, null, null, Module.KeyType.SHA512, mHash);
	sigCaptDialog.startCapture();
}*/

function captureFromSTU() {
	//capture();
	//renderSignature();
	//SigCaptDialog_();
    tabletDemo();
	return true;
	//const stuCapDialog = new StuCaptDialog();
	//stuCapDialog.addEventListener("ok", function () {
	//	renderSignature();
	//});
	//stuCapDialog.open(mSigObj, null, null, null, Module.KeyType.SHA512, mHash);
}
//--------------------------------------
//modulo de captura
//--------------------------------------
var m_btns; // The array of buttons that we are emulating.
var m_clickBtn = -1;
var intf;
var formDiv;
var protocol;
var m_usbDevices;
var tablet;
var m_capability;
var m_inkThreshold;
var m_imgData;
var m_encodingMode;
var ctx;
var canvas;
var modalBackground;
var formDiv;
var m_penData;
var lastPoint;
var isDown;
var retry = 0;
function checkForSigCaptX() {
	// Establishing a connection to SigCaptX Web Service can take a few seconds,
	// particularly if the browser itself is still loading/initialising
	// or on a slower machine.
	retry = retry + 1;
	if (WacomGSS.STU.isServiceReady()) {
		retry = 0;
		console.log("SigCaptX Web Service: ready");
	} else {
		console.log("SigCaptX Web Service: not connected");
		if (retry < 20) {
			setTimeout(checkForSigCaptX, 1000);
		}
        else {
            console.log("Unable to establish connection to SigCaptX");
			
		}
	}

}
setTimeout(checkForSigCaptX, 500);
function onDCAtimeout() {
	// Device Control App has timed-out and shut down
	// For this sample, we just closedown tabletDemo (assumking it's running)
	console.log("DCA disconnected");
	setTimeout(close, 0);
}
function Rectangle(x, y, width, height) {
	this.x = x;
	this.y = y;
	this.width = width;
	this.height = height;
	this.Contains = function (pt) {
		if (((pt.x >= this.x) && (pt.x <= (this.x + this.width))) &&
			((pt.y >= this.y) && (pt.y <= (this.y + this.height)))) {
			return true;
		} else {
			return false;
		}
	}
}

// In order to simulate buttons, we have our own Button class that stores the bounds and event handler.
// Using an array of these makes it easy to add or remove buttons as desired.
//  delegate void ButtonClick();
function Button() {
	this.Bounds; // in Screen coordinates
	this.Text;
	this.Click;
};

function Point(x, y) {
	this.x = x;
	this.y = y;
}
function createModalWindow(width, height) {
	modalBackground = document.createElement('div');
	modalBackground.id = "modal-background";
	modalBackground.className = "active";
	modalBackground.style.width = window.innerWidth;
	modalBackground.style.height = window.innerHeight;
	document.getElementsByTagName('body')[0].appendChild(modalBackground);
	formDiv = document.createElement('div');
	formDiv.id = "signatureWindow";
    formDiv.className = "active";
    formDiv.style.top = (window.scrollY + (window.innerHeight / 2)) - (height / 2) + "px";
	formDiv.style.left = (window.innerWidth / 2) - (width / 2) + "px";
	formDiv.style.width = width + "px";
    formDiv.style.height = height + "px";
    formDiv.style.position = "absolute";
	document.getElementsByTagName('body')[0].appendChild(formDiv);
	canvas = document.createElement("canvas");
	canvas.id = "myCanvas";
	canvas.height = formDiv.offsetHeight;
	canvas.width = formDiv.offsetWidth;
	formDiv.appendChild(canvas);
	ctx = canvas.getContext("2d");
	if (canvas.addEventListener) {
		canvas.addEventListener("click", onCanvasClick, false);
	} else if (canvas.attachEvent) {
		canvas.attachEvent("onClick", onCanvasClick);
	} else {
		canvas["onClick"] = onCanvasClick;
	}
}
function disconnect() {
	var deferred = Q.defer();
	if (!(undefined === tablet || null === tablet)) {
		var p = new WacomGSS.STU.Protocol();
		tablet.setInkingMode(p.InkingMode.InkingMode_Off)
			.then(function (message) {
				console.log("received: " + JSON.stringify(message));
				return tablet.endCapture();
			})
			.then(function (message) {
				console.log("received: " + JSON.stringify(message));
				if (m_imgData !== null) {
					return m_imgData.remove();
				}
				else {
					return message;
				}
			})
			.then(function (message) {
				console.log("received: " + JSON.stringify(message));
				m_imgData = null;
				return tablet.setClearScreen();
			})
			.then(function (message) {
				console.log("received: " + JSON.stringify(message));
				return tablet.disconnect();
			})
			.then(function (message) {
				console.log("received: " + JSON.stringify(message));
				tablet = null;
				// clear canvas
				clearCanvas(canvas, ctx);
			})
			.then(function (message) {
				deferred.resolve();
			})
			.fail(function (message) {
				console.log("disconnect error: " + message);
				deferred.resolve();
			})
	} else {
		deferred.resolve();
	}
	return deferred.promise;
}
window.addEventListener("beforeunload", function (e) {
	//var confirmationMessage = "";
	//WacomGSS.STU.close();
	//(e || window.event).returnValue = confirmationMessage; // Gecko + IE
	//return confirmationMessage;                            // Webkit, Safari, Chrome
});
// Error-derived object for Device Control App not ready exception
function DCANotReady() { }
DCANotReady.prototype = new Error();

function tabletDemo() {
    var p = new WacomGSS.STU.Protocol();
    var intf;
    var m_usingEncryption = false;
    var m_encH;
    var m_encH2;
    var m_encH2Impl;
    WacomGSS.STU.isDCAReady()
        .then(function (message) {
            if (!message) {
                throw new DCANotReady();
            }
            // Set handler for Device Control App timeout
            WacomGSS.STU.onDCAtimeout = onDCAtimeout;
            return WacomGSS.STU.getUsbDevices();
        })
        .then(function (message) {
            if (message == null || message.length == 0) {
                throw new Error("No STU devices found");
            }
            console.log("received: " + JSON.stringify(message));
            m_usbDevices = message;
            return WacomGSS.STU.isSupportedUsbDevice(m_usbDevices[0].idVendor, m_usbDevices[0].idProduct);
        })
        .then(function (message) {
            console.log("received: " + JSON.stringify(message));
            intf = new WacomGSS.STU.UsbInterface();
            return intf.Constructor();
        })
        .then(function (message) {
            console.log("received: " + JSON.stringify(message));
            return intf.connect(m_usbDevices[0], true);
        })
        .then(function (message) {
            console.log("received: " + JSON.stringify(message));
            console.log(0 == message.value ? "connected!" : "not connected");
            if (0 == message.value) {
                m_encH = new WacomGSS.STU.EncryptionHandler(new encryptionHandler());
                return m_encH.Constructor();
            }
        })
        .then(function (message) {
            console.log("received: " + JSON.stringify(message));
            m_encH2Impl = new encryptionHandler2();
            m_encH2 = new WacomGSS.STU.EncryptionHandler2(m_encH2Impl);
            return m_encH2.Constructor();
        })
        .then(function (message) {
            console.log("received: " + JSON.stringify(message));
            tablet = new WacomGSS.STU.Tablet();
            return tablet.Constructor(intf, m_encH, m_encH2);
        })
        .then(function (message) {
            console.log("received: " + JSON.stringify(message));
            intf = null;
            return tablet.getInkThreshold();
        })
        .then(function (message) {
            console.log("received: " + JSON.stringify(message));
            m_inkThreshold = message;
            return tablet.getCapability();
        })
        .then(function (message) {
            console.log("received: " + JSON.stringify(message));
            m_capability = message;
            createModalWindow(m_capability.screenWidth, m_capability.screenHeight);
            return tablet.getInformation();
        })
        .then(function (message) {
            console.log("received: " + JSON.stringify(message));
            return tablet.getInkThreshold();
        })
        .then(function (message) {
            console.log("received: " + JSON.stringify(message));
            return tablet.getProductId();
        })
        .then(function (message) {
            console.log("received: " + JSON.stringify(message));
            return WacomGSS.STU.ProtocolHelper.simulateEncodingFlag(message, m_capability.encodingFlag);
        })
        .then(function (message) {
            console.log("received: " + JSON.stringify(message));
            var encodingFlag = message;
            if ((encodingFlag & p.EncodingFlag.EncodingFlag_24bit) != 0) {
                return tablet.supportsWrite()
                    .then(function (message) {
                        m_encodingMode = message ? p.EncodingMode.EncodingMode_24bit_Bulk : p.EncodingMode.EncodingMode_24bit;
                    });
            } else if ((encodingFlag & p.EncodingFlag.EncodingFlag_16bit) != 0) {
                return tablet.supportsWrite()
                    .then(function (message) {
                        m_encodingMode = message ? p.EncodingMode.EncodingMode_16bit_Bulk : p.EncodingMode.EncodingMode_16bit;
                    });
            } else { // assumes 1bit is available
                m_encodingMode = p.EncodingMode.EncodingMode_1bit;
            }
        })
        .then(function (message) {
            console.log("received: " + JSON.stringify(message));
            return tablet.isSupported(p.ReportId.ReportId_EncryptionStatus); // v2 encryption
        })
        .then(function (message) {
            console.log("received: " + JSON.stringify(message));
            m_usingEncryption = message;
            // if the encryption script is missing turn off encryption regardless
            if (typeof window.sjcl == 'undefined') {
                console.log("sjcl not found - encryption disabled");
                m_usingEncryption = false;
            }
            return tablet.getDHprime();
        })
        .then(function (dhPrime) {
            console.log("received: " + JSON.stringify(dhPrime));
            return WacomGSS.STU.ProtocolHelper.supportsEncryption_DHprime(dhPrime); // v1 encryption
        })
        .then(function (message) {
            console.log("received: " + JSON.stringify(message));
            m_usingEncryption = (message ? true : m_usingEncryption);
            return tablet.setClearScreen();
        })
        .then(function (message) {
            if (m_usingEncryption) {
                return tablet.startCapture(0xc0ffee);
            }
            else {
                return message;
            }
        })
        .then(function (message) {
            if (typeof m_encH2Impl.error !== 'undefined') {
                throw new Error("Encryption failed, restarting demo");
            }
            return message;
        })
        .then(function (message) {
            console.log("received: " + JSON.stringify(message));
            return tablet.isSupported(p.ReportId.ReportId_PenDataOptionMode);
        })
        .then(function (message) {
            console.log("received: " + JSON.stringify(message));
            if (message) {
                return tablet.getProductId()
                    .then(function (message) {
                        var penDataOptionMode = p.PenDataOptionMode.PenDataOptionMode_None;
                        switch (message) {
                            case WacomGSS.STU.ProductId.ProductId_520A:
                                penDataOptionMode = p.PenDataOptionMode.PenDataOptionMode_TimeCount;
                                break;
                            case WacomGSS.STU.ProductId.ProductId_430:
                            case WacomGSS.STU.ProductId.ProductId_530:
                                penDataOptionMode = p.PenDataOptionMode.PenDataOptionMode_TimeCountSequence;
                                break;
                            default:
                                console.log("Unknown tablet supporting PenDataOptionMode, setting to None.");
                        };
                        return tablet.setPenDataOptionMode(penDataOptionMode);
                    });
            }
            else {
                m_encodingMode = p.EncodingMode.EncodingMode_1bit;
                return m_encodingMode;
            }
        })
        .then(function (message) {
            console.log("received: " + JSON.stringify(message));
            addButtons();
            var canvasImage = canvas.toDataURL("image/jpeg");
            return WacomGSS.STU.ProtocolHelper.resizeAndFlatten
                (
                    canvasImage,
                    0,
                    0,
                    0,
                    0,
                    m_capability.screenWidth,
                    m_capability.screenHeight,
                    m_encodingMode,
                    1,
                    false,
                    0,
                    true
                );
        })
        .then(function (message) {
            m_imgData = message;
            console.log("received: " + JSON.stringify(message));
            return tablet.writeImage(m_encodingMode, message);
        })
        .then(function (message) {
            if (m_encH2Impl.error) {
                throw new Error("Encryption failed, restarting demo");
            }
            return message;
        })
        .then(function (message) {
            console.log("received: " + JSON.stringify(message));
            return tablet.setInkingMode(p.InkingMode.InkingMode_On);
        })
        .then(function (message) {
            console.log("received: " + JSON.stringify(message));
            var reportHandler = new WacomGSS.STU.ProtocolHelper.ReportHandler();
            lastPoint = { "x": 0, "y": 0 };
            isDown = false;
            ctx.lineWidth = 1;

            var penData = function (report) {
                //console.log("report: " + JSON.stringify(report));
                m_penData.push(report);
                processButtons(report, canvas);
                processPoint(report, canvas, ctx);
            }
            var penDataEncryptedOption = function (report) {
                //console.log("reportOp: " + JSON.stringify(report));
                m_penData.push(report.penData[0], report.penData[1]);
                processButtons(report.penData[0], canvas);
                processPoint(report.penData[0], canvas, ctx);
                processButtons(report.penData[1], canvas);
                processPoint(report.penData[1], canvas, ctx);
            }

            var log = function (report) {
                //console.log("report: " + JSON.stringify(report));
            }

            var decrypted = function (report) {
                //console.log("decrypted: " + JSON.stringify(report));
            }
            m_penData = new Array();
            reportHandler.onReportPenData = penData;
            reportHandler.onReportPenDataOption = penData;
            reportHandler.onReportPenDataTimeCountSequence = penData;
            reportHandler.onReportPenDataEncrypted = penDataEncryptedOption;
            reportHandler.onReportPenDataEncryptedOption = penDataEncryptedOption;
            reportHandler.onReportPenDataTimeCountSequenceEncrypted = penData;
            reportHandler.onReportDevicePublicKey = log;
            reportHandler.onReportEncryptionStatus = log;
            reportHandler.decrypt = decrypted;
            return reportHandler.startReporting(tablet, true);
        })
        .fail(function (ex) {
            console.log(ex);
            
            if (ex instanceof DCANotReady) {
                // Device Control App not detected 
                // Reinitialize and re-try
                //WacomGSS.STU.Reinitialize();
                //setTimeout(tabletDemo, 1000);  No STU devices found
                setTimeout(close(), 0);
            }
            else {
                // Some other error - Inform the user and closedown 
                //alert("tabletDemo failed:\n" + ex);
                alert(ex.message);
                setTimeout(close(), 0);
            }
        });
}

function addButtons() {
    m_btns = new Array(3);
    m_btns[0] = new Button();
    m_btns[1] = new Button();
    m_btns[2] = new Button();

    if (m_usbDevices[0].idProduct != WacomGSS.STU.ProductId.ProductId_300) {
        // Place the buttons across the bottom of the screen.
        var w2 = m_capability.screenWidth / 3;
        var w3 = m_capability.screenWidth / 3;
        var w1 = m_capability.screenWidth - w2 - w3;
        var y = m_capability.screenHeight * 6 / 7;
        var h = m_capability.screenHeight - y;

        m_btns[0].Bounds = new Rectangle(0, y, w1, h);
        m_btns[1].Bounds = new Rectangle(w1, y, w2, h);
        m_btns[2].Bounds = new Rectangle(w1 + w2, y, w3, h);
    } else {
        // The STU-300 is very shallow, so it is better to utilise
        // the buttons to the side of the display instead.

        var x = m_capability.screenWidth * 3 / 4;
        var w = m_capability.screenWidth - x;

        var h2 = m_capability.screenHeight / 3;
        var h3 = m_capability.screenHeight / 3;
        var h1 = m_capability.screenHeight - h2 - h3;

        m_btns[0].Bounds = new Rectangle(x, 0, w, h1);
        m_btns[1].Bounds = new Rectangle(x, h1, w, h2);
        m_btns[2].Bounds = new Rectangle(x, h1 + h2, w, h3);
    }

    m_btns[0].Text = "OK";
    m_btns[1].Text = "Clear";
    m_btns[2].Text = "Cancel";
    m_btns[0].Click = btnOk_Click;
    m_btns[1].Click = btnClear_Click;
    m_btns[2].Click = btnCancel_Click;
    clearCanvas(canvas, ctx);
    drawButtons();
}

function drawButtons() {
    // This application uses the same bitmap for both the screen and client (window).

    ctx.save();
    ctx.setTransform(1, 0, 0, 1, 0, 0);

    ctx.beginPath();
    ctx.lineWidth = 1;
    ctx.strokeStyle = 'black';
    ctx.font = "30px Arial";

    // Draw the buttons
    for (var i = 0; i < m_btns.length; ++i) {
        //if (useColor)
        {
            ctx.fillStyle = "lightgrey";
            ctx.fillRect(m_btns[i].Bounds.x, m_btns[i].Bounds.y, m_btns[i].Bounds.width, m_btns[i].Bounds.height);
        }

        ctx.fillStyle = "black";
        ctx.rect(m_btns[i].Bounds.x, m_btns[i].Bounds.y, m_btns[i].Bounds.width, m_btns[i].Bounds.height);
        var xPos = m_btns[i].Bounds.x + ((m_btns[i].Bounds.width / 2) - (ctx.measureText(m_btns[i].Text).width / 2));
        var yOffset;
        if (m_usbDevices[0].idProduct == WacomGSS.STU.ProductId.ProductId_300)
            yOffset = 28;
        else if (m_usbDevices[0].idProduct == WacomGSS.STU.ProductId.ProductId_430)
            yOffset = 26;
        else
            yOffset = 40;
        ctx.fillText(m_btns[i].Text, xPos, m_btns[i].Bounds.y + yOffset);
    }
    ctx.stroke();
    ctx.closePath();

    ctx.restore();
}

function clearScreen() {
    clearCanvas(canvas, ctx);
    drawButtons();
    m_penData = new Array();
    tablet.writeImage(m_encodingMode, m_imgData);
}

async function btnOk_Click() {
    // You probably want to add additional processing here.
    IMGEN_WACOM = await generateImage();   
    if (IMGEN_WACOM) {
        addImagewacom(IMGEN_WACOM);
        setTimeout(close, 0);
    } 
    
}

function btnCancel_Click() {
    // You probably want to add additional processing here.
    setTimeout(close, 0);
}

function btnClear_Click() {
    // You probably want to add additional processing here.
    console.log("clear!");
    clearScreen();
}

function distance(a, b) {
    return Math.pow(a.x - b.x, 2) + Math.pow(a.y - b.y, 2);
}

function clearCanvas(in_canvas, in_ctx) {
    in_ctx.save();
    in_ctx.setTransform(1, 0, 0, 1, 0, 0);
    in_ctx.fillStyle = "white";
    in_ctx.fillRect(0, 0, in_canvas.width, in_canvas.height);
    in_ctx.restore();
}

function processButtons(point, in_canvas) {
    var nextPoint = {};
    nextPoint.x = Math.round(in_canvas.width * point.x / m_capability.tabletMaxX);
    nextPoint.y = Math.round(in_canvas.height * point.y / m_capability.tabletMaxY);
    var isDown2 = (isDown ? !(point.pressure <= m_inkThreshold.offPressureMark) : (point.pressure > m_inkThreshold.onPressureMark));

    var btn = -1;
    for (var i = 0; i < m_btns.length; ++i) {
        if (m_btns[i].Bounds.Contains(nextPoint)) {
            btn = i;
            break;
        }
    }

    if (isDown && !isDown2) {
        if (btn != -1 && m_clickBtn === btn) {
            m_btns[btn].Click();
        }
        m_clickBtn = -1;
    }
    else if (btn != -1 && !isDown && isDown2) {
        m_clickBtn = btn;
    }
    return (btn == -1);
}

function processPoint(point, in_canvas, in_ctx) {
    var nextPoint = {};
    nextPoint.x = Math.round(in_canvas.width * point.x / m_capability.tabletMaxX);
    nextPoint.y = Math.round(in_canvas.height * point.y / m_capability.tabletMaxY);
    var isDown2 = (isDown ? !(point.pressure <= m_inkThreshold.offPressureMark) : (point.pressure > m_inkThreshold.onPressureMark));

    if (!isDown && isDown2) {
        lastPoint = nextPoint;
    }

    if ((isDown2 && 10 < distance(lastPoint, nextPoint)) || (isDown && !isDown2)) {
        in_ctx.beginPath();
        in_ctx.moveTo(lastPoint.x, lastPoint.y);
        in_ctx.lineTo(nextPoint.x, nextPoint.y);
        in_ctx.stroke();
        in_ctx.closePath();
        lastPoint = nextPoint;
    }

    isDown = isDown2;
}
function ProcessTransparent(image_src) {
    try {
        var canvas = document.createElement('canvas');
        canvas.height = 250;
        canvas.width = 250;
        var context = canvas.getContext("2d");
        context.drawImage(image_src, 0, 0, canvas.width, canvas.height);
        var imageData = context.getImageData(0, 0, canvas.width, canvas.height);
        var preserveColor = function (imageData, color) {
            var data = imageData.data;
            for (var i = 0; i < data.length; i += 4) {
                var preserve = data[i] === color.r
                    && data[i + 1] === color.g
                    && data[i + 2] === color.b
                    && data[i + 3] === color.a;
                data[i + 3] = preserve ? data[i + 3] : 0;   
            }
            return imageData;
        };
        var newData = preserveColor(imageData, { r: 0, g: 0, b: 0, a: 255 });
        context.putImageData(newData, 0, 0);
        return canvas;
    } catch (ex) {
        alert("Funcion ProcessTransparent " + ex.message);
    }

}
async function generateImage() {
    try {
        if (m_penData.length == 0) {
            alert("Debe estampar la firma");
            return false;
        }
        var signatureCanvas = document.createElement("canvas");
        signatureCanvas.id = "signatureCanvas";
        signatureCanvas.height = 200;
        signatureCanvas.width = 200;
        var signatureCtx = signatureCanvas.getContext("2d");
        clearCanvas(signatureCanvas, signatureCtx);
        signatureCtx.lineWidth = 1;
        signatureCtx.strokeStyle = 'black';
        lastPoint = { "x": 0, "y": 0 };
        isDown = false;
        for (var i = 0; i < m_penData.length; i++) {
            processPoint(m_penData[i], signatureCanvas, signatureCtx);
        }
        //var image = new Image();
        //image.src = signatureCanvas.toDataURL("image/png");
        //var canvas_sig = ProcessTransparent(image);
        //document.body.appendChild(signatureCanvas);
        var ima_src = signatureCanvas.toDataURL("image/png");
        return ima_src;
    } catch (ex) {
        alert("Funcion generateImage " + ex.message)
    }
}

function close() {
    // Clear handler for Device Control App timeout
    WacomGSS.STU.onDCAtimeout = null;

    disconnect();
    document.getElementsByTagName('body')[0].removeChild(modalBackground);
    document.getElementsByTagName('body')[0].removeChild(formDiv);
}

function onCanvasClick(event) {
    // Enable the mouse to click on the simulated buttons that we have displayed.

    // Note that this can add some tricky logic into processing pen data
    // if the pen was down at the time of this click, especially if the pen was logically
    // also 'pressing' a button! This demo however ignores any that.

    var posX = event.pageX - formDiv.offsetLeft;
    var posY = event.pageY - formDiv.offsetTop;
    for (var i = 0; i < m_btns.length; i++) {
        if (m_btns[i].Bounds.Contains(new Point(posX, posY))) {
            m_btns[i].Click();
            break;
        }
    }
}