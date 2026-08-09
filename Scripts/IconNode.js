// JScript File

IconNode = function() 
{ 
    IconNode.initializeBase(this);
}

IconNode.prototype = 
{
    initialize: function() 
    {
      IconNode.callBaseMethod(this, 'initialize');
    },

    dispose: function() 
    {
        IconNode.callBaseMethod(this, 'dispose');
    },
    
    serialize: function()
	{
		var data = IconNode.callBaseMethod(this, "serialize");
		data.label = this._label;
		return data;
	},
    
    //properties
    get_label: function() 
    {
        return this._label;
    },
    set_label: function(value) 
    {
        if (this._label !== value) 
        {
            this._label = value;
            this.propertyChanged();        
        }
    }
    
};

if (typeof(Sys) !== 'undefined')
	Sys.Application.notifyScriptLoaded();