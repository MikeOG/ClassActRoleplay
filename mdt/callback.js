$( document ).ready(function() {
    console.log(fetchOnlineUsers());
});

function fetchOnlineUsers() {
    var _online = {};
    forumUrl = 'https://ca-rp.net/forum/';
    $.get(forumUrl, function(data) {
        let r = /(<span class="block-footer-counter">Total:&nbsp;)([0-9]{1,10})( \(members:&nbsp;)([0-9]{1,10})(, guests:&nbsp;)([0-9]{1,10})(\)<\/span>)/
        let f = r.exec(data);
        try {
            if(f[1] && f[3] && f !== null) {
                _online['total'] = f[2];
                _online['registered'] = f[4];
                _online['guests'] = f[6];

            } else {
                console.log("Could not fetch user statistics");
                _online['total'] = "N/A";
                _online['registered'] = "N/A";
                _online['guests'] = "N/A";
            }
        } catch(e) {
            _online['total'] = "N/A";
            _online['registered'] = "N/A";
            _online['guests'] = "N/A";
        }
    });
    return _online;
}