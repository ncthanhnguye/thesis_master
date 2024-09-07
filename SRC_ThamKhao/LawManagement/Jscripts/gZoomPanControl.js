
//add zomm pan control
function addZoomPanControl(gMap) {

    addGZoomControl(gMap);
    addGDragZoomControl(gMap);
}

function addGZoomControl(gMap) {
    try {
        // dung cua google thi bi loi tren IE 10
        var mainCtrl = document.createElement('div');
        var zoomin = document.createElement('div');
        var zoomout = document.createElement('div');

        mainCtrl.className = 'gmzoomcontrol';
        zoomin.className = 'gm_zoomin';
        zoomout.className = 'gm_zoomout';

        // zoom in
        $(zoomin).click(function () {
            gMap.setZoom(gMap.getZoom() + 1);
        }).attr('title', 'Zoom in');

        $(zoomout).click(function () {
            var zoom = gMap.getZoom() - 1;
            if (zoom < 0)
                zoom = 0;
            gMap.setZoom(zoom);
        }).attr('title', 'Zoom out');

        mainCtrl.appendChild(zoomin);
        mainCtrl.appendChild(zoomout);
        // add len map
        gMap.controls[google.maps.ControlPosition.TOP_LEFT].push(mainCtrl);
    } catch (e) { }
}

function addGDragZoomControl(gMap) {
    try {
        // magnifying glass
        gMap.enableKeyDragZoom({
            visualEnabled: true,
            visualPosition: google.maps.ControlPosition.LEFT_TOP,
            visualPositionOffset: new google.maps.Size(7, 0),
            visualPositionIndex: null,
            visualSprite: "images/icons/dragzoom_btn.png",
            visualSize: new google.maps.Size(20, 20),
            visualTips: {
                off: "Zoom In/Out",
                on: "Zoom In/Out"
            },
            boxStyle: {
                border: "4px solid black",
                backgroundColor: "transparent",
                opacity: 1.0
            },
            veilStyle: {
                backgroundColor: "gray",
                opacity: 0.40,
                cursor: "crosshair"
            }

        });
    } catch (e) { }
}

// add on map
function addOnMap(map, obj) {
    try {
        obj.setMap(map);
    } catch (e) { }
}

// rremove object
function removeOnMap(obj) {
    try {
        obj.setMap(null);
    } catch (e) { }
}

// convert LatLngToPoint
function fromLatLngToPoint(latlng, map) {
    var point;
    try {
        var scale = Math.pow(2, map.getZoom());
        var normalizedPoint = map.getProjection().fromLatLngToPoint(latlng);
        point = new google.maps.Point(normalizedPoint.x * scale, normalizedPoint.y * scale);
    } catch (e) { }

    return point;
}

function fromPointToLatLng(point, map) {
    var latlng;
    try {
        var scale = Math.pow(2, map.getZoom());
        var prj = map.getProjection();         
        latlng = prj.fromPointToLatLng(new google.maps.Point(x_top / scale, y_top / scale));
    } catch (e) { }

    return latlng;
}

function createInfoWindow(marker, content, maxWidth, maxheight) {
    var popup;

    try {       
        popup = new google.maps.InfoWindow({ maxWidth: maxWidth, maxHeight: maxheight });
        popup.setContent(content);
        popup.setPosition(marker.position);

    } catch (e) { }

    return popup;
}

function openInfoWindow(infowindow, map) {
    try {
        if (infowindow) {
            infowindow.open(map);

            if ($.browser.mozilla)
                setMapCenterAutopanTo(infowindow, map);
        }

    } catch (e) { }
}



// ham nay chi dung cho Firefox
// set lai map trong th open windowinfo va auto pan map
// dung cach nay de tri benh chuoi cua FF 18, 19
function setMapCenterAutopanTo(infoWindow, map) {
    try {
        if (infoWindow) {
            google.maps.event.addListener(infowindow, 'domready', function () {
                var point = this.map.getCenter();
                var myLatLng = new google.maps.LatLng(point.lat() + 0.00000833 / 1000, point.lng());
                map.setCenter(myLatLng);
            });
        }

    } catch (e) { }
}

/// viet object de convert latlng to pixel va nguoc lai
try {
    function overlay(map) {
        this._map = map;
        this.setMap(map);
    }

    overlay.prototype = new google.maps.OverlayView();
    overlay.prototype.draw = function () { };    

    overlay.prototype.fromLatLngToPixel = function (latlng) {
        var position;
        try {
            var projection = this.getProjection();
            position = projection.fromLatLngToDivPixel(latlng);
        } catch (e) {            
        }

        return position;
    }

    //var pixelLatLng = overlay.getProjection().fromDivPixelToLatLng(new google.maps.Point(200,200));
    overlay.prototype.fromDivPixelToLatLng = function (point) {
        var latlng;
        try {
            var projection = this.getProjection();
            latlng = projection.fromDivPixelToLatLng(point);
        } catch (e) {
            
        }

        return latlng;
    }
} catch (e) { }

// FitIn map
function fitInMapBounds(lstPolygon, map) {
    try {
        
        var polygon;
        var bounds = new google.maps.LatLngBounds();
        var vertexs;
        var iVertex;

        var minX, minY, maxX, maxY;
        
        for (var i = 0; i < lstPolygon.length; i++) {
            polygon = lstPolygon[i];

            if (i == 0) {
                iVertex = polygon.getPath().getAt(0);
                minX = maxX = iVertex.lat();
                minY = maxY = iVertex.lng();
            }

            if (polygon) {
                vertexs = polygon.getPath();
                for (var j = 0; j < vertexs.length; j++) {
                    iVertex = vertexs.getAt(j);

                    if (minX > iVertex.lat())
                        minX = iVertex.lat();

                    if (minY > iVertex.lng())
                        minY = iVertex.lng();

                    if (maxX < iVertex.lat())
                        maxX = iVertex.lat();

                    if (maxY < iVertex.lng())
                        maxY = iVertex.lng();
                }
            }
        }

        var p1 = new google.maps.LatLng(maxX, minY);
        var p2 = new google.maps.LatLng(maxX, maxY);
        var p3 = new google.maps.LatLng(minX, maxY);
        var p4 = new google.maps.LatLng(minX, minY);

        bounds.extend(p1);
        bounds.extend(p2);
        bounds.extend(p3);
        bounds.extend(p4);

        var ptNW = p3;
        var ptSE = p1;

        var center = new google.maps.LatLng((ptSE.lat() + ptNW.lat()) / 2, (ptSE.lng() + ptNW.lng()) / 2);

        map.setCenter(center);
        map.fitBounds(bounds);              

    } catch (e) { }
}

function getPolygonsToLEARN(polygons) {

    var Poly = "";
    var resPoly = "";
    var curVertex;
    var nPoint;
    var Vertexs;

    if (polygons.length > 0) {

        for (var j = 0; j < polygons.length; j++) {

            Poly = "";
            var polygon = polygons[j];

            if (polygon != null) {

                Vertexs = polygon.getPath();
                nPoint = Vertexs.length;
                if (nPoint > 0) {
                    for (i = 0; i < nPoint; i++) {
                        curVertex = Vertexs.getAt(i);
                        Poly += "," + curVertex.lng() + "," + curVertex.lat();
                    }

                    Poly = nPoint + Poly;
                }
            }

            if (Poly != '')
                resPoly += ";" + Poly;
        }
    }

    if (resPoly != "")
        resPoly = resPoly.substr(1, resPoly.length - 1);

    return resPoly;
}
function removeGoogeMapElement(mapid) {
    try {
        $('#' + mapid + ' a[title^="Report errors"]').hide();
    } catch (e) { }
}