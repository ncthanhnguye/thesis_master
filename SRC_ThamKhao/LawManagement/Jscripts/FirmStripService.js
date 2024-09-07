

var mMaxItems = 9;
var classes = 'rowhover rowselected rownormal';
var selectedRow;
var mPathIndex = 0;
var mScrollTop = 0;
var mImageIndex = new Array(); // thang nay luu image thu i dc set src
var bFilmStripAutoClick = false;
var PopUpInfo = null;
function SetFirmStrip() {
    try {
        if ($('.FirmStripScan > DIV > DIV > DIV').length > 0)
            $('.FirmStripScan').show();
        else
            $('.FirmStripScan').hide();
    } catch (e) { }

    try {
        $(".FirmStripScan").thumbnailScroller({
            scrollerType: "clickButtons",
            scrollerOrientation: "horizontal",
            scrollSpeed: 2,
            scrollEasing: "easeOutCirc",
            scrollEasingAmount: 600,
            acceleration: 4,
            scrollSpeed: 800,
            noScrollCenterSpace: 10,
            autoScrolling: 0,
            autoScrollingSpeed: 2000,
            autoScrollingEasing: "easeInOutQuad",
            autoScrollingDelay: 500
        });
        for (var i = 0; i < $(".FirmStripScan").length; i++)
            mImageIndex.push(0);
        $('#FirmStripScan > DIV > DIV > DIV').hover(
                    function () {
                        $(this).css('cursor', 'pointer');
                    });
        loadImageFirmStrip();
    }
    catch (e) { }
}

// load firm strip image
function loadImageFirmStrip() {
    try {
        $(".FirmStripScan").each(function (k) {
            var max = mImageIndex[k] + mMaxItems + 1;
            $(this).find("IMG").each(function (i) {
                try {
                    var today = new Date();
                    var div = $(this).parent();

                    if (i < max) {

                        mImageIndex[k] = i;
                        // phai gan src=javascript:void(0); tai vi gan src= null hoac = # se bi loi tren IE8
                        if ($(this).attr('src') == '' || $(this).attr('src') == 'javascript:void(0)')
                            ShowImage($(this), $('[id$=URLCarImage]').val() + String.format('?strEventID={0}&GMTTicks={1}&type=0&TableName=eventsimage&ConnString= ', div.attr('evtid'), div.attr('gmtid')));
                    }

                    // show popup carimage
                    $(this).hover(
                    function (e) {
                        var parent;
                        var parentPos;
                        try {
                            parent = $('[id$=divImageHoverFirmStrip]').parent();
                            parentPos = findPos(parent[0]);
                        }
                        catch (e) { }
                        var pos = findPos(this);
                        var divHover = $('#divImageHoverFirmStrip');
                        divHover.find('img').remove();
                        //var top = pos.top - divHover.height() - 10;
                        var top = e.pageY - (divHover.height() + 22);
                        var left = pos.left - (divHover.width() / 2) + 120;
                        // 241: do rong cua popup 
                        if ((left + divHover.width() - 30) > (parentPos.left + parent.width())) {
                            left = left - divHover.width() + 135;
                            $('#divImageHoverFirmStrip').attr('class', 'imagehoverfirmstrip_right');
                        }
                        else {
                            $('#divImageHoverFirmStrip').attr('class', 'imagehoverfirmstrip');
                        }


                        var img = $(this).clone();
                        divHover.append(img).css({ top: top, left: left }).show();
                    },
                    function () {
                        $('#divImageHoverFirmStrip').find('img').remove().end().hide();
                    });
                    //Click on Image
                    $(this).click(function () {
                        PopUpInfo = $('.popupInfo');
                        $('.closePopupInfo').click(function () {
                            try {
                                PopUpInfo.hide();
                                HideBackground();
                            } catch (e) { }
                        });
                        ShowPopUpInfo($(this).parent());
                    });
                } catch (e) { }
            });
        });        
    } catch (e) { }
}
function ShowImage(ctrlImage, imgUrl) {
    try {
        $(ctrlImage).attr('src', function (i, value) {
            return imgUrl;
        }).error(function () {
            if (imgUrl.indexOf('&c=') >= 0)
                return;
            setTimeout(function () {               
                ShowImage(this, imgUrl + '&c=' + (new Date()).getTime());
            }
        , 500);
        });
    } catch (e) {

    }
}
//0: auto click; 1: previous button; 2: next button
function loadImageFirmStrip_1(type, filmStrip) {
    try {
        var k = parseInt($(filmStrip).attr('idexFilmStrip'));
        var max = mMaxItems;
        max = mImageIndex[k] + mMaxItems + 1;

        if (type != 2)
            mImageIndex[k] -= (mMaxItems - 1);

        if (mImageIndex[k] < 0)
            mImageIndex[k] = 0;

        if (max > $(filmStrip).find("IMG").length)
            max = $(filmStrip).find("IMG").length;

        var startIndex = mImageIndex[k];
        $(filmStrip).find("IMG").each(function (i) {
            var today = new Date();
            var div = $(this).parent();

            if (i >= startIndex && i < max) {

                mImageIndex[k] = i;
                if ($(this).attr('src') == '' || $(this).attr('src') == 'javascript:void(0)')
                    ShowImage($(this), $('[id$=URLCarImage]').val() + String.format('?strEventID={0}&GMTTicks={1}&type=0&TableName=eventsimage&ConnString= ', div.attr('evtid'), div.attr('gmtid')));
            }
        });
    } catch (e) { }
}
function ShowPopUpInfo(div) {
    try {     
        ShowImage($(PopUpInfo).find('.imgPlate'), $('[id$=URLPlateImage]').val() + String.format('?strEventID={0}&GMTTicks={1}&type=1&TableName=eventsimage&ConnString= ', div.attr('evtid'), div.attr('gmtid')));
        ShowImage($(PopUpInfo).find('.imgCar'), $('[id$=URLCarImage]').val() + String.format('?strEventID={0}&GMTTicks={1}&type=0&TableName=eventsimage&ConnString= ', div.attr('evtid'), div.attr('gmtid')));
        $('[id$=hdfSelectedLatLng]').val(div.attr('lat') + ";" + div.attr('lon'));
        var inf = div.find('.divScrollerText').text().split(' ');//
        if (inf.length == 4) {
            $(PopUpInfo).find('#plateName').text(inf[0]);
            $(PopUpInfo).find('#localTime').text(inf[1] + " " + inf[2] + " "+ inf[3]); //
            $(PopUpInfo).find('#GMTTime').text(div.attr('gmttime'));
        }
        ShowBackground();   
        var Pagesize = getPageSize();
        var height = $(PopUpInfo).height();
        var width = $(PopUpInfo).width();
        var _top = (Pagesize.height - height) / 2 + Pagesize.scrollTop;
        var _left = (Pagesize.width - width) / 2;
        $(PopUpInfo).css({ top: _top, left: _left });
        PopUpInfo.show();       
           
    } catch (e) {

    }
    
}