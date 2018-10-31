
/* SWITCHERY */
function init_Switch() {
    if ($(".js-switch")[0]) {
        var elems = Array.prototype.slice.call(document.querySelectorAll('.js-switch'));
        elems.forEach(function (html) {
            var switchery = new Switchery(html, {
                color: '#26B99A'
            });
        });
    }
    if ($(".js-switch-chk")[0]) {
        var elems = Array.prototype.slice.call(document.querySelectorAll('.js-switch-chk'));
        elems.forEach(function (html) {
			if (html.style.display != "none")
			{
				var switchery = new Switchery(html, {
					color: '#26B99A'
				});
			}
        });
    }
};
/* END SWITCHERY */

/* ICHECK */
function init_ICheck() {
    if ($("input.flat")[0]) {
        $(document).ready(function () {
            $('input.flat').iCheck({
                checkboxClass: 'icheckbox_flat-green',
                radioClass: 'iradio_flat-green'
            });
        });
    }
};
/* END ICHECK */

/* CKEDITOR */
function init_Ckeditor() {
    $('.ckeditor').ckeditor();
};
/* END CKEDITOR */

$(document).ready(function () {
    init_Switch();
    init_ICheck();
    init_Ckeditor();
});