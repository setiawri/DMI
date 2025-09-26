
<link rel="shortcut icon" href="#"> <%--remove favicon.ico error--%>
<script src="assets/js/main/jquery.min.js"></script>
<script src="assets/js/main/bootstrap.bundle.min.js"></script> <%--@*fix error constructor*@--%>

<!-- Global stylesheets -->
<link href="assets/font/Roboto.css" rel="stylesheet" />
<link href="assets/css/icons/icomoon/styles.css" rel="stylesheet" type="text/css">
<link href="assets/css/icons/fontawesome/styles.min.css" rel="stylesheet" />
<link href="assets/css/bootstrap.min.css" rel="stylesheet" type="text/css">
<link href="assets/css/bootstrap_limitless.min.css" rel="stylesheet" type="text/css">
<link href="assets/css/layout.min.css" rel="stylesheet" type="text/css">
<link href="assets/css/components.min.css" rel="stylesheet" type="text/css">
<link href="assets/css/colors.min.css" rel="stylesheet" type="text/css">
<link href="assets/css/bootstrap-datetimepicker.css" rel="stylesheet" />
<link href="assets/css/animate.min.css" rel="stylesheet" />
<!-- /global stylesheets -->
<!-- Plugin -->
<script src="assets/js/popper.min.js"></script>
<script src="assets/js/plugins/charts/chart.js"></script>

<!-- /plugin -->
<!-- Core JS files -->
<script src="assets/js/plugins/loaders/blockui.min.js"></script>
<!-- /core JS files -->
<!-- Theme JS files -->
<script src="assets/js/plugins/ui/moment/moment.min.js"></script>
<script src="assets/js/plugins/pickers/anytime.min.js"></script>
<script src="assets/js/plugins/pickers/daterangepicker.js"></script>
<script src="assets/js/plugins/pickers/pickadate/picker.js"></script>
<script src="assets/js/plugins/pickers/pickadate/picker.date.js"></script>
<script src="assets/js/plugins/pickers/pickadate/picker.time.js"></script>
<script src="assets/js/plugins/forms/selects/select2.min.js"></script>
<script src="assets/js/plugins/forms/styling/uniform.min.js"></script>
<script src="assets/js/plugins/forms/styling/switchery.min.js"></script>
<script src="assets/js/plugins/forms/styling/switch.min.js"></script>
<script src="assets/js/plugins/tables/datatables/datatables.min.js"></script>
<script src="assets/js/plugins/notifications/bootbox.min.js"></script>
<script src="assets/js/plugins/forms/number/jquery.number.min.js"></script>

<script src="assets/js/plugins/multiselectdropdown/bootstrap-select.js"></script>
<link href="assets/js/plugins/multiselectdropdown/bootstrap-select.min.css" rel="stylesheet" />

<script src="assets/js/app.js"></script>
<!-- /theme JS files -->
<!--Custom Style-->
<style>
    .required::before {
        content: "* ";
        font-weight: bold;
        color: red;
    }

    textarea.input-validation-error {
        border-color: red;
    }

    .tablecellwrap {
        white-space: pre-wrap;
        word-wrap: break-word;
    }

    /* The sticky class is added to the navbar with JS when it reaches its scroll position */
    .sticky {
        position: fixed;
        top: 0;
        width: 100%;
    }

    .scrollbutton {
        width: 35px;
        height: 35px;
        position: fixed;
        display: none;
        text-indent: -9999px;
        z-index: 999999;
    }

    .scrollup {
        bottom: 100px;
        right: 50px;
        background: url('/assets/img/arrowUp.png') no-repeat;
    }

    .scrolldown {
        bottom: 50px;
        right: 50px;
        background: url('/assets/img/arrowDown.png') no-repeat;
    }

    /* Add some top padding to the page content to prevent sudden quick movement (as the navigation bar gets a new position at the top of the page (position:fixed and top:0) */
    .sticky + .content {
        padding-top: 60px;
    }

    /* Display Image Full Width */
    .imgbox {
        display: grid;
        height: 100%;
    }
    .center-fit {
        max-width: 100%;
        max-height: 100vh;
        margin: auto;
    }


</style>
<!--/custom Style-->
