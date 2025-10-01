<!-- hidden value to reset datatable state save -->

<script type="text/javascript">

    // FILTERING ===================================================================================================================================================

    function setCheckboxFilter(id, value) {
        if (value != '') {
            if ($('#' + id).is(":hidden"))
                $('#' + id).val(value);
            else
                setCheckboxState(id, value);
        }
    }

    function setDaterangepickerFilter(id, value) {
        if ($('#' + id).length != 0 && !$('#' + id).is(":hidden") && value != '')
            setDateRangePicker(id, value);
    }

    function setDatePeriodFilter(idFilter, idMonth, idYear, value) {
        if ($('#' + idMonth) != null && $('#' + idYear) != null) {
            var date = new Date(value);
            setDDLSelectedValue(idMonth, (date.getMonth() + 1).toString());
            setDDLSelectedValue(idYear, date.getFullYear().toString());
            updateFILTER_DatePeriod(idFilter, idMonth, idYear);
        }
    }

    function updateFILTER_DatePeriod(idFilter,idMonth,idYear) {
        $('#' + idFilter).val(formatDate(new Date($('#' + idYear).val(), getDDLSelectedValue(idMonth) - 1, 1, 0, 0, 0, 0), 'yyyy/MM/dd'));

    }

    function updateFILTER_DatePeriod_Month(action, idFilter, idMonth, idYear) {
        var date = new Date($('#' + idYear).val(), getDDLSelectedValue(idMonth) - 1, 1, 0, 0, 0, 0);
        if (action == 'previous') {
            date = addMonths(date, -1);
        } else if (action == 'next') {
            date = addMonths(date, 1);
        }
        setDatePeriodFilter(idFilter, idMonth, idYear, date.toString());
    }

    function getCheckboxFilterValue(id) {
        if ($('#'+id).is(":hidden"))
            return $('#' + id).val();
        else
            return $('#' + id).prop('checked');
    }

    //function Action(action, controller, newwindow, returnurl) {
    //    if (newwindow == false)
    //        showLoadingSpinner();
    //    var url = '@Url.Action("__action", "__controller", new { FILTER_Keyword = "__data" })';
    //    url = url.replace("__data", getFilter());
    //    ProcessAction(url, action, controller, newwindow, returnurl)
    //}

    //function ActionWithId(id, action, controller, newwindow, returnurl) {
    //    if(newwindow == false)
    //        showLoadingSpinner();
    //    var url = '@Url.Action("__action", "__controller", new { id = "__data" })';
    //    url = url.replace("__data", id + "?FILTER_Keyword=" + getFilter());
    //    ProcessAction(url, action, controller, newwindow, returnurl)
    //}

    //function ProcessAction(url, action, controller, newwindow, returnurl) {
    //    url = url.replace("__action", action).replace("__controller", controller);
    //    if (returnurl != undefined)
    //        url += "&ReturnUrl=" + returnurl;
    //    if (newwindow == null)
    //        newwindow = "false";
    //    OpenWindow(url, newwindow);
    //}

    function appendFilter(value, filterName) {
        if (value != null && value.toString() != "") {
            if (filterName == "")
                return value;
            else
                return "&" + filterName + "=" + value;
        }
        return "";
    }

    function appendFilterIfNumber(value, filterName) {
        if (isNaN(value) == true)
            return ""
        else
            return appendFilter(value, filterName);
    }

    function getDatatableSearch() {
        if ((search == null || search == "") && $('input[aria-controls=DataTables_Table_0]').val() != undefined)
            return $('input[aria-controls=DataTables_Table_0]').val();
        else
            return "";
    }

    function setDatatableSearch(search) {
        if ($('input[aria-controls=DataTables_Table_0]').val() != undefined)
            $('input[aria-controls=DataTables_Table_0]').val(search).keyup();
    }

    // TEXTBOX =====================================================================================================================================================

    function getValueString(id) {
        return trim($('#' + id).val());
    }

    function trim(value) { return $.trim(value); }

    function getValueInt(id) {
        return parseInt(getDefaultIfEmpty($('#'+id).val(), 0));
    }

    function getValueFloat(id) {
        return parseFloat(convertFormattedStringToFloat(getDefaultIfEmpty(getValueString(id), 0)));
    }

    function setValue(id, value) { $('#' + id).val(value); }

    function setText(id, value) { $('#' + id).text(value); }

    function resetListbox(id) { $('#'+id).val('').trigger('change'); }

    function setToZeroIfEmpty(control) {
        $(control).val(getDefaultIfEmpty($(control).val(), 0));
        if ($(control).val() == 0)
            $(control).select();
    }

    function getDefaultIfEmpty(value, defaultValue) {
        if (value == '')
            return defaultValue;
        else
            return value;
    }

    function applyEventsToInputTextboxes() {
        //45 = minus sign
        //46 = dot sign
        //48-57 = 0-9

        $('.numericonly').bind('keypress', function (e) {
            return event.charCode == 45 || event.charCode == 46 || (event.charCode >= 48 && event.charCode <= 57);
        });

        $('.positivenumericonly').bind('keypress', function (e) {
            return event.charCode == 46 || (event.charCode >= 48 && event.charCode <= 57);
        }).bind('keyup', function (e) { setToZeroIfEmpty($(this)); });

        $('.integeronly').bind('keypress', function (e) {
            return event.charCode == 45 || (event.charCode >= 48 && event.charCode <= 57);
        });

        $('.positiveintegeronly').bind('keypress', function (e) {
            return (event.charCode >= 48 && event.charCode <= 57);
        }).bind('keyup', function (e) { setToZeroIfEmpty($(this)); });

        //doesn't work for input with type='number' because it cannot have comma
        //}).bind('keyup', function (e) { $(this).val(formatN0($(this).val())); });

        //allow editing text after initial select all on focus
        $('input').on('focus', function (e) {
            $(this)
                .one('mouseup', function () {
                    $(this).select();
                    return false;
                })
                .select();
        });
    }

    function selectFirstInput() {
        $('form:not(.filter) :input:not([readonly]):visible:enabled:first').select(); //select content of first visible and enabled control
    }

    //format string to N0
    function formatN0(value1, value2, value3) {
        return sum(value1, value2, value3).toLocaleString();
    }

    function sum(value1, value2, value3) {
        if (value1 == null || value1 == "")
            value1 = 0;

        if (value2 == null || value2 == "")
            value2 = 0;

        if (value3 == null || value3 == "")
            value3 = 0;

        if (!$.isNumeric(value1))
            value1 = convertFormattedStringToInt(value1);
        if (!$.isNumeric(value2))
            value1 = convertFormattedStringToInt(value2);
        if (!$.isNumeric(value3))
            value1 = convertFormattedStringToInt(value3);

        return parseInt(value1) + parseInt(value2) + parseInt(value3);
    }

    function convertFormattedStringToInt(value) {
        return parseInt(value.replace(/,/g, ''));
    }

    function convertFormattedStringToFloat(value) {
        return parseFloat(value.replace(/,/g, ''));
    }

    // BUTTONS =====================================================================================================================================================

    function performClickButton(id) {
        $('#'+id).trigger('click');
    }

    // DROPDOWNLIST ================================================================================================================================================

    function getMonthDropDownValue(start, change) {
        var startingMonth = parseInt(start);
        var changeMonth = parseInt(change);
        var newMonth = startingMonth + changeMonth;
        if (newMonth > 12) {
            newMonth -= 12;
        }
        else if (newMonth == 0) {
            newMonth += 12;
        }
        return newMonth;
    }

    //function MonthsDropdownChange(id, action) {
    //    if (action == '@EnumActions.Previous.ToString()') {
    //        DropdownChange(id, getMonthDropDownValue($('#'+id).val(), -1));
    //    } else {
    //        DropdownChange(id, getMonthDropDownValue($('#'+id).val(), 1));
    //    }
    //}

    function DropdownChange(id, value) {
        $('#'+id).val(value).trigger('change');
    }

    //for select2
    function setDDLSelectedValue(id, value) {
        $('#' + id).val(value).trigger('change');
    }

    function appendAndSetSelect2SelectedValue(id, text, value, emptyFirst) {
        if (emptyFirst == true)
            $('#' + id).empty();
        $('#' + id).append(new Option(text, value, false, false)).trigger('change');
    }

    function getDDLSelectedValue(id) {
        return $('#' + id + ' option:selected').val();
    }

    function getDDLSelectedText(id) {
        return $('#' + id + ' option:selected').text();
    }

    function getSelectedItem(ddlid, models) {
        var result = getSelectedItems(ddlid, models);
        if (result.length > 0)
            return result[0];
        else
            return null;
    }

    function getSelectedItems(ddlid, models) {
        var result = [];
        var selecteditems = $('#' + ddlid + ' option:selected').map(function () { return this.value; }).get();
        $.each(selecteditems, function (index, id) {
            $.each(models, function (index, item) {
                if (item.Id == id) {
                    result.push(item);
                    return false;
                }
            });
        });
        return result;
    }

    // NAVIGATION MENU =============================================================================================================================================

    //function activateNavigationMenu() {
    //    $('#Menu_@ViewBag.SelectedNavigationMenu').addClass('active');
    //}

    // BOOTBOX =====================================================================================================================================================

    //function showControllerMessage() { showMessage('@TempData["BootboxMessage"]'); }
    function showMessage(message, idToSelectOnEscape) {
        if (message !== '') {
            message = message.replaceAll("&lt;", "<").replaceAll("&gt;", ">");
            bootbox.dialog({ backdrop: true, message: message, onEscape: function () { $('#'+idToSelectOnEscape).select(); } }); ////.select() is not working
        }
    }

    var spinnerDialog;
    function showLoadingSpinner() { showSpinner('Loading...'); }
    function showSpinner(message) {
        spinnerDialog = bootbox.dialog({
            message: '<p><i class="fa fa-spin fa-spinner"></i> ' + message + '</p>',
            size: "small"
        }).find('.modal-content').css({
            'padding-top':'10px',
            'font-size': '20pt',
            'text-align': 'center',
            'margin-top': function () {
                var windowHeight = $(window).height();
                var modalHeight = $(this).height();
                var topMargin = (windowHeight - modalHeight) / 3;
                return topMargin + "px";
            }
        }).find('.bootbox-close-button').css({
            'display':'none'
        });
    }
    function closeSpinner() {
        if (spinnerDialog != null) {
            spinnerDialog.modal('hide'); //doesn't work
            //bootbox.hideAll(); //not completely tested. might close other dialogs as well
        }
    }

    // TIME PICKER =================================================================================================================================================

    $('.anytime-picker').AnyTime_picker({
        format: '%H:%i'
    });

    // CALENDAR ====================================================================================================================================================

    $('.daterange-picker').daterangepicker({ singleDatePicker: true, locale: { format: 'YYYY/MM/DD' } });

    $('.daterangepicker-multidate').daterangepicker({ singleDatePicker: false, locale: { format: 'YYYY/MM/DD' } }, function (start, end, label) {
        daterangepickerCallback(start, end, label);
    });

    $('.daterangepicker-withtime').daterangepicker({
        singleDatePicker: true,
        timePicker: true,
        timePicker24Hour: true,
        timePickerIncrement: 5,
        locale: {
            format: 'YYYY/MM/DD H:mm'
        }
    });

    function setDateRangePickerValue(id, value) {
        setDateRangePicker(id, value, value);
    }

    function setDateRangePickerWithStringDate(id, start, end) { setDateRangePicker(id, parseJSONDate(start), parseJSONDate(end)); }
    function setDateRangePicker(id, start, end) {
        if ($('#' + id).is(":hidden") == false) {
            $('#' + id).data('daterangepicker').setStartDate(start);
            if (end != null)
                $('#' + id).data('daterangepicker').setEndDate(end);
            else
                $('#' + id).data('daterangepicker').setEndDate(start);
        }
    }

    function setDateRangePickerStartDate(id, daterangepickerId) { $('#' + id).val($('#' + daterangepickerId).val().split(' - ')[0]); }
    function setDateRangePickerEndDate(id, daterangepickerId) { $('#' + id).val($('#' + daterangepickerId).val().split(' - ')[1]); }

    // DATE MANIPULATOR ============================================================================================================================================

    function parseJSONDate(value) {
        if (value == null)
            return null
        else
            return new Date(parseInt(value.replace("/Date(", "").replace(")/", ""), 10));
        //return new Date(parseInt((value).match(/\d+/)[0])); //returns incorrect date when json date value is negative. (negative when earlier than year 1970)
    }

    var monthNames = ["Jan", "Feb", "Mar", "Apr",
        "May", "Jun", "Jul", "Aug",
        "Sep", "Oct", "Nov", "Dec"];

    function formatDate(value, format) {
        var result = [];
        var date = "";
        var month = "";
        var year = "";

        var delimiter = getDateDelimiter(format);

        format = format.split(delimiter);
        for (i = 0; i < format.length; i++) {

            if (format[i] == 'dd') {
                result.push((('' + value.getDate()).length < 2 ? '0' : '') + value.getDate() + ' ');
            }

            if (format[i].includes('M')) {
                if (format[i] == 'MMM') {
                    month = monthNames[value.getMonth()];
                } else if (format[i] == 'MM') {
                    month = (('' + (value.getMonth() + 1)).length < 2 ? '0' : '') + (value.getMonth() + 1) + ' ';
                }
                result.push(month);
            }

            if (format[i] == 'yyyy')
                result.push(value.getFullYear());
            else if (format[i] == 'yy')
                result.push(value.getFullYear().toString().substr(-2));

            if (format[i] == 'HH')
                result.push(padLeft(value.getHours(), format[i].length, "0"));

            if (format[i] == 'mm')
                result.push(padLeft(value.getMinutes(), format[i].length, "0"));

            if (format[i] == 'ss')
                result.push(padLeft(value.getSeconds(), format[i].length, "0"));
        }

        var finalResult = result.filter(Boolean).join(delimiter);
        if (delimiter != ' ')
            finalResult = finalResult.replaceAll(' ', '');

        return finalResult;
    }

    function getDateDelimiter(value) {
        if (value.split(" ").length > 1)
            return " ";
        else if (value.split("-").length > 1)
            return "-";
        else if (value.split("/").length > 1)
            return "/";
        else if (value.split(":").length > 1)
            return ":";
    }

    function addMonths(date, value) {
        var d = date.getDate();
        date.setMonth(date.getMonth() + +value);
        if (date.getDate() != d) {
            date.setDate(0);
        }
        return date;
    }

    function utcAsLocal(date) {
        if (isNotValidDate(date)) {
            return null;
        }

        return new Date(
            date.getUTCFullYear(),
            date.getUTCMonth(),
            date.getUTCDate(),
            date.getUTCHours(),
            date.getUTCMinutes(),
            date.getUTCSeconds(),
            date.getUTCMilliseconds()
        );
    }

    function localAsUtc(date) {
        if (isNotValidDate(date)) {
            return null;
        }

        return new Date(Date.UTC(
            date.getFullYear(),
            date.getMonth(),
            date.getDate(),
            date.getHours(),
            date.getMinutes(),
            date.getSeconds(),
            date.getMilliseconds()
        ));
    }

    function isValidDate(date) {
        return !isNotValidDate(date);
    }

    function isNotValidDate(date) {
        return date == null || isNaN(date.getTime());
    }

    // CHECKBOXES ==================================================================================================================================================

    function setCheckboxState(id, value) {
        if (value == 'True')
            value = true;
        else if (value == 'False')
            value = false;

        $('#' + id).prop('checked', value);
    }

    function setSwitcheryState(id, value) {
        if (value == 'True' || value == true)
            value = false;
        else if (value == 'False' || value == false)
            value = true;

        $('#' + id).prop('checked', value).trigger("click");
    }

    function applyCheckboxStyling() {
        $('.check-styled').uniform();
        $('.check-styled-info').uniform({
            wrapperClass: 'border-info-600 text-info-800'
        });
        $('.check-styled-danger').uniform({
            wrapperClass: 'border-danger-600 text-danger-800'
        });
    }

    function getCheckboxValue(id) {
        return $('#' + id).prop('checked');
    }

    function toggleCheckboxValue(id) {
        if (getCheckboxValue(id))
            setCheckboxState(id, false);
        else
            setCheckboxState(id, true);
    }

    function getSelectedCheckboxesId(datatablebody, checkboxClassname) {
        var idList = '';
        $('#' + datatablebody + ' tr').find('.' + checkboxClassname).each(function () {
            if ($(this).prop('checked') == true) {
                idList = $('input:checkbox.' + checkboxClassname + ':checked').map(function () {
                    return this.id;
                }).get();
            }
        });
        return idList;
    }

    // STRING MANIPULATOR ==========================================================================================================================================

    function appendChange(originalText, oldValue, newValue, pretext) {
        if (oldValue != newValue)
            return append(originalText, pretext + "'" + oldValue + "' => '" + newValue + "'");
        else
            return originalText;
    }

    function append(originalText, newValue) {
        if (originalText == "" && newValue != "")
            return newValue;
        else if (newValue == "")
            return originalText;
        else {
            if (originalText != "")
                originalText += " <BR>";
            return originalText += newValue;
        }
    }

    function appendWithDelimiter(originalText, newValue, delimiter) {
        if (originalText == "" && newValue != "")
            return newValue;
        else if (newValue == "")
            return originalText;
        else {
            if (originalText != "")
                originalText += delimiter +' ';
            return originalText += newValue;
        }
    }

    // ACTIVITY LOGS ===============================================================================================================================================

    //function Log(id) {
    //    $.ajax({
    //        type: "POST",
    //        url: '@Url.Action("Ajax_GetLog", "ActivityLogs")',
    //        data: {
    //            ReferenceId: id
    //        },
    //        success: function (data) {
    //            bootbox.dialog({
    //                title: 'Log',
    //                size: 'large',
    //                message: data.content,
    //                backdrop: true,
    //                onEscape: true
    //            }).off("shown.bs.modal"); //scroll to top
    //        }, //end success
    //        error: function (result) {
    //            bootbox.alert({ backdrop: true, message: "Error " + result });
    //        } //end error
    //    }); //end ajax
    //}

    // AJAX ========================================================================================================================================================

    //function AjaxExecute(action, controller, param1, callbackfunction, onEscapeFunction) {
    //    var url = '@Url.Action("__action", "__controller")';
    //    url = url.replace("__action", action).replace("__controller", controller);
    //    $.ajax({
    //        type: "POST", url: url, data: { param1: param1 },
    //        success: function (data) {
    //            if (data.Message != '')
    //                bootbox.dialog({ backdrop: true, message: data.Message, onEscape: function () { if (onEscapeFunction != null) onEscapeFunction(); } });

    //            if (callbackfunction != null)
    //                callbackfunction();
    //        },
    //        error: function (result) { bootbox.alert({ backdrop: true, message: "Error " + result }); }
    //    });
    //}

    //function AjaxUpdate(action, controller, id, value, executeIfHasNoMessage) {
    //    var url = '@Url.Action("__action", "__controller")';
    //    url = url.replace("__action", action).replace("__controller", controller);
    //    $.ajax({
    //        type: "POST", url: url, data: { id: id, value: value },
    //        success: function (data) {
    //            if (data.Message != '') {
    //                bootbox.alert({ backdrop: true, message: data.Message });
    //            } else {
    //                executeIfHasNoMessage();
    //            }
    //        },
    //        error: function (result) { bootbox.alert({ backdrop: true, message: "Error " + result }); }
    //    });
    //}

    //function AjaxDelete(action, controller, id, executeIfSuccess) {
    //    var url = '@Url.Action("__action", "__controller")';
    //    url = url.replace("__action", action).replace("__controller", controller);
    //    $.ajax({
    //        type: "POST", url: url, data: { id: id },
    //        success: function (data) {
    //            if (data.Message != '') {
    //                bootbox.alert({ backdrop: true, message: data.Message });
    //            } else {
    //                executeIfSuccess();
    //            }
    //        },
    //        error: function (result) { bootbox.alert({ backdrop: true, message: "Error " + result }); }
    //    });
    //}

    //function AjaxGet(id, title, size, action, controller) {
    //    var url = '@Url.Action("__action", "__controller")';
    //    url = url.replace("__action", action).replace("__controller", controller);
    //    $.ajax({
    //        type: "POST", url: url, data: { id: id },
    //        success: function (data) { bootbox.dialog({ backdrop: true, onEscape: true, title: title, size: size, message: data.content }); },
    //        error: function (result) { bootbox.alert({ backdrop: true, message: "Error " + result }); }
    //    });
    //}

    //function AjaxAssignValue(controlid, action, controller, id) {
    //    var url = '@Url.Action("__action", "__controller")';
    //    url = url.replace("__action", action).replace("__controller", controller);
    //    $.ajax({
    //        type: "POST", url: url, data: { id: id },
    //        success: function (data) { setValue(controlid, data.content); },
    //        error: function (result) { bootbox.alert({ backdrop: true, message: "Error " + result }); }
    //    });
    //}

    //function AjaxGetDropdownlistData(id, action, controller, key) {
    //    var url = '@Url.Action("__action", "__controller")';
    //    url = url.replace("__action", action).replace("__controller", controller);
    //    $('#'+id).select2({
    //        placeholder: 'Please Select',
    //        ajax: {
    //            url: url,
    //            data: function (params) {
    //                var query = {
    //                    keyword: params.term,
    //                    page: params.page || 1,
    //                    take: 20,
    //                    key: key
    //                }
    //                return query;
    //            },
    //        }
    //    });
    //}

    // DATATABLES PLUGIN ===========================================================================================================================================
    var topOfDataTable = 0;
    var datatable = $('.datatable').DataTable({
        bJQueryUI: true,
        sPaginationType: "full_numbers",
        fnDrawCallback: function (o) {
            if (o._iDisplayStart != topOfDataTable) {
                var marker = $('.content-nav');
                if (marker.length) {
                    var targetOffset = $('.topOfDataTable').offset().top;
                    $('html,body').animate({ scrollTop: targetOffset }, 500);
                    topOfDataTable = o._iDisplayStart;
                }
            }
        },
        autoWidth: false,
        pagingType: 'full_numbers',
        order: [[0, 'asc']],
        //ordering: false,
        bStateSave: true,
        stateSaveCallback: function (settings, data) {
            localStorage.setItem('DataTables_' + settings.sInstance, JSON.stringify(data))
        },
        stateLoadCallback: function (settings) {
            return JSON.parse(localStorage.getItem('DataTables_' + settings.sInstance))
        },
        stateLoadParams: function (settings, data) {
            if ($("#removeDatatablesStateSave").text() == "1") {
                data.search.search = "";
                data.start = 0;
            }
        },
        columnDefs: [
            { searchable: false, targets: "non-searchable" },
            { orderable: false, targets: "non-sortable" }
        ],
        dom: '<"datatable-header"flp>' +
            '<"table-responsive"t>' +
            '<"datatable-footer"ilp>',
        language: {
            emptyTable: "Gunakan filter dan tekan tombol LOAD untuk menampilkan data",
            search: '<span>Filter:</span> _INPUT_',
            searchPlaceholder: 'enter keyword',
            lengthMenu: '<span>Rows:</span> _MENU_',
            paginate: {
                'first': 'First', 'last': 'Last', 'next': $('html').attr('dir') == 'rtl' ? '&larr;' : '&rarr;', 'previous': $('html').attr('dir') == 'rtl' ? '&rarr;' : '&larr;'
            }
        }
    }).on('order.dt', function () {
        applyCheckboxStyling();
    });

    function getSearch() {
        if ((search == null || search == "") && $('input[aria-controls=DataTables_Table_0]').val() != undefined)
            return $('input[aria-controls=DataTables_Table_0]').val();
        else
            return "";
    }

    function setSearch(search) {
        if ($('input[aria-controls=DataTables_Table_0]').val() != undefined)
            $('input[aria-controls=DataTables_Table_0]').val(search).keyup();
    }

    // MOUSE EVENTS ================================================================================================================================================

    //disable scrollwheel mouse on input type number
    $(document).on("wheel", "input[type=number]", function (e) {
        $(this).blur();
    });

    function applyScrollButtons() {
        $(window).scroll(function () {
            //console.log("this.scrollTop:" + $(this).scrollTop() + " :: " + "this.height:" + $(this).height() + " :: " + " :: " + "document.height:" + $(document).height() + " :: " + ($(this).scrollTop() + $(this).height()) + " > " + ($(document).height()));
            if ($(this).scrollTop() > 50) {
                $('.scrollup').fadeIn();
            } else {
                $('.scrollup').fadeOut();
            }

            if ($(this).scrollTop() + $(this).height() > $(document).height() - 50) {
                $('.scrolldown').fadeOut();
            } else {
                $('.scrolldown').fadeIn();
            }
        });
    }

    // PRINTING ====================================================================================================================================================

    ////cannot be used because need to figure out how to render partial from different folder to shared folder
    function print(id) {
        var printContents = document.getElementById(id).innerHTML;
        var originalContents = document.body.innerHTML;

        document.body.innerHTML = printContents;
        window.print();
        document.body.innerHTML = originalContents;
    }

    // MISCLEANEOUS ================================================================================================================================================

    var badgeColors = [
        'badge-primary', 'badge-secondary', 'badge-info', 'badge-success', 'badge-dark', 'badge-warning', 'badge-danger', 'badge-light',
        'badge-primary', 'badge-secondary', 'badge-info', 'badge-success', 'badge-dark', 'badge-warning', 'badge-danger', 'badge-light',
        'badge-primary', 'badge-secondary', 'badge-info', 'badge-success', 'badge-dark', 'badge-warning', 'badge-danger', 'badge-light'
    ];

    function setValue(id, value) {
        if (value != null)
            $('#' + id).val(value);
    }

    function disableDivElements(id, value) {
        $('#' + id + ' :input').attr("disabled", value);
    }

    function disableElement(id, value) {
        $('#' + id).prop('disabled', value);
    }

    function toggleVisibility(id) {
        if ($('#' + id).hasClass('d-none'))
            $('#' + id).removeClass('d-none');
        else
            $('#' + id).addClass('d-none');
    }

    function setVisibility(id, value) {
        if (value)
            $('#' + id).removeClass('d-none');
        else
            $('#' + id).addClass('d-none');
    }

    function isVisible(id) {
        return !$('#' + id).hasClass('d-none');
    }

    function setTogglePanel(id, value) {
        if (value) {
            if (!$('#' + id).hasClass('show')) {
                $('#' + id).addClass('show');
            }
        }
        else {
            if ($('#' + id).hasClass('show')) {
                $('#' + id).removeClass('show');
            }
        }
    }

    function getRowNo(control) {
        var id = getElementId(control);
        return id.substring(id.indexOf('_') + 1, id.length);
    }

    function getElementId(control) {
        return $(control).prop('id');
    }

    //use data-container:'.modal-body' for modals and data-container:'body' everywhere else
    function applyPopovers() {
        $('[data-toggle="popover"]').popover({
            html: true,
            trigger: 'hover'
        });
    }

    function setModalControlToFocusOnShown(modalId, controlId) {
        $('#' + modalId).on('shown.bs.modal', function () {
            $('#' + controlId).focus();
        })
    }

    function GetRandomColor() {
        var trans = '0.3';
        var color = 'rgba(';
        for (var i = 0; i < 3; i++) {
            color += Math.floor(Math.random() * 255) + ',';
        }
        color += trans + ')';
        return color;
    }

    //==============================================================================================================================================================

    $(document).ready(function () {

        //setNavigationMenuVisibility();

        //$("input").attr("autocomplete", "off"); //turn off autocomplete
        //$('.form-input-styled').uniform(); //??
        $('.select2').select2(); //apply filtering on dropdownlists

        ////checkbox styling
        //applyCheckboxStyling();

        //applyPopovers(); //This is not working

        //selectFirstInput();

        ////on-off switch
        //if (document.querySelector('.form-check-input-switchery') != null) {
        //    var elems = Array.prototype.slice.call(document.querySelectorAll('.form-check-input-switchery'));
        //    elems.forEach(function (html) {
        //        var switchery = new Switchery(html);
        //    });
        //}

        ////Show the name of file in file input
        //$(".custom-file-input").on("change", function () {
        //    var fileName = $(this).val().split("\\").pop();
        //    $(this).siblings(".custom-file-label").addClass("selected").html(fileName);
        //});

        //$('.notesbutton').click(function (event) {
        //    var id = '#' + event.target.id;
        //    $(id + '_Div').addClass('d-none');
        //    $(id + '_Edit').removeClass('d-none');
        //    $(id + '_Notes').select();
        //});

        //$('.showPassword').mousedown(function () {
        //    $('.passwordTextbox').attr('type', 'text');
        //}).mouseup(function () {
        //    $('.passwordTextbox').attr('type', 'password');
        //}).mouseout(function () {
        //    $('.passwordTextbox').attr('type', 'password');
        //});

        //applyEventsToInputTextboxes(); //call here so it is executed initially

        //if (typeof applyLast !== 'undefined' && $.isFunction(applyLast)) applyLast();

        /*activateNavigationMenu();*/

        /*showControllerMessage();*/
    });

    function setNavigationMenuVisibility() {
        $('#navbar-navigation').children('ul:first').children('li').each(function (index) { //main menu
            var subMenu = $(this).children('div:first').children('div.dropdown-submenu'); //sub menu lvl 1

            recursivelySetSubmenuVisibility(subMenu);

            //hide main menu if has no children
            subMenu = $(this).children('div:first').children('div.dropdown-submenu');
            var notlabels = $(this).children('a:not(.dropdown-toggle)');
            var label = $(this).children('a.dropdown-toggle');
            var links = $(this).children('div:first').children('a');
            if (subMenu.length == 0 && ((notlabels.length == 0 && links.length == 0) || label == 1)) {
                $(this).remove();
            }
        });
    }

    function recursivelySetSubmenuVisibility(subMenu) {
        if (subMenu.length == 0) {
            return; //exit recursion
        } else {
            subMenu.each(function () {
                var nextSubMenu = $(this).children('div:first').children('div.dropdown-submenu');
                recursivelySetSubmenuVisibility(nextSubMenu);

                nextSubMenu = $(this).children('div:first').children('div.dropdown-submenu');
                var label = $(this).children('a.dropdown-toggle');
                var links = $(this).children('div:first').children('a');
                if (nextSubMenu.length == 0 && (links.length == 0 || label == 1))
                    $(this).remove();
            });
        }
    }

    function ScrollTo(id,duration) {
        $('html, body').animate({
            scrollTop: $('#' + id).offset().top
        }, duration);
    }

    function OpenWindow(url, opennewwindow) {
        if (opennewwindow == "true")
            window.open(url, '_blank');
        else
            window.location.href = url;
    }

    function emptyGuid() { return '00000000-0000-0000-0000-000000000000'; }
</script>