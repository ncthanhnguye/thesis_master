
var conceptList = null;
var ConceptInfo = function () {
    this.Name = $('.clsEdit_Conceptname').val();
    this.Content = $('.clsEdit_ConceptContent').val();
    this.ID = $('.clsConceptSelected').attr('conceptID');
}

function ViewConcept() {
    $('.divlawConceptDetail').empty();
    ShowProcessing();

    PageMethods.ViewConcept(function (data) {
        HideProcessing();
        var shtml = GetHTML_ConceptContent(data);
        $('.clsConceptlist').html(shtml);
        if (lastConceptID != null)
            $('.clsConceptItem[conceptID=' + lastConceptID + ']').click();

    }, function () { HideProcessing(); });

}
var lastConceptID = null;
function ChangeConcept(type) {
    var sformatName = GetRowDataHTML(['<b>Concept: </b>', '<input class="clsEdit_Conceptname" type="textbox"  value="" />'], 'class="clsTDConcept"');
    var sformatContent = GetRowDataHTML(['<b>Description: </b>', '<textarea class="clsEdit_ConceptContent" type="textbox"  value="{2}" />'], 'class="clsTDConcept"');
    var sError = GetRowDataHTML(['<div style="text-align:left; color: red" class ="clsTableConcept_Error"></div>'], 'colspan="2"');
    if (type != 1 && $('.clsConceptSelected').length == 0) {
        ShowPopup("Law Search - Warning", 'Please select Concept first.', null);
        return;
    }

    var opt = [
        {
            name:type==3?'Confirm': 'OK', fClick: function () {               
                ShowProcessing();
                concept = new ConceptInfo();
                concept.type = type;
                if (type != 3 && (concept.Name == '' || concept.Content == '')) {
                    $('.clsTableConcept_Error').text('Name or Content cannot be empty.');
                    return;
                }
             
                lastConceptID = type == 3 ? null : concept.ID;
                //clsAddnewtype_Error
                PageMethods.ChangeConcept(concept, function (data) {
                    HideProcessing(); ClosePopup();
                    ViewConcept();
                }, function () { HideProcessing(); });
            }
        }, {
            name: "Cancel", fClick: function () { ClosePopup(); }
        }
    ];
    ShowPopup("Law Search - Concept", type == 3 ? 'Are you sure you want to delete <b>"' + GetConceptName() + '"</b>' : (sError + sformatName + sformatContent), opt);
    // set value
    if (type != 1) {
        $('.clsEdit_Conceptname').val(GetConceptName());
        $('.clsEdit_ConceptContent').val($('.divlawConceptDetail').text());
    }
}
function GetConceptName() {
    return $('.clsConceptSelected').text().substr($('.clsConceptSelected').text().indexOf('.') + 2);
}
function ViewConceptContent(cID, o) {
    $('.clsConceptSelected').removeClass('clsConceptSelected');
    $(o).addClass('clsConceptSelected');
    var content = '';
    for (var i = 0; conceptList!=null&& i < conceptList.length; i++) {
        if (conceptList[i].ID == cID) {
            content = conceptList[i].Content;
            break;
        }
    }
    lastConceptID = cID;
    $('.divlawConceptDetail').html(content);
}
function GetHTML_ConceptContent(data) {
    if (data == null) return "";
    conceptList = data;
    var shtml = '';
    for (var i = 0; i < data.length; i++) {
        shtml += '<div class="clsConceptItem"  onclick= "ViewConceptContent(' + data[i].ID + ', this);" conceptID ="' + data[i].ID +'" >' + (i + 1).toString() + '. ' + data[i].Name + '</div>';
    }
    return shtml;
}