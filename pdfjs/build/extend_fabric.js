var PDFAnnotateFabric = function (container_id, url, options = {}) {
   
    this.url = url;
    this.container_id = container_id;
    var canvas = new fabric.Canvas(this.container_id, {
        backgroundColor: "green",
        fill: "red"
    });
    
    var line9 = new fabric.Line(
        [canvas.width / 2, 0, canvas.width / 2, canvas.width],
        {
            stroke: "red"
        }
    );
    this.line9_ = line9;
    line9.selectable = false;
    line9.evented = false;
    canvas.add(line9);
    // Load image
    fabric.Image.fromURL(
        this.url,
        function (image) {
            image.snapAngle = 15;
            canvas.add(image);
        }
    );
   
    canvas.setZoom(0.6);
    canvas.controlsAboveOverlay = true;
    canvas.clipPath = new fabric.Rect({
        width: 1200,
        height: 800,
        top: 50,
        left: 40,
        backgroundColor: "red"
    });
    this.canvas_ = canvas;
    // used for grids
    var snapZone = 7;
    canvas.on("object:moving", function (options) {
        let { width, height } = options.target;
        // Snap vert
        var objectMiddle = options.target.left + width / 2;
        if (
            objectMiddle > canvas.width / 2 - snapZone &&
            objectMiddle < canvas.width / 2 + snapZone
        ) {
            options.target
                .set({
                    left: canvas.width / 2 - width / 2
                })
                .setCoords();
        }

        // Snap hori
        var objectHeight = options.target.top + height / 2;

        if (
            objectHeight > canvas.height / 2 - snapZone &&
            objectHeight < canvas.height / 2 + snapZone
        ) {
            options.target
                .set({
                    top: canvas.height / 2 - height / 2
                })
                .setCoords();
        }

        // Snap to left
        var left = options.target.left;
        if (left < snapZone && left > -snapZone * 3) {
            options.target
                .set({
                    left: 0
                })
                .setCoords();
        }

        // Snap right
        var left = options.target.left;
        var inSnapAreaLeft = left + width > canvas.width - snapZone;
        var inSnapAreaRight = left + width < canvas.width + snapZone;

        if (inSnapAreaLeft && inSnapAreaRight) {
            options.target
                .set({
                    left: canvas.width - width
                })
                .setCoords();
        }

        // Snap top
        if (
            options.target.top < snapZone &&
            options.target.top > -snapZone * 3
        ) {
            options.target
                .set({
                    top: 0
                })
                .setCoords();
        }

        // Snap bottom
        if (
            options.target.top + height > canvas.height - snapZone &&
            options.target.top + height < canvas.height + snapZone
        ) {
            options.target
                .set({
                    top: canvas.height - height
                })
                .setCoords();
        }
    });
    
}