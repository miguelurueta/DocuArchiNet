CustomNode = function ()
{
	CustomNode.initializeBase(this);
	this.setNodeType(1);
	this.setLabel('label');
	this.setText('node text');
	this.setShape('Rectangle');
	this.setBrush('#fff');
	
	this.switchIcon = new Image();
	this.switchIcon.src = "Images/next.png";
	
	this.closeIcon = new Image();
	this.closeIcon.src = "Images/close.png";
}

CustomNode.prototype =
{
	initialize: function ()
	{
		CustomNode.callBaseMethod(this, 'initialize');
	},

	dispose: function ()
	{
		CustomNode.callBaseMethod(this, 'dispose');
	},

	toJson: function ()
	{
		var data = CustomNode.callBaseMethod(this, "toJson");
		data.label = this.label;
		data.nodeType = this.nodeType;
		return data;
	},

	updateCanvasElements: function (node)
	{
		CustomNode.callBaseMethod(this, 'updateCanvasElements');
		
		// add switch icon button
		this.switchButton = new MindFusion.Drawing.Image(
			new MindFusion.Drawing.Rect(this.bounds.x + this.bounds.width - 10, this.bounds.y, 5, 5));
		this.switchButton.image = this.switchIcon;
		this.switchButton.loaded = this.switchIcon.complete;
		this.graphicsContainer.content.push(this.switchButton);

		// add close button
		this.closeButton = new MindFusion.Drawing.Image(
			new MindFusion.Drawing.Rect(this.bounds.x + this.bounds.width - 5, this.bounds.y, 5, 5));
		this.closeButton.image = this.closeIcon;
		this.closeButton.loaded = this.closeIcon.complete;
		this.graphicsContainer.content.push(this.closeButton);
		
		//add label
		var font = this.getEffectiveFont();
		var label = new MindFusion.Drawing.Text(this.label,
			new MindFusion.Drawing.Rect(this.bounds.x, this.bounds.y + font.size/2, this.bounds.width - 10, 5));
		label.font = font;
		label.font.bold = true;
		label.pen = '#c30';
		label.fitInBounds = false;
		this.graphicsContainer.content.push(label);
		
        var rect = new MindFusion.Drawing.Rect(this.bounds.x, this.bounds.y + this.bounds.height, this.bounds.width, 5);
		var path = new MindFusion.Drawing.Path();
		path.addRect(rect.x, rect.y, rect.width, rect.height);
		path.setPen('transparent');
		path.setBrush('transparent');

		Array.insert(this.graphicsContainer.content, 0, path);

		this.text.setBounds(rect, 0);
	},

	getEditRect: function (point)
	{
		return this.text.bounds;
	},

	getButtonAtPoint: function (point)
	{
		if (this.switchButton.bounds.containsPoint(point)) return 'switch';
		if (this.closeButton.bounds.containsPoint(point)) return 'close';
		return '';
	},

	//properties
	getLabel: function ()
	{
		return this.label;
	},
	setLabel: function (value)
	{
		if (this.label !== value)
		{
			this.label = value;
		}
	},
	getNodeType: function ()
	{
		return this.nodeType;
	},
	setNodeType: function (value)
	{
		if (this.nodeType !== value)
		{
			this.nodeType = value;
			switch (this.nodeType)
			{
				case 1:
					this.setImageLocation('Images/icon1.png'); break;
				case 2:
					this.setImageLocation('Images/icon2.png'); break;
				case 3:
					this.setImageLocation('Images/icon3.png'); break;
			}
		}
	}
};

if (typeof(Sys) !== 'undefined')
	Sys.Application.notifyScriptLoaded();