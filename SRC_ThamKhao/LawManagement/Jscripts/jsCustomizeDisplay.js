
// properties
var mColumns = [
{ "DisplayName": "Total Detections", "Name": "TotalDetections" },
{ "DisplayName": "Times Subject Vehicle Sighted", "Name": "TotalSighted" },
{ "DisplayName": "Total Site Visits with LPR", "Name": "TotalVisits" },
    { "DisplayName": "Location Address", "Name": "Address" },
    { "DisplayName": "Date", "Name": "datetime" },
    { "DisplayName": "Time", "Name": "datetime" },
    { "DisplayName": "Plate", "Name": "carnumber" }
];
// show popup

function showCustomDisplay() {
    closeReportType();
    effectEventIcons();

    try {
        var _listColumnExist = null;
        var strExistsColumn = $("[id$=hdfColumnsDisplay]").val();
        if (strExistsColumn != "")
            _listColumnExist = jQuery.parseJSON(strExistsColumn);
        else
            _listColumnExist = [
             { "DisplayName": "Location Address", "Name": "Address" },
                { "DisplayName": "Plate", "Name": "carnumber" },
                { "DisplayName": "Date", "Name": "datetime" },
                { "DisplayName": "Time", "Name": "datetime" },
                  { "DisplayName": "GMTDateTime", "Name": "GMTDateTime" }
            ];

        InsertAvailableFiled(_listColumnExist);
    } catch (e) {

    }

    try {
        var Pagesize = getPageSize();
        var $panel = $("#divCustomizeDisplay");

        var top = (Pagesize.height / 2 - $panel.height() / 2) + Pagesize.scrollTop;
        var left = Pagesize.width / 2 - $panel.width() / 2;
        $panel.css({ top: top, left: left }).show();
        $("#divCustomizeDisplay_BG").css({ width: Pagesize.pageWidth, height: Pagesize.pageHeight, top: 0, left: 0, zIndex: 199 }).show();
    } catch (e) { }
}

// add su kien hover cho icon
function effectEventIcons() {
    try {
        $("#btnAddField").hover(
            function () {
                $(this).attr('src', 'Images/icons/add-over.bmp');
            }, function () {
                $(this).attr('src', 'Images/icons/add.bmp');
            }
        );

        $("#btnRemoveField").hover(
            function () {
                $(this).attr('src', 'Images/icons/remove-over.bmp');
            }, function () {
                $(this).attr('src', 'Images/icons/remove.bmp');
            }
        );

        $("#btnTopField").hover(
            function () {
                $(this).attr('src', 'Images/icons/top-over.bmp');
            }, function () {
                $(this).attr('src', 'Images/icons/top.bmp');
            }
        );

        $("#btnUpField").hover(
            function () {
                $(this).attr('src', 'Images/icons/up-over.bmp');
            }, function () {
                $(this).attr('src', 'Images/icons/up.bmp');
            }
        );

        $("#btnDownField").hover(
            function () {
                $(this).attr('src', 'Images/icons/down-over.bmp');
            }, function () {
                $(this).attr('src', 'Images/icons/down.bmp');
            }
        );

        $("#btnBottomField").hover(
            function () {
                $(this).attr('src', 'Images/icons/bottom-over.bmp');
            }, function () {
                $(this).attr('src', 'Images/icons/bottom.bmp');
            }
        );
    } catch (e) { }
}

// close popup
function closePopupPanel() {

    try {
        $("#divCustomizeDisplay").hide();
        $('#divCustomizeDisplay_BG').hide();
        //setFocusActiveElementBefore();        
    }
    catch (e) { }

    try {
        $('[id$=lstAvailableField]').empty();
        $('[id$=lstFieldInclude]').empty();
    } catch (e) {

    }
}

// kiem tra phan tu thuoc mang ?
function checkExist(_arr, name) {
    try {
        for (var i = 0; i < _arr.length; i++) {
            if (_arr[i].DisplayName.toLowerCase() == name.toLowerCase())
                return false;
        }
    }
    catch (e) { return false; }
    return true;
}

// inseart item for available field
function InsertAvailableFiled(_arr) {
    try {
        if (_arr == null) return;
        var _CtrlAvailable = document.getElementById($('[id$=lstAvailableField]').attr('id'));
        var _CtrlInclue = document.getElementById($('[id$=lstFieldInclude]').attr('id'));

        while (_CtrlAvailable.length > 0)
            _CtrlAvailable.remove(0);

        while (_CtrlInclue.length > 0)
            _CtrlInclue.remove(0);

        // insert list fields include
        for (var i = 0; i < _arr.length; i++) {
            insertListBox(_CtrlInclue, _arr[i].DisplayName, _arr[i].Name);
        }
        // insert list available field
        // phan nay dua danh sach cac item trong list box availabe vo mang de sort theo display name
        var _newArray = new Array();
        for (var i = 0; i < mColumns.length; i++) {
            if (checkExist(_arr, mColumns[i].DisplayName))
                _newArray.push(mColumns[i]);
        }
        _newArray.sort(function (a, b) { return (a.DisplayName < b.DisplayName) ? -1 : 1; })

        // add item vo list box available
        for (var i = 0; i < _newArray.length; i++)
            insertListBox(_CtrlAvailable, _newArray[i].DisplayName, _newArray[i].Name);

        //HighLight dong dau tien     
        if (_CtrlAvailable.options.length > 0)
            _CtrlAvailable.options[0].selected = true;
        if (_CtrlInclue.options.length > 0)
            _CtrlInclue.options[0].selected = true;
    }
    catch (e) { }
}


//insert list box
function insertListBox(Ctrl, name, value) {
    try {
        var opt = document.createElement('OPTION');
        opt.text = name;
        opt.value = value;
        Ctrl.options.add(opt);
    }
    catch (e) { }
}

// add fileds
function AddSelectField() {
    try {
        var _availaList = document.getElementById($('[id$=lstAvailableField]').attr('id'));
        var _selectedList = document.getElementById($('[id$=lstFieldInclude]').attr('id'));

        if (_availaList.options.selectedIndex < 0) return;
        var _inDex = _availaList.options.selectedIndex;

        insertListBox(_selectedList, _availaList.options[_inDex].text, _availaList.options[_inDex].value);
        _availaList.remove(_inDex);

        if (_selectedList.options.length > 0)
            _selectedList.options[0].selected = true;

        if (_availaList.options.length > 0) {
            if (_inDex >= _availaList.options.length)
                _inDex = 0;
            _availaList.options[_inDex].selected = true;
        }

    } catch (e) { }
}

// remove item from selected list and insert available list
function RemoveSelectedField() {
    try {
        var _availaList = document.getElementById($('[id$=lstAvailableField]').attr('id'));
        var _selectedList = document.getElementById($('[id$=lstFieldInclude]').attr('id'));

        if (_selectedList.options.selectedIndex < 0) return;
        var _inDex = _selectedList.options.selectedIndex;

        insertListBox(_availaList, _selectedList.options[_inDex].text, _selectedList.options[_inDex].value);
        _selectedList.remove(_inDex);

        // sort lai list
        var _arr = new Array();
        var _ItemIndex = "";
        while (_availaList.options.length > 0) {
            _ItemIndex = { 'DisplayName': _availaList.options[0].text, 'Name': _availaList.options[0].value };
            _arr.push(_ItemIndex);
            _availaList.remove(0);
        }

        _arr.sort(function (a, b) { return (a.DisplayName < b.DisplayName) ? -1 : 1; });
        for (var i = 0; i < _arr.length; i++)
            insertListBox(_availaList, _arr[i].DisplayName, _arr[i].Name);

        if (_availaList.options.length > 0)
            _availaList.options[0].selected = true;

        if (_selectedList.options.length > 0) {
            if (_inDex >= _selectedList.options.length)
                _inDex = 0;
            _selectedList.options[_inDex].selected = true;
        }

    } catch (e) { }
}

// up selected item
function MoveSelectedField(style) {
    try {
        var _selectedList = document.getElementById($('[id$=lstFieldInclude]').attr('id'));

        var _inDex = _selectedList.options.selectedIndex;
        if (_inDex < 0) return;
        if (style == 'up') {
            if (_inDex <= 0) return;
            var _tempText, _tempValue;
            // for text
            _tempText = _selectedList.options[_inDex - 1].text;
            _selectedList.options[_inDex - 1].text = _selectedList.options[_inDex].text;
            _selectedList.options[_inDex].text = _tempText;
            // for value
            _tempValue = _selectedList.options[_inDex - 1].value;
            _selectedList.options[_inDex - 1].value = _selectedList.options[_inDex].value;
            _selectedList.options[_inDex].value = _tempValue;

            _selectedList.options[_inDex - 1].selected = true;
        }
        else if (style == 'down') {
            if (_inDex >= _selectedList.options.length) return;
            var _tempText, _tempValue;
            // for text
            _tempText = _selectedList.options[_inDex + 1].text;
            _selectedList.options[_inDex + 1].text = _selectedList.options[_inDex].text;
            _selectedList.options[_inDex].text = _tempText;
            // for value
            _tempValue = _selectedList.options[_inDex + 1].value;
            _selectedList.options[_inDex + 1].value = _selectedList.options[_inDex].value;
            _selectedList.options[_inDex].value = _tempValue;

            _selectedList.options[_inDex + 1].selected = true;
        }

    } catch (e) { }
}
// move item to Top or Bottom
function MoveTopOrBottom(style) {
    try {
        var _selectedList = document.getElementById($('[id$=lstFieldInclude]').attr('id'));

        var _inDex = _selectedList.options.selectedIndex;
        if (_inDex < 0) return;

        if (style == 'top') {
            if (_inDex <= 0) return;
            var _tempText, _tempValue;

            _tempText = _selectedList.options[_inDex].text;
            _tempValue = _selectedList.options[_inDex].value;

            _selectedList.remove(_inDex);
            var _opt = document.createElement('OPTION');
            _opt.text = _tempText;
            _opt.value = _tempValue;
            _selectedList.options.add(_opt, 0);
            _selectedList.options[0].selected = true;
        }
        else if (style == 'bottom') {
            var n = _selectedList.options.length;
            if (_inDex >= n) return;
            var _tempText, _tempValue;

            _tempText = _selectedList.options[_inDex].text;
            _tempValue = _selectedList.options[_inDex].value;

            _selectedList.remove(_inDex);

            var _opt = document.createElement('OPTION');
            _opt.text = _tempText;
            _opt.value = _tempValue;
            _selectedList.options.add(_opt, n);

            if (_selectedList.options.length > 0)
                _selectedList.options[_selectedList.options.length - 1].selected = true;
        }
    } catch (e) { }
}

// lay nhung field duoc chon tu list setlected
function SelectedFields() {
    try {
        var _str = "";
        var _selectedList = document.getElementById($('[id$=lstFieldInclude]').attr('id'));

        if (_selectedList.options.length < 1) return;

        _str = "[";
        for (var i = 0; i < _selectedList.options.length; i++) {
            _str += "{\"DisplayName\":\"" + _selectedList.options[i].text + "\",\"Name\":\"" + _selectedList.options[i].value + "\"},";
        }
        if (_str[_str.length - 1] == ',')
            _str = _str.substring(0, _str.length - 1);

        _str += "]";
        // de danh su dung tren server

        $("[id$=hdfColumnsDisplay]").val(_str); //JSON.stringify(_selectedList)
        closePopupPanel();

        execExcelReport(_str);
    } catch (e) { }
}