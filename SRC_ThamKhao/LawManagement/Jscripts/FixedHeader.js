var bodyContainer;
var headThs;
var isIE10 = false;
var isIE11 = false;
var isIE9 = false;
var isChrome = /Chrome/.test(navigator.userAgent) && /Google Inc/.test(navigator.vendor);
var isSafari = /Safari/.test(navigator.userAgent) && /Apple Computer/.test(navigator.vendor);

(function ($) {

    $.fn.fixedHeader = function (o) {
        var s = { adjustWidth: $.fixedHeader.calcWidth };
        if (o) $.extend(s, o);

        return this.each(function () {
            var table = $(this); //table
            var tId = this.id;

            var scrollBarWidth = $.fixedHeader.getScrollBarWidth();
            var IE6 = $.browser.msie && $.browser.version == '6.0';
            /*@cc_on
            if (/^9/.test(@_jscript_version)) {
                isIE9 = true;
            }
            if (/^10/.test(@_jscript_version)) {
                isIE10 = true;
            }
            if (/^11/.test(@_jscript_version)) {
                isIE11 = true;
            }
            @*/


            // height table
            var _bH = s.height;
            var _bW = s.width;

            //wrap a body container
            bodyContainer = table.wrap('<div></div>').parent()
				.attr('id', tId + "_body_container")
				.css({ width: s.width, height: s.height, overflow: 'auto' });

            //Wrap with an overall container
            var tableContainer = bodyContainer.wrap('<div></div>').parent()
				.attr('id', tId + '_table_container')
				.css('position', 'relative');

            //clone the header
            var _bodyWidth = parseInt(bodyContainer.innerWidth());
            if (table.height() > s.height)
                _bodyWidth = parseInt(bodyContainer.innerWidth()) - parseInt(scrollBarWidth);

            var position = table.position();
            var headerContainer = $(document.createElement('div'))
				.attr('id', tId + '_header_container')
				.css({
				    width: _bodyWidth,
				    height: table.find('thead').outerHeight(),
				    overflow: 'hidden',
				    top: position.top, left: position.left, borderBottom: '2px solid white'
				})
				.prependTo(tableContainer);

            var headerTable;
            headerTable = table.clone(true)
				.find('tbody').remove().end()
				.attr('id', tId + "_header")
				.addClass(s.tableClass || table[0].className)
				.css({
				    'table-layout': 'fixed', //width: table.width(), 					
				    position: 'absolute',
				    top: 0, left: 0
				})
            //.append(table.find('thead').clone(true))
				.appendTo(headerContainer);

            //sync header width			
            headThs = headerTable.find('th');
            table.find('th').each(function (i) {
                headThs.eq(i).css('width', s.adjustWidth(this));
            });

            bodyContainer.find('thead input[type=checkbox]').attr('tabindex', '0');
            //sync scroll
            var selects = IE6 ? table.find("select") : null;
            bodyContainer.scroll(function () {
                if (IE6 && selects.size() > 0) {
                    selects.each(function (i) {
                        this.style.visibility =
							($(this).offset().top - bodyContainer.offset().top) <= table.find("thead").outerHeight() + 10
							? 'hidden' : 'visible';
                    });
                }
                headerTable.css({
                    left: '-' + $(this).scrollLeft() + 'px'
                });
            })

            //Move it down
            headerContainer.css({
                position: 'absolute',
                top: 0
            });
        });
    }

    $.fixedHeader = {
        calcWidth: function (th) {
            var w = $(th).width();
            var paddingLeft = $.fixedHeader.getComputedStyleInPx(th, 'paddingLeft');
            var paddingRight = $.fixedHeader.getComputedStyleInPx(th, 'paddingRight');
            var borderWidth = $.fixedHeader.getComputedStyleInPx(th, 'borderRightWidth');

            if (($.browser.msie && !isIE10 && !isIE11) || isChrome) w = w + borderWidth;
            else if ($.browser.opera) w = w + borderWidth;
            else if (isSafari) w = w + paddingLeft + paddingRight + borderWidth * 2;
            else if ($.browser.mozilla && parseFloat($.browser.version) <= 1.8) w = w + borderWidth;  //FF2 still got a border-left missing problem, this is the best I can do.
            else if (isIE11 || isIE9) w = w - borderWidth + 2;
            else if (isIE10) w = w + 2;
            else if ($.browser.mozilla && parseFloat($.browser.version) >= 27) w = w - 1;
            return w;
        },
        getComputedStyleInPx: function (elem, style) {
            var computedStyle = (typeof elem.currentStyle != 'undefined')
				? elem.currentStyle
				: document.defaultView.getComputedStyle(elem, null);
            var val = computedStyle[style];
            val = val ? parseInt(val.replace("px", "")) : 0;
            return (!val || val == 'NaN') ? 0 : val;
        },
        getScrollBarWidth: function () { //calculate or get from global the scroll bar width
            if (!$.fixedHeader.scrollBarWidth) {
                var inner = $(document.createElement('p')).css({ width: '100%', height: '100%' });
                var outer = $(document.createElement('div'))
					.css({
					    position: 'absolute',
					    top: '0px',
					    left: '0px',
					    visibility: 'hidden',
					    width: '200px',
					    height: '150px',
					    overflow: 'hidden'
					})
					.append(inner)
					.appendTo(document.body);

                var w1 = inner[0].offsetWidth;
                outer[0].style.overflow = 'scroll';
                var w2 = inner[0].offsetWidth;
                if (w1 == w2) w2 = outer[0].clientWidth;
                document.body.removeChild(outer[0]);
                $.fixedHeader.scrollBarWidth = (w1 - w2);
            }
            return $.fixedHeader.scrollBarWidth;
        }
    }

})(jQuery);
