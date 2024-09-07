// JScript File

try
{
    $(document).ready(function () {
        try {
       
        } catch (e) { }

        try {
            $('[id$=btnLogout]').hide();
        } catch (e) { }

        try {
            $('[id$=loginbox]').keypress(function (event) {
                try {
                    if (!event)
                        event = window.event;

                    if (event.keyCode == 13 || event.which == 13) {
                        event.preventDefault();

                        $('[id$=btnLogin]').click();
                    }
                } catch (e) { }
            });
                      
        } catch (e) { }

    });
}
catch(e){}

function CheckSpecialChars(strText)
{
    var DELIMS = new Array("'", "\"", "--", "select", "insert", "update", "delete", "drop");
    for (var i=0; i< DELIMS.length; i++)
    {
        if (DELIMS[i] == strText.toLowerCase())
            return true;
    }
    return false;
}

function submitForm() {

    try {
        $('INPUT[id$=hdfTimeZone]').val(getTimeZone());
        $('INPUT[id$=hdfDayLightSaving]').val(CheckDayLightSaving());
    } catch (e) { }

    var username = $('INPUT[id$=txtUserName]').val();
    var pass = $('INPUT[id$=txtPassword]').val();
            						       						   			    
    if (username == '') 
    {
        ShowMessageBox('Please input Username.', 'Law Search - Login', null);
        return false;
    }
    else if (CheckSpecialChars(username)) 
    {
        ShowMessageBox('Invalid Username!', 'Law Search - Login', null);
        return false;
    }
    
    if (pass == '') 
    {
        ShowMessageBox('Please input Password.', 'Law Search - Login', null);
        return false;
    }
    else if (CheckSpecialChars(pass)) 
    {
        ShowMessageBox('Invalid password!', 'Law Search - Login', null);
        return false;
    }
           
    return true;			    		   		    			
}

function ShowMessageBox(mess,title,option)
{
    GCTWindow.jWindow_Wrap(mess, title, option);
}
