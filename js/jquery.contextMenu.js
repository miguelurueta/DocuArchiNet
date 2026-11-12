/** jQuery.contextMenu: application context menus; preserves the legacy callback argument. */
(function ($) {
    var sequence = 0, activeClose = null, instances = [];
    $.fn.contextMenu = function (name, actions, settings) {
        instances.forEach(function (instance) { if (!instance.element.isConnected) { instance.destroy(); } });
        instances = instances.filter(function (instance) { return !instance.disposed; });
        var options = $.extend({disable_native_context_menu:false, leftClick:false}, settings);
        return this.each(function () {
            var element = $(this), registry = element.data('docContextMenu') || {}, previous = registry[name];
            if (previous) { previous.destroy(); }
            var ns = '.docContextMenu' + (++sequence), id = document.getElementById(name) ? name + '-' + sequence : name;
            var menu = $('<ul>').attr({id:id, 'class':'context-menu', role:'menu'}).hide().appendTo('body');
            var activeElement = null, invocation = null, items = [];
            function close(restore) {
                if (!menu.is(':visible')) { return; }
                menu.hide(); $(document).off('mousedown' + ns); $(window).off('resize' + ns + ' scroll' + ns);
                menu.trigger('closed');
                if (options.hideMenu) { options.hideMenu.call(menu, activeElement, invocation); }
                if (activeClose === close) { activeClose = null; }
                if (restore && invocation && invocation.invoker && invocation.invoker.isConnected) { invocation.invoker.focus(); }
            }
            $.each(actions, function (label, itemOptions) {
                var link = itemOptions.link ? $(itemOptions.link) : $('<a href="#">').text(label);
                if (itemOptions.klass) { link.prepend($('<i>').addClass(itemOptions.klass).attr('aria-hidden','true'), document.createTextNode(' ')); }
                link.attr({role:'menuitem', tabindex:'-1'});
                var item = $('<li role="none">').append(link).appendTo(menu);
                items.push({element:item, link:link, options:itemOptions});
                item.on('click' + ns, function (event) {
                    event.preventDefault(); event.stopPropagation();
                    if (link.attr('aria-disabled') === 'true') { return; }
                    var context = invocation, target = activeElement;
                    close(true);
                    if (typeof itemOptions.click === 'function') { itemOptions.click(target, context); }
                });
            });
            function focusable() { return menu.find('[role="menuitem"]').filter(':visible').filter(function () { return $(this).attr('aria-disabled') !== 'true'; }); }
            function open(event, keyboard) {
                if (activeClose) { activeClose(false); }
                activeElement = $(this);
                var target = event.target, invoker = $(target).closest('a,button,input,select,[tabindex]')[0] || this;
                invocation = {target:target, invoker:invoker, keyboard:!!keyboard, data:options.capture ? options.capture.call(activeElement, target) : null};
                items.forEach(function (item) {
                    var enabled = item.options.disabled !== true && (!item.options.enabled || item.options.enabled(activeElement, invocation) !== false);
                    item.link.attr('aria-disabled', enabled ? 'false' : 'true'); item.element.toggleClass('context-disabled', !enabled);
                });
                menu.css({visibility:'hidden',position:'fixed',zIndex:1000000000,boxSizing:'border-box',overflowY:'auto',overflowX:'hidden',maxHeight:Math.max(1,window.innerHeight-16)+'px',maxWidth:Math.max(1,window.innerWidth-16)+'px',minWidth:Math.min(200,Math.max(1,window.innerWidth-16))+'px'}).show();
                if (options.showMenu) { options.showMenu.call(menu, activeElement, invocation); }
                var bounds = invoker.getBoundingClientRect(), x = keyboard ? bounds.left : event.clientX, y = keyboard ? bounds.bottom : event.clientY;
                if (typeof x !== 'number') { x = event.pageX - $(window).scrollLeft(); }
                if (typeof y !== 'number') { y = event.pageY - $(window).scrollTop(); }
                var width=menu.outerWidth(true), height=menu.outerHeight(true);
                menu.css({visibility:'visible',left:Math.max(8,Math.min(x,window.innerWidth-width-8))+'px',top:Math.max(8,Math.min(y,window.innerHeight-height-8))+'px'});
                activeClose = close;
                $(document).on('mousedown' + ns, function (e) { if (!menu[0].contains(e.target)) { close(false); } });
                $(window).on('resize' + ns + ' scroll' + ns, function () { close(false); });
                if (keyboard) { focusable().first().trigger('focus'); }
                event.preventDefault(); event.stopPropagation(); return false;
            }
            var mouseEvent = (options.leftClick ? 'click' : 'contextmenu') + ns;
            function keyOpen(event) { if (event.key === 'ContextMenu' || (event.key === 'F10' && event.shiftKey)) { return open.call(this,event,true); } }
            if (options.delegateEventTo) { element.on(mouseEvent,options.delegateEventTo,function (e) { return open.call(this,e,false); }).on('keydown'+ns,options.delegateEventTo,keyOpen); }
            else { element.on(mouseEvent,function (e) { return open.call(this,e,false); }).on('keydown'+ns,keyOpen); }
            menu.on('keydown'+ns, function (event) {
                if (event.key === 'Escape' || event.key === 'Tab') { close(true); if (event.key === 'Escape') { event.preventDefault(); event.stopPropagation(); } return; }
                if (event.key === 'Enter' || event.key === ' ') { $(document.activeElement).filter('[role="menuitem"]').trigger('click'); event.preventDefault(); event.stopPropagation(); return; }
                var choices=focusable(), index=choices.index(document.activeElement), next;
                if (!choices.length) { return; }
                if (event.key === 'ArrowDown') { next=(index+1)%choices.length; }
                else if (event.key === 'ArrowUp') { next=(index+choices.length-1)%choices.length; }
                else if (event.key === 'Home') { next=0; }
                else if (event.key === 'End') { next=choices.length-1; }
                if (next !== undefined) { choices.eq(next).trigger('focus'); event.preventDefault(); event.stopPropagation(); }
            });
            $(document).on('contextmenu'+ns,function (event) { if (options.disable_native_context_menu) { event.preventDefault(); } if (!element[0].contains(event.target)) { close(false); } });
            registry[name] = {element:element[0],disposed:false,destroy:function () { close(false); element.off(ns); $(document).off(ns); $(window).off(ns); menu.remove(); this.disposed=true; }};
            instances.push(registry[name]);
            element.data('docContextMenu',registry);
        });
    };
})(jQuery);
