function fDragging(obj, e, limit, frame)
{ 
    if(!e) e=window.event; 
    obj=document.getElementById(obj);
    frame=document.getElementById(frame);
    var x=parseInt(obj.style.left); 
    var y=parseInt(obj.style.top); 
     
    var x_=e.clientX-x; 
    var y_=e.clientY-y; 
     
    if(document.addEventListener){ 
        document.addEventListener("mousemove", inFmove, true); 
        document.addEventListener("mouseup", inFup, true); 
    } else if(document.attachEvent){ 
        document.attachEvent("onmousemove", inFmove); 
        document.attachEvent("onmouseup", inFup); 
    } 
     
    inFstop(e);     
    inFabort(e) 
     
    function inFmove(e){ 
        var evt; 
        if(!e)e=window.event; 
         
        if(limit){ 
            var scrollLeft = parseInt(document.body.scrollLeft);
            var left = parseInt(document.body.clientWidth) + scrollLeft - 5;
            
            if((e.clientX-x_)<0) return false; 
            else if((e.clientX - x_ + parseInt(obj.offsetWidth) + scrollLeft ) > left) return false; 
             
            var scrollTop = parseInt(document.body.scrollTop);
            var Top = parseInt(document.body.clientHeight) + scrollTop;
            if(e.clientY-y_<0) return false; 
            else if((e.clientY - y_ + parseInt(obj.offsetHeight) + scrollTop) > Top) return false;

        } 
         
        obj.style.left=e.clientX-x_+"px"; 
        obj.style.top=e.clientY-y_+"px";
        if(frame != null) 
            Setframe();             
        inFstop(e); 
        return false;
    }
    function inFup(e){ 
        var evt; 
        if(!e)e=window.event; 
         
        if(document.removeEventListener){ 
            document.removeEventListener("mousemove", inFmove, true); 
            document.removeEventListener("mouseup", inFup, true); 
        } else if(document.detachEvent){ 
            document.detachEvent("onmousemove", inFmove); 
            document.detachEvent("onmouseup", inFup); 
        } 
         
        inFstop(e); 
        IsPopupMoved = true;
        return false;
    }

    function inFstop(e){ 
        if(e.stopPropagation) return e.stopPropagation(); 
        else return e.cancelBubble=true;             
    }
    function inFabort(e){ 
        if(e.preventDefault) return e.preventDefault(); 
        else return e.returnValue=false; 
    } 
    function Setframe()
    {	        	       
        obj.style.display = "block";
        frame.style.width = obj.offsetWidth-2;	      
        frame.style.height = obj.offsetHeight-2;
        frame.style.top = 0;
        frame.style.left = 0;
        frame.style.zIndex = obj.style.zIndex - 1; 
        frame.style.display = "block";
        frame.style.position = "absolute";
    }
} 


//Ham tim toa do cua 1 control
//Tra ve kieu {x,y}

function findPos(obj)
{
    var curTop = 0;
    var curLeft = 0;
    var tempObj = obj;
    
    if (document.getElementById || document.all) 
    {
        while (tempObj.offsetParent) 
        {
            curTop += tempObj.offsetTop;
            curLeft += tempObj.offsetLeft
            if (typeof(tempObj.scrollTop) == 'number')
                curTop -= tempObj.scrollTop;
            tempObj = tempObj.offsetParent;
        }
    }
    else if (document.layers)
    {
        curTop += tempObj.y;
        curLeft += tempObj.x;
    }
    return {"left":curLeft,"top":curTop};
}

function pResizePanel(panel,e,pContent,_minWidth,_minHeight)
{  
    if(!e) e=window.event;

    _panel = document.getElementById(panel);
    _pContent = document.getElementById(pContent);
           
    _pTop = parseInt(_panel.style.top); 
    _pLeft = parseInt(_panel.style.left);
    
    _pctTop = _pTop + 30;
    _pctLeft = _pLeft + 20;
               
    var scrollP = {x:0,y:0};
    scrollP.y = document.documentElement.scrollTop;
    scrollP.x = document.documentElement.scrollLeft;
        
    if (document.addEventListener)
    {
        document.addEventListener("mousemove",inFsize,true);
        document.addEventListener("mouseup",inFup,true);
    }
    else if (document.attachEvent) 
    {
        document.attachEvent("onmousemove",inFsize);
        document.attachEvent("onmouseup",inFup);
    }
         
    inFstop(e);
    inFabort(e);
    return false;
          
    function inFsize(e)
    {
        if(!e) e = window.event;
        
        if ((e.clientX + scrollP.x) - parseInt(_pLeft) < _minWidth ) return false;  
        if ((e.clientY + scrollP.y) - parseInt(_pTop) < _minHeight ) return false;
        
        try
        {
            _pContent.style.width = e.clientX + scrollP.x - _pctLeft - 20 + "px";
            _pContent.style.height = e.clientY + scrollP.y - _pctTop - 80 + "px";
            
            _panel.style.width = e.clientX + scrollP.x - _pLeft + "px";
            _panel.style.height = e.clientY + scrollP.y - _pTop + "px";            
            
        }catch(e){}
                  
        inFstop(e);
        inFabort(e);
        return false;
    }
    function inFup(e)
    { 
        if(!e) e= window.event;
        if(document.removeEventListener)
        {
            document.removeEventListener("mousemove",inFsize,true);
            document.removeEventListener("mouseup",inFup,true);
        }
        else if (document.detachEvent) 
        {
            document.detachEvent("onmousemove",inFsize);
            document.detachEvent("onmouseup",inFup);
        }
        

        
        inFstop(e);
        inFabort(e);
        return false;
    }
    function inFstop(e)
    {
        if(!e) e=window.event; 
            if(e.stopPropagation)
                return e.stopPropagation();
        else
            return e.cancelBubble = true;
    }
    function inFabort(e)
    { 
        if(e.preventDefault) return e.preventDefault(); 
        else return e.returnValue=false; 
    }    
}