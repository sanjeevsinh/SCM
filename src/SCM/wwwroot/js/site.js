// Define utility methods for the client-side application

var SCM = {
};

SCM.Utilities = (function () {

    "use strict";


    // Define spinner options

    var showSpinner = function (target) {

        var opts = {
            lines: 13 // The number of lines to draw
          , length: 28 // The length of each line
          , width: 14 // The line thickness
          , radius: 42 // The radius of the inner circle
          , scale: 1 // Scales overall size of the spinner
          , corners: 1 // Corner roundness (0..1)
          , color: '#000' // #rgb or #rrggbb or array of colors
          , opacity: 0.25 // Opacity of the lines
          , rotate: 0 // The rotation offset
          , direction: 1 // 1: clockwise, -1: counterclockwise
          , speed: 1 // Rounds per second
          , trail: 60 // Afterglow percentage
          , fps: 20 // Frames per second when using setTimeout() as a fallback for CSS
          , zIndex: 2e9 // The z-index (defaults to 2000000000)
          , className: 'spinner' // The CSS class to assign to the spinner
          , top: '50%' // Top position relative to parent
          , left: '50%' // Left position relative to parent
          , shadow: false // Whether to render a shadow
          , hwaccel: false // Whether to use hardware acceleration
          , position: 'absolute' // Element positioning
        };

        // Append spinner to target element and dim the background
        $('body').addClass('modalBackground');
        var spinner = new Spinner(opts).spin();
        spinner.el.hidden = true;
        target.appendChild(spinner.el);

        $(spinner.el).fadeIn('slow');
    };

    var connectToNetworkSyncHub = function (args) {

        var $successMessage = $('#successMessage'),
            $errorMessage = $('#errorMessage'),
            $successMessageContainer = $('#successMessageContainer'),
            $errorMessageContainer = $('#errorMessageContainer');

        var errorGlyph = '<div class="alert alert-danger" role="alert">'
          + '<span class="glyphicon glyphicon-exclamation-sign" aria-hidden="true"></span>'
          + '<span class="sr-only">Error:</span>'
          + '</div>';

        var successGlyph = '<div class="alert alert-success" role="alert">'
          + '<span class="glyphicon glyphicon-ok-sign" aria-hidden="true"></span>'
          + '<span class="sr-only">Success:</span>'
          + '</div>';


        var onConnectionSuccess = function (hub) {

            // Join the hub group for the current Attachment Set
            hub.server.joinGroup(args.groupName);

            var $spinnerContainer = $('.row-spinner');

            // Handle Sync All and Check Sync All button click events
            if (args.syncAllUrl) {

                handleButtonClick({
                    $button: $('#Sync'),
                    $spinnerContainer: $spinnerContainer,
                    url: args.syncAllUrl,
                    beforeSend: function () {

                        initSpinners($spinnerContainer);

                        // Disable all buttons, including row buttons, whilst CheckSyncAll executes
                        $('.btn').prop('disabled', true);
                    },
                    onSuccess: function (responseItem) {

                        $spinnerContainer.each(function () {

                            $(this).data('spinner').stop();
                        });
                        $('.btn').prop('disabled', false);

                        if (responseItem.success) {

                            showSuccessMessage(responseItem.message);
                        }
                        else {

                            showErrorMessage(responseItem.message);
                        }
                    },
                    onError: function (errorThrown) {

                        $('.btn').prop('disabled', false);
                        $spinnerContainer.each(function () {

                            $(this).data('spinner').stop();
                        });
                        showErrorMessage(errorThrown);
                    }
                });
            }

            if (args.checkSyncAllUrl) {

                handleButtonClick({
                    $button: $('#CheckSync'),
                    $spinnerContainer: $spinnerContainer,
                    url: args.checkSyncAllUrl,
                    beforeSend: function () {

                        initSpinners($spinnerContainer);

                        // Disable all buttons, including row buttons, whilst CheckSyncAll executes
                        $('.btn').prop('disabled', true);
                    },
                    onSuccess: function (responseItem) {

                        $('.btn').prop('disabled', false);
                        $spinnerContainer.each(function () {

                            $(this).data('spinner').stop();
                        });

                        if (responseItem.success) {

                            showSuccessMessage(responseItem.message);
                        }
                        else {

                            showErrorMessage(responseItem.message);
                        }
                    },
                    onError: function (errorThrown) {

                        $('.btn').prop('disabled', false);
                        $spinnerContainer.each(function () {

                            $(this).data('spinner').stop();
                        });
                        showErrorMessage(errorThrown);
                    }
                });
            }

            // Handle click events for buttons in each table row

            $('.table .btn-sync, .table .btn-checksync').each(function () {

                var $this = $(this);
                var $buttons = $this.parents('td').children('.btn');
                var data = $this.data('item');
                var $spinnerContainer = $this.parents('tr').children('.row-spinner');
                var $requiresSync = $this.parents('tr').children('.checkbox-insync').children('input');

                // Calculate URL and replace tokens in url with data 
                var url = $this.hasClass('btn-sync') ? args.syncUrl : args.checkSyncUrl;

                for (var item in data) {

                    var token = '{' + item + '}';
                    url = url.replace(token,
                        function (match) {
                            return data[item];
                        });
                }

                handleButtonClick({
                    $button: $this,
                    $spinnerContainer: $spinnerContainer,
                    url: url,
                    beforeSend: function () {

                        initSpinners($spinnerContainer);

                        // Disable row buttons
                        $buttons.prop('disabled', true);
                    },
                    onSuccess: function (responseItem) {

                        $spinnerContainer.data('spinner').stop();
                        $buttons.prop('disabled', false);

                        // Set requiresSync checkbox state
                        $requiresSync.prop('checked', !responseItem.success);
                      
                        if (responseItem.success) {

                            showSuccessMessage(responseItem.message);
                            $spinnerContainer.html(successGlyph);
                        }
                        else {

                            showErrorMessage(responseItem.message);
                            $spinnerContainer.html(errorGlyph);
                        }
                    },
                    onError: function (errorThrown) {

                        $spinnerContainer.data('spinner').stop();
                        $buttons.prop('disabled', false);
                        showErrorMessage(errorThrown);

                        // Set requiresSync checkbox state
                        $requiresSync.prop('checked', true);

                        // Show error glyph
                        $spinnerContainer.html(errorGlyph);
                    }
                });
            });
        };

        initMessaging();
        initHub();

        function initMessaging() {

            if ($successMessage.html().length === 0) {

                $successMessageContainer.hide();
            }

            if ($errorMessage.html().length === 0) {

                $errorMessageContainer.hide();
            }
        }

        function initHub() {

            // Reference the auto-generated proxy for the hub.
            var hub = $.connection.networkSyncHub;

            // Start the connection.
            $.connection.hub.start()
                .done(function () {
                    onConnectionSuccess(hub);
                })
                .fail(function () {
                    showErrorMessage("Failed to establish a connection to the server.");
                });

            // Create a method that the hub calls when processing of a single item has completed
            hub.client.onSingleComplete = function (item, success, message) {

                // Stop the spinner
                var $syncStatus = $('#syncStatus_' + item[args.itemKey]);
                $syncStatus.data('spinner').stop();

                // Set requiresSync checkbox state
                $('#requiresSync_' + item[args.itemKey] + ' > input').prop('checked', !success);

                // Enable buttons
                $syncStatus.parents('tr').find('.btn').prop('disabled', false);

                // Show success or error glyph and message
                if (success) {

                    $syncStatus.html(successGlyph);
                    if (message) showSuccessMessage(message);
                }
                else {

                    $syncStatus.html(errorGlyph);
                    if (message) showErrorMessage(message);
                }
            };
        }

        function handleButtonClick(args) {

            args.$button.on('click', function () {

                if (typeof args.beforeSend === 'function') {

                    args.beforeSend();
                }

                // Call the server
                $.ajax({
                    url: args.url,
                    method: 'POST',
                    success: function (data, textStatus, jqXHR) {

                        if (typeof args.onSuccess === 'function') {

                            args.onSuccess.call(this, data);
                        }
                    },
                    error: function (jqXHR, textStatus, errorThrown) {

                        if (typeof args.onError === 'function') {

                            args.onError.call(this, errorThrown);
                        }
                    }
                });
            });
        }

        function parseToHtml(message) {


        }

        function initSpinners($item) {

            $item.each(function () {

                var $this = $(this);
                $this.empty();

                var spinnerOpts = {
                    lines: 13,
                    length: 28,
                    width: 14,
                    radius: 42,
                    scale: 0.15
                };

                var spinner = new Spinner(spinnerOpts).spin(this);
                $this.data('spinner', spinner);
            });
        }

        function showSuccessMessage(message) {

            $successMessage.html(message);
            $successMessageContainer.show();
            $errorMessage.empty();
            $errorMessageContainer.hide();
        }

        function showErrorMessage(message) {

            $errorMessage.html(message);
            $errorMessageContainer.show();
            $successMessage.empty();
            $successMessageContainer.hide();
        }
    };

    return {

        showSpinner: showSpinner,
        connectToNetworkSyncHub: connectToNetworkSyncHub
    };

}(jQuery));
