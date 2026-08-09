(function ($) {
    $.fn.imageResize = function (options) {
        var that = this;
        var settings = {
            width: 100,
            height: 80
        };
        options = $.extend(settings, options);
        if (!that.is('img')) {
            return;
        }
        return that.each(function () {

            var maxWidth = options.width;
            var maxHeight = options.height;
            var ratio = 0;
            var vale = $('#draggable');
            var width = $('#draggable').width()-10;
            var height = $('#draggable').height()-10;
            //var vale = $('#draggable').height();
            $(that).css('height', height);
            $(that).css('width', width);
           
            //$('#Hiddenintercambio').attr("value", width);
            //alert($('#Hiddenintercambio').value);
            //var width = $(that).width();
            //var height = $(that).height();
            /*
            if (width > maxWidth) {
            ratio = maxWidth / width;
            $(that).css('width', maxWidth);
            $(that).css('height', height * ratio);

            }
            
            if (height > maxHeight) {
            ratio = maxHeight / height;
            $(that).css('height', maxHeight);
            $(that).css('width', width * ratio);

            }
            */
        });

    };
})(jQuery);
