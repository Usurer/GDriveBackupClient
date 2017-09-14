var app = {}

app.init = function() {
    function getLocalTree(pathEncoded) {
        $.ajax({
            method: 'GET',
            url: '/api/list/GetLocalFolders?rootPathEncoded=' + pathEncoded,
            error: function(e) {
                console.info(e);
                handleLocalError();
            },
            success: function (data) {
                console.info(data);
                addLocalChildren(pathEncoded, data);
            }
        });
    }

    function getRemoteTree(rootId) {
        $.ajax({
            method: 'GET',
            url: '/api/list/GetRemoteFolders?rootId=' + rootId,
            error: function(e) {
                console.info(e);
                handleRemoteError();
            },
            success: function(data) {
                console.info(data);
                addRemoteChildren(rootId, data);
            }
        });
    }

    function addChildren(parentElement, children) {
        var list = document.createElement('ul');

        for (var i = 0; i < children.length; i++) {
            var item = document.createElement('li');
            item.innerHTML = children[i];
            list.append(item);
        }

        parentElement.append(list);
    }

    function addLocalChildren(parent, tree) {
        parent = atob(parent);
        var parentContainer = $('.js-localTree__container').find('[data-path=' + parent + ']');
        if (parentContainer.length === 0) {
            parentContainer = $('.js-localTree__container').first();
        } else {
            parentContainer = parentContainer.first();
        }

        addChildren(parentContainer, tree);

        $('.js-localTree__container').removeClass('loader');
    }

    function addRemoteChildren(parent, tree) {
        var parentContainer = $('.js-remoteTree__container').find('[data-rootId=' + parent + ']');
        if (parentContainer.length === 0) {
            parentContainer = $('.js-remoteTree__container').first();
        } else {
            parentContainer = parentContainer.first();
        }

        addChildren(parentContainer, tree);
        $('.js-remoteTree__container').removeClass('loader');
    }

    function handleError(el) {
        el.removeClass('loader');
        el.html('ERROR');
    }

    function handleLocalError(el) {
        handleError($('.js-localTree__container'));
    }

    function handleRemoteError(el) {
        handleError($('.js-remoteTree__container'));
    }

    $('.js-localTree__container').click(function(e) {
        var path = $(e.target).attr('data-path');
        var pathEncoded = '';

        if (path !== undefined && path !== null && path.length > 0) {
            pathEncoded = btoa(path);    
        }

        $('.js-localTree__container').addClass('loader');
        $('.js-localTree__container').html('');

        getLocalTree(pathEncoded);
    });

    $('.js-remoteTree__container').click(function (e) {
        var rootId = $(e.target).attr('data-rootId');
        if (rootId === undefined || rootId === null) {
            rootId = '';
        }

        $('.js-remoteTree__container').addClass('loader');
        $('.js-remoteTree__container').html('');

        getRemoteTree(rootId);
    });

}

$(document).ready(app.init);