// number count for stats, using jQuery animate

$('.counting').each(function () {
    var $this = $(this),
        countTo = $this.attr('data-count');

    $({
        countNum: $this.text()
    }).animate({
        countNum: countTo
    }, {
        duration: 3000,
        easing: 'linear',
        step: function () {
            $this.text(Math.floor(this.countNum));
        },
        complete: function () {
            $this.text(this.countNum);
        }
    });
});

// check box shop
function checkBoxShop(n) {

    var main = n.closest('.col-checkbox-shop-pc');
    var arr = document.getElementsByClassName('col-checkbox-shop-pc');
    n.value = main.querySelector(".span-name-cb-shop").innerHTML;
    n.checked = true;

    for (let i = 0; i < arr.length; i++) {
        const element = arr[i];
        var name = element.querySelector('.span-name-cb-shop').innerHTML;
        if (name != n.value) {
            element.querySelector('.checkbox-shop-pc').checked = false;
            element.querySelector('.checkbox-shop-pc').value = "";
        }
    }
}

//Check visible
function checkVisible(elm, eval) {
    eval = eval || "visible";
    var vpH = $(window).height(), // Viewport Height
        st = $(window).scrollTop(), // Scroll Top
        y = $(elm).offset().top,
        elementHeight = $(elm).height();

    if (eval == "visible") return ((y < (vpH + st)) && (y > (st - elementHeight)));
    if (eval == "above") return ((y < (vpH + st)));
}


//click icon cart
function clickShowCartViewFirt() {
    var a = $('.cart-view-firt');
    if (a.height() == 0) {
        cartHoverOn();
    } else {
        cartHoverOut();
    }
}


//cartOut
function cartHoverOut() {
    var a = $('.cart-view-firt');
    var b = $('.check-out-hovercart');
    a.css({
        'height': '0px',
    })
    b.css({
        'height': '0px',
    })

}
//cartHover
function cartHoverOn() {
    var a = $('.cart-view-firt');
    var b = $('.check-out-hovercart');
    a.css({
        'height': '100vh',
    })
    b.css({
        'height': '100vh',
    })

}




//click plus cart
function clickPlusCartFirt(n) {
    var a = n.closest('.number-cart-firt').querySelector(".num-card");
    var b = n.closest('.number-cart-firt').closest('.item-cart-firt');

    var price = b.querySelector(".price-card-firt");

    var set = n.closest('.item-cart-firt').querySelector(".price-cart-set").innerHTML;

    a.value = parseInt(a.value) + 1;
    price.innerHTML = parseInt(a.value * parseInt(set)).toLocaleString();
    totalCartPrice();
    updateStorageCart();
}


//update price on cart
function updatePriceCartFirt(n) {
    var a = n.closest('.number-cart-firt').querySelector(".num-card");
    var b = n.closest('.number-cart-firt').closest('.item-cart-firt');

    var price = b.querySelector(".price-card-firt");

    var set = n.closest('.item-cart-firt').querySelector(".price-cart-set").innerHTML;

    price.innerHTML = parseInt(a.value * parseInt(set)).toLocaleString();

    totalCartPrice();
    updateStorageCart();

}
$(document).ready(function () {
    //dot in price
    var a = document.getElementsByClassName("price");
    var b = document.getElementsByClassName("price-delete");
    for (var i = 0; i < a.length; i++) {
        var number = parseInt(a[i].innerHTML);
        a[i].innerHTML = number.toLocaleString();
    }
    for (var i = 0; i < b.length; i++) {
        var number = parseInt(b[i].innerHTML);
        b[i].innerHTML = number.toLocaleString();
    }

    //click add cart view

    $('.btn-add-cart').click(function () {
        var result = localStorage.getItem("itemCart");
        if (result != null) {
            var obj = JSON.parse(result);
            var nameStorage = obj.name;
            var priceStorage = obj.price;
            var imageStorage = obj.image;
            var numberStorage = obj.number;
        } else {
            var nameStorage = [];
            var priceStorage = [];
            var imageStorage = [];
            var numberStorage = [];
        }

        var main = this.closest('.item-product-parent');
        var image = main.querySelector('.image-product').style.backgroundImage.slice(4, -1).replace(/["']/g, "");

        var name = main.querySelector('.name-item-product').innerHTML;
        var price = main.querySelector('.price').innerHTML
        if (price.indexOf(',') > 0) {
            price = price.split(',').join("");
        } else {
            price = price.split('.').join("");
        }

        nameStorage.push(name);
        priceStorage.push(price);
        imageStorage.push(image);
        numberStorage.push(1);

        var itemStorage = {
            "name": nameStorage,
            "price": priceStorage,
            "image": imageStorage,
            "number": numberStorage
        };

        localStorage.setItem("itemCart", JSON.stringify(itemStorage))

        loadCart();

    })

    //click add cart detail

    $('#btn-add-cart-detail').click(function () {
        var result = localStorage.getItem("itemCart");
        if (result != null) {
            var obj = JSON.parse(result);
            var nameStorage = obj.name;
            var priceStorage = obj.price;
            var imageStorage = obj.image;
            var numberStorage = obj.number;
        } else {
            var nameStorage = [];
            var priceStorage = [];
            var imageStorage = [];
            var numberStorage = [];
        }

        var main = this.closest('.row-content-detail-product');
        var name = main.querySelector('.tensanpham-detail').innerHTML;
        var price = main.querySelector('.price').innerHTML.split('.').join("");
        var image = main.querySelectorAll('.image-product-detail')[0].style.backgroundImage.slice(4, -1).replace(/["']/g, "");

        if (price.indexOf(',') > 0) {
            price = price.split(',').join("");
        } else {
            price = price.split('.').join("");
        }

        nameStorage.push(name);
        priceStorage.push(price);
        imageStorage.push(image);
        numberStorage.push(1);

        var itemStorage = {
            "name": nameStorage,
            "price": priceStorage,
            "image": imageStorage,
            "number": numberStorage
        };

        localStorage.setItem("itemCart", JSON.stringify(itemStorage))

        loadCart();

    })

    loadCart();


});

//press enter cart input
function enterCartInput(a) {
    var x = a.which || a.keyCode;
    if (x == 13) {
        a.preventDefault();
    }
}
// onkeydown cart input

function onKeyDownInput(a) {
    var val = a.value;
    if (val == "") {
        a.value = 1;
    }
    updatePriceCartFirt(a);
}


//* load current cart
function loadCart() {

    var result = localStorage.getItem("itemCart");
    if (result != null) {
        var obj = JSON.parse(result);
        var name = obj.name;
        var price = obj.price;
        var image = obj.image;
        var num = obj.number;
    } else {
        var name = [];
        var price = [];
        var image = [];
        var num = [];
    }

    const menu = document.getElementById("main-content-cart");
    const mainItem = document.getElementsByClassName("item-cart-firt");
    menu.innerHTML = '';
    if (name.length == 1) {
        mainItem[0].querySelector('.name-item-cart').value = name[0];
        mainItem[0].querySelector('.num-card').value = num[0];
        mainItem[0].querySelector('.price-card-firt').innerHTML = (price[0] * num[0]).toLocaleString();
        mainItem[0].querySelector('.price-cart-set').innerHTML = price[0];
        mainItem[0].querySelector('.image-item-cart').style.backgroundImage = "url('" + image[0] + "')";

    }
    for (var i = 0; i < name.length; i++) {
        var sp = $("#clone-item-cart").clone().attr('id', 'clone-item-cart-' + i).appendTo($('#main-content-cart'));
        sp.css({
            'height': 'auto',
            'width': 'auto',
        });

        if (i == 0) {
            mainItem[i].querySelector('.name-item-cart').value = name[0];
        }
        mainItem[i].querySelector('.name-item-cart').value = name[i];
        mainItem[i].querySelector('.num-card').value = num[i];
        mainItem[i].querySelector('.price-card-firt').innerHTML = (price[i] * num[i]).toLocaleString();
        mainItem[i].querySelector('.price-cart-set').innerHTML = price[i];
        mainItem[i].querySelector('.image-item-cart').style.backgroundImage = "url('" + image[i] + "')";
    }
    totalCartPrice();
}

// minus click item
function clickMinusCartFirt(n) {
    var a = n.closest('.number-cart-firt').querySelector(".num-card");
    var b = n.closest('.number-cart-firt').closest('.item-cart-firt');

    var price = b.querySelector(".price-card-firt");

    var set = n.closest('.item-cart-firt').querySelector(".price-cart-set").innerHTML;
    if (parseInt(a.value) > 1) {
        a.value = parseInt(a.value) - 1;
        price.innerHTML = parseInt(a.value * parseInt(set)).toLocaleString();
    }
    totalCartPrice();
    updateStorageCart();
}
// cal total
function totalCartPrice() {

    var a = 0;
    var text = document.getElementById('total-prices');
    var b = document.getElementsByClassName('price-card-firt');
    var hide = document.getElementById("clone-item-cart").querySelector('.price-card-firt').innerHTML

    if (hide.indexOf(',') > 0) {
        hide = hide.split(',').join("");
    } else {
        hide = hide.split('.').join("");
    }

    for (var i = 0; i < b.length; i++) {
        if (b[i].innerHTML.indexOf(',') > 0) {
            a = parseInt((b[i].innerHTML).split(',').join("")) + a;
        } else {
            a = parseInt((b[i].innerHTML).split('.').join("")) + a;
        }
    }

    text.innerHTML = parseInt(a - parseInt(hide)).toLocaleString();
    countItemCartBuy();

}

function countItemCartBuy() {
    var countNumber = $('.cart-count-item');
    var number = document.getElementById("main-content-cart").childElementCount;

    var imgCartEmpty = document.getElementById("none-cart-image");
    var footerCart = document.getElementById("footer-cart");


    countNumber.html(number);


    if (parseInt(countNumber.html()) <= 0) {
        footerCart.style.height = 0;
        footerCart.style.opacity = 0;

        imgCartEmpty.classList.remove("none");
        imgCartEmpty.classList.add("flex");

    } else {
        imgCartEmpty.classList.remove("flex");
        imgCartEmpty.classList.add("none");

        footerCart.style.height = 'auto';
        footerCart.style.opacity = 1;
    }
}
//delete card

function deleteItemCard(n) {
    var item = n.closest('.item-cart-firt');

    item.remove();
    totalCartPrice();

    updateStorageCart();

}

function updateStorageCart() {
    const menu = document.getElementById("main-content-cart");

    var name = menu.querySelectorAll('.name-item-cart'),
        number = menu.querySelectorAll('.num-card'),
        price = menu.querySelectorAll('.price-cart-set'),
        image = menu.querySelectorAll('.image-item-cart');

    var nameStorage = [],
        numberStorage = [],
        priceStorage = [],
        imageStorage = [];

    for (var i = name.length; i--; nameStorage.unshift(name[i].value), numberStorage.unshift(number[i].value), priceStorage.unshift(price[i].innerHTML), imageStorage.unshift(image[i].style.backgroundImage.slice(4, -1).replace(/["']/g, "")));

    var itemStorage = {
        "name": nameStorage,
        "price": priceStorage,
        "image": imageStorage,
        "number": numberStorage
    };

    localStorage.setItem("itemCart", JSON.stringify(itemStorage))


}


//loading loader

$(window).on('load', function () {
    var a = document.getElementById('loading-holder').style;
    a.height = "0px";
    deleteLinkSomee();
});


// Clone notification
function createNotification(p) {
    var holder = document.getElementById("holder-notification");
    var goodAlert = document.getElementById("good-alert");
    var badAlert = document.getElementById("bad-alert");

    if (p.search("Bad") == -1) {
        setTimeout(function () {
            var rep = p.replace("Good ", "");
            goodAlert.querySelector("span").innerHTML = rep;
            var node = goodAlert.cloneNode(true);
            holder.appendChild(node);
            var alert = node.querySelector(".alert-thongbao").style;
            var onut = node.style;
            onut.opacity = 1;
            alert.width = "100%";
            setTimeout(function () {
                onut.opacity = 0;
                alert.width = "0%";
                onut.whiteSpace = "nowrap";
            }, 5000);
            setTimeout(function () {
                node.remove();
            }, 6000);

        }, 100)

    } else {
        setTimeout(function () {
            var rep = p.replace("Bad ", "");
            badAlert.querySelector("span").innerHTML = rep;
            var node = badAlert.cloneNode(true);
            holder.appendChild(node);
            var alert = node.querySelector(".alert-thongbao").style;
            var onut = node.style;
            onut.opacity = 1;
            alert.width = "100%";
            setTimeout(function () {
                onut.opacity = 0;
                onut.whiteSpace = "nowrap";
                alert.width = "0%";
            }, 5000);
            setTimeout(function () {
                node.remove();
            }, 6000);
        }, 100)

    }
}

// btn nav account
function showNavAccount() {
    var nav = $(".nav-account-mob");
    if (nav.width() == 0) {
        $("#back-den").css('height', '100vh');
        nav.css('width', '100vh');
    } else {
        $("#back-den").css('height', '0px');
        nav.css('width', '0px');
    }
}

// avatar change account then convert to base 64
function getImageInput(input) {
    $('.loader-image-avatar').show();
    if (input.files && input.files[0]) {
        var reader = new FileReader();
        reader.onload = function (e) {
            $('#imgAvatar').css('background-image', 'url(' + e.target.result + ')');
            $('.loader-image-avatar').hide();
        }
        reader.readAsDataURL(input.files[0]); // convert to base64 string

    }
}

//canvas image to base64

function getBase64Image(img) {
    var canvas = document.createElement("canvas");
    canvas.width = img.width;
    canvas.height = img.height;
    var ctx = canvas.getContext("2d");
    ctx.drawImage(img, 0, 0, img.width, img.height);
    var dataURL = canvas.toDataURL("image/png");
    return dataURL;
}

//Recommened List

function nextSameProduct(a) {
    var main = a.closest('.sameProduct');
    main.scrollLeft += $(".sameProduct").width();
}

function prevSameProduct(a) {
    var main = a.closest('.sameProduct');
    main.scrollLeft -= $(".sameProduct").width();
}

// Image List Product


currentImageDetail(document.getElementById("mainImage").style.backgroundImage.slice(4, -1).replace(/["']/g, ""));

function nextDetailImage() {
    document.getElementById('moreImage').scrollLeft += $("#moreImage").width();
}

function prevDetailProduct() {
    document.getElementById('moreImage').scrollLeft -= $("#moreImage").width();
}

function setImageDetail(a) {
    var img = a.style.backgroundImage.slice(4, -1).replace(/["']/g, "");
    $("#mainImage").css({
        background: 'url("' + img + '") center / contain no-repeat'
    });
    currentImageDetail(img);
}

function currentImageDetail(img) {
    var c = document.querySelectorAll(".imageDetail");
    for (let i = 0; i < c.length; i++) {
        const element = c[i];
        var eimg = element.style.backgroundImage.slice(4, -1).replace(/["']/g, "");
        if (img == eimg) {
            element.style.border = '5px solid rgb(36,150,59)';
        } else {
            element.style.border = '1px solid rgb(85,85,85)';
        }
    }
}

function nextBigImageDetail() {
    var img = document.getElementById("mainImage").style.backgroundImage.slice(4, -1).replace(/["']/g, "");
    var all = document.querySelectorAll(".imageDetail");
    for (let i = 0; i < all.length; i++) {
        const element = all[i];
        var eimg = element.style.backgroundImage.slice(4, -1).replace(/["']/g, "");
        if (eimg == img) {
            okimg = all[i + 1].style.backgroundImage.slice(4, -1).replace(/["']/g, "");
            $("#mainImage").css({
                background: 'url("' + okimg + '") center / contain no-repeat'
            });
            currentImageDetail(okimg);
            break;
        }
    }
}

function prevBigImageDetail() {
    var img = document.getElementById("mainImage").style.backgroundImage.slice(4, -1).replace(/["']/g, "");
    var all = document.querySelectorAll(".imageDetail");
    for (let i = 0; i < all.length; i++) {
        const element = all[i];
        var eimg = element.style.backgroundImage.slice(4, -1).replace(/["']/g, "");
        if (eimg == img) {
            okimg = all[i - 1].style.backgroundImage.slice(4, -1).replace(/["']/g, "");
            $("#mainImage").css({
                background: 'url("' + okimg + '") center / contain no-repeat'
            });
            currentImageDetail(okimg);
            break;
        }
    }
}


function opensearchMob() {

    var a = $(".search-hidden-top");
    if (a.height() == 0) {
        a.css({
            height: '100vh',
        });
    } else {
        a.css({
            height: '0px',
        });
    }
}

//DisableSomee
function deleteLinkSomee() {
    var c = document.querySelectorAll("center a");
    for (let i = 0; i < c.length; i++) {
        const element = c[i];
        if (element.href.includes("somee")) {
            element.style.display = "none";
        }
    }
    setTimeout(function(){
        // This hides the address bar:
        window.scrollTo(0, 1);
    }, 0);
}