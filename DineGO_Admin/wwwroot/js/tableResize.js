document.querySelectorAll('table').forEach(function (table) {
    const thList = Array.from(table.querySelectorAll('th'));
    table.querySelectorAll('th.resizable-col').forEach(function (th) {
        var thIndex = thList.indexOf(th);
        var resizer = document.createElement('div');
        resizer.className = 'resizer';
        th.appendChild(resizer);

        let startX, startWidth;
        resizer.addEventListener('mousedown', function (e) {
            startX = e.pageX;
            startWidth = th.offsetWidth;
            document.documentElement.style.cursor = 'col-resize';

            function onMouseMove(e) {
                var newWidth = startWidth + (e.pageX - startX);
                if (newWidth > 40) {
                    th.style.width = newWidth + 'px';
                    th.style.minWidth = newWidth + 'px';
                    table.querySelectorAll('tbody tr').forEach(function (row) {
                        if (row.children[thIndex]) {
                            row.children[thIndex].style.width = newWidth + 'px';
                            row.children[thIndex].style.minWidth = newWidth + 'px';
                        }
                    });
                }
            }
            function onMouseUp() {
                document.removeEventListener('mousemove', onMouseMove);
                document.removeEventListener('mouseup', onMouseUp);
                document.documentElement.style.cursor = '';
            }
            document.addEventListener('mousemove', onMouseMove);
            document.addEventListener('mouseup', onMouseUp);
        });
    });
});