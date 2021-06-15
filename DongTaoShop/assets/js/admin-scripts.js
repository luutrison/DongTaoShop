//onchange dac san
function onchangeds(a) {
    var km = $("#col-dacsan");
    if (a.selectedIndex == 0) {
        km.hide();
    } else {
        km.show();
    }
}
// click item quick-action
function clickQuickAction(b) {
    var a = $(".col-item-quick-ad-detail");
    if (a.height() == 0) {
        a.toggleClass("slideUp");
        a.css({
            height: 'auto',
            display: 'block',
        })
        b.style.transform = "rotate(70deg)";
        a.toggleClass("slideDown");
    } else {
        a.toggleClass("slideDown");
        setTimeout(function(){
            a.css({
                height: '0px',
                display: 'none',
            })
        },400)
        b.style.transform = "rotate(0deg)";
        a.toggleClass("slideUp");
    }

}