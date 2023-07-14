$(function () {
    var userId = $('#notificationsDropdown').data('user-id');

    if (userId) {
        loadNotifications();

        function loadNotifications() {
            var cachedNotifications = sessionStorage.getItem('notifications_' + userId);
            if (cachedNotifications) {
                displayNotifications(JSON.parse(cachedNotifications));
                return;
            }

            fetchNotifications();
        }

        function fetchNotifications() {
            var token = $('input[name="__RequestVerificationToken"]').val();

            $.ajax({
                url: '/Profile/Notifications',
                method: 'get',
                headers: {
                    "RequestVerificationToken": token,
                    "User-ID": userId
                },
                success: function (data) {
                    updateCacheAndDisplayNotifications(data);
                },
                error: function () {
                    console.error('Error occurred while fetching notifications data.');
                }
            });
        }

        function updateCacheAndDisplayNotifications(notifications) {
            sessionStorage.setItem('notifications_' + userId, JSON.stringify(notifications));
            displayNotifications(notifications);
        }

        function displayNotifications(notifications) {
            var container = $('#notificationsContainer');
            var countContainer = $('#notificationsCount');

            container.empty();

            if (notifications.length > 0) {
                countContainer.text(notifications.length);

                $.each(notifications, function (index, notification) {
                    var notificationHtml =
                        '<a class="dropdown-item">' +
                        'From: ' + notification.sender + ', ' +
                        'Content: ' + notification.content + ' ' +
                        '<button class="btn btn-primary btn-sm mark-read-btn" data-notification-id="' + notification.id + '">Mark as Read</button>' +
                        '</a>';

                    container.append(notificationHtml);
                });
            } else {
                container.append('<span class="dropdown-item">No new notifications</span>');
                countContainer.text('');
            }

            $('.mark-read-btn').click(function () {
                var notificationId = $(this).data('notification-id');
                markNotificationAsRead(notificationId);
            });
        }

        function markNotificationAsRead(id) {
            var token = $('input[name="__RequestVerificationToken"]').val();

            $.ajax({
                url: '/Profile/MarkAsRead',
                method: 'post',
                headers: {
                    "RequestVerificationToken": token,
                    "User-ID": userId
                },
                data: { id: id },
                success: function (response) {
                    var cachedNotifications = sessionStorage.getItem('notifications_' + userId);

                    if (cachedNotifications) {
                        var notifications = JSON.parse(cachedNotifications);
                        var updatedNotifications = notifications.filter(function (notification) {
                            return notification.id !== id;
                        });

                        sessionStorage.setItem('notifications_' + userId, JSON.stringify(updatedNotifications));
                    }

                    loadNotifications();
                },
                error: function () {
                    console.error('Error occurred while marking notification as read.');
                }
            });
        }
    }
});