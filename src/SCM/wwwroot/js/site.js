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

            var $sync = $('#Sync'),
                $checkSync = $('#CheckSync');

            handleButtonClick($sync, args.syncUrl);
            handleButtonClick($checkSync, args.checkSyncUrl);
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

            // Create a method that the hub calls when processing of all VPNs is complete

            hub.client.onAllComplete = function (message, success) {

                // Enable submit buttons

                $('.btn').prop('disabled', false);

                if (success) {
                    showSuccessMessage(message);
                }
                else {
                    showErrorMessage(message);
                }
            };

            // Create a method that the hub calls when processing of a single vpn has completed

            hub.client.onSingleComplete = function (item, success) {

                var $syncStatus = $('#syncStatus_' + item[args.itemKey]);

                // Set requiresSync checkbox state

                $('#requiresSync_' + item[args.itemKey] + ' > input').prop('checked', !success);

                $syncStatus.data('spinner').stop();

                if (success) {
                    $syncStatus.html(successGlyph);
                }
                else {
                    $syncStatus.html(errorGlyph);
                }
            };
        }

        function handleButtonClick($button, url) {

            $button.on('click', function (e) {

                // Prevent double-click

                $('.btn').prop('disabled', true);

                var $spinnerElems = $(".row-spinner");
                $spinnerElems.each(function () {

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

                // Call the server

                $.ajax({
                    url: url,
                    method: 'POST',
                    data: { id: args.contextVal }
                });
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
