/*
 * - autoSmoothScroll -
 * Licence MIT
 * Written by Gabriel Delépine
 * Current version  1.3.1 (2014-10-22)
 * Previous version 1.3.0 (2014-07-23)
 * Previous version 1.2.0 (2014-02-13)
 * Previous version 1.0.1 (2013-11-08)
 * Previous version 1.0.0 (2013-10-27)
 * Requirement : No one, it is a framework-free fonction (ie : You do not need to include any other file in your page such as jQuery)
 * Fork-me in github : 
 * */

function $(item) {
	return (typeof item) == "string" ? document.getElementById(item) : item;
}

displayCache = {} 

function hide(el) {
	if (!el.hasAttribute('displayOld')) {
		el.setAttribute("displayOld", el.style.display)
	}

	el.style.display = "none"
}

function isHidden(el) {
	var width = el.offsetWidth, height = el.offsetHeight,
		tr = el.nodeName.toLowerCase() === "tr"

	return width === 0 && height === 0 && !tr ?
		true : width > 0 && height > 0 && !tr ? false : getRealDisplay(el)
}
function slideDown (element, duration, finalheight, callback) {
    var s = element.style;
    s.height = '0px';
    s.display = "block";

    var y = 0;
    var framerate = 60;
    var one_second = 1000;
    var interval = one_second*duration/framerate;
    var totalframes = one_second*duration/interval;
    var heightincrement = finalheight/totalframes;
    var tween = function () {
        y += heightincrement;
        s.height = y+'px';
        if (y<finalheight) {
            setTimeout(tween,interval);
        }
    }

    tween();
}

window.onload = function () {
	var donateButton = $("donate");
	var donationForm = $("donation-form");
	var height = donationForm.getAttribute("height");
	donateButton.addEventListener('click', function (e) {
		hide(donateButton);
		slideDown(donationForm, 0.5, height);
	});


	(function(window, undefined) // Code in a function to create an isolate scope
	{
	    'use strict';
	    var height_fixed_header = 0, // For layout with header with position:fixed. Write here the height of your header for your anchor don't be hiden behind
	        speed = 500,
	        moving_frequency = 15, // Affects performance ! High number makes scroll more smooth
	        links = document.getElementsByTagName('a'),
	        href;
	    
	    for(var i=0; i<links.length; i++)
	    {
	        href = (links[i].attributes.href === undefined) ? null : links[i].attributes.href.nodeValue.toString();
	        if(href !== null && href.length > 1 && href.indexOf('#') != -1) // href.substr(0, 1) == '#'
	        {
	            links[i].onclick = function()
	            {
	                var element,
	                    href = this.attributes.href.nodeValue.toString(),
	                    url = href.substr(0, href.indexOf('#')),
	                    id = href.substr(href.indexOf('#')+1);
	                if(element = document.getElementById(id))
	                {
	                    var hop_count = (speed - (speed % moving_frequency)) / moving_frequency, // Always make an integer
	                        getScrollTopDocumentAtBegin = getScrollTopDocument(),
	                        gap = (getScrollTopElement(element) - getScrollTopDocumentAtBegin) / hop_count;
	                    
	                    if(window.history && typeof window.history.pushState == 'function')
	                    	try {
	                    		window.history.pushState({}, undefined, url+'#'+id);
	                    	} catch (e) {}
	                    
	                    for(var i = 1; i <= hop_count; i++)
	                    {
	                        (function()
	                        {
	                            var hop_top_position = gap*i;
	                            setTimeout(function(){  window.scrollTo(0, hop_top_position + getScrollTopDocumentAtBegin); }, moving_frequency*i);
	                        })();
	                    }
	                    
	                    return false;
	                }
	            };
	        }
	    }
	    
	    var getScrollTopElement =  function(e)
	    {
	        var top = height_fixed_header * -1;

	        while (e.offsetParent != undefined && e.offsetParent != null)
	        {
	            top += e.offsetTop + (e.clientTop != null ? e.clientTop : 0);
	            e = e.offsetParent;
	        }
	        
	        return top;
	    };
	    
	    var getScrollTopDocument = function()
	    {
	        return window.pageYOffset !== undefined ? window.pageYOffset : document.documentElement.scrollTop !== undefined ? document.documentElement.scrollTop : document.body.scrollTop;
	    };
	})(window);
};